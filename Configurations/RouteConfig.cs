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
                    options.ViewLocationFormats.Add("/Views/Pages/AuthPage/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Pages/DocumentosPage/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Pages/AccountPage/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Pages/Components/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Error/{0}.cshtml");
                });
        }

        public static void UseCustomRouting(this WebApplication app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Rotas MVC específicas
            app.MapControllerRoute(
                name: "default",
                pattern: "Index",
                defaults: new { controller = "IndexMvc", action = "Index" });

            app.MapControllerRoute(
                name: "Login",
                pattern: "Login",
                defaults: new { controller = "LoginMvc", action = "Login" });

            app.MapControllerRoute(
                name: "MeuPerfil",
                pattern: "MeuPerfil",
                defaults: new { controller = "UserInfoMvc", action = "MeuPerfil" });

            app.MapControllerRoute(
                name: "Documentos",
                pattern: "Documentos",
                defaults: new { controller = "DocumentosMvc", action = "Documentos" });

            app.MapControllerRoute(
                name: "UploadDocumento",
                pattern: "UploadDocumento",
                defaults: new { controller = "UploadDocumentoMvc", action = "Upload" });

            app.MapControllerRoute(
                name: "LinkRedefinirSenha",
                pattern: "LinkRedefinirSenha",
                defaults: new { controller = "LoginMvc", action = "LinkRedefinirSenha" });

            app.MapControllerRoute(
                name: "RecuperarSenha",
                pattern: "RecuperarSenha",
                defaults: new { controller = "LoginMvc", action = "RecuperarSenha" });

            app.MapControllerRoute(
                name: "RecuperarEmail",
                pattern: "RecuperarEmail",
                defaults: new { controller = "LoginMvc", action = "RecuperarEmail" });


            // Rotas de erro
            app.MapControllerRoute(
                name: "LoginNotFound",
                pattern: "LoginNotFound",
                defaults: new { controller = "ErrorMvc", action = "LoginNotFound" });

            app.MapControllerRoute(
                name: "NotFoundPage",
                pattern: "NotFoundPage",
                defaults: new { controller = "ErrorMvc", action = "NotFoundPage" });

            app.MapControllerRoute(
                name: "Navbar",
                pattern: "NavbarMvc/{action=Logout}",
                defaults: new { controller = "NavbarMvc", action = "Logout" });

            app.MapControllerRoute(
                name: "ErrorTokenInvalido",
                pattern: "ErrorTokenInvalido",
                defaults: new { controller = "ErrorMvc", action = "ErrorTokenInvalido" });

            // Intercepta 404 e redireciona internamente
            app.UseStatusCodePagesWithReExecute("/NotFoundPage");
        }
    }
}