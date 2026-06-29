using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using SilentMoon.Application.DTOs.GoogleAuth;
using SilentMoon.Application.Interfaces.GoogleAuthService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly string _clientId;


        public GoogleAuthService(
            IConfiguration configuration)
        {
            _clientId =
                configuration["Google:ClientId"];
        }


        public async Task<GoogleUserInfo?> ValidateToken(
            string token)
        {
            var payload =
                await GoogleJsonWebSignature
                .ValidateAsync(
                    token,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[]
                        {
                        _clientId
                        }
                    });


            return new GoogleUserInfo
            {
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                GoogleId = payload.Subject
            };
        }
    }
}
