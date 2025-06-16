using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Business.Rules;
using VoxDocs.BusinessRules;
using VoxDocs.Configurations;
using VoxDocs.Data;
using VoxDocs.Data.Repositories;
using VoxDocs.Interfaces;
using VoxDocs.Repositories;
using VoxDocs.Repository;
using VoxDocs.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Logging & Application Insights ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole().AddDebug();
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// --- DataProtection em memória (não grava chaves em disco) ---
builder.Services
    .AddDataProtection()
    .UseEphemeralDataProtectionProvider()  // chaves apenas em memória
    .SetApplicationName("VoxDocs");

// --- BlobService Client ---
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var conn = config["AzureBlobStorage:ConnectionString"];
    if (string.IsNullOrEmpty(conn))
        throw new InvalidOperationException("AzureBlobStorage:ConnectionString faltando!");
    return new BlobServiceClient(conn);
});

// --- EF Core SQL Server ---
builder.Services.AddDbContext<VoxDocsContext>(opts =>
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("ConnectionBddVoxDocs"),
        sql => sql.EnableRetryOnFailure()
    )
);

// --- Políticas de autorização ---
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("PermissionAccount", "admin"));
    options.AddPolicy("PagePolicy", policy =>
        policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser());
    options.AddPolicy("ApiPolicy", policy =>
        policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser());
});

// --- Swagger & HttpClient ---
builder.Services.AddSwaggerConfiguration();
builder.Services.AddHttpClient("VoxDocsApi", c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? string.Empty)
);

// --- Azure Blob Storage Service ---
builder.Services.AddScoped<AzureBlobService>();

// --- Repositorios ---

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPlanosVoxDocsRepository, PlanosVoxDocsRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IEmpresasContratanteRepository, EmpresasContratanteRepository>();
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();
builder.Services.AddScoped<IPastaPrincipalRepository,PastaPrincipalRepository>();
builder.Services.AddScoped<ISubPastaRepository, SubPastaRepository>();
builder.Services.AddScoped<IConfiguracaoDocumentoRepository,ConfiguracaoDocumentoRepository>();

// --- BusinessRules ---

builder.Services.AddScoped<IUserBusinessRules, UserBusinessRules>();
builder.Services.AddScoped<IPlanosVoxDocsBusinessRules, PlanosVoxDocsBusinessRules>();
builder.Services.AddScoped<IPagamentoBusinessRules, PagamentoBusinessRules>();
builder.Services.AddScoped<ILogBusinessRules, LogBusinessRules>();
builder.Services.AddScoped<IEmpresasContratanteBusinessRules, EmpresasContratanteBusinessRules>();
builder.Services.AddScoped<IDocumentoBusinessRules, DocumentoBusinessRules>();
builder.Services.AddScoped<IDocumentoOfflineBusinessRules, DocumentoOfflineBusinessRules>();
builder.Services.AddScoped<IPastaPrincipalBusinessRules, PastaPrincipalBusinessRules>();
builder.Services.AddScoped<ISubPastaBusinessRules, SubPastaBusinessRules>();
builder.Services.AddScoped<IAdminStatisticsBusinessRules, AdminStatisticsBusinessRules>();
builder.Services.AddScoped<IConfiguracaoDocumentoBusinessRules, ConfiguracaoDocumentoBusinessRules>();

// --- Serviços ---

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlanosVoxDocsService, PlanosVoxDocsService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IEmpresasContratanteService , EmpresasContratanteService>();
builder.Services.AddScoped<IDocumentosPastasService, DocumentosPastasService>();
builder.Services.AddScoped<IDocumentosOfflineService, DocumentosOfflineService>();
builder.Services.AddScoped<IConfiguracaoDocumentoService,ConfiguracaoDocumentoService>();
builder.Services.AddScoped<IAdminStatisticsService,AdminStatisticsService>();


// --- MVC, Views e Session customizados ---
builder.Services.AddControllersWithViews();
builder.Services.AddAuthenticationConfig(builder.Configuration);
builder.Services.AddCustomSession();
builder.Services.AddCustomControllersWithViews();
builder.Services.AddCustomRoutingWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Usuários V1");
    c.RoutePrefix = "swagger";
});

app.UseCustomRouting();

app.MapDefaultControllerRoute();

app.Run();
