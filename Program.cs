using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Configurations;
using VoxDocs.Data;
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

// --- Serviços ---
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<IPastaPrincipalService, PastaPrincipalService>();
builder.Services.AddScoped<ISubPastaService, SubPastaService>();
builder.Services.AddScoped<IEmpresasContratanteService, EmpresasContratanteService>();
builder.Services.AddScoped<IPlanosVoxDocsService, PlanosVoxDocsService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();

// --- Repositorios ---
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();

// --- BusinessRules ---
builder.Services.AddScoped<IPagamentoBusinessRules, PagamentoBusinessRules>();

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
