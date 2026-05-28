using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Helpers;
using UrlShortener.Models;
using StackExchange.Redis;

namespace UrlShortener.Services;

public class UrlService : IUrlService
{
    private readonly AppDbContext _db;
    private readonly IDatabase _cache;

    public UrlService(AppDbContext db, IConnectionMultiplexer redis)
    {
        _db = db;
        _cache = redis.GetDatabase();
    }

    public async Task<string> CreateShortUrl(string originalUrl, string? customKey = null, DateTime? expiresAt = null)
    {
        if (customKey != null)
        {
            var exists = await _db.Urls.AnyAsync(u => u.ShortKey == customKey);
            if (exists)
                throw new Exception("Custom key already exists.");

            var url = new Url { OriginalUrl = originalUrl, ShortKey = customKey, ExpiresAt = expiresAt };
            _db.Urls.Add(url);
            await _db.SaveChangesAsync();

            await _cache.StringSetAsync($"url:{customKey}", originalUrl, TimeSpan.FromHours(24));
            
            return customKey; 
        }

        var newUrl = new Url { OriginalUrl = originalUrl, ShortKey = "", ExpiresAt = expiresAt };
        _db.Urls.Add(newUrl);
        await _db.SaveChangesAsync();

        var shortKey = Base62Helper.Encode(newUrl.Id);
        newUrl.ShortKey = shortKey;
        await _db.SaveChangesAsync();

        await _cache.StringSetAsync($"url:{shortKey}", originalUrl, TimeSpan.FromHours(24));

        return shortKey;
    }

    public async Task<(UrlResult status, string? url)> GetOriginalUrl(string key)
    {   

        // 1. Check Cache Redis
        var cachedUrl = await _cache.StringGetAsync($"url:{key}");
        if (cachedUrl.HasValue)
        {
            return (UrlResult.Found, cachedUrl.ToString());
        }

        // 2. Query Database
        var url = await _db.Urls
            .FirstOrDefaultAsync(u => u.ShortKey == key && u.IsActive);

        if (url == null) 
            return (UrlResult.NotFound, null);

        if (url.ExpiresAt != null && url.ExpiresAt < DateTime.UtcNow)
        {
            url.IsActive = false;
            await _db.SaveChangesAsync();
            return (UrlResult.Expired, null);
        }

        // 3. Set Cache Redis
        await _cache.StringSetAsync($"url:{key}", url.OriginalUrl, TimeSpan.FromHours(24));

        return (UrlResult.Found, url.OriginalUrl);
    }

    public async Task<bool> DeleteShortUrl(string key)
    {
        var url = await _db.Urls.FirstOrDefaultAsync(u => u.ShortKey == key);
        if (url == null) return false;

        url.IsActive = false;
        await _db.SaveChangesAsync();

        await _cache.KeyDeleteAsync($"url:{key}");

        return true;
    }

    public async Task<UrlStatsResponse?> GetStats(string key)
    {
        var url = await _db.Urls.FirstOrDefaultAsync(u => u.ShortKey == key);
        if (url == null) return null;

        var clicks = await _db.Clicks
            .Where(c => c.ShortKey == key)
            .ToListAsync();

        return new UrlStatsResponse
        {
            ShortKey     = key,
            OriginalUrl  = url.OriginalUrl,
            TotalClicks  = clicks.Count,
            ClickByDate = clicks
                .GroupBy(c => c.CreatedAt.Date.ToString("yyyy-MM-dd"))
                .Select(g => new ClickByDate { Date = g.Key, Count = g.Count() })
                .ToList(),
            ClickByDevice = clicks
                .GroupBy(c => c.DeviceType)
                .Select(g => new ClickByDevice { Device = g.Key, Count = g.Count() })
                .ToList()
        };
    }
}