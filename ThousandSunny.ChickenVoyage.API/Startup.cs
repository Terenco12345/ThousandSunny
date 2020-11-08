using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ThousandSunny.ChickenVoyage.API.Models;
using ThousandSunny.ChickenVoyage.API.Security.Handlers;
using ThousandSunny.ChickenVoyage.API.Security.Requirements;

namespace ThousandSunny.ChickenVoyage.API
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
            var thousandSunnyConnection = Configuration["ConnectionStrings:TSPingContext"];
            services.AddDbContext<TSChickenVoyageContext>(opt => opt.UseSqlServer(thousandSunnyConnection));

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
                    .AddRequirements(new DefaultAuthRequirement())
                    .RequireAuthenticatedUser()
                    .Build();
            });
            services.AddSingleton<IAuthorizationHandler, DefaultAuthHandler>();
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