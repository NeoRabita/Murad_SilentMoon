using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Features.User.Commands.Otp;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Security;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.Resgister
{
    public class RegisterCommandHandler
        : ICommandHandler<RegisterCommand>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<RegisterCommandHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IOtpService _otpService;
        private readonly ICacheService _cacheService;

        public RegisterCommandHandler(
            IUow uow,
            IAppLogger<RegisterCommandHandler> logger,
            IEmailService emailService,
            IPasswordHasher passwordHasher,
            IOtpService otpService,
            ICacheService cacheService)
        {
            _uow = uow;
            _logger = logger;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _otpService = otpService;
            _cacheService = cacheService;
        }

        public async Task<Result> Handle(
            RegisterCommand command,
            CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();

            var existUser = await userRepo.FirstOrDefaultAsync(x => x.Email == command.Email, ct);

            if (existUser != null)
            {
                return Error.Validation(
                    "Email",
                    "Email already exists");
            }

            var user = new ApplicationUser
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                PasswordHash = _passwordHasher.Hash(command.Password),
                IsEmailConfirmed = false
            };

            await userRepo.AddAsync(user,ct);
            await _uow.SaveChangesAsync(ct);

            var otpCode = _otpService.Generate();

            await _cacheService.SetAsync(CacheExtensions.EmailVerification(user.Id), otpCode, TimeSpan.FromMinutes(10));

            await _emailService.SendOtpEmailAsync(user.Email,user.FirstName,otpCode);

            return Result.Success();
        }
    }
}
