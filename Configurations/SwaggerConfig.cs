using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace VoxDocs.Configurations
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API de Usuários",
                    Version = "v1",
                    Description = "API para gerenciar usuários no sistema."
                });

                // Definição do esquema de segurança JWT
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer", // obrigatório em minúsculo para funcionar corretamente com JWT
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Insira o token JWT no campo abaixo.\n\nExemplo: Bearer {seu_token}",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                // Registra o esquema de segurança
                c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

                // Aplica o esquema globalmente
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });
        }
    }
}
