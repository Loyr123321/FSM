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
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationReasonController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReasons()
        {
            try
            {
                CancellationReasonService service = new(CollectionNames.CancellationReasons.ToString());
                var reasons = service.GetAll().OrderBy(x => x.Position).ToList();

                var response = new List<CancellationReasonMobile>();
                foreach (var reason in reasons) 
                {
                    response.Add(new CancellationReasonMobile(reason));
                }

                if (response.Count == 0)
                {
                    return StatusCode(204, new BaseMobileResponse("[]", (int)ErrorHandling.ErrorCodes.ERROR_NO_CONTENT));
                }

                return Ok(new BaseMobileResponse(response, (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse("[]", ex.Message));
            }
        }
    }
}
