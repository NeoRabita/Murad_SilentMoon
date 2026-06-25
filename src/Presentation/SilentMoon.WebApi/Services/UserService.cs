using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SilentMoon.Application.Interfaces.Services;

namespace SilentMoon.WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor accessor;

        public UserService(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public ClaimsPrincipal GetUser()
        {
            return accessor?.HttpContext?.User;
        }
        public string GetUserId()
        {
            return accessor?.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        }
        public string GetUserEmail()
        {
            return accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

    }
}