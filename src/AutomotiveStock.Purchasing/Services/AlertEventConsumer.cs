using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutomotiveStock.Shared.Contracts;
using AutomotiveStock.Shared.Services;
using Serilog;

namespace AutomotiveStock.Purchasing.Services
{
    public class AlertEventConsumer
    {
        private readonly RabbitMQServices _rabbitMQServices;
        public AlertEventConsumer(RabbitMQServices rabbitMQServices)
        {
            _rabbitMQServices = rabbitMQServices;
        }

        public void StartListening()
        {
            Log.Information("Iniciando consumidor...");
            Log.Information("----------------------------------------------");

            string queue = "queue.purchasing.alerts";
            string routingKey = "alert.lowstock"; // Verificar

            _rabbitMQServices.StartConsuming(queue, routingKey, OnMessageReceived);
        }

        private void OnMessageReceived(string message)
        {
            try
            {
                var consumptionEvent = JsonSerializer.Deserialize<LowStockAlertEvent>(message);

                if (consumptionEvent != null)
                {
                    Log.Information(
                        "ORDEM DE COMPRA GERADA: Material {MaterialCode} | Estoque Atual: {CurrentStock} kg",
                        consumptionEvent.MaterialCode,
                        consumptionEvent.CurrentStock
                    );
                }
            } catch (Exception ex)
            {
                Log.Error(ex, "Falha ao processar alerta de estoque baixo");
            }
        }
    }
}