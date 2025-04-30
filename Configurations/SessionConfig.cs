using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace VoxDocs.Configurations
{
    public static class SessionConfig
    {
        public static IServiceCollection AddCustomSession(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache(); // Usa memória para armazenar sessões

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2); // Tempo de vida da sessão
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }

        public static IApplicationBuilder UseCustomSession(this IApplicationBuilder app)
        {
            app.UseSession();
            return app;
        }
    }
}
