using AutomotiveStock.CentralStock.Data.Repositories;
using AutomotiveStock.Shared.Contracts;
using AutomotiveStock.Shared.Services;
using Serilog;
using System.Text.Json;

namespace AutomotiveStock.CentralStock.Consumers
{
    public class StockEventConsumer
    {
        private readonly RabbitMQServices _rabbitMQServices;
        private readonly IStockRepository _stockRepository; // <-- Depende do "Contrato"

        // 1. Recebe as dependências
        public StockEventConsumer(RabbitMQServices rabbitMQServices, IStockRepository stockRepository)
        {
            _rabbitMQServices = rabbitMQServices;
            _stockRepository = stockRepository;
        }

        // 2. Método principal para iniciar
        public void StartListening()
        {
            Log.Information("Iniciando consumidor RabbitMQ...");
            Log.Information("Aguardando eventos...");
            Log.Information("----------------------------------------------");

            string queueName = "queue.central.stock"; //
            string routingKeyConsumption = "consumption.*";      //
            string routingKeyReplenishment = "replenishment.*";

            // 3. Passa o método privado como o "Callback"
            _rabbitMQServices.StartConsuming(queueName, routingKeyConsumption, OnMessageReceived);
            _rabbitMQServices.BindQueue(queueName, routingKeyReplenishment);
        }

        // 4. Esta é a LÓGICA DE NEGÓCIO (o antigo 'handleConsumptionEvent')
        private void OnMessageReceived(string message)
        {
            try
            {
                

                if(message != null && message.Contains("QtyReceived")){
                    var replenishmentEvent = JsonSerializer.Deserialize<MaterialReplenishmentEvent>(message);

                    Log.Information(
                        "Evento Recebido: Planta {Plant} | Material {MaterialCode} | Qty {QtyReceived}",
                        replenishmentEvent.DestinyPlant,
                        replenishmentEvent.MaterialCode,
                        replenishmentEvent.QtyReceived
                    );

                    // TODO: Atualizar estoque no banco.
                    _stockRepository.UpdateStockFromReplenishment(replenishmentEvent);
                } else
                {
                    var consumptionEvent = JsonSerializer.Deserialize<MaterialConsumptionEvent>(message);
                    
                    if (consumptionEvent != null)
                    {
                        Log.Information(
                            "Evento Recebido: Planta {Plant} | Material {MaterialCode} | Qty {Quantity}",
                            consumptionEvent.PlantConsumption,
                            consumptionEvent.MaterialCode,
                            consumptionEvent.QtyConsumed
                        );

                        _stockRepository.UpdateStockFromConsumption(
                            consumptionEvent.MaterialCode, 
                            consumptionEvent.QtyConsumed
                        );
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                Log.Error(jsonEx, "Falha ao desserializar a mensagem: {MessageBody}", message);
            }
            catch (Exception ex)
            {
                // Se o UpdateStockFromConsumption falhar, será pego aqui
                Log.Error(ex, "Erro desconhecido ao processar evento.");
            }
        }
    }
}