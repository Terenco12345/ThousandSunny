using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ThousandSunny.ChickenVoyage.API.Models;
using ThousandSunny.ChickenVoyage.API.Utils;

namespace ThousandSunny.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TSChickenVoyageContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;

        public UserController(TSChickenVoyageContext context, ILogger<UserController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Create a new user account, when supplied with a new email.
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            _logger.LogInformation("Register request", registerRequest);
            // Check for duplicate user
            User duplicateUser = _context.User.SingleOrDefault(user => user.Email == registerRequest.Email);
            if (duplicateUser != null)
            {
                return Forbid("A user already exists with this email.");
            }

            // Generate a new user and add it to the database
            User user = new User()
            {
                DisplayName = registerRequest.DisplayName,
                Email = registerRequest.Email,
                Password = HashUtils.HashAndSaltPassword(registerRequest.Password),
                CreationDate = DateTime.Now,
                IsActive = true
            };

            _context.User.Add(user);
            _context.SaveChanges();

            return Ok("Registered user successfully.");
        }

        /// <summary>
        /// Attempt to login, and retrieve a JWT token.
        /// </summary>
        /// <param name="loginRequest">Login details</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            User user = _context.User.SingleOrDefault(user => user.Email == loginRequest.Email);
            if (user == null)
            {
                return Unauthorized("Could not find user with this email.");
            }
            bool verified = HashUtils.VerifyHashedPassword(user.Password, loginRequest.Password);
            if(!verified)
            {
                return Unauthorized("Password was wrong.");
            }

            string token = JWTUtils.GenerateTokenFromUser(_configuration, user.Id);
            HttpContext.Response.Headers.Add("Authorization", "Bearer " + token);

            return Ok("User logged in successfully.");
            
        }

        /// <summary>
        /// Attempt to get the user's profile.
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult GetUserProfile()
        {
            try
            {
                // Get userId from JWT identity
                ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
                string userId = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value;

                // Get user from context
                User user = _context.User.Find(Guid.Parse(userId));

                // Return OK
                return Ok(user);
            } catch
            {
                return Unauthorized("User does not exist.");
            }
        }

        /// <summary>
        /// Remove the token from the header, and log out a user.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Headers.Add("Authorization", "");
            return Ok("User logged out successfully.");
        }
    }
}
