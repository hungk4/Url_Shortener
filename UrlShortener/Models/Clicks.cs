using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Models;


[Table("Clicks")]
public class Clicks
{
    [Key]
    [Column("id")]
    public long Id { get; set;}

    [Column("short_key")]
    public string ShortKey { get; set; } = string.Empty;

    [Column("ip")]
    public string Ip { get; set; } = string.Empty;

    [Column("user_agent")]
    public string UserAgent { get; set; } = string.Empty; // Chrome Windows, Safari iPhone

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("referrer")]
    public string? Referrer { get; set; } // Trang nào dẫn người dùng đến short link (facebook.com, tiktok.com)

    [Column("device_type")]
    public string DeviceType { get; set; } = string.Empty; // mobile, desktop

}