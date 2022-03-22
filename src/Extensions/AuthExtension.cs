using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace EG.Gateway.Microservice.Extensions
{
    public static class AuthExtension
    {
        private static IConfiguration Configuration;

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            
            using(var sp = services.BuildServiceProvider())
            {
                Configuration = sp.GetRequiredService<IConfiguration>();
            }

            services.AddAuthentication()
                .AddJwtBearer("SECURITY-TOKEN", options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            return services;
        }

    }
}
