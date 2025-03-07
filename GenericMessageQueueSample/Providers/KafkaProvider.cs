using Confluent.Kafka;
using GenericMessageQueueSample.Interfaces;
using System.Reflection;
using System.Text.Json;
using static GenericMessageQueueSample.Interfaces.IMessageQueueProvider;

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
                BootstrapServers = "localhost:29092",
                MessageSendMaxRetries = 1,
                LingerMs = 5, // Batch işleme için bekleme süresi
                BatchSize = 16384, // Batch boyutu
                CompressionType = CompressionType.Snappy, // Sıkıştırma ekleyelim
                Acks = Acks.Leader // Sadece leader'ın onayını bekleyelim
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:29092",
                GroupId = "kafka-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                FetchMinBytes = 1024 * 1, // Minimum fetch boyutu
                FetchMaxBytes = 1024 * 1024, // Maximum fetch boyutu
                MaxPartitionFetchBytes = 1024 * 1024, // Partition başına maximum fetch
                SessionTimeoutMs = 10000, // Session timeout süresini azaltalım
                HeartbeatIntervalMs = 3000 // Heartbeat aralığını azaltalım
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            _consumer.Subscribe(Topic);
        }
        public async void Publish(string message, Priority priority, MicroserviceToBeDelivered microserviceToBeDelivered)
        {
            var payload = new MessagePayload
            {
                Message = message,
                SentAt = DateTime.UtcNow,
                Priority = priority,
                MicroserviceToBeDelivered = microserviceToBeDelivered
            };

            var jsonMessage = JsonSerializer.Serialize(payload);

            await _producer.ProduceAsync(Topic, new Message<Null, string> { Value = jsonMessage });
            Console.WriteLine($"Kafka: Sent {message} at {payload.SentAt:O}");
        }

        public void Dispose()
        {
            _producer?.Flush();
            _producer?.Dispose();
            _consumer?.Close();
            _consumer?.Dispose();
        }

        public void Consume()
        {
            try
            {
                var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5));
                if (consumeResult != null && consumeResult.Message != null)
                {
                    var payload = JsonSerializer.Deserialize<MessagePayload>(consumeResult.Message.Value);
                    if (payload != null)
                    {
                        var receivedAt = DateTime.UtcNow;
                        payload.ElapsedTimeMs = (receivedAt - payload.SentAt).TotalMilliseconds;

                        Console.WriteLine($"Kafka: Received {payload.Message} at {receivedAt:O}");
                        Console.WriteLine($"Priority: {payload.Priority}");
                        // Console.WriteLine($"Microservice: {payload.MicroserviceToBeDelivered}");
                        Console.WriteLine($"Elapsed time: {payload.ElapsedTimeMs:F2} ms");
                        switch (payload.MicroserviceToBeDelivered)
                        {
                            case MicroserviceToBeDelivered.MicroserviceA:
                                Console.WriteLine("GOTO: MicroserviceA");
                                break;
                            case MicroserviceToBeDelivered.MicroserviceB:
                                Console.WriteLine("GOTO: MicroserviceB");
                                break;
                            case MicroserviceToBeDelivered.MicroserviceC:
                                Console.WriteLine("GOTO: MicroserviceC");
                                break;
                            case MicroserviceToBeDelivered.MicroserviceD:
                                Console.WriteLine("GOTO: MicroserviceD");
                                break;
                    }
                }
                else
                {
                    Console.WriteLine("Kafka: No messages received.");
                }

            }
            catch (ConsumeException ex)
            {
                Console.WriteLine($"Kafka consume error: {ex.Error.Reason}");
            }
            finally
            {
                _consumer.Close();
                _consumer.Dispose();
            }
        }
        public void Consume(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    if (consumeResult?.Message != null)
                    {
                        var payload = JsonSerializer.Deserialize<MessagePayload>(consumeResult.Message.Value);
                        if (payload != null)
                        {
                            var receivedAt = DateTime.UtcNow;
                            payload.ElapsedTimeMs = (receivedAt - payload.SentAt).TotalMilliseconds;

                            Console.WriteLine($"Kafka: Received {payload.Message} at {receivedAt:O}");
                            Console.WriteLine($"Priority: {payload.Priority}");
                            Console.WriteLine($"Microservice: {payload.MicroserviceToBeDelivered}");
                            Console.WriteLine($"Elapsed time: {payload.ElapsedTimeMs:F2} ms");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Kafka consumer canceled.");
            }
        }
        public class MessagePayload
        {
            public string Message { get; set; }
            public DateTime SentAt { get; set; }
            public Priority Priority { get; set; }
            public MicroserviceToBeDelivered MicroserviceToBeDelivered { get; set; }
            public double ElapsedTimeMs { get; set; }  // Yeni eklenen özellik
        }
    }
}
