using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThousandSunny.API.Security.Requirements
{
    public class ThousandSunnyAuthorizationRequirement : IAuthorizationRequirement
    {
        public string TokenType { get; }

        public ThousandSunnyAuthorizationRequirement(string tokenType)
        {
            TokenType = tokenType;
        }
    }
}
