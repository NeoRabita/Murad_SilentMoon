using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
    {
        private readonly IUow _uow;
        private readonly IOtpSender _otpSender;

        public ForgotPasswordCommandHandler(IUow uow, IOtpSender otpSender)
        {
            _uow = uow;
            _otpSender = otpSender;
        }

        public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();

            var user = await userRepo.FirstOrDefaultAsync(x => x.Email == command.Email, ct);

            if (user == null)
            {
                return Error.NotFound("User", "User not found");
            }

            await _otpSender.SendAsync(user.Email, user.FirstName, CacheExtensions.ForgotPasswordPurpose, ct);

            return Result.Success();
        }
    }
}
