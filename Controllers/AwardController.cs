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
    public class AwardController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public AwardController(
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


        // ✅ GET: Ambil semua penghargaan
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var award = await _context.Awards
                .Where(s => s.TalentId == talentId)
                .ProjectTo<AwardGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(award);
        }


        // ✅ POST: Tambah penghargaan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AwardPostDTO dto)
        {
            var award = _mapper.Map<Award>(dto);
            award.AwardId = Guid.NewGuid().ToString();
            award.CreatedAt = DateTime.Now;
            award.UpdatedAt = DateTime.Now;

            _context.Awards.Add(award);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Penghargaan berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update bahasa
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AwardPutDTO dto)
        {
            var award = await _context.Awards.FindAsync(id);
            if (award == null) return NotFound();

            _mapper.Map(dto, award);  // Langsung timpa seluruh field DTO ke model
            award.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Penghargaan berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus pendidikan
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var award = await _context.Awards.FindAsync(id);
            if (award == null) return NotFound();

            _context.Awards.Remove(award);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Penghargaan berhasil dihapus" });
        }
    }
}
