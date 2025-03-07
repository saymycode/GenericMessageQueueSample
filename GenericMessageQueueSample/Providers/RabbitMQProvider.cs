using GenericMessageQueueSample.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using static GenericMessageQueueSample.Interfaces.IMessageQueueProvider;

namespace GenericMessageQueueSample.Providers
{
    public class RabbitMQProvider : IMessageQueueProvider
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;  

        public RabbitMQProvider()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };  
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel(); 

            _channel.QueueDeclare(
                queue: "rabbitmq-queue", 
                durable: false,  
                exclusive: false,
                autoDelete: false, 
                arguments: null  
            );
        }
        public void Publish(string message, Priority priority, MicroserviceToBeDelivered microserviceToBeDelivered)
        {
            var payload = new
            {
                Message = message,
                SentAt = DateTime.UtcNow,
                Priority = priority,
                MicroserviceToBeDelivered = microserviceToBeDelivered,
            };

            var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(payload));

            _channel.BasicPublish(
                exchange: "",
                routingKey: "rabbitmq-queue", 
                basicProperties: null, 
                body: body 
            );
            
            Console.WriteLine($"RabbitMQ: Sent {message} at {payload.SentAt:O}");
        }

        public void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();  
                var messageJson = Encoding.UTF8.GetString(body);  
                var payload = System.Text.Json.JsonSerializer.Deserialize<MessagePayload>(messageJson);  
                
                if (payload != null)
                {
                    var receivedAt = DateTime.UtcNow; 
                    var elapsedTime = receivedAt - payload.SentAt;  
                    
                    Console.WriteLine($"RabbitMQ: Received {payload.Message} at {receivedAt:O}");
                    Console.WriteLine($"Elapsed time: {elapsedTime.TotalMilliseconds} ms");
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
            };
            
            _channel.BasicConsume(
                queue: "rabbitmq-queue", 
                autoAck: true, 
                consumer: consumer 
            );
        }

        public class MessagePayload
        {
            public string Message { get; set; } = string.Empty; 
            public DateTime SentAt { get; set; }
            public Priority Priority { get; set; }  
            public MicroserviceToBeDelivered MicroserviceToBeDelivered { get; set; }


        }

        public void Dispose()
        {
            _channel?.Close(); 
            _connection?.Close(); 
        }
        public void Consume(CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); 
        }
    }
}
