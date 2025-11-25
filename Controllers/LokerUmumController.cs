using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vocafind_api.DTO;
using vocafind_api.Models;
using vocafind_api.Services;

namespace vocafind_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LokerUmumController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public LokerUmumController(
            TalentcerdasContext context,
            IWebHostEnvironment env,
            IMapper mapper,
            IEmailService emailService,
            ILogger<TalentsController> logger,
            JwtService jwtService)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
            _jwtService = jwtService;
        }

        //Get all loker umum
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LokerUmumDTO>>> GetAll()
        {
            var lokerDTO = await _context.JobVacancies
                .Include(j => j.Company)
                .Where(j => j.Status == "aktif")
                .ProjectTo<LokerUmumDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(lokerDTO);
        }

        // GET: api/LokerUmum/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LokerUmumDetailDTO>> GetById(string id)
        {
            var lokerQuery = _context.JobVacancies
                .Include(j => j.Company)
                .Include(j => j.JobQualifications)
                .Include(j => j.JobBenefits)
                .Include(j => j.JobAdditionalRequirements)
                .Include(j => j.JobAdditionalFacilities)
                .Where(j => j.LowonganId == id && j.Status == "aktif");

            var loker = await lokerQuery
                .ProjectTo<LokerUmumDetailDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (loker == null)
                return NotFound(new { message = "Lowongan tidak ditemukan." });

            return Ok(loker);
        }



        // GET: api/LokerUmum/search?keyword={keyword}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<LokerUmumDTO>>> SearchLoker(
            [FromQuery] string keyword = "")
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Keyword pencarian tidak boleh kosong." });
            }

            var lokerDTO = await _context.JobVacancies
                .Include(j => j.Company)
                .Where(j => j.Status == "aktif" &&
                    (j.Posisi.Contains(keyword) ||
                     j.DeskripsiPekerjaan.Contains(keyword) ||
                     j.Company.NamaPerusahaan.Contains(keyword) ||
                     j.Lokasi.Contains(keyword) ||
                     j.JenisPekerjaan.Contains(keyword) ||
                     j.TingkatPengalaman.Contains(keyword)))
                .ProjectTo<LokerUmumDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(new
            {
                total = lokerDTO.Count,
                data = lokerDTO
            });
        }





        // GET: api/LokerUmum/filter?jenisPekerjaan={jenis}&lokasi={lokasi}&pengalaman={pengalaman}&remote={remote}&rangeGaji={range}
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<LokerUmumDTO>>> FilterLoker(
            [FromQuery] string? jenisPekerjaan = null,
            [FromQuery] string? lokasi = null,
            [FromQuery] bool? remote = null,
            [FromQuery] string? minimalLulusan = null,
            [FromQuery] string? rangeGaji = null)
        {
            var query = _context.JobVacancies
                .Include(j => j.Company)
                .Where(j => j.Status == "aktif")
                .AsQueryable();

            // Filter berdasarkan jenis pekerjaan
            if (!string.IsNullOrWhiteSpace(jenisPekerjaan))
            {
                query = query.Where(j => j.JenisPekerjaan.Contains(jenisPekerjaan));
            }

            // Filter berdasarkan lokasi
            if (!string.IsNullOrWhiteSpace(lokasi))
            {
                query = query.Where(j => j.Lokasi.Contains(lokasi));
            }

           

            // Filter berdasarkan opsi kerja remote
            if (remote.HasValue)
            {
                query = query.Where(j => j.OpsiKerjaRemote == remote.Value);
            }

            // Filter berdasarkan minimal lulusan
            if (!string.IsNullOrWhiteSpace(minimalLulusan))
            {
                query = query.Where(j => j.MinimalLulusan == minimalLulusan);
            }

            // Filter berdasarkan range gaji
            if (!string.IsNullOrWhiteSpace(rangeGaji))
            {
                var allJobs = await query.ToListAsync();

                switch (rangeGaji.ToLower())
                {
                    case "dibawah10jt":
                    case "below10m":
                        allJobs = allJobs.Where(j => GetMaxSalary(j.Gaji) < 10000000).ToList();
                        break;

                    case "10jt-20jt":
                    case "10m-20m":
                        allJobs = allJobs.Where(j =>
                        {
                            var maxGaji = GetMaxSalary(j.Gaji);
                            return maxGaji >= 10000000 && maxGaji <= 20000000;
                        }).ToList();
                        break;

                    case "diatas20jt":
                    case "above20m":
                        allJobs = allJobs.Where(j => GetMaxSalary(j.Gaji) > 20000000).ToList();
                        break;
                }

                var lokerDTOWithGaji = _mapper.Map<List<LokerUmumDTO>>(allJobs);

                return Ok(new
                {
                    total = lokerDTOWithGaji.Count,
                    data = lokerDTOWithGaji
                });
            }

            var lokerDTO = await query
                .ProjectTo<LokerUmumDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(new
            {
                total = lokerDTO.Count,
                data = lokerDTO
            });
        }




        // GET: api/LokerUmum/search-filter?keyword={keyword}&jenisPekerjaan={jenis}&lokasi={lokasi}&pengalaman={pengalaman}&remote={remote}&rangeGaji={range}
        [HttpGet("search-filter")]
        public async Task<ActionResult<IEnumerable<LokerUmumDTO>>> SearchAndFilterLoker(
            [FromQuery] string? keyword = null,
            [FromQuery] string? jenisPekerjaan = null,
            [FromQuery] string? lokasi = null,
            [FromQuery] bool? remote = null,
            [FromQuery] string? minimalLulusan = null,
            [FromQuery] string? rangeGaji = null)
        {
            var query = _context.JobVacancies
                .Include(j => j.Company)
                .Where(j => j.Status == "aktif")
                .AsQueryable();

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(j =>
                    j.Posisi.Contains(keyword) ||
                    j.DeskripsiPekerjaan.Contains(keyword) ||
                    j.Company.NamaPerusahaan.Contains(keyword) ||
                    j.Lokasi.Contains(keyword) ||
                    j.JenisPekerjaan.Contains(keyword));
            }

            // Filter berdasarkan jenis pekerjaan
            if (!string.IsNullOrWhiteSpace(jenisPekerjaan))
            {
                query = query.Where(j => j.JenisPekerjaan.Contains(jenisPekerjaan));
            }

            // Filter berdasarkan lokasi
            if (!string.IsNullOrWhiteSpace(lokasi))
            {
                query = query.Where(j => j.Lokasi.Contains(lokasi));
            }

          
            // Filter berdasarkan opsi kerja remote
            if (remote.HasValue)
            {
                query = query.Where(j => j.OpsiKerjaRemote == remote.Value);
            }

            // Filter berdasarkan minimal lulusan
            if (!string.IsNullOrWhiteSpace(minimalLulusan))
            {
                query = query.Where(j => j.MinimalLulusan == minimalLulusan);
            }

            // Filter berdasarkan range gaji
            if (!string.IsNullOrWhiteSpace(rangeGaji))
            {
                var allJobs = await query.ToListAsync();

                switch (rangeGaji.ToLower())
                {
                    case "dibawah10jt":
                    case "below10m":
                        allJobs = allJobs.Where(j => GetMaxSalary(j.Gaji) < 10000000).ToList();
                        break;

                    case "10jt-20jt":
                    case "10m-20m":
                        allJobs = allJobs.Where(j =>
                        {
                            var maxGaji = GetMaxSalary(j.Gaji);
                            return maxGaji >= 10000000 && maxGaji <= 20000000;
                        }).ToList();
                        break;

                    case "diatas20jt":
                    case "above20m":
                        allJobs = allJobs.Where(j => GetMaxSalary(j.Gaji) > 20000000).ToList();
                        break;
                }

                var lokerDTOWithGaji = _mapper.Map<List<LokerUmumDTO>>(allJobs);

                return Ok(new
                {
                    total = lokerDTOWithGaji.Count,
                    data = lokerDTOWithGaji
                });
            }

            var lokerDTO = await query
                .ProjectTo<LokerUmumDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(new
            {
                total = lokerDTO.Count,
                data = lokerDTO
            });
        }




        // Helper method untuk mendapatkan nilai gaji maksimum dari range
        // Format: "Rp 7.000.000 - Rp 8.000.000" atau "Rp 7.000.000"
        private decimal GetMaxSalary(string gaji)
        {
            if (string.IsNullOrWhiteSpace(gaji))
                return 0;

            try
            {
                // Jika ada range (tanda "-"), ambil nilai maksimum (setelah tanda "-")
                if (gaji.Contains("-"))
                {
                    var parts = gaji.Split('-');
                    if (parts.Length == 2)
                    {
                        return ParseSalaryString(parts[1].Trim());
                    }
                }

                // Jika tidak ada range, parse langsung
                return ParseSalaryString(gaji);
            }
            catch
            {
                return 0;
            }
        }




        // Helper method untuk parsing string gaji menjadi decimal
        private decimal ParseSalaryString(string salaryStr)
        {
            if (string.IsNullOrWhiteSpace(salaryStr))
                return 0;

            // Hapus semua karakter non-digit (Rp, titik, spasi, dll)
            var numericString = new string(salaryStr.Where(char.IsDigit).ToArray());

            if (decimal.TryParse(numericString, out decimal result))
                return result;

            return 0;
        }


        // GET: api/LokerUmum/locations
        [HttpGet("locations")]
        public async Task<ActionResult<IEnumerable<string>>> GetLocations()
        {
            try
            {
                var locations = await _context.JobVacancies
                    .Where(j => j.Status == "aktif" && !string.IsNullOrEmpty(j.Lokasi))
                    .Select(j => j.Lokasi.Trim())
                    .Distinct()
                    .OrderBy(l => l)
                    .ToListAsync();

                return Ok(new
                {
                    total = locations.Count,
                    data = locations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting locations");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data lokasi." });
            }
        }
    }
}