using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ThousandSunny.ChickenVoyage.API.Models;

namespace ThousandSunny.ChickenVoyage.API.Controllers
{
    [Authorize]
    public class CharacterSheetController: Controller
    {
        private readonly TSChickenVoyageContext _context;
        private readonly ILogger<CharacterSheetController> _logger;
        private readonly IConfiguration _configuration;

        public CharacterSheetController(TSChickenVoyageContext context, ILogger<CharacterSheetController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Create()
        {
            // Get userId from JWT identity
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            string userId = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value;

            return Ok();
        }

        public IActionResult Read()
        {
            // Get userId from JWT identity
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            string userId = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value;

            return Ok();
        }

        public IActionResult Update()
        {
            // Get userId from JWT identity
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            string userId = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value;

            return Ok();
        }

        public IActionResult Delete()
        {
            // Get userId from JWT identity
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            string userId = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value;

            return Ok();
        }

    }
}
