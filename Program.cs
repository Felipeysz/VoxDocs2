using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VoxDocs.Configurations;
using VoxDocs.Data;
using VoxDocs.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1) Configuração de Logging ----------------
builder.Logging.ClearProviders();                          // Limpa provedores padrão
builder.Logging.AddConsole();                              // Adiciona console :contentReference[oaicite:5]{index=5}
builder.Logging.AddDebug();                                // Adiciona debug

// --- 2) Application Insights ---------------------
builder.Services.AddApplicationInsightsTelemetry(         // Coleta telemetria e exceções :contentReference[oaicite:6]{index=6}
    builder.Configuration["ApplicationInsights:ConnectionString"]);

// --- 3) Data Protection ---------------------------
var dpSection = builder.Configuration.GetSection("DataProtection");
var blobConn  = dpSection["BlobConnectionString"];
var container = dpSection["BlobContainerName"];
var blobName  = dpSection["BlobName"];

// Cria container se não existir
var blobClientContainer = new BlobContainerClient(blobConn, container);
blobClientContainer.CreateIfNotExists();

// Persiste chaves em Blob Storage :contentReference[oaicite:7]{index=7}
builder.Services.AddDataProtection()
    .SetApplicationName("VoxDocs")
    .PersistKeysToAzureBlobStorage(blobClientContainer.GetBlobClient(blobName));

// --- 4) EF Core + SQL Server com retry -------------
builder.Services.AddDbContext<VoxDocsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionBddVoxDocs"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// --- 5) JWT Authentication & Authorization --------
builder.Services.AddJwtConfiguration(builder.Configuration);

// --- 6) Swagger ------------------------------------
builder.Services.AddSwaggerConfiguration();

// --- 7) HttpClientFactory para API ----------------
builder.Services.AddHttpClient("VoxDocsApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
});

// --- 8) Session -----------------------------------
builder.Services.AddCustomSession();

// --- 9) DI dos serviços ---------------------------
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();

// --- 10) Controllers, Views e Rotas ---------------
builder.Services.AddCustomControllersWithViews();
builder.Services.AddCustomRoutingWithViews();

var app = builder.Build();

// --- Pipeline de Middlewares ----------------------
// Erro/Exceções
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

// Ordem correta: Routing → Authentication → Authorization :contentReference[oaicite:8]{index=8}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Session, Swagger e MapRoutes
app.UseCustomSession();
app.UseCustomRouting();

app.Run();
