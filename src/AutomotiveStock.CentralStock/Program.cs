using AutomotiveStock.CentralStock.Consumers;     // 1. Importar o Consumidor
using AutomotiveStock.CentralStock.Data;
using AutomotiveStock.CentralStock.Data.Repositories; // 2. Importar o Repositório
using AutomotiveStock.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/central_stock_.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

RabbitMQServices rabbitMQServices = null;

try
{
    Log.Information("--- Sistema Central de Estoque INICIADO ---");

    // --- 2. CONFIGURAR BANCO DE DADOS ---
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var connectionString = configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        Log.Fatal("A string de conexão 'DefaultConnection' não foi encontrada.");
        return; 
    }
    Log.Information("Configuração do Banco de Dados carregada.");

    var dbContextOptions = new DbContextOptionsBuilder<CentralStockDbContext>()
        .UseSqlServer(connectionString)
        .Options;

    // --- 3. CRIAR E INICIAR SERVIÇOS ---

    // 3a. Criar o Repositório
    IStockRepository stockRepository = new StockRepository(dbContextOptions);
    
    // 3b. Popular o banco (só vai rodar se estiver vazio)
    stockRepository.InitializeDatabaseSeed();
    Log.Information("Repositório e Banco de dados inicializados.");

    // 3c. Criar o Serviço de Mensageria
    rabbitMQServices = new RabbitMQServices("localhost");

    // 3d. Criar o Serviço Consumidor (injetando suas dependências)
    var consumerService = new StockEventConsumer(rabbitMQServices, stockRepository);

    // 3e. Iniciar o Consumidor (que vai rodar em background)
    consumerService.StartListening();

    // --- 4. MANTER A APLICAÇÃO VIVA ---
    Console.WriteLine("Pressione [Enter] para sair.");
    Console.ReadLine();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação do Sistema Central Falhou inesperadamente.");
}
finally
{
    Log.Information("--- Sistema Central de Estoque ENCERRANDO ---");
    rabbitMQServices?.DisposeConsumer();
    Log.CloseAndFlush();
}