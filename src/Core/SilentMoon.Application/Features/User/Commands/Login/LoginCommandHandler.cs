using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.Security;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
    {
        private readonly IUow _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGeneratorService _tokenGenerator;


        public LoginCommandHandler(IUow uow, IPasswordHasher passwordHasher, ITokenGeneratorService tokenGenerator)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }


        public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken ct)
        {
            var userRepo = _uow.GetRepository<ApplicationUser>();


            var user = await userRepo.FirstOrDefaultAsync(x => x.Email == command.Email,ct);


            if (user == null)
            {
                return Error.Unauthorized("Auth.InvalidCredentials","Invalid email or password");
            }


            var valid =_passwordHasher.Verify(command.Password,user.PasswordHash);


            if (!valid)
            {
                return Error.Unauthorized("Auth.InvalidCredentials","Invalid email or password");
            }


            if (!user.IsEmailConfirmed)
            {
                return Error.Validation("Email.NotConfirmed","Email not confirmed");
            }


            var accessToken = await _tokenGenerator.GenerateJwtAccessTokenAsync(user);

            var (refreshToken, refreshTokenExpires) = await _tokenGenerator.GenerateRefreshTokenAsync();

            var refreshRepo = _uow.GetRepository<Domain.Entities.RefreshToken>();

            await refreshRepo.AddAsync(new Domain.Entities.RefreshToken
            {
                ApplicationUserId = user.Id,

                Token = refreshToken,

                Expires = refreshTokenExpires,
            }, ct);



            return new LoginResponse
            {
                AccessToken = accessToken,

                RefreshToken = refreshToken
            };
        }
    }
}
