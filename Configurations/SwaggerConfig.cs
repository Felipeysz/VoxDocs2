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
                    Title       = "API de Usuários",
                    Version     = "v1",
                    Description = "API para gerenciar usuários no sistema."
                });

                // Define o esquema de segurança JWT no Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT no campo abaixo.\n\nExemplo: Bearer {seu_token}",
                    Name        = "Authorization",
                    In          = ParameterLocation.Header,
                    Type        = SecuritySchemeType.Http,    // <- Aqui mudar para Http
                    Scheme      = "bearer",                    // <- minúsculo 'bearer' obrigatório para JWT
                    BearerFormat = "JWT"
                });


                // Aplica o esquema de segurança globalmente
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference 
                            { 
                                Type = ReferenceType.SecurityScheme, 
                                Id   = "Bearer" 
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
