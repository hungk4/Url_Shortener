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

    public async Task<string> CreateShortUrl(string originalUrl, string? customKey = null)
    {   
        if( customKey != null)
        {
            // Check trùng 
            

        }
        var url = new Url { OriginalUrl = originalUrl, ShortKey = "" };
        _db.Urls.Add(url);
        await _db.SaveChangesAsync();

        var shortKey = Base62Helper.Encode(url.Id);
        url.ShortKey = shortKey;
        await _db.SaveChangesAsync();

        return shortKey;
    }

    public async Task<string?> GetOriginalUrl(string key)
    {
        var url = await _db.Urls
            .FirstOrDefaultAsync(u => u.ShortKey == key && u.IsActive);

        return url?.OriginalUrl;
    }
}