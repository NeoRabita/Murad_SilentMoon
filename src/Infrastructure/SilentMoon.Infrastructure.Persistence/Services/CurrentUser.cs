using SilentMoon.Application.Interfaces.Authentication;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class CurrentUser : ICurrentUser
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
    }
}
