using System.Diagnostics;
using Ada.UrlShortner.Data;
using Ada.UrlShortner.Models;
using Ada.UrlShortner.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Ada.UrlShortner.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;
        private readonly UrlRedirectService _urlRedirectService;

        public HomeController(AppDbContext db, 
                              ILogger<HomeController> logger,
                              IMemoryCache cache,
                              UrlRedirectService urlRedirectService)
        {
            _db = db;
            _logger = logger;
            _cache = cache;
            _urlRedirectService = urlRedirectService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/{shortCode}")]
        public async Task<IActionResult> ShortCode(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                TempData["Error"] = "Invalid short link.";
                return RedirectToAction(nameof(Index));
            }

            var (result, url, domain) = await _urlRedirectService.GetRedirectResultAsync(shortCode);

            switch (result)
            {
                case RedirectResultType.Direct:
                    return Redirect(url!);

                case RedirectResultType.ShowPreview:
                    ViewBag.TargetDomain = domain;
                    ViewBag.FullUrl = url;
                    ViewBag.ConfirmedUrl = $"/go/{shortCode}"; // Final redirect route
                    return View("Preview");

                case RedirectResultType.NotFound:
                default:
                    TempData["Error"] = $"The short link '/{shortCode}' does not exist.";
                    return RedirectToAction(nameof(Index));
            }
        }
        private async Task IncrementClickAsync(string shortCode)
        {
            var record = await _db.UrlRecords
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode);
            if (record != null)
            {
                record.ClickCount++;
                _db.UrlRecords.Update(record);
                await _db.SaveChangesAsync();

                _cache.Set(shortCode, record, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
            }
        }

        [Route("/go/{shortCode}")]
        public async Task<IActionResult> Go(string shortCode)
        {
            var (result, url, domain) = await _urlRedirectService.GetRedirectResultAsync(shortCode);

            if (result == RedirectResultType.NotFound)
            {
                TempData["Error"] = "Link not found.";
                return RedirectToAction(nameof(Index));
            }

            await IncrementClickAsync(shortCode);

            return Redirect(url!);
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.Identity!.Name;
            var userUrls = await _db.UrlRecords
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            ViewBag.UserUrls = userUrls;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
