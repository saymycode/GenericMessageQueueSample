namespace GenericMessageQueueSample.Interfaces
{
    public interface IMessageQueueProvider
    {
        void Publish(string message);
        void Consume();
        void Consume(CancellationToken cancellationToken);
    }
}
