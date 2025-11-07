using AutomotiveStock.Shared.Contracts;
using AutomotiveStock.Shared.Services;
using System.Text.Json;
using Serilog;
using Microsoft.Extensions.Configuration;
using AutomotiveStock.CentralStock.Data; // <--- MUDANÇA: Importar nosso DbContext
using Microsoft.EntityFrameworkCore;    // <--- MUDANÇA: Importar EF Core

// 1. Configurar Logger (Está perfeito)
Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/central_stock_.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

RabbitMQServices rabbitMQServices = null; // <--- MUDANÇA: Declarar fora para o 'finally' ter acesso

try
{
    Log.Information("--- Sistema Central de Estoque INICIADO ---");

    // 2. Configurar o Banco de Dados (DEVE VIR PRIMEIRO)
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory) // Garante que ele ache o appsettings.json
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var connectionString = configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        Log.Fatal("A string de conexão 'DefaultConnection' não foi encontrada no appsettings.json");
        return; // Encerra a aplicação se não houver banco
    }

    Log.Information("Configuração do Banco de Dados carregada com sucesso.");

    // <--- MUDANÇA: Guardar as opções do DbContext para usar no futuro
    // Precisamos disso para que o callback do RabbitMQ possa criar instâncias do DbContext
    var dbContextOptions = new DbContextOptionsBuilder<CentralStockDbContext>()
        .UseSqlServer(connectionString)
        .Options;


    // 3. Configurar o Consumidor RabbitMQ
    Log.Information("Iniciando consumidor RabbitMQ...");
    Log.Information("Aguardando eventos...");
    Log.Information("----------------------------------------------");

    rabbitMQServices = new RabbitMQServices("localhost"); // <--- MUDANÇA: Inicializar a variável

    Action<string> handleConsumptionEvent = (message) =>
    {
        try
        {
            var consumptionEvent = JsonSerializer.Deserialize<MaterialConsumptionEvent>(message);

            if (consumptionEvent != null)
            {
                Log.Information(
                    "Evento Recebido: Planta {Plant} | Material {MaterialCode} | Qty {Quantity} | Ordem {Order}",
                    consumptionEvent.PlantConsumption,
                    consumptionEvent.MaterialCode,
                    consumptionEvent.QtyConsumed,
                    consumptionEvent.ProductionOrder
                );
                
                // <--- MUDANÇA: LÓGICA FUTURA
                // No próximo passo, é AQUI que vamos usar o 'dbContextOptions' para
                // criar um 'using (var db = new CentralStockDbContext(dbContextOptions))'
                // e atualizar o banco de dados.
            }
        }
        catch (JsonException jsonEx)
        {
            Log.Error(jsonEx, "Falha ao desserializar a mensagem: {MessageBody}", message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro desconhecido ao processar evento.");
        }
    };

    string queueName = "queue.central.stock";
    string routingKey = "consumption.*";

    // 4. Iniciamos o "Ouvinte"
    rabbitMQServices.StartConsuming(queueName, routingKey, handleConsumptionEvent);

    // 5. Manter o Console app rodando (APENAS UM READLINE NO FINAL)
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

    // 6. Limpar (Dispose) na ordem correta
    rabbitMQServices?.DisposeConsumer(); // O '?' checa se é nulo antes de chamar
    Log.CloseAndFlush();
}