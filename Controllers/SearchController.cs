using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using WebScrapingApi.Services;

namespace WebScrapingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        // Rate limiting (por IP o identificador lógico)
        private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _requestCounts = new();
        private const int REQUEST_LIMIT = 20;
        private static readonly TimeSpan WINDOW_DURATION = TimeSpan.FromMinutes(1);

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
                return BadRequest(new { error = "Missing 'entity' parameter." });

            if (IsRateLimitExceeded(HttpContext))
                return StatusCode(429, new { error = "Too many requests. Limit is 20 per minute." });

            var scraper = new WorldBankScraper();
            var (count, results) = await scraper.SearchEntity(entity);

            return Ok(new
            {
                hits = count,
                results = results
            });
        }

        private bool IsRateLimitExceeded(HttpContext context)
        {

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var now = DateTime.UtcNow;

            if (_requestCounts.TryGetValue(ip, out var entry))
            {
                var (count, windowStart) = entry;

                if (now - windowStart > WINDOW_DURATION)
                {
                    // Reiniciar ventana de tiempo
                    _requestCounts[ip] = (1, now);
                    return false;
                }

                if (count >= REQUEST_LIMIT)
                {
                    return true;
                }

                // Actualizar el contador (sin TryUpdate)
                _requestCounts[ip] = (count + 1, windowStart);
                return false;
            }
            else
            {
                // Primer acceso desde esta IP
                _requestCounts[ip] = (1, now);
                return false;
            }
        }
    }
}
