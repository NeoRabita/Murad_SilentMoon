using Application.Abstractions.Messaging;
using SilentMoon.Application.DTOs.Email;
using SilentMoon.Application.Features.Accounts.Commands.ResendOtp;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.Otp
{
    public class ResendOtpCommandHandler
        : ICommandHandler<ResendOtpCommand>
    {
        private readonly IUow _uow;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;


        public ResendOtpCommandHandler(
            IUow uow,
            IOtpService otpService,
            IEmailService emailService)
        {
            _uow = uow;
            _otpService = otpService;
            _emailService = emailService;
        }


        public async Task<Result> Handle(
            ResendOtpCommand command,
            CancellationToken ct)
        {
            var userRepo =
                _uow.GetRepository<ApplicationUser>();


            var user =
                await userRepo.FirstOrDefaultAsync(
                    x => x.Email == command.Email,
                    ct);


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



            var otpRepo =
                _uow.GetRepository<Domain.Entities.Otp>();


            var oldOtp =
                await otpRepo.FirstOrDefaultAsync(
                    x =>
                    x.UserId == user.Id
                    &&
                    !x.IsUsed,
                    ct);



            if (oldOtp != null)
            {
                oldOtp.IsUsed = true;
                otpRepo.Update(oldOtp);
            }



            var otp = new Domain.Entities.Otp
            {
                UserId = user.Id,

                Code = _otpService.Generate(),

                ExpireDate =
                    DateTime.UtcNow.AddMinutes(10),

                IsUsed = false
            };


            await otpRepo.AddAsync(
                otp,
                ct);



            await _emailService.SendAsync(
                new EmailRequest
                {
                    To = user.Email,

                    Subject = "Confirm your account",

                    Body = $@"
                    <h2>Hello {user.FirstName}</h2>
                    <p>Your new OTP:</p>
                    <h1>{otp.Code}</h1>
                    <p>Expires in 10 minutes.</p>"
                });



            await _uow.SaveChangesAsync(ct);


            return Result.Success();
        }
    }
}