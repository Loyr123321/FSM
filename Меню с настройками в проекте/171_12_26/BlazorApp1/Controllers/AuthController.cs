using BlazorApp1.Auth;
using BlazorApp1.Models;
using BlazorApp1.Models.Mobile;
using BlazorApp1.Models.Mobile.Responses;
using BlazorApp1.Services;
using BlazorApp1.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using static BlazorApp1.Auth.AuthOptions;

namespace BlazorApp1.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //public AuthController(IHttpContextAccessor httpContextAccessor)
        //{
        //    _httpContextAccessor = httpContextAccessor;
        //}

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/auth/employee")]
        public async Task<IActionResult> Auth(string login, string password) //Для мобильной версии
        {
            try
            {
                EmployeeService service = new EmployeeService(CollectionNames.Employees.ToString(), CollectionNames.EmployeesHistory.ToString());
                var employee = service.GetEmployeeByLogin(login);
                if (employee == null)
                {
                    return Unauthorized(new BaseMobileResponce(new EmployeeAuthResponse(null, null, null), (int)ErrorHandling.ErrorCodes.ERROR_USER_NOT_FOUND));
                }

                bool access = SecretHasher.Verify(password, employee.Password);
                EmployeeAuthResponse response;
                if (access==false) 
                {
                    return Unauthorized(new BaseMobileResponce(new EmployeeAuthResponse(null, null, null), (int)ErrorHandling.ErrorCodes.ERROR_INCORRECT_PASSWORD));
                }

                string accessToken = AuthOptions.CreateJWT(employee, JWT_TYPE.Access);
                string refreshToken = AuthOptions.CreateJWT(employee, JWT_TYPE.Refresh);

                //UpdateWhiteList
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                JwtWhiteListService whitleList = new(CollectionNames.JwtWhiteList.ToString());
                var jwtRow = whitleList.GetJwtRowByUserId(employee.Id);
                if (jwtRow == null)
                {
                    jwtRow = new();
                    jwtRow.UserId = employee.Id;
                }
                jwtRow.AccessTokenId = handler.ReadJwtToken(accessToken).Id;
                jwtRow.RefreshTokenId = handler.ReadJwtToken(refreshToken).Id;
                whitleList.SaveOrUpdate(jwtRow);

                return Ok(new BaseMobileResponce(new EmployeeAuthResponse(employee.Id, accessToken, refreshToken), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponce(new EmployeeAuthResponse(null, null, null), ex.Message));
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/auth/employee/refresh_token")]
        public async Task<IActionResult> RefreshTokens(string refreshToken)
        {
            try
            {
                bool isValid = AuthOptions.ValidateToken(refreshToken, AuthOptions.GetSymmetricSecurityRefreshKey());
                if (isValid == false) 
                {
                    return Unauthorized(new BaseMobileResponce(new EmployeeAuthResponse(null, null, null), (int)ErrorHandling.ErrorCodes.ERROR_INVALID_TOKEN));
                }

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(refreshToken);
                if(jwtSecurityToken.Subject != AuthOptions.JWT_TYPE.Refresh.ToString()) 
                {
                    return Unauthorized(new BaseMobileResponce(new EmployeeAuthResponse(null, null, null), (int)ErrorHandling.ErrorCodes.ERROR_INVALID_TOKEN));
                }

                Employee? tokenEmployee = Newtonsoft.Json.JsonConvert.DeserializeObject<Employee>(jwtSecurityToken.Claims.First(claim => claim.Type == "employee").Value);
                if (tokenEmployee == null || string.IsNullOrEmpty(tokenEmployee.Id))
                {
                    return Unauthorized(new BaseMobileResponce(new EmployeeAuthResponse(null, null, null), (int)ErrorHandling.ErrorCodes.ERROR_INVALID_USER));
                }

                //CheckWhiteList
                string refreshId = jwtSecurityToken.Id.ToString();
                JwtWhiteListService whitleList = new(CollectionNames.JwtWhiteList.ToString());
                JwtRow? row = whitleList.GetJwtRowByTokenId(refreshId);
                if (row == null) 
                {
                    return Unauthorized(new BaseMobileResponce(new EmployeeAuthResponse(null, null, null), (int)ErrorHandling.ErrorCodes.ERROR_TOKEN_IS_NOT_WHITELISTED));
                }

                //UpdateWhiteList
                string accessToken = AuthOptions.CreateJWT(tokenEmployee, JWT_TYPE.Access);
                string newRefreshToken = AuthOptions.CreateJWT(tokenEmployee, JWT_TYPE.Refresh);
                row.AccessTokenId = handler.ReadJwtToken(accessToken).Id;
                row.RefreshTokenId = handler.ReadJwtToken(newRefreshToken).Id;
                whitleList.SaveOrUpdate(row);

                return Ok(new BaseMobileResponce(new EmployeeAuthResponse(tokenEmployee.Id, accessToken, newRefreshToken), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponce(new EmployeeAuthResponse(null, null, null), ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/auth/employee/changepass")]
        public async Task<IActionResult> ChangePass(string id, string newpassword)
        {
            try
            {
                EmployeeService service = new EmployeeService(CollectionNames.Employees.ToString(), CollectionNames.EmployeesHistory.ToString());
                var employee = service.GetOne(id);
                if (employee == null)
                {
                    return NotFound(new BaseMobileResponce(new ShortResponce(false), (int)ErrorHandling.ErrorCodes.ERROR_USER_NOT_FOUND));
                }

                employee.Password = employee.GetHash(newpassword);
                service.SaveOrUpdate(employee);

                return Ok(new BaseMobileResponce(new ShortResponce(true), (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                //return StatusCode(500, e.Message);
                return StatusCode(500, new BaseMobileResponce(new ShortResponce(false), ex.Message));
            }
        }

    }
}
