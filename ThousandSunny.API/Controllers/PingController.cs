using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ThousandSunny.API.Models;

namespace ThousandSunny.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        private readonly ThousandSunnyContext _context;
        private readonly ILogger<PingController> _logger;

        public PingController(ThousandSunnyContext context, ILogger<PingController> logger)
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
        public IActionResult PostPing(Ping ping)
        {
            Ping dupePing = _context.Ping.Find(ping.ID);
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
