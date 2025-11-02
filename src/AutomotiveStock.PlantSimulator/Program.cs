using AutomotiveStock.Shared.Contracts;
using AutomotiveStock.Shared.Enums;
using AutomotiveStock.Shared.Services;
using System.Threading;
using Serilog;


Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/plant_simulator_.txt", 
        rollingInterval: RollingInterval.Day, 
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

try
{
    Log.Information("--- Simulador da Planta Automotiva (Publisher) INICIADO ---");

    // 1. Connectamos ao RabbitMQ 
    var rabbitMqServices = new RabbitMQServices("localhost");

    // 2. Criamos um loop para simular o consumo
    int eventCount = 1;
    while (true)
    {
        Log.Information("\n------------------------------------------");
        Log.Information("Simulando envio do evento #{EventCount}...", eventCount);

        // 3. Criamos um evento de consumo falso, para teste
        var consumptionEvent = new MaterialConsumptionEvent
        {
            DateTimeConsumption = DateTime.UtcNow,
            PlantConsumption = "Planta A",
            MaterialCode = "ACO-1020",
            QtyConsumed = 150.5,
            ProductionOrder = "OP-2025-001",
            ConsumedReason = ConsumedReasons.Production
        };

        // 4. Definimos a rota
        var routingKey = "consumption.planta-a";

        // 5. Publicamos
        try
        {
            rabbitMqServices.PublishEvent(routingKey, consumptionEvent);

            Log.Information("Evento Publicado: {RoutingKey} | EventId: {EventId}", routingKey, consumptionEvent.EventId);
            eventCount++;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[!] Falha ao conectar/publicar");
        }

        // Aguarda 5 segundos para o próximo evento
        Thread.Sleep(5000);
    }
}catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação do Simulador falhou inesperadamente.");
}
finally
{
    // 5. ESSENCIAL: Garantir que os logs sejam gravados antes de fechar
    Log.Information("--- Simulador da Planta Automotiva (Publisher) ENCERRADO ---");
    Log.CloseAndFlush();
}