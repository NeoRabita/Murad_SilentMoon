using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Authentication
{
    public interface ITokenGeneratorService
    {
        Task<List<Claim>> CreateClaims(ApplicationUser user);
        Task<string> GenerateJwtAccessTokenAsync(List<Claim> claims);
        Task<string> GenerateRefreshTokenAsync(List<Claim> claims, int userId);
    }
}
