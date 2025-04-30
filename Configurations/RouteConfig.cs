using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace VoxDocs.Configurations
{
    public static class RouteConfiguration
    {
        public static void AddCustomRoutingWithViews(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Clear();
                    options.ViewLocationFormats.Add("/Views/Pages/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Pages/PrivatePages/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
                });
        }

        public static void UseCustomRouting(this WebApplication app)
        {
            app.UseRouting();

            // Se suas controllers MVC usam [Authorize], já foi chamado UseAuthentication/UseAuthorization

            // Mapeia rotas da API
            app.MapControllerRoute(
                name: "api",
                pattern: "api/{controller}/{action=Index}/{id?}");

            // Páginas MVC
            app.MapControllerRoute(
                name: "Login",
                pattern: "Login",
                defaults: new { controller = "LoginMvc", action = "Login" });

            app.MapControllerRoute(
                name: "Buscar",
                pattern: "Buscar",
                defaults: new { controller = "BuscarMvc", action = "Buscar" });

            // Raiz redireciona ao login
            app.MapGet("/", ctx =>
            {
                ctx.Response.Redirect("/Login");
                return Task.CompletedTask;
            });
        }
    }
}
