using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Authentication
{
    public interface ITokenGeneratorService
    {
        Task<string> GenerateJwtAccessTokenAsync(ApplicationUser user);
        Task<(string Token, DateTime Expires)> GenerateRefreshTokenAsync();
    }
}
