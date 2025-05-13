using Microsoft.AspNetCore.Authentication.Cookies;

namespace VoxDocs.Configurations
{
    public static class AuthenticationConfig
    {
        public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    // Redireciona para nossa action de erro se não autenticado
                    options.LoginPath = "/LoginNotFound";
                    options.AccessDeniedPath = "/LoginNotFound";

                    options.Cookie.Name = "VoxDocsAuthCookie";
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                    options.SlidingExpiration = true;

                    // Para chamadas API, retornar JSON em vez de redirecionar
                    options.Events.OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return ctx.Response.WriteAsJsonAsync(new { Message = "Não autenticado." });
                        }
                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToAccessDenied = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return ctx.Response.WriteAsJsonAsync(new { Message = "Acesso negado." });
                        }
                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    };
                });

            return services;
        }
    }
}
