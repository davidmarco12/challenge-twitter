using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwittetAPI.Domain.Abstractions;

namespace Infrastructure.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, bool> _cacheKeys;
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(15);

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _cacheKeys = new ConcurrentDictionary<string, bool>();
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };


        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                if (_cache.TryGetValue(key, out string? jsonString) && !string.IsNullOrEmpty(jsonString))
                {
                    var result = JsonSerializer.Deserialize<T>(jsonString, JsonOptions);
                    return Task.FromResult(result);
                }
                else
                {
                    return Task.FromResult<T?>(null);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON deserialization error for cache key: {CacheKey}", key);

                // Limpiar cache entry corrupto
                _cache.Remove(key);
                _cacheKeys.TryRemove(key, out _);

                return Task.FromResult<T?>(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving cache key: {CacheKey}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                // ✅ Serializar a JSON
                var jsonString = JsonSerializer.Serialize(value, JsonOptions);

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration,
                    SlidingExpiration = TimeSpan.FromMinutes(5),
                    Priority = CacheItemPriority.Normal,
                    Size = 1
                };

                // Callback para cleanup
                options.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration
                {
                    EvictionCallback = (key, value, reason, state) =>
                    {
                        _cacheKeys.TryRemove(key.ToString()!, out _);
                    }
                });

                _cache.Set(key, jsonString, options);
                _cacheKeys.TryAdd(key, true);


                return Task.CompletedTask;
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON serialization error for cache key: {CacheKey}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error setting cache key: {CacheKey}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _cache.Remove(key);
                _cacheKeys.TryRemove(key, out _);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error removing cache key: {CacheKey}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                var regex = new Regex(pattern, RegexOptions.Compiled);
                var keysToRemove = _cacheKeys.Keys
                    .Where(key => regex.IsMatch(key))
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                    _cacheKeys.TryRemove(key, out _);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error removing cache keys by pattern: {Pattern}", pattern);
                return Task.CompletedTask;
            }
        }
    }
}
