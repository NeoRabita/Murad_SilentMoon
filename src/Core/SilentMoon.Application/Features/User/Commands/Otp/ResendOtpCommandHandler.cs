using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Features.Accounts.Commands.ResendOtp;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.Otp
{
    public class ResendOtpCommandHandler : ICommandHandler<ResendOtpCommand>
    {
        private readonly IUow _uow;
        private readonly IOtpSender _otpSender;

        public ResendOtpCommandHandler(IUow uow, IOtpSender otpSender)
        {
            _uow = uow;
            _otpSender = otpSender;
        }

        public async Task<Result> Handle(ResendOtpCommand command, CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();

            var user = await userRepo.FirstOrDefaultAsync(x => x.Email == command.Email, ct);

            if (user == null)
            {
                return Error.NotFound("User", "User not found");
            }

            if (user.IsEmailConfirmed)
            {
                return Error.Validation("Email.AlreadyConfirmed", "Email already confirmed");
            }

            await _otpSender.SendAsync(user.Email, user.FirstName, CacheExtensions.EmailVerificationPurpose, ct);

            return Result.Success();
        }
    }
}
