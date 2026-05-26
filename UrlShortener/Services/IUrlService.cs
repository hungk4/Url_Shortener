namespace UrlShortener.Services;

public interface IUrlService
{
    Task<string> CreateShortUrl(string originalUrl);
    Task<string?> GetOriginalUrl(string key)   ; 
};