using SilentMoon.Application.Interfaces.Caching;
using System;
using System.Threading.Tasks;

namespace SilentMoon.Application.Common.Extensions
{
    public static class CacheExtensions
    {
        public static async Task<T> GetOrAddAsync<T>(
            this ICacheService cacheService,
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiration = null)
        {
            var cachedData = await cacheService.GetAsync<T>(key);

            if (cachedData != null)
                return cachedData;

            var result = await factory();

            if (result != null)
                await cacheService.SetAsync(key, result, expiration);

            return result;
        }

        public static string EmailVerification(int userId) =>
          $"otp:user:{userId}";
    }
}