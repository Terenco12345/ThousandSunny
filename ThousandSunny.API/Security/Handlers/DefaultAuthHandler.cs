using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThousandSunny.API.Security.Requirements;

namespace ThousandSunny.API.Security.Handlers
{
    public class ThousandSunnyAuthorizationHandler : AuthorizationHandler<ThousandSunnyAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ThousandSunnyAuthorizationRequirement requirement)
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
