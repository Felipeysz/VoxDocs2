using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace VoxDocs.Configurations
{
    public static class JWTConfig
    {
        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidIssuer              = configuration["Jwt:Issuer"],
                    ValidateAudience         = true,
                    ValidAudience            = configuration["Jwt:Audience"],
                    ValidateLifetime         = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ClockSkew                = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync(new
                        {
                            StatusCode = 401,
                            Message = "Token inválido ou expirado."
                        }.ToString()); // Você pode usar JSON serialization real aqui
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse(); // impede a resposta padrão
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync(new
                        {
                            StatusCode = 401,
                            Message = "Token ausente ou não autorizado."
                        }.ToString()); // idem acima
                    }
                };
            });
            services.AddAuthorization();
        }
    }
}
