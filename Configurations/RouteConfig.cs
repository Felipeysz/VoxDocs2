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

            // Redirecionamento da raiz para Swagger ou Login conforme a porta
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value?.ToLower();
                if (path == "/" || path == "/index.html")
                {
                    var port = context.Request.Host.Port;
                    if (port == 5151)
                    {
                        context.Response.Redirect("/swagger/index.html");
                    }
                    else
                    {
                        context.Response.Redirect("/Login");
                    }
                    return;
                }

                await next();
            });

            // Ativa Swagger apenas em desenvolvimento
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Usuários v1");
                    c.RoutePrefix = "swagger";
                });
            }

            // Mapeamento de rotas
            app.MapControllers();

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




            //Route para o Admin Pages
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
