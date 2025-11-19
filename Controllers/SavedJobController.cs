using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using vocafind_api.DTO;
using vocafind_api.Models;

namespace vocafind_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedJobsController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly ILogger<SavedJobsController> _logger;
        private readonly IMapper _mapper;

        public SavedJobsController(TalentcerdasContext context, ILogger<SavedJobsController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        //---------------------------------------------------GET SAVED JOBS----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpGet("saya")]
        public async Task<IActionResult> GetSavedJobs()
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                _logger.LogInformation($"🔍 Memulai GetSavedJobs untuk talent: {talentId}");

                // 1. Cek apakah talent ada
                var talentExists = await _context.Talents.AnyAsync(t => t.TalentId == talentId);
                _logger.LogInformation($"✅ Talent exists: {talentExists}");

                if (!talentExists)
                {
                    return NotFound(new { message = "Talent tidak ditemukan." });
                }

                // 2. Cek apakah ada saved jobs
                var savedJobsCount = await _context.SavedJob
                    .Where(s => s.TalentId == talentId)
                    .CountAsync();

                _logger.LogInformation($"📊 Jumlah saved jobs: {savedJobsCount}");

                if (savedJobsCount == 0)
                {
                    _logger.LogInformation("📭 Tidak ada saved jobs, return empty array");
                    return Ok(new List<object>());
                }

                // 3. Cek data sample untuk debug
                var sampleSavedJob = await _context.SavedJob
                    .Where(s => s.TalentId == talentId)
                    .FirstOrDefaultAsync();

                if (sampleSavedJob != null)
                {
                    _logger.LogInformation($"📋 Sample SavedJob - ID: {sampleSavedJob.saved_job_ID}, LowonganId: {sampleSavedJob.LowonganId}, TalentId: {sampleSavedJob.TalentId}");
                }

                // 4. Cek apakah lowongan terkait ada
                var lowonganExists = await _context.JobVacancies.AnyAsync();
                _logger.LogInformation($"🏢 JobVacancies exists: {lowonganExists}");

                var companiesExists = await _context.Companies.AnyAsync();
                _logger.LogInformation($"🏛️ Companies exists: {companiesExists}");

                // 5. Eksekusi query utama dengan try-catch detail
                _logger.LogInformation("🚀 Menjalankan query utama...");

                var savedJobs = await (from saved in _context.SavedJob
                                       join lowongan in _context.JobVacancies on saved.LowonganId equals lowongan.LowonganId
                                       join company in _context.Companies on lowongan.CompanyId equals company.CompanyId
                                       where saved.TalentId == talentId
                                       orderby saved.CreatedAt descending
                                       select new
                                       {
                                           saved.saved_job_ID,
                                           saved.LowonganId,
                                           saved.CreatedAt,
                                           Lowongan = new
                                           {
                                               lowongan.Posisi,
                                               lowongan.DeskripsiPekerjaan,
                                               lowongan.Lokasi,
                                               lowongan.Gaji,
                                               lowongan.MinimalLulusan,
                                               lowongan.JenisPekerjaan,
                                               lowongan.TingkatPengalaman,
                                               lowongan.Status,
                                               lowongan.TanggalPosting,
                                               lowongan.BatasLamaran,
                                               lowongan.JumlahPelamar,
                                               lowongan.BatasPelamar,
                                               lowongan.OpsiKerjaRemote,
                                               Company = new
                                               {
                                                   company.NamaPerusahaan,
                                                   company.Logo,
                                                   company.BidangUsaha
                                               }
                                           }
                                       }).ToListAsync();

                _logger.LogInformation($"✅ Query berhasil! Jumlah hasil: {savedJobs.Count}");

                return Ok(savedJobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Gagal mengambil data saved jobs");

                // Log detail error
                _logger.LogError($"📛 Error Message: {ex.Message}");
                _logger.LogError($"📛 Inner Exception: {ex.InnerException?.Message}");
                _logger.LogError($"📛 Stack Trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    message = "Terjadi kesalahan saat mengambil data lowongan tersimpan.",
                    detail = ex.Message // Untuk development
                });
            }
        }

        //---------------------------------------------------SAVE JOB----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpPost("simpan/{lowonganId}")]
        public async Task<IActionResult> SaveJob(string lowonganId)
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                // Validasi: Apakah lowongan dengan ID ini ada?
                var lowongan = await _context.JobVacancies
                    .FirstOrDefaultAsync(j => j.LowonganId == lowonganId);

                if (lowongan == null)
                {
                    return NotFound(new { message = "Lowongan tidak ditemukan." });
                }

                // Cek apakah sudah disimpan sebelumnya
                var sudahDisimpan = await _context.SavedJob
                    .AnyAsync(s => s.LowonganId == lowonganId && s.TalentId == talentId);

                if (sudahDisimpan)
                {
                    return BadRequest(new { message = "Lowongan sudah disimpan sebelumnya." });
                }

                // Create saved job
                var savedJob = new SavedJob
                {
                    saved_job_ID = Guid.NewGuid().ToString(),
                    TalentId = talentId,
                    LowonganId = lowonganId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.SavedJob.Add(savedJob);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Lowongan berhasil disimpan." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal menyimpan lowongan");
                return StatusCode(500, new { message = "Terjadi kesalahan saat menyimpan lowongan." });
            }
        }

        //---------------------------------------------------UNSAVE JOB----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpDelete("hapus/{savedJobId}")]
        public async Task<IActionResult> UnsaveJob(string savedJobId)
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                var savedJob = await _context.SavedJob
                    .FirstOrDefaultAsync(s => s.saved_job_ID == savedJobId && s.TalentId == talentId);

                if (savedJob == null)
                {
                    return NotFound(new { message = "Saved job tidak ditemukan." });
                }

                _context.SavedJob.Remove(savedJob);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Lowongan berhasil dihapus dari daftar tersimpan." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal menghapus saved job");
                return StatusCode(500, new { message = "Terjadi kesalahan saat menghapus lowongan tersimpan." });
            }
        }

        //---------------------------------------------------UNSAVE JOB BY LOWONGAN ID----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpDelete("hapus-by-lowongan/{lowonganId}")]
        public async Task<IActionResult> UnsaveJobByLowonganId(string lowonganId)
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                var savedJob = await _context.SavedJob
                    .FirstOrDefaultAsync(s => s.LowonganId == lowonganId && s.TalentId == talentId);

                if (savedJob == null)
                {
                    return NotFound(new { message = "Lowongan tidak ditemukan dalam daftar tersimpan." });
                }

                _context.SavedJob.Remove(savedJob);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Lowongan berhasil dihapus dari daftar tersimpan." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal menghapus saved job by lowongan id");
                return StatusCode(500, new { message = "Terjadi kesalahan saat menghapus lowongan tersimpan." });
            }
        }

        //---------------------------------------------------CHECK IF JOB IS SAVED----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpGet("cek/{lowonganId}")]
        public async Task<IActionResult> CheckIfJobIsSaved(string lowonganId)
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                var isSaved = await _context.SavedJob
                    .AnyAsync(s => s.LowonganId == lowonganId && s.TalentId == talentId);

                return Ok(new { isSaved });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengecek status saved job");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengecek status lowongan." });
            }
        }

        //---------------------------------------------------GET SAVED JOBS COUNT----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpGet("jumlah")]
        public async Task<IActionResult> GetSavedJobsCount()
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                var count = await _context.SavedJob
                    .Where(s => s.TalentId == talentId)
                    .CountAsync();

                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengambil jumlah saved jobs");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil jumlah lowongan tersimpan." });
            }
        }
    }
}