using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Configurations;
using VoxDocs.Data;
using VoxDocs.Services;
using VoxDocs.BusinessRules;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- Logging, AppInsights & DataProtection
builder.Logging.ClearProviders();
builder.Logging.AddConsole().AddDebug();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:ConnectionString"]);
builder.Services.AddCustomDataProtection(builder.Configuration);

// --- EF Core
builder.Services.AddDbContext<VoxDocsContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionBddVoxDocs"),
        sql => sql.EnableRetryOnFailure()));

// --- Autenticação (Cookies + JWT) - Configuração Centralizada
builder.Services.AddAuthenticationConfig(builder.Configuration);

// --- Autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser());

    options.AddPolicy("PagePolicy", policy =>
        policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser());

    // Política que suporta ambos os esquemas (Cookies e JWT)
    options.AddPolicy("MultiAuthPolicy", policy =>
        policy.AddAuthenticationSchemes(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            JwtBearerDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser());
});

// --- Swagger
builder.Services.AddSwaggerConfiguration();

// --- HttpClient
builder.Services.AddHttpClient("VoxDocsApi", c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]));

// --- DI de Serviços
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserBusinessRules>();
builder.Services.AddScoped<TokenService>();

// --- MVC + Views + Rotas
builder.Services.AddCustomControllersWithViews();
builder.Services.AddCustomRoutingWithViews();

builder.Services.AddCustomSession();

var app = builder.Build();

// Pipeline de Middlewares
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

// --- Middleware de autenticação/autorizações
app.UseAuthentication();
app.UseAuthorization();

// --- Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VoxDocs API v1");
    c.RoutePrefix = "swagger";
});

// --- Rotas
app.UseCustomRouting();

app.Run();
