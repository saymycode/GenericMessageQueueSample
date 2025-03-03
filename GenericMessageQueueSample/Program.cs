using GenericMessageQueueSample.Interfaces;
using GenericMessageQueueSample.Providers;

class Program
{
    static void Main(string[] args)
    {
        // Create an instance of RabbitMQProvider which implements IMessageQueueProvider
        // This demonstrates the flexibility of the IMessageQueueProvider interface,
        // which can be implemented by different queue providers like RabbitMQ, Kafka, etc.
        IMessageQueueProvider rabbitMQProvider = new RabbitMQProvider();

        // Publish a message to RabbitMQ queue
        rabbitMQProvider.Publish("Hi!");

        // Consume messages from RabbitMQ queue
        rabbitMQProvider.Consume();

        // Create an instance of KafkaProvider which also implements IMessageQueueProvider
        // This shows that the same interface can be used with different providers
        IMessageQueueProvider kafkaProvider = new KafkaProvider();

        // Publish a message to Kafka queue
        kafkaProvider.Publish("Hi!");

        // Consume messages from Kafka queue
        kafkaProvider.Consume();

        // Inform the user to press enter to exit
        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    }
}
