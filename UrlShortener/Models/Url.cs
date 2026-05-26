using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Models;

[Table("urls")]
[Index(nameof(ShortKey), IsUnique = true, Name = "idx_short_key")]
public class Url
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("short_key")]
    [Required]
    [MaxLength(10)]
    public string ShortKey { get; set; } = string.Empty;

    [Column("original_url")]
    [Required]
    public string OriginalUrl { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}