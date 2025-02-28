using Confluent.Kafka;
using GenericMessageQueueSample.Interfaces;
using System.Text.Json;

namespace GenericMessageQueueSample.Providers
{
    public class KafkaProvider : IMessageQueueProvider
    {
        private readonly IProducer<Null, string> _producer;
        private readonly IConsumer<Null, string> _consumer;
        private const string Topic = "kafka-topic";

        public KafkaProvider()
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9000" // 'kafka:9092' yerine 'localhost:9092' kullan
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9000",
                GroupId = "kafka-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };


            _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            _consumer.Subscribe(Topic);
        }

        public async void Publish(string message)
        {
            var payload = new MessagePayload
            {
                Message = message,
                SentAt = DateTime.UtcNow
            };

            var jsonMessage = JsonSerializer.Serialize(payload);

            await _producer.ProduceAsync(Topic, new Message<Null, string> { Value = jsonMessage });
            Console.WriteLine($"Kafka: Sent {message} at {payload.SentAt:O}");
        }

        public void Consume()
        {
            try
            {
                while (true)
                {
                    var consumeResult = _consumer.Consume();

                    if (consumeResult?.Message != null)
                    {
                        var payload = JsonSerializer.Deserialize<MessagePayload>(consumeResult.Message.Value);
                        if (payload != null)
                        {
                            var receivedAt = DateTime.UtcNow;
                            var elapsedTime = receivedAt - payload.SentAt;

                            Console.WriteLine($"Kafka: Received {payload.Message} at {receivedAt:O}");
                            Console.WriteLine($"Elapsed time: {elapsedTime.TotalMilliseconds} ms");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Kafka consumer canceled.");
            }
        }

        public void Dispose()
        {
            _producer?.Flush();
            _producer?.Dispose();
            _consumer?.Close();
            _consumer?.Dispose();
        }

        // Mesaj modelini tanımla
        public class MessagePayload
        {
            public string Message { get; set; }
            public DateTime SentAt { get; set; }
        }
    }
}
