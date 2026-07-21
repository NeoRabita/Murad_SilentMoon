using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> GetPresignedUrlAsync(string objectKey, CancellationToken ct = default);
    }
}
