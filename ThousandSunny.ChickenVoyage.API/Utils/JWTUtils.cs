using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ThousandSunny.ChickenVoyage.API.Config;

namespace ThousandSunny.ChickenVoyage.API.Utils
{
    public static class JWTUtils
    {
        /// <summary>
        /// Create a JWT token.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GenerateTokenFromUser(IConfiguration config, Guid userId)
        {
            // Generate security key
            var mySecret = config.GetSecurityAuthTokenSecret();
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            // Get issuer and audience
            var myIssuer = config.GetSecurityAuthTokenIssuer();
            var myAudience = config.GetSecurityAuthTokenAudience();

            // Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            // Return token
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Create validation parameters for JWT tokens. To be used in validation.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static TokenValidationParameters GenerateTokenValidationParameters(IConfiguration config)
        {
            var tokenSecret = config.GetSecurityAuthTokenSecret();
            var tokenIssuer = config.GetSecurityAuthTokenIssuer();
            var tokenAudience = config.GetSecurityAuthTokenAudience();

            return new TokenValidationParameters
            {
                // Clock skew compensates for server time drift.
                ClockSkew = TimeSpan.FromMinutes(2),

                // Specify the key used to sign the token:
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSecret)),
                RequireSignedTokens = true,

                // Ensure the token hasn't expired:
                RequireExpirationTime = true,
                ValidateLifetime = true,

                // Ensure the token audience matches our audience value (default true):
                ValidateAudience = true,
                ValidAudience = tokenAudience,

                // Ensure the token was issued by a trusted authorization server (default true):
                ValidateIssuer = true,
                ValidIssuer = tokenIssuer
            };
        }
    }
}
