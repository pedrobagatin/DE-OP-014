using System.ComponentModel.DataAnnotations;

namespace Ada.UrlShortner.Models;

public class UrlRecord
{
    public int Id { get; set; }

    [Required]
    [Url]
    public string OriginalUrl { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string ShortCode { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty; 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ClickCount { get; set; } = 0;
}