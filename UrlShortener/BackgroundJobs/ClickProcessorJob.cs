using StackExchange.Redis;
using UrlShortener.Data;
using UrlShortener.Models;
using UAParser;

namespace UrlShortener.BackgroundJobs;

public class ClickProcessorJob : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceScopeFactory _scopeFactory;

    public ClickProcessorJob(IConnectionMultiplexer redis, IServiceScopeFactory scopeFactory)
    {
        _redis = redis;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var db = _redis.GetDatabase();

        while (!stoppingToken.IsCancellationRequested)
        {
            // 1. Lấy từ queue (chờ tối đa 5s nếu queue trống)
            var data = await db.ListLeftPopAsync("clicks_queue");
            if (data.IsNullOrEmpty)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            // 2. Parse data
            var parts = data.ToString().Split('|');
            if (parts.Length < 5) continue;

            var shortKey  = parts[0];
            // var createdAt = DateTime.Parse(parts[1]);
            var createdAt = DateTime.Parse(parts[1]).ToUniversalTime(); // createdAt lưu trong redis theo giờ UTC, chuyển về UTC để tránh lỗi Npgsql khi lưu vào DB PostgreSQL (timestamptz)
            var ip        = parts[2];
            var userAgent = parts[3];
            var referrer  = parts[4];

            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(userAgent);
            var deviceType = clientInfo.Device.Family == "Other" ? "desktop" : "mobile";

            // 3. Lưu DB
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Clicks.Add(new Clicks
            {
                ShortKey  = shortKey,
                Ip        = ip,
                UserAgent = userAgent,
                Referrer  = referrer,
                CreatedAt = createdAt, 
                DeviceType = deviceType
            });

            await dbContext.SaveChangesAsync();
        }
    }
}