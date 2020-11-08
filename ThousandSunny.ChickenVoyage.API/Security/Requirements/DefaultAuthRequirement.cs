using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThousandSunny.ChickenVoyage.API.Security.Requirements
{
    public class DefaultAuthRequirement : IAuthorizationRequirement
    {

        public DefaultAuthRequirement()
        {

        }
    }
}
