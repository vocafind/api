using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vocafind_api.Models;

namespace vocafind_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SecurityController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(TalentcerdasContext context, ILogger<SecurityController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET SECURITY STATISTICS
        [HttpGet("stats")]
        public async Task<IActionResult> GetSecurityStats()
        {
            var last24Hours = DateTime.UtcNow.AddHours(-24);

            var stats = new
            {
                FailedLogins = await _context.LoginAttempts
                    .CountAsync(la => !la.IsSuccess && la.AttemptTime > last24Hours),

                SuccessfulLogins = await _context.LoginAttempts
                    .CountAsync(la => la.IsSuccess && la.AttemptTime > last24Hours),

                BlockedIps = await _context.BlockedIps
                    .CountAsync(b => b.BlockedUntil > DateTime.UtcNow),

                TopFailedEmails = await _context.LoginAttempts
                    .Where(la => !la.IsSuccess && la.AttemptTime > last24Hours)
                    .GroupBy(la => la.Email)
                    .Select(g => new { Email = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync(),

                TopFailedIPs = await _context.LoginAttempts
                    .Where(la => !la.IsSuccess && la.AttemptTime > last24Hours)
                    .GroupBy(la => la.IpAddress)
                    .Select(g => new { IP = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync()
            };

            return Ok(stats);
        }

        // GET BLOCKED IPS
        [HttpGet("blocked-ips")]
        public async Task<IActionResult> GetBlockedIps()
        {
            var blockedIps = await _context.BlockedIps
                .Where(b => b.BlockedUntil > DateTime.UtcNow)
                .OrderByDescending(b => b.BlockedAt)
                .ToListAsync();

            return Ok(blockedIps);
        }

        // UNBLOCK IP
        // UNBLOCK IP + RESET FAILED ATTEMPTS
        [HttpPost("unblock-ip")]
        public async Task<IActionResult> UnblockIp([FromBody] string ipAddress)
        {
            // 1. Hapus dari blocked_ips
            var blocked = await _context.BlockedIps
                .Where(b => b.IpAddress == ipAddress)
                .ToListAsync();

            if (!blocked.Any())
                return NotFound(new { message = "IP tidak ditemukan dalam daftar blokir" });

            _context.BlockedIps.RemoveRange(blocked);

            // 2. ✅ HAPUS FAILED ATTEMPTS dari IP ini (5 menit terakhir)
            var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);
            var failedAttempts = await _context.LoginAttempts
                .Where(la =>
                    la.IpAddress == ipAddress &&
                    !la.IsSuccess &&
                    la.AttemptTime > fiveMinutesAgo
                )
                .ToListAsync();

            _context.LoginAttempts.RemoveRange(failedAttempts);

            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ IP unblocked and failed attempts cleared: {IP} ({Count} attempts removed)",
                ipAddress, failedAttempts.Count);

            return Ok(new
            {
                message = $"IP {ipAddress} berhasil di-unblock",
                clearedAttempts = failedAttempts.Count
            });
        }

        // GET LOGIN ATTEMPTS HISTORY
        [HttpGet("login-attempts")]
        public async Task<IActionResult> GetLoginAttempts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var attempts = await _context.LoginAttempts
                .OrderByDescending(la => la.AttemptTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var total = await _context.LoginAttempts.CountAsync();

            return Ok(new
            {
                data = attempts,
                total = total,
                page = page,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }
    }
}