using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.User.Commands.Login;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler: ICommandHandler<RefreshTokenCommand, LoginResponse>
    {
        private readonly IUow _uow;
        private readonly ITokenGeneratorService _tokenGenerator;


        public RefreshTokenCommandHandler(
            IUow uow,
            ITokenGeneratorService tokenGenerator)
        {
            _uow = uow;
            _tokenGenerator = tokenGenerator;
        }


        public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand command,CancellationToken ct)
        {
            var refreshRepo = _uow.GetRepository<Domain.Entities.RefreshToken>();

            var refreshToken =  await refreshRepo.FirstOrDefaultAsync(x =>x.Token == command.RefreshToken,ct);


            if (refreshToken == null)
            {
                return Error.Unauthorized(
                    "RefreshToken",
                    "Invalid refresh token");
            }


            if (refreshToken.IsExpired)
            {
                return Error.Unauthorized(
                    "RefreshToken",
                    "Token revoked");
            }


            if (refreshToken.Expires < DateTime.UtcNow)
            {
                return Error.Unauthorized(
                    "RefreshToken",
                    "Token expired");
            }

            var userRepo = _uow.GetRepository<ApplicationUser>();

            var user =await userRepo.GetByIdAsync(refreshToken.ApplicationUserId,ct);

            if (user == null)
            {
                return Error.Unauthorized(
                    "User",
                    "User not found");
            }

            var claims =await _tokenGenerator.CreateClaims(user);

            var accessToken =await _tokenGenerator.GenerateJwtAccessTokenAsync(claims);

            var newRefreshToken =await _tokenGenerator.GenerateRefreshTokenAsync(claims,user.Id);

            await refreshRepo.AddAsync(
                new Domain.Entities.RefreshToken
                {
                    ApplicationUserId = user.Id,

                    Token = newRefreshToken,

                    Expires =
                        DateTime.UtcNow.AddDays(7)
                },
                ct);

            return new LoginResponse
            {
                AccessToken = accessToken,

                RefreshToken = newRefreshToken
            };
        }
    }
}
