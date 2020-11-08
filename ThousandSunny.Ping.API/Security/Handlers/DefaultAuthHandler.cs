using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThousandSunny.Ping.API.Security.Requirements;

namespace ThousandSunny.Ping.API.Security.Handlers
{
    public class DefaultAuthHandler : AuthorizationHandler<DefaultAuthRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DefaultAuthRequirement requirement)
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
