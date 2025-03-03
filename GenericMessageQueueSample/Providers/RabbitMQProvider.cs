using GenericMessageQueueSample.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace GenericMessageQueueSample.Providers
{
    // RabbitMQProvider class implementing IMessageQueueProvider for interacting with RabbitMQ messaging
    public class RabbitMQProvider : IMessageQueueProvider
    {
        private readonly IConnection _connection;  // RabbitMQ connection object
        private readonly IModel _channel;  // RabbitMQ channel object

        public RabbitMQProvider()
        {
            // Establish a connection to the RabbitMQ broker
            var factory = new ConnectionFactory() { HostName = "localhost" };  // Create a factory for connection with the broker
            _connection = factory.CreateConnection();  // Establish the connection
            _channel = _connection.CreateModel();  // Create a channel for communication with RabbitMQ

            // Declare a queue (if it doesn't exist, it will be created)
            _channel.QueueDeclare(
                queue: "rabbitmq-queue", 
                durable: false,   // Messages will not survive a broker restart
                exclusive: false, // Queue is accessible by multiple connections
                autoDelete: false, // Queue won't be deleted when no consumers are connected
                arguments: null    // No additional queue arguments
            );
        }

        /// <summary>
        /// Publishes a message to the RabbitMQ queue.
        /// </summary>
        /// <param name="message">The message to be published.</param>
        public void Publish(string message)
        {
            // Create a payload with the current timestamp
            var payload = new
            {
                Message = message,
                SentAt = DateTime.UtcNow // Timestamp when the message was sent
            };

            // Serialize the payload into JSON format
            var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(payload));

            // Publish the message to the RabbitMQ queue
            _channel.BasicPublish(
                exchange: "", // Default exchange
                routingKey: "rabbitmq-queue", // Queue name
                basicProperties: null, 
                body: body // Message body
            );
            
            // Log the sent message and timestamp
            Console.WriteLine($"RabbitMQ: Sent {message} at {payload.SentAt:O}");
        }

        /// <summary>
        /// Consumes messages from the RabbitMQ queue asynchronously.
        /// </summary>
        public void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            // Event handler for message reception
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();  // Extract the message body from the event arguments
                var messageJson = Encoding.UTF8.GetString(body);  // Convert the byte array into a string (JSON)
                var payload = System.Text.Json.JsonSerializer.Deserialize<MessagePayload>(messageJson);  // Deserialize JSON into a MessagePayload object
                
                if (payload != null)
                {
                    var receivedAt = DateTime.UtcNow;  // Timestamp when the message was received
                    var elapsedTime = receivedAt - payload.SentAt;  // Calculate latency between sending and receiving
                    
                    // Log the received message and elapsed time
                    Console.WriteLine($"RabbitMQ: Received {payload.Message} at {receivedAt:O}");
                    Console.WriteLine($"Elapsed time: {elapsedTime.TotalMilliseconds} ms");
                }
            };
            
            // Start consuming messages from the queue
            _channel.BasicConsume(
                queue: "rabbitmq-queue", 
                autoAck: true, // Automatically acknowledge message receipt
                consumer: consumer // Attach the consumer to the queue
            );
        }

        // Defines the message structure for serialization/deserialization
        public class MessagePayload
        {
            public string Message { get; set; } = string.Empty;  // The message content
            public DateTime SentAt { get; set; }  // Timestamp when the message was sent
        }

        /// <summary>
        /// Disposes of the RabbitMQ resources (channel and connection).
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();  // Close the channel
            _connection?.Close(); // Close the connection
        }

        /// <summary>
        /// Placeholder method for consuming with a cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for stopping the consume operation.</param>
        public void Consume(CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); // Placeholder for future implementation
        }
    }
}
