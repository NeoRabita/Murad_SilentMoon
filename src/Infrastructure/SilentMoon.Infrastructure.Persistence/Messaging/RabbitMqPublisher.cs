using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Infrastructure.Persistence.Settings;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly RabbitMqSettings _settings;

        public RabbitMqPublisher(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port     = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            await using var connection = await factory.CreateConnectionAsync(ct);
            await using var channel   = await connection.CreateChannelAsync(cancellationToken: ct);

            await channel.QueueDeclareAsync(
                queue:      queueName,
                durable:    true,
                exclusive:  false,
                autoDelete: false,
                arguments:  null,
                cancellationToken: ct);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var props = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange:   string.Empty,
                routingKey: queueName,
                mandatory:  false,
                basicProperties: props,
                body:       body,
                cancellationToken: ct);
        }
    }
}
