using AuthWebAPI.Domain.Entities;
using AuthWebAPI.Domain.UserDto;
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
                return BadRequest("Username or Email or Password is empty");
            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<TokenRegenerateDto?>> Login(UserDto userdto)
        {
            var token =await Service.LoginAsync(userdto);
            if (token == null)
                return BadRequest("UserName or Email or Password is incorrect.");
            return Ok(token);
        }
        
        [HttpPost("Refresh-Token")]
        public async Task<ActionResult<TokenRegenerateDto?>> RefreshToken(RefreshTokenRequestDto refreshtoken)
        {
            var token =await Service.RefreshTokenAsync(refreshtoken);
            if (token == null)
                return BadRequest("Invalid/Expire Token.");
            return Ok(token);
        }

    }
}
