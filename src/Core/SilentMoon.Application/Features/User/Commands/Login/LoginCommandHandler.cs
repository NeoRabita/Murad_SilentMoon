using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.Security;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.Login
{
    public class LoginCommandHandler
        : ICommandHandler<LoginCommand, LoginResponse>
    {
        private readonly IUow _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGeneratorService _tokenGenerator;


        public LoginCommandHandler(
            IUow uow,
            IPasswordHasher passwordHasher,
            ITokenGeneratorService tokenGenerator)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }


        public async Task<Result<LoginResponse>> Handle(
            LoginCommand command,
            CancellationToken ct)
        {
            var userRepo =
                _uow.GetRepository<ApplicationUser>();


            var user = await userRepo.FirstOrDefaultAsync(
                x => x.Email == command.Email,
                ct);


            if (user == null)
            {
                return Error.Unauthorized(
     "Auth.InvalidCredentials",
     "Invalid email or password");
            }


            var valid =
                _passwordHasher.Verify(
                    command.Password,
                    user.PasswordHash);


            if (!valid)
            {
                return Error.Unauthorized(
       "Auth.InvalidCredentials",
       "Invalid email or password");
            }


            if (!user.IsEmailConfirmed)
            {
                return Error.Validation(
                    "Email",
                    "Email not confirmed");
            }


            var claims =
                await _tokenGenerator
                    .CreateClaims(user);


            var accessToken =
                await _tokenGenerator
                    .GenerateJwtAccessTokenAsync(claims);



            var refreshToken =
                await _tokenGenerator
                    .GenerateRefreshTokenAsync(
                        claims,
                        user.Id);



            var refreshRepo =
                _uow.GetRepository<Domain.Entities.RefreshToken>();


            await refreshRepo.AddAsync(
                new Domain.Entities.RefreshToken
                {
                    ApplicationUserId = user.Id,

                    Token = refreshToken,

                    Expires =
                        DateTime.UtcNow.AddDays(1),
                },
                ct);



            await _uow.SaveChangesAsync(ct);



            return new LoginResponse
            {
                AccessToken = accessToken,

                RefreshToken = refreshToken
            };
        }
    }
}
