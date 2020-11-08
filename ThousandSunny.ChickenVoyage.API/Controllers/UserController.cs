using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ThousandSunny.ChickenVoyage.API.Models;

namespace ThousandSunny.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TSChickenVoyageContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(TSChickenVoyageContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}
