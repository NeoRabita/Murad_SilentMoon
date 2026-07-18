using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.DTOs.Otp;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Application.Interfaces.Services;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Services
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

        public async Task SendAsync(string email, string firstName, CancellationToken ct = default)
        {
            var otpCode = GenerateOtp();

            var otpHash = OtpHasher.Hash(otpCode);

            await _cacheService.SetAsync(CacheExtensions.OtpCacheKey(email),otpHash,TimeSpan.FromMinutes(10));

            var message = new OtpEmailMessage
            {
                Email = email,
                FirstName = firstName,
                OtpCode = otpCode
            };

            await _publisher.PublishAsync(message, "otp.email", ct);
        }
    }
}
