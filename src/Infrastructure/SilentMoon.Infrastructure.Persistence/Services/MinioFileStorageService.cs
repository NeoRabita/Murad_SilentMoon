using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class MinioFileStorageService : IFileStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _settings;

        public MinioFileStorageService(
            IMinioClient minioClient,
            IOptions<MinioSettings> settings)
        {
            _minioClient = minioClient;
            _settings = settings.Value;
        }

        public async Task<string> GetPresignedUrlAsync(
            string objectKey,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
            {
                return null;
            }

            var args = new PresignedGetObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(objectKey)
                .WithExpiry(_settings.PresignedUrlExpiryInSeconds);

            return await _minioClient.PresignedGetObjectAsync(args);
        }
    }
}
