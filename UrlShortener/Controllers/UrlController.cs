using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;

namespace UrlShortener.Controllers;

[ApiController]
[Route("api/urls")]
public class UrlController : ControllerBase
{
    private readonly IUrlService _urlService;

    public UrlController(IUrlService urlService)
    {
        _urlService = urlService;
    }

    // POST /api/urls/shorten
    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] ShortenRequest request)
    {
        if (string.IsNullOrEmpty(request.OriginalUrl))
            return BadRequest("URL không được để trống");
        
        if (!request.OriginalUrl.StartsWith("http://") && !request.OriginalUrl.StartsWith("https://"))
            return BadRequest("URL không hợp lệ. Vui lòng nhập đầy đủ https://...");

        var shortKey = await _urlService.CreateShortUrl(request.OriginalUrl);
        var shortUrl = $"{Request.Scheme}://{Request.Host}/{shortKey}";
        // Request.Scheme =  http hoặc https
        // Request.Host =  localhost:5088

        return Ok(new { shortUrl });
    }

    // GET /{key}
    [HttpGet("/{key}")]
    public async Task<IActionResult> RedirectOriginalUrl(string key)
    {
        var originalUrl = await _urlService.GetOriginalUrl(key);

        if (originalUrl == null)
            return NotFound("Link không tồn tại");

        return Redirect(originalUrl);
    }
}

// Request model
public record ShortenRequest(string OriginalUrl);