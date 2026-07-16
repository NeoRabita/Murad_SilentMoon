using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Caching;
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
    public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
    {
        private readonly IUow _uow;
        private readonly ICacheService _cacheService;

        public VerifyEmailCommandHandler(IUow uow, ICacheService cacheService)
        {
            _uow = uow;
            _cacheService = cacheService;
        }

        public async Task<Result> Handle(VerifyEmailCommand command, CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();

            var user = await userRepo.FirstOrDefaultAsync(x => x.Email == command.Email,ct);

            if (user == null)
            {
                return Error.NotFound("User", "User not found");
            }

            if (user.IsEmailConfirmed)
            {
                return Error.Validation("Email", "Already confirmed");
            }

            var key = CacheExtensions.OtpCacheKey(command.Email);

            var cachedHash = await _cacheService.GetAsync<string>(key);

            if (cachedHash == null)
            {
                return Error.Validation("OTP", "Code expired");
            }

            var submittedHash = OtpHasher.Hash(command.Code);

            if (cachedHash != submittedHash)
            {
                return Error.Validation("OTP", "Invalid code");
            }

            await _cacheService.RemoveAsync(key);

            user.IsEmailConfirmed = true;
            userRepo.Update(user);

            return Result.Success();
        }
    }
}
