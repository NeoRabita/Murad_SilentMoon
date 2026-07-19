using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.DTOs.Otp;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Messaging
{
    public class OtpEmailConsumer : RabbitMqConsumerBase<OtpEmailMessage>
    {
        protected override string QueueName => "otp.email";

        public OtpEmailConsumer(IOptions<RabbitMqSettings> settings, IServiceScopeFactory scopeFactory, ILogger<OtpEmailConsumer> logger) : base(settings, scopeFactory, logger)
        {
        }

        protected override async Task HandleMessageAsync(IServiceProvider scopedProvider,OtpEmailMessage message,CancellationToken ct)
        {
            var emailService = scopedProvider.GetRequiredService<IEmailService>();

            if (message.Purpose == CacheExtensions.ForgotPasswordPurpose)
            {
                await emailService.SendForgotPasswordEmailAsync(
                    message.Email,
                    message.FirstName,
                    message.OtpCode);
            }
            else
            {
                await emailService.SendOtpEmailAsync(
                    message.Email,
                    message.FirstName,
                    message.OtpCode);
            }
        }
    }
}
