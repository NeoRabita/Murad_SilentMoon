using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Accounts.Commands.ResendOtp;
using SilentMoon.Application.Features.User.Commands.GoogleLogin;
using SilentMoon.Application.Features.User.Commands.Login;
using SilentMoon.Application.Features.User.Commands.Otp;
using SilentMoon.Application.Features.User.Commands.RefreshToken;
using SilentMoon.Application.Features.User.Commands.Resgister;
using SilentMoon.Application.Features.User.Commands.VerifyEmail;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    public class AccountController : BaseController
    {

        [HttpPost("register")]
        public async Task<IResult> Register(
            [FromBody] RegisterCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }


        [HttpPost("verify-email")]
        public async Task<IResult> VerifyEmail(
            [FromBody] VerifyEmailCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }


        [HttpPost("login")]
        public async Task<IResult> Login(
            [FromBody] LoginCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenRequest request)
        {
            var result = await Dispatcher.Send(new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken
            });

            return HandleResult(result);
        }

        [HttpPost("resend-otp")]
        public async Task<IResult> ResendOtp(
        [FromBody] ResendOtpRequest command)
        {
            var result = await Dispatcher.Send(new ResendOtpCommand
            {
                Email = command.Email
            });

            return HandleResult(result);
        }

        [HttpPost("google-login")]
        public async Task<IResult> GoogleLogin(
        [FromBody] GoogleLoginCommand command)
        {
            var result = await Dispatcher.Send(command);

            return HandleResult(result);
        }
    }
}