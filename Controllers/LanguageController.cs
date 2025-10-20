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
    public class LanguageController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public LanguageController(
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


        // ✅ GET: Ambil semua bahasa
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var language = await _context.Languages
                .Where(s => s.TalentId == talentId)
                .ProjectTo<LanguageGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(language);
        }


        // ✅ POST: Tambah bahasa
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LanguagePostDTO dto)
        {
            var language = _mapper.Map<Language>(dto);
            language.LanguageId = Guid.NewGuid().ToString();
            language.CreatedAt = DateTime.Now;
            language.UpdatedAt = DateTime.Now;

            _context.Languages.Add(language);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bahasa berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update bahasa
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] LanguagePutDTO dto)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language == null) return NotFound();

            _mapper.Map(dto, language);  // Langsung timpa seluruh field DTO ke model
            language.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Bahasa berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus pendidikan
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language == null) return NotFound();

            _context.Languages.Remove(language);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bahasa berhasil dihapus" });
        }
    }
}
