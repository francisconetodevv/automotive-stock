using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomotiveStock.Shared.Services;

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
            
        }
    }
}