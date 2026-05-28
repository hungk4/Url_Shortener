namespace UrlShortener.Services;
public interface IClickService
{
    Task EnqueueClick(string shortKey, string ip, string userAgent, string? referrer);
}