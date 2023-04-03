﻿using System.Text;
using System.Text.Json;
using Common.CachHelper;
using Microsoft.Extensions.Caching.Distributed;

namespace Common.CachHelper;


public static class DistributedCacheExtensions
{
    public static async Task<T?> GetOrSet<T>(this IDistributedCache cache, string key, Func<Task<T>> func, CacheOptions options)
    {
        var val = await cache.GetAsync(key);
        if (val == null)
        {
            var res = await func();
            if (res == null)
                return default;

            await cache.SetCache(key, res, options);
            return res;
        }
        var data = JsonSerializer.Deserialize<T>(val);
        return data;
    }

    public static async Task<T?> GetOrSet<T>(this IDistributedCache cache, string key, Func<Task<T>> func)
    {
        var val = await cache.GetAsync(key);
        if (val == null)
        {
            var res = await func();
            if (res == null)
                return default;

            await cache.SetCache(key, res);
            return res;
        }
        var data = JsonSerializer.Deserialize<T>(val);
        return data;
    }

    public static Task SetCache<T>(this IDistributedCache cache, string key, T value)
    {
        return cache.SetCache(key, value, new CacheOptions());
    }

    private static async Task SetCache<T>(this IDistributedCache cache, string key, T value, CacheOptions options)
    {
        var json = JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(json);

        await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.AbsoluteExpirationCacheFromMinutes),
            SlidingExpiration = TimeSpan.FromMinutes(options.ExpireSlidingCacheFromMinutes),
        });
    }

    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
    {
        var val = await cache.GetAsync(key);
        if (val == null)
            return default;

        var value = JsonSerializer.Deserialize<T>(val);
        return value;
    }

    public static async Task<bool> RemoveAsync<T>(this IDistributedCache cache, string key)
    {
        var val = cache.RemoveAsync(key);
        if (val.IsCompletedSuccessfully)
            return true;

        return false;
    }

    public static async Task<bool> RemoveAsync<T>(this IDistributedCache cache, string key, CacheOptions options)
    {
        var val = cache.RemoveAsync(key);
        if (val.IsCompletedSuccessfully)
            return true;

        return false;
    }
}

public class CacheOptions
{
    public int ExpireSlidingCacheFromMinutes { get; set; } = 5;
    public int AbsoluteExpirationCacheFromMinutes { get; set; } = 10;
}