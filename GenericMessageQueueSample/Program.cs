using GenericMessageQueueSample.Interfaces;
using GenericMessageQueueSample.Providers;

class Program
{
    static void Main(string[] args)
    {
        IMessageQueueProvider rabbitMQProvider = new RabbitMQProvider();
        rabbitMQProvider.Publish("Hello from RabbitMQ!");
        rabbitMQProvider.Consume();

        IMessageQueueProvider kafkaProvider = new KafkaProvider();
        kafkaProvider.Publish("Hello from Kafka!");
        kafkaProvider.Consume();       
        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine(); // Konsolun açık kalmasını sağlar
    }

}
