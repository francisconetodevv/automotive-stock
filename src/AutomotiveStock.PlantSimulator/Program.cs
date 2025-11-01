using AutomotiveStock.Shared.Contracts;
using AutomotiveStock.Shared.Enums;
using AutomotiveStock.Shared.Services;
using System.Threading;

Console.WriteLine("-- Simulador da Planta Automotiva (Publisher)");

// 1. Connectamos ao RabbitMQ 
var rabbitMqServices = new RabbitMQServices("localhost");

// 2. Criamos um loop para simular o consumo
int eventCount = 1;
while (true)
{
    Console.WriteLine("\n------------------------------------------");
    Console.WriteLine($"Simulando envio de eventos #{eventCount}...");

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
    } catch (Exception ex)
    {
        Console.WriteLine($" [!] Falha ao conectar/publicar: {ex.Message}");
        Console.WriteLine("    Verifique se o RabbitMQ está em execução.");
    }

    // Aguarda 5 segundos para o próximo evento
    Thread.Sleep(5000);
}