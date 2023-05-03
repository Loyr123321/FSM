using BlazorApp1.Auth;
using BlazorApp1.Models;
using BlazorApp1.Models.Mobile;
using BlazorApp1.Models.Mobile.Responses;
using BlazorApp1.Services;
using BlazorApp1.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
                    return StatusCode(204, new BaseMobileResponce("{}", (int)ErrorHandling.ErrorCodes.ERROR_NO_CONTENT));
                }

                //установить прочитано исполнителем
                order.IsReadByExecutor = true;
                service.SaveOrUpdate(order);

                var response = new OrderMobile(order);
                return Ok(new BaseMobileResponce(response, (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponce("{}", ex.Message));
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
                    return StatusCode(204, new BaseMobileResponce("[]", (int)ErrorHandling.ErrorCodes.ERROR_NO_CONTENT));
                }

                var response = new List<OrderMobile>();
                foreach (var order in orders)
                {
                    response.Add(new OrderMobile(order));
                }

                return Ok(new BaseMobileResponce(response, (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponce("[]", ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]//[HttpPost("executor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/filtered/executor")]
        public async Task<IActionResult> GetFilteredOrdersForExecutor(string id, string[] statusCodes, DateTime dateFrom, DateTime dateTo, bool isEarlyFirst = true)
        {
            try
            {
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
                    return StatusCode(204, new BaseMobileResponce("[]", (int)ErrorHandling.ErrorCodes.ERROR_NO_CONTENT));
                }

                var response = new List<OrderMobile>();
                foreach (var order in orders)
                {
                    response.Add(new OrderMobile(order));
                }

                if (isEarlyFirst)
                {
                    response = response.OrderBy(x => x.PlannedDateTime).ToList();
                }
                else
                {
                    response = response.OrderByDescending(x => x.PlannedDateTime).ToList();
                }

                return Ok(new BaseMobileResponce(response, (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponce("[]", ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]//[HttpPost("complete")]
        [HttpPut]//[HttpPut("complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/complete")]
        public async Task<IActionResult> CompleteOrder(string id, List<TemplateField> inputFields)
        {
            try
            {
                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());
                Order order = service.GetOne(id);
                if (order == null)
                {
                    return NotFound(new BaseMobileResponce(new ShortResponce(false), (int)ErrorHandling.ErrorCodes.ERROR_NOT_FOUND));
                }

                if (order.OrderStatus.Equals(OrderStatus.ACTIVE))
                {
                    foreach (var field in inputFields)
                    {
                        if (field._value != null)
                        {

                            string json = System.Text.Json.JsonSerializer.Serialize(((JsonElement)field._value)); //убираем kindvalue
                            switch (field.Type)
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
                                        int? valueDateTime = System.Text.Json.JsonSerializer.Deserialize<int>(json);
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
                                        DataListMobile? dataListMobile = Newtonsoft.Json.JsonConvert.DeserializeObject<DataListMobile>(json);
                                        //DataList? valueDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<DataList>(json);
                                        var valueDataList = dataListMobile.ToOriginalList();
                                        order.Template.ExecutorFields.Find(x => x.Id.Equals(field.Id))._value = valueDataList;
                                    }
                                    catch (Exception ex) { Console.WriteLine("CompleteOrder_(FieldNotFound)Exception: " + ex.Message); }
                                    break;
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

                    return Ok(new BaseMobileResponce(new ShortResponce(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
                }
                else
                {
                    return BadRequest(new BaseMobileResponce(new ShortResponce(false), (int)ErrorHandling.ErrorCodes.ERROR_INVALID_STATUS));
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponce(new ShortResponce(false), ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]//[HttpPost("setstatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/order/setstatus")]
        public async Task<IActionResult> SetOrderStatus(string orderId, string statusCode)
        {
            try
            {
                OrderService service = new(CollectionNames.Orders.ToString(), CollectionNames.OrdersHistory.ToString());
                var order = service.GetOne(orderId);
                if (order == null)
                    return NotFound(new BaseMobileResponce(new ShortResponce(false), (int)ErrorHandling.ErrorCodes.ERROR_NOT_FOUND));

                var newStatus = OrderStatus.statuses.First(x => x.StatusCode.Equals(statusCode));
                if (newStatus == null)
                    return NotFound(new BaseMobileResponce(new ShortResponce(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_STATUS_NOT_FOUND));

                if (order.OrderStatus.Equals(OrderStatus.REJECTED) || order.OrderStatus.Equals(OrderStatus.CANCELED) || order.OrderStatus.Equals(OrderStatus.COMPLETED) || order.OrderStatus.Equals(OrderStatus.DELETED))
                    return BadRequest(new BaseMobileResponce(new ShortResponce(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_FINAL_STATUS));

                if (newStatus.Equals(OrderStatus.INROAD))
                    order.RoadTimeStart = DateTime.Now;

                if (newStatus.Equals(OrderStatus.ACTIVE)) //начал работу
                {
                    order.RoadTimeStop = DateTime.Now;
                    order.StartTime = DateTime.Now;
                }
                if (newStatus.Equals(OrderStatus.REJECTED)) //Отклонено
                {
                    order.RoadTimeStart = null;
                    order.RoadTimeStop = null;
                    order.StartTime = null;
                    order.StopTime = null;


                    //Увеличить счетчик отклоненныхМонтажником
                    EmployeeService employeeService = new(CollectionNames.Employees.ToString(), CollectionNames.EmployeesHistory.ToString());
                    var executor = order.OrderEmployeeExecutor;

                    if (executor != null)
                    {
                        executor.RejectedCounter++;
                        employeeService.SaveOrUpdateWithoutHistory(executor);
                    }
                    //
                }

                //|| newStatus.Equals(OrderStatus.SUSPENDED)


                order.OrderStatus = newStatus;
                service.SaveOrUpdate(order);

                return Ok(new BaseMobileResponce(new ShortResponce(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponce(new ShortResponce(false), ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]//[HttpPost("cancel")]
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
                    return NotFound(new BaseMobileResponce(new ShortResponce(false), (int)ErrorHandling.ErrorCodes.ERROR_NOT_FOUND));
                }

                if (order.OrderStatus.Equals(OrderStatus.REJECTED) || order.OrderStatus.Equals(OrderStatus.CANCELED) || order.OrderStatus.Equals(OrderStatus.COMPLETED) || order.OrderStatus.Equals(OrderStatus.DELETED))
                    return BadRequest(new BaseMobileResponce(new ShortResponce(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_FINAL_STATUS));

                var status = OrderStatus.CANCELED;

                var reason = reasonService.GetOne(reasonId);
                if (reason == null)
                    return NotFound(new BaseMobileResponce(new ShortResponce(false), (int)ErrorHandling.ErrorCodes.ERROR_ORDER_REASON_NOT_FOUND));

                if (!string.IsNullOrEmpty(note))
                    reason.ReasonNote = note;

                order.OrderStatus = status;
                order.IsReadByExecutor = true;
                order.CancellationReason = reason;
                orderService.SaveOrUpdate(order);

                return Ok(new BaseMobileResponce(new ShortResponce(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponce(new ShortResponce(false), ex.Message));
            }
        }





    }
}
