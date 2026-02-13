using System.Text.Json;
using Confluent.Kafka;
using DistributedSystemAPI.Models.NotificationModels;
using DistributedSystemAPI.Interfaces;

namespace DistributedSystemAPI.Services
{
    public class KafkaNotificationProducer : INotificationProducer
    {
        private readonly IProducer<Null, string> _producer;
        private const string Topic = "notifications";

        public KafkaNotificationProducer(IConfiguration config)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig)
                .Build();
        }

        public async Task ProduceAsync(NotificationEvent notification)
        {
            var message = JsonSerializer.Serialize(notification);

            await _producer.ProduceAsync(Topic, new Message<Null, string>
            {
                Value = message
            });
        }
    }
}
