using DTC.Application.Interfaces.RabbitMQ;
using DTC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using IModel = RabbitMQ.Client.IModel;
using RabbitConnection = RabbitMQ.Client.IConnection;

namespace DTC.Infrastructure.Services.RabbitMQ
{
    public class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
    {
        private readonly RabbitConnection _connection;
        private readonly IModel _channel;

        public RabbitMqPublisher(IOptions<RabbitMqOptions> options)
        {
            var factory = new ConnectionFactory()
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password,
                Port = options.Value.Port,
                VirtualHost = string.IsNullOrEmpty(options.Value.VirtualHost) ? "/" : options.Value.VirtualHost,
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        public void Publish<T>(T message, string routing)
        {
            try
            {
                // Объявляем очередь при каждой публикации (идемпотентно)
                _channel.QueueDeclare(
                    queue: routing,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var messageJson = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(messageJson);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: routing,
                    basicProperties: properties,
                    body: body);
            }
            catch (Exception ex)
            {
                // Обработка ошибки
                throw new InvalidOperationException($"Failed to publish message to {routing}", ex);
            }
        }
    }
}
