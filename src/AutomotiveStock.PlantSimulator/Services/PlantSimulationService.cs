using AutomotiveStock.Shared.Services;
using Serilog;
using AutomotiveStock.Shared.Contracts;
using AutomotiveStock.Shared.Enums;
using System.Threading; // <-- Adicionei este 'using'

namespace AutomotiveStock.PlantSimulator.Services
{
    public class PlantSimulationService
    {
        private readonly RabbitMQServices _rabbitMQServices;

        public PlantSimulationService(RabbitMQServices rabbitMQServices)
        {
            _rabbitMQServices = rabbitMQServices;
        }

        public void RunSimulation()
        {
            int eventCount = 1;
            while (true)
            {
                Log.Information("\n------------------------------------------");
                Log.Information("Simulando envio do evento #{EventCount}...", eventCount);

                // A lógica de 'try/catch' e 'routingKey' foi movida para DENTRO
                // dos blocos 'if' e 'else'
                
                if (eventCount % 10 == 0)
                {
                    // --- BLOCO DE REABASTECIMENTO [RF03] ---

                    // 1. Criamos o evento de Reabastecimento
                    var replenishmentEvent = new MaterialReplenishmentEvent
                    {
                        EventId = Guid.NewGuid(), // Boa prática adicionar ID
                        DateTimeEntry = DateTime.UtcNow,
                        DestinyPlant = "Planta A",
                        QtyReceived = 5000,
                        MaterialCode = "ACO-1020"
                    };

                    // 2. Definimos o Routing Key CORRETO
                    var routingKey = "replenishment.planta-a";

                    // 3. Publicamos
                    try
                    {
                        _rabbitMQServices.PublishEvent(routingKey, replenishmentEvent);
                        Log.Information("Evento de REABASTECIMENTO Publicado: {RoutingKey} | EventId: {EventId}", 
                            routingKey, 
                            replenishmentEvent.EventId);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "[!] Falha ao conectar/publicar REABASTECIMENTO");
                    }
                }
                else
                {
                    // --- BLOCO DE CONSUMO [RF01] ---

                    // 1. Criamos o evento de Consumo
                    var consumptionEvent = new MaterialConsumptionEvent
                    {
                        DateTimeConsumption = DateTime.UtcNow,
                        PlantConsumption = "Planta A",
                        MaterialCode = "ACO-1020",
                        QtyConsumed = 150.5,
                        ProductionOrder = "OP-2025-001",
                        ConsumedReason = ConsumedReasons.Production
                    };

                    // 2. Definimos o Routing Key CORRETO
                    var routingKey = "consumption.planta-a";

                    // 3. Publicamos
                    try
                    {
                        _rabbitMQServices.PublishEvent(routingKey, consumptionEvent);
                        Log.Information("Evento de CONSUMO Publicado: {RoutingKey} | EventId: {EventId}", 
                            routingKey, 
                            consumptionEvent.EventId);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "[!] Falha ao conectar/publicar CONSUMO");
                    }
                }

                // Incrementamos o contador e esperamos
                eventCount++;
                Thread.Sleep(5000); // 5 segundos
            }
        }
    }
}