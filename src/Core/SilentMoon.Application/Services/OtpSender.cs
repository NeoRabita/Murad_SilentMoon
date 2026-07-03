using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Services
{
    public class OtpSender : IOtpSender
    {
        private const string OtpQueueName = "otp.email";

        private readonly IOtpService _otpService;
        private readonly ICacheService _cacheService;
        private readonly IMessagePublisher _publisher;

        public OtpSender(
            IOtpService otpService,
            ICacheService cacheService,
            IMessagePublisher publisher)
        {
            _otpService = otpService;
            _cacheService = cacheService;
            _publisher = publisher;
        }

        public async Task SendAsync(int userId, string email, string firstName, CancellationToken ct = default)
        {
            var otpCode = _otpService.Generate();

            // OTP-ni cache-ə yaz (10 dəqiqəlik)
            await _cacheService.SetAsync(
                CacheExtensions.EmailVerification(userId),
                otpCode,
                TimeSpan.FromMinutes(10));

            // Email göndərmə məsuliyyətini Consumer-ə ötür
            var message = new OtpEmailMessage
            {
                UserId = userId,
                Email = email,
                FirstName = firstName,
                OtpCode = otpCode
            };

            await _publisher.PublishAsync(message, OtpQueueName, ct);
        }
    }
}
