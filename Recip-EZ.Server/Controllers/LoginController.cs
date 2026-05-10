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

    /// <summary>
    /// Represents the result of a login operation, including user identification, status, and an optional message.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// The ID of the authenticated user.
        /// </summary>
        public int UserId { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation completed successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
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

    /// <summary>
    /// Represents the result of an authentication operation, including user identity and status information.
    /// </summary>
    /// <remarks>This class is typically used to return user details and authentication status from
    /// authentication endpoints or services. It provides basic user information for clients after a successful or
    /// failed authentication attempt.</remarks>
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

        /// <summary>
        /// Retrieves information about the currently authenticated user.
        /// </summary>
        /// <remarks>This endpoint requires authentication. The response includes the user's ID, username,
        /// and display name as obtained from the current authentication context.</remarks>
        /// <returns>An <see cref="OkObjectResult"/> containing the authenticated user's details if the user is authorized;
        /// otherwise, an <see cref="UnauthorizedResult"/> if the user is not authenticated.</returns>
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

        /// <summary>
        /// Logs out the current user by removing the authentication cookie from the response.
        /// </summary>
        /// <remarks>This action requires the user to be authenticated. After calling this method, the client will no
        /// longer be authenticated on subsequent requests unless a new login is performed.</remarks>
        /// <returns>An <see cref="OkObjectResult"/> containing a success flag and a message indicating the logout was successful.</returns>
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

        /// <summary>
        /// Generates a JSON Web Token (JWT) for the specified user, containing user identity and authentication claims.
        /// </summary>
        /// <remarks>The generated token includes claims for the user's identifier, name, and email
        /// address. The token's expiration and signing credentials are determined by configuration settings. Ensure
        /// that the configuration contains valid JWT settings for correct token generation.</remarks>
        /// <param name="user">The user for whom the JWT will be generated. Must not be null and should contain valid user identification
        /// and authentication information.</param>
        /// <returns>A string representing the generated JWT, which can be used for authenticating the user in subsequent
        /// requests.</returns>
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
