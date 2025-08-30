using Ada.UrlShortner.Data;
using Ada.UrlShortner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Ada.UrlShortner.Controllers;

[Authorize]
public class UrlController : Controller
{
    private readonly AppDbContext _db;

    public UrlController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Shorten(string OriginalUrl, string? CustomShortCode)
    {
        if (string.IsNullOrWhiteSpace(OriginalUrl))
        {
            TempData["Error"] = "URL is required.";
            return RedirectToAction("Index", "Home");
        }

        //if (!Uri.IsWellFormedUriString(OriginalUrl, UriKind.Absolute))
        //{
        //    TempData["Error"] = "Invalid URL.";
        //    return RedirectToAction("Index", "Home");
        //}
        string normalizedUrl;
        if (OriginalUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            OriginalUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            normalizedUrl = OriginalUrl;
        }
        else
        {
            normalizedUrl = "https://" + OriginalUrl;
        }

        if (!Uri.TryCreate(normalizedUrl, UriKind.Absolute, out Uri? uriResult) ||
        (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            TempData["Error"] = "Invalid URL. Must be a valid web address (e.g. google.com or https://google.com).";
            return RedirectToAction("Index", "Home");
        }

        string userId = User.Identity!.Name!;

        string shortCode = CustomShortCode?.Trim() ?? GenerateShortCode();

        if (!IsValidShortCode(shortCode))
        {
            TempData["Error"] = "Short code can only contain letters, numbers, '-', or '_' and be 3–20 chars.";
            return RedirectToAction("Index", "Home");
        }

        if (await _db.UrlRecords.AnyAsync(u => u.ShortCode == shortCode))
        {
            TempData["Error"] = $"Short code '{shortCode}' is already taken.";
            return RedirectToAction("Index", "Home");
        }

        var urlRecord = new UrlRecord
        {
            OriginalUrl = uriResult.ToString(),
            ShortCode = shortCode,
            UserId = userId!,
            CreatedAt = DateTime.UtcNow
        };

        _db.UrlRecords.Add(urlRecord);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Success! Your short link: <a href='/" + shortCode + "' class='alert-link'>r.lcdev.com.br/" + shortCode + "</a>";
        return RedirectToAction("Dashboard", "Home");
    }

    private static string GenerateShortCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static bool IsValidShortCode(string code)
    {
        return !string.IsNullOrWhiteSpace(code) &&
               code.Length >= 3 && code.Length <= 20 &&
               Regex.IsMatch(code, @"^[a-zA-Z0-9_-]+$");
    }
}