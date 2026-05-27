namespace UrlShortener.Services;

public interface IUrlService
{
    Task<string> CreateShortUrl(string originalUrl, string? customKey = null);
    Task<string?> GetOriginalUrl(string key)   ; 
};