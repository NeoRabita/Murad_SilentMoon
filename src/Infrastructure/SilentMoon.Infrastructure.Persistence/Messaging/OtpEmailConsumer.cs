using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Messages;
using SilentMoon.Infrastructure.Persistence.Settings;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Messaging
{
    public class OtpEmailConsumer : BackgroundService
    {
        private readonly RabbitMqSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OtpEmailConsumer> _logger;

        private IConnection _connection;
        private IChannel _channel;

        public OtpEmailConsumer(
            IOptions<RabbitMqSettings> settings,
            IServiceScopeFactory scopeFactory,
            ILogger<OtpEmailConsumer> logger)
        {
            _settings = settings.Value;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await ConnectAndConsumeAsync(ct);
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    _logger.LogWarning(ex,
                        "error");

                    await Task.Delay(TimeSpan.FromSeconds(10), ct);
                }
            }
        }

        private async Task ConnectAndConsumeAsync(CancellationToken ct)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port     = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync(ct);
            _channel    = await _connection.CreateChannelAsync(cancellationToken: ct);

            await _channel.QueueDeclareAsync(
                queue:      _settings.QueueName,
                durable:    true,
                exclusive:  false,
                autoDelete: false,
                arguments:  null,
                cancellationToken: ct);

            await _channel.BasicQosAsync(
                prefetchSize:  0,
                prefetchCount: 1,
                global:        false,
                cancellationToken: ct);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json    = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonSerializer.Deserialize<OtpEmailMessage>(json);

                    if (message is not null)
                    {
                        using var scope  = _scopeFactory.CreateScope();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                        await emailService.SendOtpEmailAsync(
                            message.Email,
                            message.FirstName,
                            message.OtpCode);

                        _logger.LogInformation(
                            "OTP: {Email}", message.Email);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OTP email error");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue:    _settings.QueueName,
                autoAck:  false,
                consumer: consumer,
                cancellationToken: ct);

            _logger.LogInformation(
                "OtpEmailConsumer '{Queue}'", _settings.QueueName);

            var tcs = new TaskCompletionSource();
            ct.Register(() => tcs.TrySetResult());
            await tcs.Task;
        }

        public override async Task StopAsync(CancellationToken ct)
        {
            await base.StopAsync(ct);
            await CleanupAsync();
        }

        private async Task CleanupAsync()
        {
            try
            {
                if (_channel is { IsOpen: true })
                    await _channel.CloseAsync();

                if (_connection is { IsOpen: true })
                    await _connection.CloseAsync();
            }
            catch { }
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
