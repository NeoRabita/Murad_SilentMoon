using Microsoft.AspNetCore.Http;
using SilentMoon.Application.Interfaces.Authentication;
using System;
using System.Security.Claims;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(value, out var userId))
                {
                    throw new InvalidOperationException("No authenticated user found in the current request.");
                }

                return userId;
            }
        }

        public string UserName =>
            _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.Name)?.Value;
    }
}
