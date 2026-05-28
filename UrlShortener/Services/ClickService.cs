using StackExchange.Redis;
using UrlShortener.Data;


namespace UrlShortener.Services;


public class ClickService : IClickService
{
    private readonly AppDbContext _db;
    private readonly IDatabase _cache;

    public ClickService(AppDbContext db, IConnectionMultiplexer redis)
    {
        _db = db;
        _cache = redis.GetDatabase();
    }

    public async Task EnqueueClick(string shortKey, string ip, string userAgent, string? referrer)
    {
        var data = $"{shortKey}|{DateTime.UtcNow:o}|{ip}|{userAgent}|{referrer}";
        await _cache.ListRightPushAsync("clicks_queue", data);
    }
}