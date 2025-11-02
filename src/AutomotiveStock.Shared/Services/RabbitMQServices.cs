using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.RabbitMQ.Client;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;

namespace AutomotiveStock.Shared.Services
{
    public class RabbitMQServices
    {
        private readonly ConnectionFactory _factory;
        private readonly string _exchangeName = "stock.events";

        private IConnection _consumerConnection;
        private IModel _consumerChannel;

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

        public void StartConsuming(string queueName, string routingKey, Action<string> onMessageReceived)
        {
            // 1. Criamos um canal de conexão e canal de longa duração
            _consumerConnection = _factory.CreateConnection();
            _consumerChannel = _consumerConnection.CreateModel();

            // 2. Declaramos a Exchange (para garantir que existe)
            _consumerChannel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);

            // 3. Declaramos a fila 
            _consumerChannel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // 4. Fazemos o "Binding" da fila na Exchange com o Routing Key
            _consumerChannel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: routingKey);

            // 5. Criamos o consumidor
            var consumer = new EventingBasicConsumer(_consumerChannel);

            // 6. Implementamos o evento "Received" (O que fazer quando a msg chegar)
            consumer.Received += (model, ea) =>
            {
                try
                {
                    // 6.1. Pegamos a mensagem e convertemos de Byte para string
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // 6.2. Chamamos a função de callback que o program.cs nos passou
                    onMessageReceived(message);

                    // 6.3. Avisamos ao RabbitMQ: "Ok, recebi e processei a msg. Podemos apagar da fila"
                    _consumerChannel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Erro ao processar mensagem: {ex.Message}");
                }
            };

            // 7. Inciamos o consumo
            _consumerChannel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }
        
        // 8. Fecha a conexão e o canal do consumidor
        public void DisposeConsumer()
        {
            _consumerChannel?.Dispose();
            _consumerConnection?.Dispose();
        }
    }
}