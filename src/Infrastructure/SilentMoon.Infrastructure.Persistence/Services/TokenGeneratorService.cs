using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.Security;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using SilentMoon.Infrastructure.Persistence.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class TokenGeneratorService : ITokenGeneratorService
    {
        private readonly APIAppSettings _settings;


        public TokenGeneratorService(
            IOptions<APIAppSettings> settings)
        {
            _settings = settings.Value;
        }


        public Task<List<Claim>> CreateClaims(
            ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Name,
                    user.UserName)
            };


            return Task.FromResult(claims);
        }



        public Task<string> GenerateJwtAccessTokenAsync(
            List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JWTSettings.Key));


            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _settings.JWTSettings.Issuer,

                audience: _settings.JWTSettings.Audience,

                claims: claims,

                expires: DateTime.UtcNow.AddMinutes(
                    _settings.JWTSettings.DurationInMinutes),

                signingCredentials: credentials);


            return Task.FromResult(
                new JwtSecurityTokenHandler()
                .WriteToken(token));
        }




        public Task<string> GenerateRefreshTokenAsync(
            List<Claim> claims,
            int userId)
        {
            var refreshToken =
                Guid.NewGuid().ToString();


            return Task.FromResult(refreshToken);
        }
    }
}