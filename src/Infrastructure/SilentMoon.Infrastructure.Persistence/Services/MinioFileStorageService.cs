using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SilentMoon.Application.DTOs.Storage;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class MinioFileStorageService : IFileStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _settings;

        public MinioFileStorageService(IMinioClient minioClient,IOptions<MinioSettings> settings)
        {
            _minioClient = minioClient;
            _settings = settings.Value;
        }

        public async Task<string> GetPresignedUrlAsync(MinioBucket bucket,string objectKey,CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
            {
                return null;
            }

            var args = new PresignedGetObjectArgs()
                .WithBucket(BucketNameFor(bucket))
                .WithObject(objectKey)
                .WithExpiry(_settings.PresignedUrlExpiryInSeconds);

            return await _minioClient.PresignedGetObjectAsync(args);
        }

        public async Task<ObjectRangeResult> GetObjectRangeAsync(
            MinioBucket bucket,
            string objectKey,
            long? rangeStart,
            long? rangeEnd,
            CancellationToken ct = default)
        {
            var bucketName = BucketNameFor(bucket);

            var stat = await _minioClient.StatObjectAsync(
                new StatObjectArgs().WithBucket(bucketName).WithObject(objectKey),
                ct);

            var totalSize = stat.Size;

            var start = rangeStart ?? 0;

            var end = rangeEnd ?? totalSize - 1;

            if (end >= totalSize)
            {
                end = totalSize - 1;
            }

            var length = end - start + 1;

            var buffer = new MemoryStream();

            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey)
                    .WithOffsetAndLength(start, length)
                    .WithCallbackStream(stream => stream.CopyTo(buffer)),
                ct);

            buffer.Position = 0;

            return new ObjectRangeResult
            {
                Content = buffer,
                TotalSize = totalSize,
                RangeStart = start,
                RangeEnd = end,
                ContentType = stat.ContentType
            };
        }

        private string BucketNameFor(MinioBucket bucket) => bucket switch
        {
            MinioBucket.Icons => _settings.IconsBucketName,
            MinioBucket.Tracks => _settings.TracksBucketName,
            _ => _settings.BucketName
        };
    }
}
