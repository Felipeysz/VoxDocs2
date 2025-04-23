using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System;

namespace VoxDocs.Configurations
{
    public static class SessionConfig
    {
        public static void AddCustomSession(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        public static void UseCustomSession(this IApplicationBuilder app)
        {
            app.UseSession();
        }
    }
}
