using GenericMessageQueueSample.Interfaces;
using GenericMessageQueueSample.Providers;
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        string providerName = configuration.GetSection("MessageQueueSettings:Provider").Value ?? "RabbitMQ";
        
        IMessageQueueProvider messageQueueProvider = CreateProvider(providerName);

        try
        {
            SendTestMessages(messageQueueProvider);

            messageQueueProvider.Consume();

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
        finally
        {
            if (messageQueueProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    private static IMessageQueueProvider CreateProvider(string providerName)
    {
        switch (providerName)
        {
            case "RabbitMQ":
                return new RabbitMQProvider();
            case "Kafka":
                return new KafkaProvider();
            default:
                throw new ArgumentException($"Unsupported provider: {providerName}");
        }
    }

    private static void SendTestMessages(IMessageQueueProvider provider)
    {
        var testMessages = new[]
        {
            (message: "Hi!", priority: IMessageQueueProvider.Priority.Low, microservice: IMessageQueueProvider.MicroserviceToBeDelivered.MicroserviceA),
            (message: "Hi!", priority: IMessageQueueProvider.Priority.Medium, microservice: IMessageQueueProvider.MicroserviceToBeDelivered.MicroserviceB),
            (message: "Hi!", priority: IMessageQueueProvider.Priority.High, microservice: IMessageQueueProvider.MicroserviceToBeDelivered.MicroserviceC),
            (message: "Hi!", priority: IMessageQueueProvider.Priority.High, microservice: IMessageQueueProvider.MicroserviceToBeDelivered.MicroserviceD)
        };

        foreach (var (message, priority, microservice) in testMessages)
        {
            provider.Publish(message, priority, microservice);
        }
    }
}