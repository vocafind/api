using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
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
    public class SocialController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public SocialController(
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



        // ✅ GET: Ambil semua social media talent
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var socials = await _context.Socials
                .Where(s => s.TalentId == talentId)
                .ProjectTo<SocialGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(socials);
        }



        // ✅ POST: Tambah akun sosial media
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SocialPostDTO dto)
        {
            var social = _mapper.Map<Social>(dto);
            social.SocialId = Guid.NewGuid().ToString();
            social.CreatedAt = DateTime.Now;
            social.UpdatedAt = DateTime.Now;

            _context.Socials.Add(social);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Social media berhasil ditambahkan" });
        }



        // 🛠 PATCH: Update akun sosial
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SocialPutDTO dto)
        {
            var social = await _context.Socials.FindAsync(id);
            if (social == null) return NotFound();

            _mapper.Map(dto, social);  // Langsung timpa seluruh field DTO ke model
            social.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Social media berhasil diperbarui" });
        }



        // ❌ DELETE: Hapus akun sosial
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var social = await _context.Socials.FindAsync(id);
            if (social == null) return NotFound();

            _context.Socials.Remove(social);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Social media berhasil dihapus" });
        }
    }
}
