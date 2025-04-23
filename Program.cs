using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VoxDocs.Configurations; // configurações customizadas
using VoxDocs.Data;
using VoxDocs.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// 1) EF Core + SQL Server com retry
builder.Services.AddDbContext<VoxDocsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionBddVoxDocs"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// 2) JWT Authentication & Authorization
builder.Services.AddJwtConfiguration(builder.Configuration);

// 3) Swagger
builder.Services.AddSwaggerConfiguration();

// 4) HttpClientFactory para API
builder.Services.AddHttpClient("VoxDocsApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
});

// 5) Session
builder.Services.AddCustomSession();

// 6) Injeção de Dependência
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();


// 7) Controllers + Views e rotas customizadas
builder.Services.AddCustomControllersWithViews();
builder.Services.AddCustomRoutingWithViews();

var app = builder.Build();

// 8) Middlewares de erro e HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseDeveloperExceptionPage();

// 9) Session
app.UseCustomSession();

// 10) Routing, AuthN/Z, Swagger & MapRoutes
app.UseCustomRouting();

app.Run();
