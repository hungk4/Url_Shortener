using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Helpers;
using UrlShortener.Models;

namespace UrlShortener.Services;

public class UrlService : IUrlService
{
    private readonly AppDbContext _db;

    public UrlService(AppDbContext db)
    {
        _db = db;
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
            return customKey; 
        }

        var newUrl = new Url { OriginalUrl = originalUrl, ShortKey = "", ExpiresAt = expiresAt };
        _db.Urls.Add(newUrl);
        await _db.SaveChangesAsync();

        var shortKey = Base62Helper.Encode(newUrl.Id);
        newUrl.ShortKey = shortKey;
        await _db.SaveChangesAsync();

        return shortKey;
    }

    public async Task<(UrlResult status, string? url)> GetOriginalUrl(string key)
    {
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

        return (UrlResult.Found, url.OriginalUrl);
    }

    public async Task<bool> DeleteShortUrl(string key)
    {
        var url = await _db.Urls.FirstOrDefaultAsync(u => u.ShortKey == key);
        if (url == null) return false;

        url.IsActive = false;
        await _db.SaveChangesAsync();
        return true;
    }
}