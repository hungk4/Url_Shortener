using UrlShortener.Models;

namespace UrlShortener.Services;

public interface IUrlService
{
    Task<string> CreateShortUrl(string originalUrl, string? customKey = null, DateTime? expiresAt = null);
    Task<(UrlResult status, string? url)> GetOriginalUrl(string key);
    Task<bool> DeleteShortUrl(string key);

    Task<UrlStatsResponse?> GetStats(string key);
}