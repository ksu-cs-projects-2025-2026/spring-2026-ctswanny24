using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Recip_EZ.Server.Models;
using Recip_EZ.Server.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Recip_EZ.Server.Controllers
{
    public class LoginResponse
    {
        public int UserId { get; init; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Class that represents the login request information. Username & Password.
    /// </summary>
    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class AuthenticatedUserResponse
    {
        public bool Success { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Controller for all Login and Authentication related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor to allow for the use of the UserService in the methods of this controller. 
        /// This is where the connection to the database is established for this controller through the service layer.
        /// </summary>
        /// <param name="userService">The UserService instance to be used by this controller</param>
        public LoginController(UserService userService, IConfiguration config)
        {
            _config = config;
            _userService = userService;
        }

        /// <summary>
        /// Authenticates a user based on the provided login credentials.
        /// </summary>
        /// <param name="loginData">The login credentials submitted by the client.</param>
        /// <returns>Sets the auth cookie when authentication is successful.</returns>
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest loginData)
        {
            var user = _userService.GetUser(loginData.Username, loginData.Password);

            if (user == null)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Check your username and password and try again."
                });
            }

            var tokenString = GenerateJwtToken(user);
            var duration = _config.GetValue<int>("Jwt:DurationInMinutes");

            Response.Cookies.Append("recip-ez-auth", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(duration),
                IsEssential = true
            });

            return Ok(new LoginResponse
            {
                Success = true,
                UserId = user.UserId,
                Message = "Login successful."
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var name = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            return Ok(new AuthenticatedUserResponse
            {
                Success = true,
                UserId = userId,
                Username = username,
                Name = name
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("recip-ez-auth", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true
            });

            return Ok(new { Success = true, Message = "Logged out successfully." });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var duration = _config.GetValue<int>("Jwt:DurationInMinutes");

            var name = user.FirstName + " " + user.LastName;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, user.Username),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Experimenting with DB connection. This Get request accesses the db and retrieves all users and brings that to the front end.
        /// THIS WILL ONLY WORK WHILE THE DB IS SMALL IN SCALE.
        /// </summary>
        /// <returns>IActionResult with the JSON of the db response</returns>
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }
    }
}
