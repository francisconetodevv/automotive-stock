using AutomotiveStock.CentralStock.Data;
using Microsoft.EntityFrameworkCore;
using AutomotiveStock.WebDashboard.Components; // Necessário para encontrar o App.razor

var builder = WebApplication.CreateBuilder(args);

// 1. Adicionar serviços do Blazor Moderno (.NET 8)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Configuração do Banco de Dados (Mantida)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CentralStockDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configuração do Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery(); // Novo middleware necessário no .NET 8

// 3. Mapeamento Moderno (.NET 8)
// Em vez de procurar por "_Host", ele usa o componente "App" como raiz
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();