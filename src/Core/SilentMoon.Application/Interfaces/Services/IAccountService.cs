
using System.Threading.Tasks;
using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.DTOs.JWT;
using SilentMoon.Domain.Entities;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<string> RegisterAsync(RegisterRequest request);
        Task<string> SendEmailVerification(ApplicationUser user);
        Task<string> ConfirmEmailAsync(string email, string code);
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request, string ipAddress);
        Task<JwtTokenDto> RevokeByRefreshToken(string token);
        Task<string> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<string> ResetPasswordAsync(ResetPasswordRequest request);
    }
}