

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Models;

public class UrlStatsResponse
{
    [Key]
    [Column("id")]
    public long Id { get; set;}

    [Column("original_url")]
    public string OriginalUrl { get; set; } = string.Empty;
    [Column("short_key")]
    public string ShortKey { get; set; } = string.Empty;

    [Column("total_clicks")]
    public int TotalClicks { get; set; }

    [Column("clicks_by_date")]
    public List<ClickByDate> ClickByDate { get; set; } = new List<ClickByDate>();

    [Column("clicks_by_device")]
    public List<ClickByDevice> ClickByDevice { get; set; } = new List<ClickByDevice>();
}

public class ClickByDate
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ClickByDevice
{
    public string Device { get; set; } = string.Empty;
    public int Count { get; set; }
}

// {
//   "shortKey": "000001",
//   "totalClicks": 100,
//   "clicksByDate": [
//     { "date": "2026-05-26", "count": 50 },
//     { "date": "2026-05-27", "count": 50 }
//   ],
//   "clicksByDevice": [
//     { "device": "mobile", "count": 60 },
//     { "device": "desktop", "count": 40 }
//   ]
// }