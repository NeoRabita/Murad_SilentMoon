using Application.Abstractions.Messaging;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler
    : ICommandHandler<VerifyEmailCommand>
    {
        private readonly IUow _uow;


        public VerifyEmailCommandHandler(IUow uow)
        {
            _uow = uow;
        }


        public async Task<Result> Handle(
            VerifyEmailCommand command,
            CancellationToken ct)
        {
            var userRepo =
                _uow.GetRepository<ApplicationUser>();


            var user = await userRepo
                .FirstOrDefaultAsync(
                    x => x.Email == command.Email,
                    ct);


            if (user == null)
            {
                return Error.NotFound(
                    "User",
                    "User not found");
            }


            var otpRepo =
                _uow.GetRepository<Otp>();


            var otp = await otpRepo.FirstOrDefaultAsync(
                x =>
                x.UserId == user.Id &&
                x.Code == command.Code &&
                !x.IsUsed,
                ct);


            if (otp == null)
            {
                return Error.Validation(
                    "OTP",
                    "Invalid code");
            }


            if (otp.ExpireDate < DateTime.UtcNow)
            {
                return Error.Validation(
                    "OTP",
                    "Code expired");
            }



            user.IsEmailConfirmed = true;

            otp.IsUsed = true;


            userRepo.Update(user);
            otpRepo.Update(otp);


            await _uow.SaveChangesAsync(ct);


            return Result.Success();
        }
    }
}
