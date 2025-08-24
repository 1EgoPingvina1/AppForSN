using DTC.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using DTC.Application.DTO.Project;

namespace DTC.Infrastructure.Services.RabbitMQ
{
    public class ReviewQueueConsumer : BackgroundService
    {
        private readonly ILogger<ReviewQueueConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string QueueName = "project-review-queue";
        private const string DeadLetterQueueName = "project-review-dead-letter-queue";
        private const string DeadLetterExchangeName = "review-dlx";

        public ReviewQueueConsumer(ILogger<ReviewQueueConsumer> logger, IServiceScopeFactory scopeFactory, IConnection connection)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _connection = connection;
            _channel = _connection.CreateModel();

            SetupQueues();
        }

        private void SetupQueues()
        {
            try
            {
                _channel.ExchangeDeclare(DeadLetterExchangeName, ExchangeType.Fanout);
                _channel.QueueDeclare(DeadLetterQueueName, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(DeadLetterQueueName, DeadLetterExchangeName, routingKey: "");

                var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", DeadLetterExchangeName }
        };
                _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to setup RabbitMQ queues.");
                throw; // rethrow to fail fast during startup
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("Received message: {Content}", content);

                try
                {
                    // Обрабатываем сообщение
                    await ProcessMessage(content);

                    // Отправляем подтверждение (ACK), что сообщение успешно обработано
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process message: {Content}", content);

                    // Отправляем NACK, requeue: false означает, что сообщение не будет
                    // возвращено в основную очередь, а будет отправлено в DLX
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                }
            };

            // Начинаем слушать очередь. autoAck: false - ручное подтверждение
            _channel.BasicConsume(QueueName, autoAck: false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessMessage(string content)
        {
            // ВАЖНО: BackgroundService является Singleton'ом.
            // DbContext и UnitOfWork регистрируются как Scoped.
            // Поэтому мы должны создавать новый scope для каждой обработки сообщения.
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // Десериализуем сообщение
            var message = JsonConvert.DeserializeObject<ProjectSubmittedForReviewEvent>(content);

            if (message == null || message.ProjectId == 0)
            {
                throw new ArgumentException("Invalid message content", nameof(content));
            }


            _logger.LogInformation("Processing review for project {ProjectId}", message.ProjectId);

            // Пример:
            // var review = new Review { ProjectId = message.ProjectId, ... };
            // unitOfWork.Reviews.Add(review);
            // await unitOfWork.SaveChangesAsync();

            await Task.Delay(1000); // Имитация работы
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
