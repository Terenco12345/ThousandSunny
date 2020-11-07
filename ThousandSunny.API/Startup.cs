using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThousandSunny.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ThousandSunny.API.Security.Handlers;
using ThousandSunny.API.Security.Requirements;
using System.Text;
using System;

namespace ThousandSunny.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Connect to Thousand Sunny SQL database
            var thousandSunnyConnection = Configuration["ConnectionStrings:ThousandSunnyContext"];
            services.AddDbContext<ThousandSunnyContext>(opt => opt.UseSqlServer(thousandSunnyConnection));

            ConfigureSecurity(services);

            services.AddControllers();
        }

        /// <summary>
        /// Configure authentication and authorization.
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureSecurity(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                var serviceTokenSecret = Configuration["Security:AuthTokenSecret"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(serviceTokenSecret))
                };
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new ThousandSunnyAuthorizationRequirement("access"))
                    .RequireAuthenticatedUser()
                    .Build();
            });
            services.AddSingleton<IAuthorizationHandler, ThousandSunnyAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
