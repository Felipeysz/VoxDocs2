using Microsoft.EntityFrameworkCore;
using VoxDocs.Configurations;
using VoxDocs.Data;
using VoxDocs.Services;
using VoxDocs.BusinessRules;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --- Limpa o cookie de autenticação VoxDocsAuthCookie no modo Development ---
if (builder.Environment.IsDevelopment())
{
    // Remove a pasta de chaves de Data Protection para invalidar todos os cookies de autenticação
    var keysFolder = Path.Combine(AppContext.BaseDirectory, "DataProtection-Keys");
    if (Directory.Exists(keysFolder))
    {
        Directory.Delete(keysFolder, true);
    }
}

// --- Logging & Application Insights ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole().AddDebug();
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// --- DataProtection com fallback ---
builder.Services.AddCustomDataProtection(builder.Configuration);

// --- EF Core SQL Server ---
builder.Services.AddDbContext<VoxDocsContext>(opts =>
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("ConnectionBddVoxDocs"),
        sql => sql.EnableRetryOnFailure()
    )
);

// --- Autenticação via Cookie (usando nossa extensão) ---
builder.Services.AddAuthenticationConfig(builder.Configuration);

// --- Políticas de autorização ---
builder.Services.AddAuthorization(options =>
{
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

// --- Serviços ---
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<ITipoDocumentoService, TipoDocumentoService>();
builder.Services.AddScoped<IAreasDocumentoService, AreasDocumentoService>();
builder.Services.AddScoped<IDocumentoUploadService, DocumentoUploadService>();

// --- Azure Blob Storage Service ---
builder.Services.AddScoped<AzureBlobService>();

// Regra de Negocios
builder.Services.AddScoped<UserBusinessRules>();

// --- Controllers + Views + Localizações customizadas + Session ---
builder.Services.AddCustomControllersWithViews();      // caso contenha outras configs MVC
builder.Services.AddCustomRoutingWithViews();          // configura localização das Views
builder.Services.AddCustomSession();

var app = builder.Build();

// --- Pipeline padrão ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// --- Swagger UI ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VoxDocs API v1");
    c.RoutePrefix = "swagger";
});

// --- Rotas customizadas MVC + Erros + API (se tiver) ---
app.UseCustomRouting();

app.Run();