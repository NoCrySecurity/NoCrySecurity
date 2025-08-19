#nullable enable
using Microsoft.EntityFrameworkCore;
using SecureGuard.Api.Data;
using SecureGuard.Api.Services;
using SecureGuard.Api.Models; // 👈 Importa os monitores

var builder = WebApplication.CreateBuilder(args);

var isDev = builder.Environment.IsDevelopment();
var skipDb = (Environment.GetEnvironmentVariable("SKIP_DB") ?? "false")
             .Equals("true", StringComparison.OrdinalIgnoreCase);

// MVC (site)
builder.Services.AddControllersWithViews();

// Opcional: Swagger só para testes de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB (opcional)
if (skipDb)
    builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("SecureGuardDev"));
else
    builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// 💡 Serviços de monitoramento

builder.Services.AddHostedService<FileMonitorService>();
builder.Services.AddHostedService<ProcessMonitorService>();

var app = builder.Build();

if (isDev)
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    // app.UseHttpsRedirection(); // pode habilitar depois
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Rota padrão do SITE: / -> Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.Run();
