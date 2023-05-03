using BlazorApp1.Auth;
using BlazorApp1.Models.Mobile;
using BlazorApp1.Models.Mobile.Responses;
using BlazorApp1.Services;
using BlazorApp1.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BlazorApp1.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("api/employee")]
        public async Task<IActionResult> GetEmployee(string id)
        {
            try
            {
                EmployeeService service = new EmployeeService(CollectionNames.Employees.ToString(), CollectionNames.EmployeesHistory.ToString());
                var response = new EmployeeMobile(service.GetOne(id));

                if (response == null)
                {
                    return NotFound(new BaseMobileResponce("{}", (int)ErrorHandling.ErrorCodes.ERROR_USER_NOT_FOUND));
                }

                return Ok(new BaseMobileResponce(response, (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponce("{}", ex.Message));
            }
        }
    }
}
