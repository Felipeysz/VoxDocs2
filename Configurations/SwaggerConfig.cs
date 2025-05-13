using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

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

                // Permite upload de arquivos via Swagger (IFormFile + [FromForm])
                c.OperationFilter<FileUploadOperationFilter>();
            });
        }
    }

    // Adicione esta classe para suportar upload de arquivos no Swagger
    public class FileUploadOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    {
        public void Apply(OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
        {
            var hasFileUpload = false;
            foreach (var parameter in context.MethodInfo.GetParameters())
            {
                if (parameter.ParameterType == typeof(Microsoft.AspNetCore.Http.IFormFile))
                {
                    hasFileUpload = true;
                    break;
                }
            }

            if (hasFileUpload)
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["file"] = new OpenApiSchema { Type = "string", Format = "binary" },
                                    ["areaId"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                                    ["tipoId"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                                    ["usuario"] = new OpenApiSchema { Type = "string" },
                                    ["descricao"] = new OpenApiSchema { Type = "string" }
                                },
                                Required = new HashSet<string> { "file", "areaId", "tipoId", "usuario" }
                            }
                        }
                    }
                };
            }
        }
    }
}