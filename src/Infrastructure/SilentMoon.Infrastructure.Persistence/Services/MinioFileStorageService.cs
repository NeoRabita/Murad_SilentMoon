using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SilentMoon.Application.DTOs.Storage;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class MinioFileStorageService : IFileStorageService
    {
        // IHttpClientFactory istifadə et — static HttpClient connection pool problemini həll edir
        private static readonly HttpClient HttpClient = new();

        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _settings;

        public MinioFileStorageService(IMinioClient minioClient, IOptions<MinioSettings> settings)
        {
            _minioClient = minioClient;
            _settings = settings.Value;
        }

        public async Task<string> GetPresignedUrlAsync(MinioBucket bucket, string objectKey, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                return null;

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
            var presignedUrl = await GetPresignedUrlAsync(bucket, objectKey, ct);

            var request = new HttpRequestMessage(HttpMethod.Get, presignedUrl);

            if (rangeStart.HasValue || rangeEnd.HasValue)
            {
                request.Headers.Range = new RangeHeaderValue(rangeStart, rangeEnd);
            }

            // ResponseHeadersRead → body-ni stream olaraq al, tam yükləmə
            // response-u DISPOSE ETMİRİK — stream response-a bağlıdır
            var response = await HttpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct);

            // EnsureSuccessStatusCode() yerinə — exception atmırıq, caller Result.Failure alır
            if (!response.IsSuccessStatusCode)
            {
                response.Dispose();
                return null; // handler NotFound qaytaracaq
            }

            var contentRange = response.Content.Headers.ContentRange;
            var totalSize    = contentRange?.Length ?? response.Content.Headers.ContentLength ?? 0;
            var start        = contentRange?.From   ?? 0;
            var end          = contentRange?.To     ?? (totalSize > 0 ? totalSize - 1 : 0);
            var isPartial    = response.StatusCode == HttpStatusCode.PartialContent;

            // Network stream-i MemoryStream-ə kopyala ki response dispose olsun
            // Audio fayllar üçün yer problemi olmaya biləcək, amma bu daha etibarlıdır.
            // Böyük fayllar üçün aşağıdakı PassthroughStream yanaşması daha yaxşıdır.
            var memoryStream = new MemoryStream();
            await response.Content.CopyToAsync(memoryStream, ct);
            response.Dispose(); // artıq copy etdik, dispose etmək olar

            memoryStream.Position = 0;

            return new ObjectRangeResult
            {
                Content     = memoryStream,
                TotalSize   = totalSize,
                RangeStart  = start,
                RangeEnd    = end,
                ContentType = response.Content.Headers.ContentType?.MediaType ?? "audio/mpeg",
                IsPartial   = isPartial
            };
        }

        private string BucketNameFor(MinioBucket bucket) => bucket switch
        {
            MinioBucket.Icons   => _settings.IconsBucketName,
            MinioBucket.Tracks  => _settings.TracksBucketName,
            _                   => _settings.BucketName
        };
    }
}
