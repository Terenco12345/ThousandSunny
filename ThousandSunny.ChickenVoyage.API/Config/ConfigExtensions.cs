using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThousandSunny.ChickenVoyage.API.Config
{
    public static class ConfigExtensions
    {
        // Connection Strings
        public static string GetTSChickenVoyageContextConnectionString(this IConfiguration config)
        {
            return config["ConnectionStrings:TSChickenVoyageContext"];
        }

        // Security
        public static string GetSecurityAuthTokenSecret(this IConfiguration config)
        {
            return config["Security:AuthTokenSecret"];
        }

        public static string GetSecurityAuthTokenIssuer(this IConfiguration config)
        {
            return config["Security:AuthTokenIssuer"];
        }

        public static string GetSecurityAuthTokenAudience(this IConfiguration config)
        {
            return config["Security:AuthTokenAudience"];
        }
    }
}
