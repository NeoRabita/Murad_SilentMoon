using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IOtpSender
    {
        Task SendAsync(int userId, string email, string firstName, CancellationToken ct = default);
    }
}
