using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SilentMoon.Application.DTOs.Reminder;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Messaging
{
    public class ReminderEmailConsumer : RabbitMqConsumerBase<ReminderEmailMessage>
    {
        protected override string QueueName => "reminder.email";

        public ReminderEmailConsumer(IOptions<RabbitMqSettings> settings, IServiceScopeFactory scopeFactory, ILogger<ReminderEmailConsumer> logger) : base(settings, scopeFactory, logger)
        {
        }

        protected override async Task HandleMessageAsync(IServiceProvider scopedProvider,ReminderEmailMessage message,CancellationToken ct)
        {
            var emailService = scopedProvider.GetRequiredService<IEmailService>();

            await emailService.SendReminderEmailAsync(
                message.Email,
                message.FirstName,
                message.Time);
        }
    }
}
