using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vocafind_api.DTO;
using vocafind_api.Models;

namespace vocafind_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobfairController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<JobfairController> _logger;

        public JobfairController(
            TalentcerdasContext context,
            IMapper mapper,
            IWebHostEnvironment env,
            ILogger<JobfairController> logger)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
            _logger = logger;
        }

        // GET: api/Jobfair - Get semua jobfair aktif
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobfairDTO>>> GetAll()
        {
            var jobfairDTO = await _context.AcaraJobfairs
                .Include(j => j.AdminVokasi)
                .Include(j => j.FlyerAcaras)
                .Include(j => j.AcaraJobfairCompanies)
                .Include(j => j.LowonganAcaras)
                    .ThenInclude(la => la.Lowongan)
                .Where(j => j.TanggalSelesaiAcara >= DateOnly.FromDateTime(DateTime.Now))
                .OrderByDescending(j => j.TanggalMulaiAcara)
                .Select(j => new JobfairDTO
                {
                    Id = j.Id,
                    NamaAcara = j.NamaAcara,
                    AcaraBkk = j.AcaraBkk,
                    TanggalAwalPendaftaranAcara= j.TanggalAwalPendaftaranAcara,
                    TanggalAkhirPendaftaranAcara = j.TanggalAwalPendaftaranAcara,
                    TanggalMulaiAcara = j.TanggalMulaiAcara,
                    TanggalSelesaiAcara = j.TanggalSelesaiAcara,
                    MaxCapacity = j.MaxCapacity,
                    TotalLowongan = j.LowonganAcaras.Count(la => la.Lowongan.Status == "aktif"),
                    TotalPerusahaan = j.AcaraJobfairCompanies.Count,
                    NamaAdminVokasi = j.AdminVokasi.Name,
                    FlyerUrl = j.FlyerAcaras.FirstOrDefault().FlyerUrl
                })
                .ToListAsync();

            return Ok(jobfairDTO);
        }

        // GET: api/Jobfair/{id} - Get detail jobfair by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<JobfairDetailDTO>> GetById(ulong id)
        {
            var jobfairQuery = _context.AcaraJobfairs
                .Include(j => j.FlyerAcaras)
                .Include(j => j.AcaraJobfairCompanies)
                    .ThenInclude(ajc => ajc.Company)
                .Include(j => j.LowonganAcaras)
                    .ThenInclude(la => la.Lowongan)
                    .ThenInclude(l => l.Company)
                .Include(j => j.AdminVokasi)
                .Where(j => j.Id == id);

            var jobfair = await jobfairQuery
                .ProjectTo<JobfairDetailDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (jobfair == null)
                return NotFound(new { message = "Jobfair tidak ditemukan." });

            return Ok(jobfair);
        }



        // GET: api/Jobfair/{id}/lowongan - Get lowongan khusus untuk jobfair tertentu
        [HttpGet("{id}/lowongan")]
        public async Task<ActionResult<IEnumerable<LowonganAcaraDTO>>> GetLowonganByJobfair(ulong id)
        {
            var lowonganDTO = await _context.LowonganAcaras
                .Include(la => la.Lowongan)
                    .ThenInclude(l => l.Company)
                .Where(la => la.AcaraJobfairId == id &&
                            la.Lowongan.Status == "aktif")
                .ProjectTo<LowonganAcaraDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(lowonganDTO);
        }

        // POST: api/Jobfair/{id}/flyer - Upload flyer untuk jobfair
        [HttpPost("{id}/flyer")]
        public async Task<ActionResult<FlyerAcaraResponseDTO>> PostFlyer(ulong id, [FromForm] FlyerAcaraRequestDTO request)
        {
            try
            {
                // Cari acara jobfair
                var acara = await _context.AcaraJobfairs
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (acara == null)
                {
                    return NotFound(new { message = "Acara jobfair tidak ditemukan." });
                }

                // TODO: Implementasi authorization check
                // Sesuaikan dengan sistem authentication Anda
                /*
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var adminVokasi = await _context.AdminVokasis
                    .FirstOrDefaultAsync(av => av.AdminVokasiId == adminId);
                
                if (adminVokasi == null || adminVokasi.AdminVokasiId != acara.AdminVokasiId)
                {
                    return Unauthorized(new { message = "Anda tidak memiliki akses untuk menambah flyer." });
                }
                */

                // Validasi file
                if (request.FlyerImage == null || request.FlyerImage.Length == 0)
                {
                    return BadRequest(new { message = "File flyer harus diisi." });
                }

                // Validasi tipe file
                var allowedExtensions = new[] { ".jpeg", ".jpg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(request.FlyerImage.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Format file tidak didukung. Gunakan JPEG, PNG, atau GIF." });
                }

                // Validasi ukuran file (2MB max)
                if (request.FlyerImage.Length > 2 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Ukuran file maksimal 2MB." });
                }

                // Buat direktori jika belum ada
                var flyersDirectory = Path.Combine(_env.WebRootPath, "uploads", "flyer");
                if (!Directory.Exists(flyersDirectory))
                {
                    Directory.CreateDirectory(flyersDirectory);
                }

                // Generate nama file unik
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(flyersDirectory, fileName);
                var relativePath = Path.Combine("uploads", "flyer", fileName).Replace("\\", "/");

                // Simpan file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.FlyerImage.CopyToAsync(stream);
                }

                // Simpan ke database
                var flyerAcara = new FlyerAcara
                {
                    AcaraJobfairId = id,
                    FlyerUrl = relativePath,
                    Title = request.Title,
                    Description = request.Description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.FlyerAcaras.Add(flyerAcara);
                await _context.SaveChangesAsync();

                // Mapping ke response DTO
                var response = _mapper.Map<FlyerAcaraResponseDTO>(flyerAcara);

                return CreatedAtAction(
                    nameof(GetFlyer),
                    new { flyerId = flyerAcara.Id },
                    new
                    {
                        message = "Flyer berhasil ditambahkan.",
                        data = response
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading flyer for jobfair {JobfairId}", id);
                return StatusCode(500, new { message = "Gagal mengunggah flyer: " + ex.Message });
            }
        }

        // GET: api/Jobfair/flyer/{flyerId} - Untuk mendapatkan detail flyer
        [HttpGet("flyer/{flyerId}")]
        public async Task<ActionResult<FlyerAcaraResponseDTO>> GetFlyer(ulong flyerId)
        {
            var flyer = await _context.FlyerAcaras
                .Where(f => f.Id == flyerId)
                .ProjectTo<FlyerAcaraResponseDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (flyer == null)
            {
                return NotFound(new { message = "Flyer tidak ditemukan." });
            }

            return Ok(flyer);
        }

        // GET: api/Jobfair/{id}/flyers - Untuk mendapatkan semua flyer acara
        [HttpGet("{id}/flyers")]
        public async Task<ActionResult<IEnumerable<FlyerAcaraResponseDTO>>> GetFlyersByAcara(ulong id)
        {
            var flyers = await _context.FlyerAcaras
                .Where(f => f.AcaraJobfairId == id)
                .OrderByDescending(f => f.CreatedAt)
                .ProjectTo<FlyerAcaraResponseDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(flyers);
        }

        // DELETE: api/Jobfair/flyer/{flyerId} - Hapus flyer
        [HttpDelete("flyer/{flyerId}")]
        public async Task<IActionResult> DeleteFlyer(ulong flyerId)
        {
            try
            {
                var flyer = await _context.FlyerAcaras
                    .Include(f => f.AcaraJobfair)
                    .FirstOrDefaultAsync(f => f.Id == flyerId);

                if (flyer == null)
                {
                    return NotFound(new { message = "Flyer tidak ditemukan." });
                }

                // TODO: Implementasi authorization check
                /*
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var adminVokasi = await _context.AdminVokasis
                    .FirstOrDefaultAsync(av => av.AdminVokasiId == adminId);
                
                if (adminVokasi == null || adminVokasi.AdminVokasiId != flyer.AcaraJobfair.AdminVokasiId)
                {
                    return Unauthorized(new { message = "Anda tidak memiliki akses untuk menghapus flyer." });
                }
                */

                // Hapus file fisik
                var filePath = Path.Combine(_env.WebRootPath, flyer.FlyerUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Hapus dari database
                _context.FlyerAcaras.Remove(flyer);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Flyer berhasil dihapus." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting flyer {FlyerId}", flyerId);
                return StatusCode(500, new { message = "Gagal menghapus flyer: " + ex.Message });
            }
        }
    }
}