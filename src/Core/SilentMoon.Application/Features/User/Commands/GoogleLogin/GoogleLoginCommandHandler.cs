using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.User.Commands.Login;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.GoogleAuthService;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.GoogleLogin
{
    public class GoogleLoginCommandHandler: ICommandHandler<GoogleLoginCommand, LoginResponse>
    {

        private readonly IUow _uow;
        private readonly IGoogleAuthService _google;
        private readonly ITokenGeneratorService _token;


        public GoogleLoginCommandHandler(
         IUow uow,
         IGoogleAuthService google,
         ITokenGeneratorService token)
        {
            _uow = uow;
            _google = google;
            _token = token;
        }



        public async Task<Result<LoginResponse>> Handle(GoogleLoginCommand command,CancellationToken ct)
        {

            var googleUser =await _google.ValidateToken(command.IdToken);


            if (googleUser == null)
            {
                return Error.Unauthorized(
                "Google",
                "Invalid token");
            }

            var repo =
             _uow.GetRepository<ApplicationUser>();



            var user =
             await repo.FirstOrDefaultAsync(
             x => x.Email == googleUser.Email,
             ct);



            if (user == null)
            {

                user = new ApplicationUser
                {
                    Email = googleUser.Email,

                    UserName = googleUser.Email,

                    FirstName = googleUser.FirstName,

                    LastName = googleUser.LastName,

                    IsEmailConfirmed = true
                };


                await repo.AddAsync(user, ct);

                await _uow.SaveChangesAsync(ct);

            }

            var access = await _token.GenerateJwtAccessTokenAsync(user);

            var (refresh, _) = await _token.GenerateRefreshTokenAsync();


            return new LoginResponse
            {
                AccessToken = access,

                RefreshToken = refresh
            };


        }

    }
}
