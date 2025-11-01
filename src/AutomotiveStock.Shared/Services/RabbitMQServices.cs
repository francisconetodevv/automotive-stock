using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.RabbitMQ.Client;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace AutomotiveStock.Shared.Services
{
    public class RabbitMQServices
    {
        private readonly ConnectionFactory _factory;
        private readonly string _exchangeName = "stock.events";

        public RabbitMQServices(string hostname)
        {
            _factory = new ConnectionFactory()
            {
                HostName = hostname
            };
        }
        
        public void PublishEvent(string routingKey, object payload)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // 1. Declaração da Exchange
                channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);

                // 2. Serializamos o evento para estilo do JSON
                var message = JsonSerializer.Serialize(payload);
                var body = Encoding.UTF8.GetBytes(message);

                // 3. Configuramos as propriedades da mensagem
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                // 4. Publicamos a mensagem
                channel.BasicPublish(exchange: _exchangeName, routingKey: routingKey, basicProperties: properties, body: body);

                Console.WriteLine($"[x] Event Publish: '{routingKey}'");
                Console.WriteLine($"Message: {message}");
            }
        }
    }
}