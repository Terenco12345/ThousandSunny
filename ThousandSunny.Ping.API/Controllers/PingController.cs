using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ThousandSunny.Ping.API.Models;

namespace ThousandSunny.Ping.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        private readonly TSPingContext _context;
        private readonly ILogger<PingController> _logger;

        public PingController(TSPingContext context, ILogger<PingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("")]
        public IActionResult NormalPing()
        {
            _logger.LogInformation("Normal ping called.");
            return new OkResult();
        }

        [HttpGet("auth")]
        public IActionResult AuthPing()
        {
            _logger.LogInformation("Auth ping called.");
            return new OkResult();
        }

        [AllowAnonymous]
        [HttpGet("exception")]
        public IActionResult ExceptionPing()
        {
            _logger.LogInformation("Exception ping called.");
            throw new Exception("Expected exception in response to ping.");
        }

        [HttpPost("")]
        public IActionResult PostPing(Models.Ping ping)
        {
            Models.Ping dupePing = _context.Ping.Find(ping.ID);
            if(dupePing == null)
            {
                _context.Ping.Add(ping);
                _context.SaveChanges();
                return new OkResult();
            } else
            {
                return new ForbidResult();
            }
        }
    }
}
