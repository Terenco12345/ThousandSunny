using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Net.Http;
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
                return Unauthorized(new { Message = "Email already exists." });
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

            return Ok(
                new { 
                    Message = "Registered user successfully.",

                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    CreationDate = user.CreationDate.ToString()
                }
            );
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
                _logger.LogInformation($"{loginRequest.Email} tried to log in, but no user was found under that email. ");
                return Unauthorized(new { Message = "Email is not registered to any user." });
            }
            bool verified = HashUtils.VerifyHashedPassword(user.Password, loginRequest.Password);
            if(!verified)
            {
                _logger.LogInformation($"{loginRequest.Email} tried to log in, but failed to provide correct password. ");
                return Unauthorized(new { Message = "Incorrect password." });
            }

            string token = JWTUtils.GenerateTokenFromUser(_configuration, user.Id);

            _logger.LogInformation($"{loginRequest.Email} logged in successfully, and generated a token.");
            return Ok(
                new { 
                    Message = "User logged in successfully.", 
                    Token = token,

                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    CreationDate = user.CreationDate.ToString()
                }
            );
            
        }

        /// <summary>
        /// Attempt to get the user's profile.
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult GetUserProfile()
        {
            // Get userId from JWT identity
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            string userId = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value;

            try
            {
                // Get user from context
                User user = _context.User.Find(Guid.Parse(userId));

                _logger.LogInformation($"{user.Email} retrieved their profile information. ");
                // Return OK
                return Ok(new 
                {
                    Message = "Profile obtained successfully.",

                    DisplayName = user.DisplayName, 
                    Email = user.Email, 
                    CreationDate = user.CreationDate.ToString() 
                });
            } catch
            {
                _logger.LogInformation($" {userId} tried to retrieve their profile information, but failed. ");
                return NotFound(new { 
                    Message = "Could not find profile."
                });
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
            return Ok(new 
                { 
                    Message = "User logged out successfully." 
                }
            );
        }
    }
}
