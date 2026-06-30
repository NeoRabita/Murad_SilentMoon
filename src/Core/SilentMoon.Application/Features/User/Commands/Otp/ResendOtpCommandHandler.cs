using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.DTOs.Email;
using SilentMoon.Application.Features.Accounts.Commands.ResendOtp;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.Otp
{
    public class ResendOtpCommandHandler: ICommandHandler<ResendOtpCommand>
    {
        private readonly IUow _uow;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ICacheService _cacheService;

        public ResendOtpCommandHandler(
            IUow uow,
            IOtpService otpService,
            IEmailService emailService,
            ICacheService cacheService)
        {
            _uow = uow;
            _otpService = otpService;
            _emailService = emailService;
            _cacheService = cacheService;
        }

        public async Task<Result> Handle(
            ResendOtpCommand command,
            CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();

            var user = await userRepo.FirstOrDefaultAsync(x => x.Email == command.Email,ct);

            if (user == null)
            {
                return Error.NotFound(
                    "User",
                    "User not found");
            }

            if (user.IsEmailConfirmed)
            {
                return Error.Validation(
                    "Email",
                    "Email already confirmed");
            }

            var otpCode = _otpService.Generate();

            await _cacheService.SetAsync(CacheExtensions.EmailVerification(user.Id),otpCode,TimeSpan.FromMinutes(10));

            await _emailService.SendAsync(
                new EmailRequest
                {
                    To = user.Email,
                    Subject = "Confirm your account",
                    Body = $@"
                    <h2>Hello {user.FirstName}</h2>
                    <p>Your new OTP:</p>
                    <h1>{otpCode}</h1>
                    <p>Expires in 10 minutes.</p>"
                });

            return Result.Success();
        }
    }
}
