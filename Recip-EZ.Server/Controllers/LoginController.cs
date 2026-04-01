using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Recip_EZ.Server.Controllers
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Class that represents the login request information. Username & Password.
    /// </summary>
    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }


    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        /// <summary>
        /// Authenticates a user based on the provided login credentials.
        /// </summary>
        /// <param name="loginData">The login credentials submitted by the client. Must include valid user identification and password
        /// information.</param>
        /// <returns>An HTTP 200 response with a success flag and a token if authentication is successful; otherwise, an HTTP 401
        /// response with an error message.</returns>
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest loginData)
        {

            if (Authenticate(loginData))
            {
                Console.WriteLine("Login Success!!!");
                return Ok(new LoginResponse { Success = true, Token = "fake-jwt-token", Message = "Login Successful" });
            }
            else
            {
                Console.WriteLine("Login Failed");
                return Unauthorized(new LoginResponse { Success = false, Token = null, Message = "Check your fields and try again" });

            }
        }

        private bool Authenticate(LoginRequest data)
        {
            // Implement your authentication logic here (e.g., EVENTUALLY check against a database)
            //TEMP IMPLEMENTATION 
            return(data.Username == "caden@gmail.com" && data.Password == "1234z");

        }
    }
}
