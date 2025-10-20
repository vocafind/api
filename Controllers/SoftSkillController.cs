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
    public class SoftSkillController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public SoftSkillController(
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


        // ✅ GET: Ambil semua soft skill
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var softskill = await _context.SoftSkills
                .Where(s => s.TalentId == talentId)
                .ProjectTo<SoftSkillGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(softskill);
        }


        // ✅ POST: Tambah penghargaan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SoftSkillPostDTO dto)
        {
            var softskill = _mapper.Map<SoftSkill>(dto);
            softskill.SoftskillsId = Guid.NewGuid().ToString();
            softskill.CreatedAt = DateTime.Now;
            softskill.UpdatedAt = DateTime.Now;

            _context.SoftSkills.Add(softskill);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Soft Skill berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update bahasa
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SoftSkillPutDTO dto)
        {
            var softskill = await _context.SoftSkills.FindAsync(id);
            if (softskill == null) return NotFound();

            _mapper.Map(dto, softskill);  // Langsung timpa seluruh field DTO ke model
            softskill.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Soft Skill berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus pendidikan
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var softskill = await _context.SoftSkills.FindAsync(id);
            if (softskill == null) return NotFound();

            _context.SoftSkills.Remove(softskill);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Soft Skill berhasil dihapus" });
        }
    }
}
