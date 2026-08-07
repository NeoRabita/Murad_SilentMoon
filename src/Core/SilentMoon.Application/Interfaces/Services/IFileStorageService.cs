using SilentMoon.Application.DTOs.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> GetPresignedUrlAsync(MinioBucket bucket, string objectKey, CancellationToken ct = default);

        Task<ObjectRangeResult> GetObjectRangeAsync(
            MinioBucket bucket,
            string objectKey,
            long? rangeStart,
            long? rangeEnd,
            CancellationToken ct = default);
    }
}
