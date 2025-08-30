using Ada.UrlShortner.Data;
using Ada.UrlShortner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Ada.UrlShortner.Services;

public class UrlRedirectService
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly bool _enablePreview;

    public UrlRedirectService(
        AppDbContext db,
        IMemoryCache cache,
        IConfiguration config)
    {
        _db = db;
        _cache = cache;
        _enablePreview = config.GetValue<bool>("UrlShortener:EnablePreview");
    }

    public async Task<(RedirectResultType result, string? url, string? domain)> GetRedirectResultAsync(string shortCode)
    {
        string url = null!;
        string domain = null!;

        if (!_cache.TryGetValue(shortCode, out UrlRecord record))
        {
            record = (await _db.UrlRecords
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode))!;

            if (record == null)
                return (RedirectResultType.NotFound, null, null);

            var cacheEntry = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            _cache.Set(shortCode, record, cacheEntry);
        }

        url = record!.OriginalUrl;
        domain = GetDomain(record.OriginalUrl);

        if (!_enablePreview)
            return (RedirectResultType.Direct, url, domain);

        var trustedDomains = new[] { "yourcompany.com", "internal.app" };
        if (trustedDomains.Contains(domain, StringComparer.OrdinalIgnoreCase))
            return (RedirectResultType.Direct, url, domain);

        return (RedirectResultType.ShowPreview, url, domain);
    }

    public string GetDomain(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return new Uri("https://example.com").Host;

        return uri.Host.Replace("www.", "");
    }
}
