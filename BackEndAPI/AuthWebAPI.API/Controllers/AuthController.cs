using AuthWebAPI.Domain.Entities;
using AuthWebAPI.Domain.UserDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebAPI.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService Service) : ControllerBase
    {

        [HttpPost("Register")]
        public async Task<ActionResult<User?>> Register(UserDto userdto)
        {

            var user =await Service.RegisterAsync(userdto);
            if (user == null)
                return BadRequest("Username or Email or Password is empty or already exist!");
            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<TokenRegenerateDto?>> Login(UserDto userdto)
        {
            var token =await Service.LoginAsync(userdto);
            if (token == null)
                return BadRequest("UserName or Email or Password is incorrect, Please try again with Correct Credentials!");
            return Ok(token);
        }
        
        [HttpPost("refreshtoken")]
        public async Task<ActionResult<TokenRegenerateDto?>?> RefreshToken()
        {
            
            // refresh token comes from HttpOnly cookie
            if (!HttpContext!.Request.Cookies.TryGetValue("refreshToken", out var PlainRefreshToken))
                return BadRequest("Empty Token.");
            var token =await Service.RefreshTokenAsync(PlainRefreshToken);
            if (token == null)
                return BadRequest("Invalid/Expire Token.");
            return Ok(token);
        }

        [HttpPost("logout")]
        public async Task<IActionResult?> Logout()
        {
            if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var PlainRefreshToken))
                return BadRequest("Token is Empty.");
            
            var Plaintoken = await Service.LogoutAsync(PlainRefreshToken);
            if(Plaintoken != null)
            {
                // delete cookie
                HttpContext.Response.Cookies.Delete("refreshToken");
            }

            return Ok();
        }
    }
}
