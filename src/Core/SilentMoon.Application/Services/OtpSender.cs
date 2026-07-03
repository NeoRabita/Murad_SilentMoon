using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Messages;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Services
{
    public class OtpSender : IOtpSender
    {
        private readonly ICacheService _cacheService;
        private readonly IMessagePublisher _publisher;

        public OtpSender(
            ICacheService cacheService,
            IMessagePublisher publisher)
        {
            _cacheService = cacheService;
            _publisher = publisher;
        }

        private string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();
        }

        public async Task SendAsync(int userId, string email, string firstName, CancellationToken ct = default)
        {
            var otpCode = GenerateOtp();

            await _cacheService.SetAsync(CacheExtensions.EmailVerification(userId),otpCode,TimeSpan.FromMinutes(10));

            var message = new OtpEmailMessage
            {
                UserId = userId,
                Email = email,
                FirstName = firstName,
                OtpCode = otpCode
            };

            await _publisher.PublishAsync(message, "otp.email", ct);
        }
    }
}
