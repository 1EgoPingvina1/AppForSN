namespace DTC.Application.Interfaces.RabbitMQ
{
    public interface IRabbitMqPublisher
    {
        void Publish<T>(T message, string routing);
    }
}
