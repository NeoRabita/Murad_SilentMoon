using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.Security;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
    {
        private readonly IUow _uow;
        private readonly ICacheService _cacheService;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordCommandHandler(IUow uow,ICacheService cacheService,IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _cacheService = cacheService;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> Handle(ResetPasswordCommand command,CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();

            var user = await userRepo.FirstOrDefaultAsync(x => x.Email == command.Email, ct);

            if (user == null)
            {
                return Error.NotFound(
                    "User",
                    "User not found");
            }

            var key = CacheExtensions.OtpCacheKey(CacheExtensions.ForgotPasswordPurpose, command.Email);

            var cachedHash = await _cacheService.GetAsync<string>(key);

            if (cachedHash == null)
            {
                return Error.Validation(
                    "OTP",
                    "Code expired");
            }

            var submittedHash = OtpHasher.Hash(command.Code);

            if (cachedHash != submittedHash)
            {
                return Error.Validation(
                    "OTP",
                    "Invalid code");
            }

            await _cacheService.RemoveAsync(key);

            user.PasswordHash = _passwordHasher.Hash(command.Password);
            userRepo.Update(user);

            return Result.Success();
        }
    }
}
