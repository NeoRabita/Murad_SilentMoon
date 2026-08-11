using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Security;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.Resgister
{
    public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<RegisterCommandHandler> _logger;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IOtpSender _otpSender;

        public RegisterCommandHandler(IUow uow, IAppLogger<RegisterCommandHandler> logger, IPasswordHasher passwordHasher, IOtpSender otpSender)
        {
            _uow = uow;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _otpSender = otpSender;
        }

        public async Task<Result> Handle(RegisterCommand command, CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();

            var existUser = await userRepo.FirstOrDefaultAsync(x => x.Email == command.Email, ct);

            if (existUser != null)
            {
                return Error.Validation("Email.AlreadyExists", "Email already exists");
            }

            var (firstName, lastName) = SplitName(command.Name);

            var user = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = command.Name,
                Email = command.Email,
                PasswordHash = _passwordHasher.Hash(command.Password),
                IsEmailConfirmed = false
            };

            await userRepo.AddAsync(user, ct);

            await _otpSender.SendAsync(user.Email, user.FirstName, CacheExtensions.EmailVerificationPurpose, ct);

            return Result.Success();
        }

        private static (string FirstName, string LastName) SplitName(string fullName)
        {
            var parts = fullName.Trim().Split(' ', 2);

            return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], string.Empty);
        }
    }
}
