using AutomotiveStock.Shared.Contracts;
using AutomotiveStock.Shared.Services;
using System.Text.Json;
using Serilog;

// Funcionalidade para geração de Logs
Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/central_stock_.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

try
{
    Log.Information("--- Sistema Central de Estoque (Consumer) ---");
    Log.Information("Aguardando eventos...");
    Log.Information("----------------------------------------------");

    // 1. Instanciamos nosso serviços
    var rabbitMQServices = new RabbitMQServices("localhost");

    // 2. Defininmos a função de callback
    // Obs.: Lógica executada a cada mensagem recebida
    Action<string> handleConsumptionEvent = (message) =>
    {
        try
        {
            // 2.1. Desserializamos a string json de volta para o nosso objeto em C#
            var consumptionEvent = JsonSerializer.Deserialize<MaterialConsumptionEvent>(message);

            if (consumptionEvent != null)
            {
                // 2.1.1. [MVP] Apenas imprimimos os dados
                // Obs.: No futuro, aqui é onde chamaremos para atualizar o estoque
                Log.Information(
                    "Evento Recebido: Planta {Plant} | Material {MaterialCode} | Qty {Quantity} | Ordem {Order}",
                    consumptionEvent.PlantConsumption,
                    consumptionEvent.MaterialCode,
                    consumptionEvent.QtyConsumed,
                    consumptionEvent.ProductionOrder
                );
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

    // 3. Definimos os parâmetros da seção de documentos
    string queueName = "queue.central.stock";
    string routingKey = "consumption.*";

    // 4. Iniciamos o "Ouvinte"
    rabbitMQServices.StartConsuming(queueName, routingKey, handleConsumptionEvent);

    // 5. Mantemos o Console app rodando
    Console.WriteLine("Pressione [Enter] para sair.");
    Console.ReadLine();

    // 6. Limpamos a conexão ao sair
    rabbitMQServices.DisposeConsumer();
    Console.WriteLine("Encerrando consumidor");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação do Sistema Central Falhou inesperadamente.");
} finally
{
    Log.Information("--- Sistema Central de Estoque (Consumer) ENCERRADO ---");
    Log.CloseAndFlush();
}