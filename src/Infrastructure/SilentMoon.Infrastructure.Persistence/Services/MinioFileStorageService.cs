using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SilentMoon.Application.DTOs.Storage;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class MinioFileStorageService : IFileStorageService
    {
        // Shared instance - HttpClient is designed to be reused across calls rather
        // than created per-request (avoids socket exhaustion under load).
        private static readonly HttpClient HttpClient = new();

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
            // The MinIO .NET SDK's GetObjectAsync throws PartialContentException on a
            // successful ranged (206) response and doesn't reliably deliver the bytes
            // through its callback stream in that case. Going through the presigned URL
            // with a plain HTTP request sidesteps that SDK bug entirely.
            var presignedUrl = await GetPresignedUrlAsync(bucket, objectKey, ct);

            var request = new HttpRequestMessage(HttpMethod.Get, presignedUrl);

            if (rangeStart.HasValue || rangeEnd.HasValue)
            {
                request.Headers.Range = new RangeHeaderValue(rangeStart, rangeEnd);
            }

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

            response.EnsureSuccessStatusCode();

            var contentRange = response.Content.Headers.ContentRange;

            var totalSize = contentRange?.Length ?? response.Content.Headers.ContentLength ?? 0;

            var start = contentRange?.From ?? 0;

            var end = contentRange?.To ?? totalSize - 1;

            var stream = await response.Content.ReadAsStreamAsync(ct);

            return new ObjectRangeResult
            {
                Content = stream,
                TotalSize = totalSize,
                RangeStart = start,
                RangeEnd = end,
                ContentType = response.Content.Headers.ContentType?.ToString(),
                IsPartial = response.StatusCode == HttpStatusCode.PartialContent
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
