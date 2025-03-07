namespace GenericMessageQueueSample.Interfaces
{
    public interface IMessageQueueProvider
    {
        void Publish(string message, Priority priority, MicroserviceToBeDelivered microserviceToBeDelivered);
        void Consume();
        void Consume(CancellationToken cancellationToken);

        public enum Priority
        {
            Low,
            Medium,
            High,
            Critical
        }
        public enum MicroserviceToBeDelivered
        {
            MicroserviceA,
            MicroserviceB,
            MicroserviceC,
            MicroserviceD
        }
    }
}
