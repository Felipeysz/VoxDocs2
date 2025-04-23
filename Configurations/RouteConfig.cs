using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace VoxDocs.Configurations
{
    public static class RouteConfiguration
    {
        // Método para configurar Controllers e Razor Options
        public static void AddCustomRoutingWithViews(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Clear();
                    options.ViewLocationFormats.Add("/Views/Pages/{0}.cshtml"); // Continua suportando /Views/Pages
                    options.ViewLocationFormats.Add("/Views/Pages/PrivatePages/{0}.cshtml"); // Agora também suporta /Views/PrivatePages
                    options.ViewLocationFormats.Add("/Views/Pages/AdminPages/{0}.cshtml"); // Agora também suporta /Views/PrivatePages
                    options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml"); // Agora também suporta /Views/Shared
                    options.ViewLocationFormats.Add("/Views/NavBar/{0}.cshtml"); // Agora também suporta /Views/Shared
                });
        }

        // Método para configurar o uso das rotas
        public static void UseCustomRouting(this WebApplication app)
        {
            app.UseRouting();

            // Autenticação e Autorização
            app.UseAuthentication();
            app.UseAuthorization();

            // Ativa Swagger em qualquer ambiente
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Usuários v1");
                c.RoutePrefix = "swagger"; // Mantém o Swagger disponível em /swagger
            });


            // Mapeamento de rotas

            app.MapControllerRoute(
                name: "api",
                pattern: "api/{controller}/{action=Index}/{id?}");

            // Depois mapear as rotas específicas das páginas MVC
            app.MapControllerRoute(
                name: "Login",
                pattern: "Login",
                defaults: new { controller = "LoginMvc", action = "Login" });

            app.MapControllerRoute(
                name: "Navbar",
                pattern: "NavBar",
                defaults: new { controller = "NavbarMvc", action = "NavBar" });

            app.MapControllerRoute(
                name: "Buscar",
                pattern: "Buscar",
                defaults: new { controller = "BuscarMvc", action = "Buscar" });

            // Rotas Admin Pages
            app.MapControllerRoute(
                name: "Dashboard",
                pattern: "Dashboard",
                defaults: new { controller = "DashboardMvc", action = "Dashboard" });

            app.MapControllerRoute(
                name: "Tokens",
                pattern: "Tokens",
                defaults: new { controller = "TokensMvc", action = "Tokens" });

            app.MapControllerRoute(
                name: "Upload",
                pattern: "Upload",
                defaults: new { controller = "UploadMvc", action = "Upload" });

            app.MapControllerRoute(
                name: "UsersAdmin",
                pattern: "UsersAdmin",
                defaults: new { controller = "UsersAdminMvc", action = "UsersAdmin" });

        }
    }
}
