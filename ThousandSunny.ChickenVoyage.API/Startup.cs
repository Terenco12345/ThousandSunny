using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThousandSunny.ChickenVoyage.API.Config;
using ThousandSunny.ChickenVoyage.API.Models;
using ThousandSunny.ChickenVoyage.API.Security.Handlers;
using ThousandSunny.ChickenVoyage.API.Security.Requirements;
using ThousandSunny.ChickenVoyage.API.Utils;

namespace ThousandSunny.ChickenVoyage.API
{
    public class Startup
    {
        readonly string AllowChickenVoyageWebCORS = "ChickenVoyageWebCORSPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Connect to Thousand Sunny SQL database
            var thousandSunnyConnection = Configuration.GetTSChickenVoyageContextConnectionString();
            services.AddDbContext<TSChickenVoyageContext>(opt => opt.UseSqlServer(thousandSunnyConnection));

            ConfigureSecurity(services);
            ConfigureCORS(services);

            services.AddControllers();
        }

        /// <summary>
        /// Configure authentication and authorization.
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureSecurity(IServiceCollection services)
        {
            // Configure authentication to use JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JWTUtils.GenerateTokenValidationParameters(Configuration);
            });

            // Configure authorization
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new UserIsValidRequirement())
                    .RequireAuthenticatedUser()
                    .Build();
            });
            services.AddScoped<IAuthorizationHandler, UserIsValidHandler>();
        }

        private void ConfigureCORS(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowChickenVoyageWebCORS,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:4200")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });
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

            app.UseCors(AllowChickenVoyageWebCORS);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
