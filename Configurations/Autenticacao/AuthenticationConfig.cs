using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

namespace VoxDocs.Configurations
{
    public static class AuthenticationConfig
    {
       public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/AuthMvc/Login"; // Corrigido para apontar para seu controller real
                    options.AccessDeniedPath = "/AuthMvc/AccessDenied";
                    options.LogoutPath = "/AuthMvc/Logout";

                    // Configurações reforçadas do Cookie
                    options.Cookie.Name = "VoxDocsAuthCookie";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Obrigatório para HTTPS
                    options.Cookie.SameSite = SameSiteMode.Lax; // Ou Strict dependendo dos requisitos
                    
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                    options.SlidingExpiration = true;

                    // Configuração para APIs
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = ctx =>
                        {
                            if (ctx.Request.Path.StartsWithSegments("/api"))
                            {
                                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                return ctx.Response.WriteAsJsonAsync(new { Message = "Não autenticado." });
                            }
                            ctx.Response.Redirect(ctx.RedirectUri);
                            return Task.CompletedTask;
                        },
                        OnRedirectToAccessDenied = ctx =>
                        {
                            if (ctx.Request.Path.StartsWithSegments("/api"))
                            {
                                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                                return ctx.Response.WriteAsJsonAsync(new { Message = "Acesso negado." });
                            }
                            ctx.Response.Redirect(ctx.RedirectUri);
                            return Task.CompletedTask;
                        },
                        OnValidatePrincipal = context =>
                        {
                            // Adicione validações adicionais do principal se necessário
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}
