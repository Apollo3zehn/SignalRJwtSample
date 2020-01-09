using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalRJwtSample
{
    public class Startup
    {
        public static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite()
                .AddDbContext<DatabaseContext>();

            services.AddSignalR();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        LifetimeValidator = (before, expires, token, parameters) => expires > DateTime.UtcNow,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateActor = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = Startup.SecurityKey
                    };

                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            if (!string.IsNullOrEmpty(accessToken) && context.Request.Headers["Upgrade"] == "websocket")
                                context.Token = accessToken;

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSingleton<UserService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseFileServer();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<Broadcaster>("/broadcast");
            });
        }
    }
}
