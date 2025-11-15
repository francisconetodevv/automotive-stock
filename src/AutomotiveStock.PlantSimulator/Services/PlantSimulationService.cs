using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomotiveStock.Shared.Services;
using Serilog;
using AutomotiveStock.Shared.Contracts;
using AutomotiveStock.Shared.Enums;


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
                    _rabbitMQServices.PublishEvent(routingKey, consumptionEvent);

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
        }
    }
}