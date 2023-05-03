using BlazorApp1.Auth;
using BlazorApp1.Models;
using BlazorApp1.Models.Mobile;
using BlazorApp1.Models.Mobile.Responses;
using BlazorApp1.Services;
using BlazorApp1.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Net.Http.Headers;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace BlazorApp1.Controllers
{
    //Любой залогинненый сотрудник может работать с любыми заказами если знает их id
    [ApiController]
    public class OrderController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            try
            {
                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());
                var order = service.GetOne(id);

                if (order == null)
                {
                    return StatusCode(204, new BaseMobileResponse("{}", (int)ErrorHandling.ErrorCodes.ERROR_NO_CONTENT));
                }

                //Проверить что монтажник который обращается к api назначен на данный заказ
                var jwtTokenString = HttpContext.Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", string.Empty);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(jwtTokenString);
                string executorId = (string)jwtSecurityToken?.Claims?.First(claim => claim.Type == "id")?.Value;
                if (order?.OrderEmployeeExecutor?.Id != executorId)
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_INVALID_EXECUTOR));
                }

                //установить прочитано исполнителем
                order.IsReadByExecutor = true;
                service.SaveOrUpdate(order);

                var response = new OrderMobile(order);
                return Ok(new BaseMobileResponse(response, (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse("{}", ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]//[HttpPost("executor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/executor")]
        public async Task<IActionResult> GetOrdersForExecutor(string id, string[] statusCodes)
        {
            try
            {
                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());

                List<Order> allOrders = service.GetOrdersByExecutorId(id);
                List<Order> orders = new List<Order>();
                foreach (var statusCode in statusCodes)
                {
                    var response_i = allOrders.Where(x => x.OrderStatus.StatusCode == statusCode);
                    orders.AddRange(response_i);
                }

                if (orders == null || orders.Count == 0)
                {
                    return StatusCode(204, new BaseMobileResponse("[]", (int)ErrorHandling.ErrorCodes.ERROR_NO_CONTENT));
                }

                var response = new List<OrderMobile>();
                foreach (var order in orders)
                {
                    response.Add(new OrderMobile(order));
                }

                return Ok(new BaseMobileResponse(response, (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse("[]", ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]//[HttpPost("executor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/filtered/executor")]
        public async Task<IActionResult> GetFilteredOrdersForExecutor(string id, string[]? statusCodes, DateTime? dateFrom, DateTime? dateTo, bool? isEarlyFirst)
        {
            try
            {
                if (dateFrom == null)
                    dateFrom = DateTime.MinValue;
                if (dateTo == null)
                    dateFrom = DateTime.MaxValue;

                if(statusCodes == null || statusCodes.Count() == 0)
                {
                    statusCodes[0] = "NEW";
                    statusCodes[1] = "ACCEPTED";
                    statusCodes[2] = "INROAD";
                    statusCodes[3] = "ACTIVE";
                    statusCodes[4] = "REJECTED";
                    statusCodes[5] = "CANCELED";
                    statusCodes[6] = "SUSPENDED";
                    statusCodes[7] = "CHANGED";
                    statusCodes[8] = "COMPLETED";
                    //statusCodes[9] = "DELETED";
                }

                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());

                List<Order> allOrders = service.GetOrdersByExecutorId(id);

                allOrders = allOrders.Where(x => x.PlannedDateTime != null).ToList();
                List<Order> orders = new List<Order>();
                foreach (var statusCode in statusCodes)
                {
                    var response_i = allOrders.Where(x => x.OrderStatus.StatusCode == statusCode
                    && x.PlannedDateTime >= dateFrom
                    && x.PlannedDateTime <= dateTo);
                    orders.AddRange(response_i);
                }

                if (orders == null || orders.Count == 0)
                {
                    return StatusCode(204, new BaseMobileResponse("[]", (int)ErrorHandling.ErrorCodes.ERROR_NO_CONTENT));
                }

                var response = new List<OrderMobile>();
                foreach (var order in orders)
                {
                    response.Add(new OrderMobile(order));
                }

                if ((bool)isEarlyFirst)
                {
                    response = response.OrderBy(x => x.PlannedDateTime).ToList();
                }
                else
                {
                    response = response.OrderByDescending(x => x.PlannedDateTime).ToList();
                }

                return Ok(new BaseMobileResponse(response, (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse("[]", ex.Message));
            }
        }

        //Принять
        //NEW(0)_TO_ACCEPTED(1)
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/accept")]
        public async Task<IActionResult> SetOrderStatusAccept(string orderId)
        {
            try
            {
                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());
                var order = service.GetOne(orderId);
                if (order == null)
                    return NotFound(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_NOT_FOUND));

                //Проверить что монтажник который обращается к api назначен на данный заказ
                var jwtTokenString = HttpContext.Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", string.Empty);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(jwtTokenString);
                string executorId = (string)jwtSecurityToken?.Claims?.First(claim => claim.Type == "id")?.Value;
                if(order?.OrderEmployeeExecutor?.Id != executorId)
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_INVALID_EXECUTOR));
                }

                OrderStatus newStatus = OrderStatus.ACCEPTED;
                if (!order.OrderStatus.Equals(OrderStatus.NEW)) 
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_WRONG_STATUS));
                }
                
                order.OrderStatus = newStatus;
                service.SaveOrUpdate(order);

                return Ok(new BaseMobileResponse(new ShortResponse(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse(new ShortResponse(false), ex.Message));
            }
        }

        //Выехать
        //ACCEPTED(1)_TO_INROAD(2)
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/inroad")]
        public async Task<IActionResult> SetOrderStatusInRoad(string orderId)
        {
            try
            {
                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());
                var order = service.GetOne(orderId);
                if (order == null)
                    return NotFound(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_NOT_FOUND));

                //Проверить что монтажник который обращается к api назначен на данный заказ
                var jwtTokenString = HttpContext.Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", string.Empty);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(jwtTokenString);
                string executorId = (string)jwtSecurityToken?.Claims?.First(claim => claim.Type == "id")?.Value;
                if (order?.OrderEmployeeExecutor?.Id != executorId)
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_INVALID_EXECUTOR));
                }

                OrderStatus newStatus = OrderStatus.INROAD;
                if (!order.OrderStatus.Equals(OrderStatus.ACCEPTED))
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_WRONG_STATUS));
                }

                //SET_ROAD_TIME_START
                order.RoadTimeStart = DateTime.Now;
                order.OrderStatus = newStatus;
                service.SaveOrUpdate(order);

                return Ok(new BaseMobileResponse(new ShortResponse(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse(new ShortResponse(false), ex.Message));
            }
        }

        //Начать работу
        //INROAD(2)_TO_ACTIVE(3)
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/active")]
        public async Task<IActionResult> SetOrderStatusActive(string orderId)
        {
            try
            {
                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());
                var order = service.GetOne(orderId);
                if (order == null)
                    return NotFound(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_NOT_FOUND));

                //Проверить что монтажник который обращается к api назначен на данный заказ
                var jwtTokenString = HttpContext.Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", string.Empty);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(jwtTokenString);
                string executorId = (string)jwtSecurityToken?.Claims?.First(claim => claim.Type == "id")?.Value;
                if (order?.OrderEmployeeExecutor?.Id != executorId)
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_INVALID_EXECUTOR));
                }

                OrderStatus newStatus = OrderStatus.ACTIVE;
                if (!order.OrderStatus.Equals(OrderStatus.INROAD))
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_WRONG_STATUS));
                }

                //SET_ROAD_TIME_STOP & SET_START_TIME
                order.RoadTimeStop = DateTime.Now;
                order.StartTime = DateTime.Now;
                order.OrderStatus = newStatus;
                service.SaveOrUpdate(order);

                return Ok(new BaseMobileResponse(new ShortResponse(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse(new ShortResponse(false), ex.Message));
            }
        }

        //Завершить заказ
        //COMPLETE ACTIVE(3)_TO_COMPLETED(8)
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]//[HttpPost("complete")]
        [HttpPut]//[HttpPut("complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/complete")]
        public async Task<IActionResult> CompleteOrder(string id, List<TemplateFieldMobileComplete> inputFields)
        {
            try
            {
                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());
                Order order = service.GetOne(id);
                if (order == null)
                {
                    return NotFound(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_NOT_FOUND));
                }

                //Проверить что монтажник который обращается к api назначен на данный заказ
                var jwtTokenString = HttpContext.Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", string.Empty);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(jwtTokenString);
                string executorId = (string)jwtSecurityToken?.Claims?.First(claim => claim.Type == "id")?.Value;
                if (order?.OrderEmployeeExecutor?.Id != executorId)
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_INVALID_EXECUTOR));
                }

                if (order.OrderStatus.Equals(OrderStatus.ACTIVE))
                {
                    foreach (var field in inputFields)
                    {
                        if (field.Value != null)
                        {
                            string json = System.Text.Json.JsonSerializer.Serialize(((JsonElement)field.Value)); //убираем kindvalue

                            //Проверка на совпадение Типа поля (которы пришел с тем который в базе)
                            if (field.Type == order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id)).Type)
                            {
                                switch (field.Type.ToString())
                                {
                                    case FieldType.FTText:
                                    case FieldType.FTLink:
                                        try
                                        {
                                            //string? valueString = System.Text.Json.JsonSerializer.Deserialize<string>(json);
                                            string? valueString = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(json);
                                            order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value = valueString;
                                        }
                                        catch (Exception ex) { Console.WriteLine("CompleteOrder_(FieldNotFound)Exception: " + ex.Message); }
                                        break;

                                    case FieldType.FTRuble:
                                    case FieldType.FTDouble:
                                        try
                                        {
                                            double? valueDouble = Newtonsoft.Json.JsonConvert.DeserializeObject<double>(json);
                                            order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value = valueDouble;
                                        }
                                        catch (Exception ex) { Console.WriteLine("CompleteOrder_(FieldNotFound)Exception: " + ex.Message); }
                                        break;

                                    case FieldType.FTLong:
                                        try
                                        {
                                            long? valueLong = Newtonsoft.Json.JsonConvert.DeserializeObject<long>(json);
                                            order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value = valueLong;
                                        }
                                        catch (Exception ex) { Console.WriteLine("CompleteOrder_(FieldNotFound)Exception: " + ex.Message); }
                                        break;

                                    case FieldType.FTYesNo:
                                        try
                                        {
                                            bool? valueBool = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(json);
                                            order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value = valueBool;
                                        }
                                        catch (Exception ex) { Console.WriteLine("CompleteOrder_(FieldNotFound)Exception: " + ex.Message); }
                                        break;

                                    case FieldType.FTDate:
                                    case FieldType.FTDateTime:
                                        try
                                        {
                                            DateTime? valueDateTime = System.Text.Json.JsonSerializer.Deserialize<DateTime>(json);
                                            order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value = valueDateTime;
                                        }
                                        catch (Exception ex) { Console.WriteLine("CompleteOrder_(FieldNotFound)Exception: " + ex.Message); }
                                        break;

                                    case FieldType.FTFile:
                                    case FieldType.FTPhoto:
                                        try
                                        {
                                            List<TFile>? valueTFiles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TFile>>(json);
                                            order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value = valueTFiles;
                                        }
                                        catch (Exception ex) { Console.WriteLine("CompleteOrder_(FieldNotFound)Exception: " + ex.Message); }
                                        break;

                                    case FieldType.FTList:
                                        try
                                        {
                                            //!!! Проверить все ли нормально вставляется
                                            DataListMobile? dataListMobile = Newtonsoft.Json.JsonConvert.DeserializeObject<DataListMobile>(json);

                                            var valueDataList = dataListMobile.ToOriginalList();
                                            //order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value = valueDataList; //целиком
                                            //Вставить не целиком а только выбранные значения
                                            DataList datalist = order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value as DataList;
                                            datalist.SelectedSingleValue = valueDataList.SelectedSingleValue;
                                            datalist.SelectedValues = valueDataList.SelectedValues;
                                        }
                                        catch (Exception ex) { Console.WriteLine("CompleteOrder_(FieldNotFound)Exception: " + ex.Message); }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (order.Template.ExecutorFields != null && order.Template.ExecutorFields.Count > 0)
                            {
                                try
                                {
                                    order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value = null;
                                }
                                catch (Exception ex) { Console.WriteLine("CompleteOrder_(fieldNullValue)Exception: " + ex.Message); }
                            }
                        }
                    }

                    order.OrderStatus = OrderStatus.COMPLETED;
                    order.StopTime = DateTime.Now;
                    service.SaveOrUpdate(order);

                    return Ok(new BaseMobileResponse(new ShortResponse(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
                }
                else
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_WRONG_STATUS));
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse(new ShortResponse(false), ex.Message));
            }
        }


        //Отклонить4(Виноват монтажник)
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/reject")]
        public async Task<IActionResult> RejectOrder(string orderId)
        {
            try
            {
                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());
                var order = service.GetOne(orderId);
                if (order == null)
                    return NotFound(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_NOT_FOUND));

                //Проверить что монтажник который обращается к api назначен на данный заказ
                var jwtTokenString = HttpContext.Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", string.Empty);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(jwtTokenString);
                string executorId = (string)jwtSecurityToken?.Claims?.First(claim => claim.Type == "id")?.Value;
                if (order?.OrderEmployeeExecutor?.Id != executorId)
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_INVALID_EXECUTOR));
                }

                OrderStatus newStatus = OrderStatus.REJECTED;
                if (order.OrderStatus.Equals(OrderStatus.REJECTED) || order.OrderStatus.Equals(OrderStatus.CANCELED) || order.OrderStatus.Equals(OrderStatus.COMPLETED) /*|| order.OrderStatus.Equals(OrderStatus.DELETED)*/)
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_FINAL_STATUS));


                //Отклонено
                order.OrderStatus = newStatus;
                service.SaveOrUpdate(order);

                //Увеличить счетчик отклоненныхМонтажником
                EmployeeService employeeService = new(CollectionNames.Employees.ToString(), CollectionNames.EmployeesHistory.ToString());
                var executor = order.OrderEmployeeExecutor;
                if (executor != null)
                {
                    executor.RejectedCounter++;
                    employeeService.SaveOrUpdateWithoutHistory(executor);
                }
                //

                return Ok(new BaseMobileResponse(new ShortResponse(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse(new ShortResponse(false), ex.Message));
            }
        }


        //Отменить5(Виноват клиент или 3е лицо)
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/cancel")]
        public async Task<IActionResult> CancelOrder(string orderId, string reasonId, string? note)
        {
            try
            {
                OrderService orderService = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());
                CancellationReasonService reasonService = new(CollectionNames.CancellationReasons.ToString());
                var order = orderService.GetOne(orderId);
                if (order == null)
                {
                    return NotFound(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_NOT_FOUND));
                }

                //Проверить что монтажник который обращается к api назначен на данный заказ
                var jwtTokenString = HttpContext.Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", string.Empty);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(jwtTokenString);
                string executorId = (string)jwtSecurityToken?.Claims?.First(claim => claim.Type == "id")?.Value;
                if (order?.OrderEmployeeExecutor?.Id != executorId)
                {
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_INVALID_EXECUTOR));
                }

                if (order.OrderStatus.Equals(OrderStatus.REJECTED) || order.OrderStatus.Equals(OrderStatus.CANCELED) || order.OrderStatus.Equals(OrderStatus.COMPLETED) /*|| order.OrderStatus.Equals(OrderStatus.DELETED)*/)
                    return BadRequest(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_FINAL_STATUS));

                var newStatus = OrderStatus.CANCELED;

                var reason = reasonService.GetOne(reasonId);
                if (reason == null)
                    return NotFound(new BaseMobileResponse(new ShortResponse(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_REASON_NOT_FOUND));

                if (!string.IsNullOrEmpty(note))
                    reason.ReasonNote = note;

                order.OrderStatus = newStatus;
                order.IsReadByExecutor = true;
                order.CancellationReason = reason;
                orderService.SaveOrUpdate(order);

                return Ok(new BaseMobileResponse(new ShortResponse(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse(new ShortResponse(false), ex.Message));
            }
        }

        //Приостановить6(SUSPENDED) будет ставить диспетчер
        //Изменено7(CHANGED) пока нигде не используется
    }
}
