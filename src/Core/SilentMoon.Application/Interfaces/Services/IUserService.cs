using System.Security.Claims;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IUserService
    {
        ClaimsPrincipal GetUser();
        public string GetUserId();
        public string GetUserEmail();
    }
}