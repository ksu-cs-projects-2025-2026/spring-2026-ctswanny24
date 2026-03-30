using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Recip_EZ.Server.Controllers
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest loginData)
        {

            if (loginData.Username == "caden@gmail.com" && loginData.Password == "1234z")
            {
                return Ok(new { Success = true, Token = "fake-jwt-token" });
            }
            else
            {
                return Unauthorized(new { Success = false, Message = "Invalid credentials" });
            }
        }
    }
}
