using Confluent.Kafka;
using CoreBanking.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreBanking.Infrastructure.Messaging
{
    public class KafkaProducer : IEventProducer
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(IConfiguration config)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]

            };
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            var payload = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = payload
            });
        }
    }
}
