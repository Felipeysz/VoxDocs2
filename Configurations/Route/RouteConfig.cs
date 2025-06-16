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
                    //Pages
                    options.ViewLocationFormats.Add("/Views/Pages/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Pages/Perfil/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Pages/Documentos/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Pages/Upload/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Pages/AuthPage/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Pages/AuthPage/RecuperarContas/{0}.cshtml");

                    options.ViewLocationFormats.Add("/Views/Pages/PagamentosPages/{0}.cshtml");


                    //PartialViews/Components/ErrosPages
                    options.ViewLocationFormats.Add("/Views/Shared/ErrosPage/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Shared/Components/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Shared/Components/SuporteIA/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Shared/Components/Navbar/{0}.cshtml");

                });
        }

        public static void UseCustomRouting(this WebApplication app)
        {
            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

            // Rotas MVC Index/Padrão
            app.MapControllerRoute(
            name: "/",
            pattern: "/",
            defaults: new { controller = "IndexMvc", action = "Index" });
            app.MapControllerRoute(
            name: "index",
            pattern: "index",
            defaults: new { controller = "IndexMvc", action = "Index" });



            // --- Auth Pages ---
            app.MapControllerRoute(
                name: "Login",
                pattern: "Login",
                defaults: new { controller = "AuthMvc", action = "Login" });

            app.MapControllerRoute(
                name: "ForgotPassword",
                pattern: "ForgotPassword",
                defaults: new { controller = "AuthMvc", action = "ForgotPassword" });

            app.MapControllerRoute(
                name: "ForgotPasswordConfirmation",
                pattern: "ForgotPasswordConfirmation",
                defaults: new { controller = "AuthMvc", action = "ForgotPasswordConfirmation" });

            app.MapControllerRoute(
                name: "ResetPassword",
                pattern: "ResetPassword",
                defaults: new { controller = "AuthMvc", action = "ResetPassword" });

            app.MapControllerRoute(
                name: "Logout",
                pattern: "Logout",
                defaults: new { controller = "AuthMvc", action = "Logout" });



            // --- Pagamento ---
            app.MapControllerRoute(
                name: "PlanoPagamento",
                pattern: "PlanoPagamento",
                defaults: new { controller = "PagamentosMvc", action = "PlanoPagamento" });

            app.MapControllerRoute(
                name: "CriarCadastroPagamento",
                pattern: "CriarCadastroPagamento",
                defaults: new { controller = "PagamentosMvc", action = "CriarCadastroPagamento" });

            app.MapControllerRoute(
                name: "ErrorTokenPlano",
                pattern: "ErrorTokenPlano",
                defaults: new { controller = "PagamentosMvc", action = "ErrorTokenPlano" });




            //--- Perfil Conta ---
            app.MapControllerRoute(
                name: "MeuPerfil",
                pattern: "MeuPerfil",
                defaults: new { controller = "PerfilMvc", action = "MeuPerfil" });



            // --- Documentos ---
            app.MapControllerRoute(
                name: "Documentos",
                pattern: "Documentos",
                defaults: new { controller = "DocumentosMvc", action = "Documentos" });
            
            app.MapControllerRoute(
                name: "Upload",
                pattern: "Upload",
                defaults: new { controller = "UploadMvc", action = "Upload" });



            //Rotas Administrativas
            app.MapControllerRoute(
                name: "Dashboard",
                pattern: "Dashboard",
                defaults: new { controller = "AdminMvc", action = "Dashboard" });



            //--- Rotas de erro ---
            app.MapControllerRoute(
                name: "LoginNotFound",
                pattern: "LoginNotFound",
                defaults: new { controller = "ErrorMvc", action = "LoginNotFound" });

            app.MapControllerRoute(
                name: "SemTokenConfirmandoPagamentoPix",
                pattern: "SemTokenConfirmandoPagamentoPix",
                defaults: new { controller = "ErrorMvc", action = "SemTokenConfirmandoPagamentoPix" });

            app.MapControllerRoute(
                name: "NotFoundPage",
                pattern: "NotFoundPage",
                defaults: new { controller = "ErrorMvc", action = "NotFoundPage" });

            app.MapControllerRoute(
                name: "ErrorTokenInvalido",
                pattern: "ErrorTokenInvalido",
                defaults: new { controller = "ErrorMvc", action = "ErrorTokenInvalido" });

            app.MapControllerRoute(
                name: "ErrorOfflinePage",
                pattern: "ErrorOfflinePage",
                defaults: new { controller = "ErrorMvc", action = "ErrorOfflinePage" });


            // Intercepta 404 e redireciona internamente
            app.UseStatusCodePagesWithReExecute("/NotFoundPage");
        }
    }
}