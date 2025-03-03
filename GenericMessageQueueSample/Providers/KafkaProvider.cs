using Confluent.Kafka;
using GenericMessageQueueSample.Interfaces;
using System.Text.Json;

namespace GenericMessageQueueSample.Providers
{
    // KafkaProvider class implementing IMessageQueueProvider for interacting with Kafka messaging
    public class KafkaProvider : IMessageQueueProvider
    {
        private readonly IProducer<Null, string> _producer;  // Kafka producer for sending messages
        private readonly IConsumer<Null, string> _consumer;  // Kafka consumer for receiving messages
        private const string Topic = "kafka-topic";  // The Kafka topic to use for sending and receiving messages

        public KafkaProvider()
        {
            // Kafka producer configuration
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"  // Kafka server address
            };

            // Kafka consumer configuration
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",  // Kafka server address
                GroupId = "kafka-group",  // Consumer group ID
                AutoOffsetReset = AutoOffsetReset.Earliest,  // Start consuming from the earliest message if no offset is found
                EnableAutoCommit = true  // Automatically commit offsets
            };

            // Initializing the producer and consumer
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            _consumer.Subscribe(Topic);  // Subscribing the consumer to the specified Kafka topic
        }

        /// <summary>
        /// Publishes a message to the Kafka topic.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        public async void Publish(string message)
        {
            // Create a message payload with the current time
            var payload = new MessagePayload
            {
                Message = message,
                SentAt = DateTime.UtcNow
            };

            // Serialize the payload to JSON
            var jsonMessage = JsonSerializer.Serialize(payload);

            // Produce the message asynchronously to the Kafka topic
            await _producer.ProduceAsync(Topic, new Message<Null, string> { Value = jsonMessage });
            Console.WriteLine($"Kafka: Sent {message} at {payload.SentAt:O}");  // Log the sent message with timestamp
        }

        /// <summary>
        /// Disposes of the producer and consumer resources.
        /// </summary>
        public void Dispose()
        {
            _producer?.Flush();  // Ensure all messages are sent before disposing
            _producer?.Dispose();  // Dispose the producer
            _consumer?.Close();  // Close the consumer
            _consumer?.Dispose();  // Dispose the consumer
        }

        /// <summary>
        /// Consumes a message from Kafka with a timeout of 5 seconds.
        /// </summary>
        public void Consume()
        {
            try
            {
                // Attempt to consume a message within 5 seconds
                var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5)); // Wait for a maximum of 5 seconds
                if (consumeResult != null && consumeResult.Message != null)
                {
                    // Deserialize the message payload
                    var payload = JsonSerializer.Deserialize<MessagePayload>(consumeResult.Message.Value);
                    if (payload != null)
                    {
                        // Calculate the elapsed time since the message was sent
                        var receivedAt = DateTime.UtcNow;
                        var elapsedTime = receivedAt - payload.SentAt;

                        // Log the received message and elapsed time
                        Console.WriteLine($"Kafka: Received {payload.Message} at {receivedAt:O}");
                        Console.WriteLine($"Elapsed time: {elapsedTime.TotalMilliseconds} ms");
                    }
                }
                else
                {
                    Console.WriteLine("Kafka: No messages received.");
                }
            }
            catch (ConsumeException ex)
            {
                // Handle Kafka consume errors
                Console.WriteLine($"Kafka consume error: {ex.Error.Reason}");
            }
            finally
            {
                // Ensure the consumer is properly closed and disposed
                _consumer.Close();
                _consumer.Dispose();
            }
        }
        
        /// <summary>
        /// Consumes messages from the Kafka topic.
        /// This method runs in a loop until cancellation is requested.
        /// </summary>
        /// <param name="cancellationToken">A token to signal cancellation of the consume operation.</param>
        public void Consume(CancellationToken cancellationToken)
        {
            try
            {
                // Consume messages in a loop until cancellation is requested
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Attempt to consume a message
                    var consumeResult = _consumer.Consume(cancellationToken);
                    if (consumeResult?.Message != null)
                    {
                        // Deserialize the message payload
                        var payload = JsonSerializer.Deserialize<MessagePayload>(consumeResult.Message.Value);
                        if (payload != null)
                        {
                            // Calculate the elapsed time since the message was sent
                            var receivedAt = DateTime.UtcNow;
                            var elapsedTime = receivedAt - payload.SentAt;

                            // Log the received message and elapsed time
                            Console.WriteLine($"Kafka: Received {payload.Message} at {receivedAt:O}");
                            Console.WriteLine($"Elapsed time: {elapsedTime.TotalMilliseconds} ms");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Handle the case when the consume operation is cancelled
                Console.WriteLine("Kafka consumer canceled.");
            }
        }
        // Message model for serializing/deserializing Kafka messages
        public class MessagePayload
        {
            public string Message { get; set; }  // The message content
            public DateTime SentAt { get; set; }  // The timestamp when the message was sent
        }
    }
}
