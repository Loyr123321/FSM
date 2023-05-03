using BlazorApp1.Auth;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorApp1.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        //---------------------------------------------
        private readonly IConfiguration _configuration;
        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //---------------------------------------------


        [HttpGet]
        [AllowAnonymous]
        [Route("api/test/gettoken")]
        public async Task<IActionResult> CreateJWT(/*User user*/)
        {
            User user = new(); user.Email = "pivo555@yandex.ru";
            var token = AuthOptions.CreateJWT(inputUser:user, AuthOptions.JWT_TYPE.Register);
            return Ok(token);
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/test/test0")]
        public async Task<IActionResult> TT()
        {
            try 
            {
                return Ok("api/test/test0_mysuperresult");
            }
            catch(Exception ex) 
            {
                return BadRequest("api / test / test0_exception"+ ex.Message);
            }
        }

        [Authorize(Policy = "MyAccessPolicy")]
        [HttpGet]
        [Route("api/test/test111")]
        public async Task<IActionResult> test111()
        {
            try
            {
                return Ok("api/test/test111");
            }
            catch (Exception ex)
            {
                return BadRequest("api / test / test0_exception" + ex.Message);
            }
        }
    }
}
