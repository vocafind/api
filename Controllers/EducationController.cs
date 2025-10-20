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
    public class EducationController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public EducationController(
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



        // ✅ GET: Ambil semua pendidikan talent
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var education = await _context.Educations
                .Where(s => s.TalentId == talentId)
                .ProjectTo<EducationGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(education);
        }



        // ✅ POST: Tambah pendidikan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EducationPostDTO dto)
        {
            var education = _mapper.Map<Education>(dto);
            education.EducationId = Guid.NewGuid().ToString();
            education.CreatedAt = DateTime.Now;
            education.UpdatedAt = DateTime.Now;

            _context.Educations.Add(education);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pendidikan berhasil ditambahkan" });
        }



        // 🛠 PATCH: Update pendidikan
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] EducationPutDTO dto)
        {
            var education = await _context.Educations.FindAsync(id);
            if (education == null) return NotFound();

            _mapper.Map(dto, education);  // Langsung timpa seluruh field DTO ke model
            education.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Pendidikan berhasil diperbarui" });
        }



        // ❌ DELETE: Hapus pendidikan
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var education = await _context.Educations.FindAsync(id);
            if (education == null) return NotFound();

            _context.Educations.Remove(education);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pendidikan berhasil dihapus" });
        }
    }
}
