using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ThousandSunny.ChickenVoyage.API.Models;
using ThousandSunny.ChickenVoyage.API.Security.Requirements;

namespace ThousandSunny.ChickenVoyage.API.Security.Handlers
{
    public class UserIsValidHandler : AuthorizationHandler<UserIsValidRequirement>
    {
        private readonly TSChickenVoyageContext _dbContext;
        private readonly ILogger<UserIsValidHandler> _logger;

        public UserIsValidHandler(TSChickenVoyageContext dbContext, ILogger<UserIsValidHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsValidRequirement requirement)
        {
            // Get userId from JWT identity
            ClaimsIdentity identity = context.User.Identity as ClaimsIdentity;
            Claim userIdClaim = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType);
            if (userIdClaim == null)
            {
                _logger.LogInformation($"No user claim was found.");
                context.Fail();
                return Task.CompletedTask;
            }

            _logger.LogInformation($"Checking to see if user {userIdClaim.Value} is valid.");

            // Get user from context
            User user = _dbContext.User.Find(Guid.Parse(userIdClaim.Value));
            if (user == null)
            {
                _logger.LogInformation($"Could not find {userIdClaim.Value}'s user.");
                context.Fail();
            }
            else
            {
                _logger.LogInformation($"{userIdClaim.Value}'s user was found as {user.Email}.");
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
