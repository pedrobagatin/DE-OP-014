using System.ComponentModel.DataAnnotations;

namespace Ada.UrlShortner.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
}