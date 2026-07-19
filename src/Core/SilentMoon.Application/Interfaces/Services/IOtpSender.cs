using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IOtpSender
    {
        Task SendAsync(string email, string firstName, string purpose, CancellationToken ct = default);
    }
}
