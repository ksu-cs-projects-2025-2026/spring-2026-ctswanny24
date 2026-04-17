using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using System.ComponentModel.Design;

namespace Recip_EZ.Server.Controllers
{
    public class LoginResponse
    {
        public int UserId { get; init; }
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
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        readonly UserService _userService;

        public LoginController(UserService userService)
        {
            _userService = userService;
        }



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
            User user = FetchUser(loginData);

            if (user.Username == loginData.Username && user.Password == loginData.Password)
            {
                Console.WriteLine("Login Success!!!");
                return Ok(new LoginResponse { Success = true, UserId = user.UserId, Message = "Login Successful" });
            }
            else
            {
                Console.WriteLine("Login Failed");
                return Unauthorized(new LoginResponse { Success = false, Token = null, Message = "Check your fields and try again" });

            }
        }

        /// <summary>
        /// Experimenting with DB connection. This Get request accesses the db and retrieves all users and brings that to the front end.
        /// THIS WILL ONLY WORK WHILE THE DB IS SMALL IN SCALE.
        /// </summary>
        /// <returns>IActionResult with the JSON of the db response</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        private User FetchUser(LoginRequest data)
        {
            User? user = _userService.GetUser(data.Username, data.Password);

            return (user);
        }
    }
}
