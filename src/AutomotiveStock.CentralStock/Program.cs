using AutomotiveStock.Shared.Contracts;
using AutomotiveStock.Shared.Services;
using System.Text.Json;

Console.WriteLine("--- Sistema Central de Estoque (Consumer) ---");
Console.WriteLine("Aguardando eventos...");
Console.WriteLine("----------------------------------------------");

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
            Console.WriteLine($"[+] Evento recebido (Planta: {consumptionEvent.PlantConsumption})");
            Console.WriteLine($"Material: {consumptionEvent.MaterialCode}");
            Console.WriteLine($"Quantidade: {consumptionEvent.QtyConsumed}");
            Console.WriteLine($"Ordem: {consumptionEvent.ProductionOrder}");
            Console.WriteLine($"Motivo: {consumptionEvent.ConsumedReason}");
            Console.WriteLine("------------------------------------------------------------------");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[!] Erro ao desserializar/processar: {ex.Message}");
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