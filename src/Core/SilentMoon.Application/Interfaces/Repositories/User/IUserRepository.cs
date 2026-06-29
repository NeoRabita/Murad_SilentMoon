using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Repositories.User
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);

        Task<ApplicationUser?> GetByUserNameAsync(string userName);

        Task AddAsync(ApplicationUser user);
    }
}
