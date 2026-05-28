using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;

namespace UrlShortener.Controllers;

[ApiController]
[Route("api/urls")]
public class UrlController : ControllerBase
{
    private readonly IUrlService _urlService;
    private readonly IClickService _clickService;

    public UrlController(IUrlService urlService, IClickService clickService)
    {
        _urlService = urlService;
        _clickService = clickService;
    }

    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] ShortenRequest request)
    {
        if (string.IsNullOrEmpty(request.OriginalUrl))
            return BadRequest("URL không được để trống");

        if (!request.OriginalUrl.StartsWith("http://") && !request.OriginalUrl.StartsWith("https://"))
            return BadRequest("URL không hợp lệ. Vui lòng nhập đầy đủ https://...");

        if (request.OriginalUrl.Length > 2048)
            return BadRequest("URL quá dài. Vui lòng nhập URL có độ dài tối đa 2048 ký tự.");

        if (request.CustomKey != null && request.CustomKey.Length > 10)
            return BadRequest("Custom key không được quá 10 ký tự.");

        try
        {
            var shortKey = await _urlService.CreateShortUrl(
                request.OriginalUrl,
                request.CustomKey,
                request.expiresAt
            );
            var shortUrl = $"{Request.Scheme}://{Request.Host}/{shortKey}";
            return Ok(new { shortUrl });
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet("/{key}")]
    public async Task<IActionResult> RedirectOriginalUrl(string key)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = Request.Headers.UserAgent.ToString();
        var referrer = Request.Headers["Referer"].ToString();
        Console.WriteLine($"Redirect request: key={key}, ip={ip}, userAgent={userAgent}, referrer={referrer}");

        var (status, originalUrl) = await _urlService.GetOriginalUrl(key);

        if (status == UrlResult.Found)
            await _clickService.EnqueueClick(key, ip, userAgent, referrer);

        return status switch
        {
            UrlResult.NotFound => NotFound("Link không tồn tại"),
            UrlResult.Expired => StatusCode(410, "Link đã hết hạn"),
            _ => Redirect(originalUrl!)
        };
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> DeleteShortUrl(string key)
    {
        var result = await _urlService.DeleteShortUrl(key);
        return result ? NoContent() : NotFound("Link không tồn tại");
    }


    [HttpGet("{key}/stats")]
    public async Task<IActionResult> GetUrlStats(string key)
    {
        var stats = await _urlService.GetStats(key);
        return stats == null ? NotFound("Link không tồn tại") : Ok(stats);
    }
}

// Request model
public record ShortenRequest(string OriginalUrl, string? CustomKey = null, DateTime? expiresAt = null);