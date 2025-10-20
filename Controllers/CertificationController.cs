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
    public class CertificationController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public CertificationController(
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


        // ✅ GET: Ambil semua certification
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var certification = await _context.Certifications
                .Where(s => s.TalentId == talentId)
                .ProjectTo<CertificationGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(certification);
        }


        // ✅ POST: Tambah sertifikasi
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CertificationPostDTO dto)
        {
            var certification = _mapper.Map<Certification>(dto);
            certification.CertificationId = Guid.NewGuid().ToString();
            certification.CreatedAt = DateTime.Now;
            certification.UpdatedAt = DateTime.Now;

            _context.Certifications.Add(certification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sertifikasi berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update sertifikasi
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CertificationPutDTO dto)
        {
            var certification = await _context.Certifications.FindAsync(id);
            if (certification == null) return NotFound();

            _mapper.Map(dto, certification);  // Langsung timpa seluruh field DTO ke model
            certification.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sertifikasi berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus sertifikasi
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var certification = await _context.Certifications.FindAsync(id);
            if (certification == null) return NotFound();

            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sertifikasi berhasil dihapus" });
        }
    }
}
