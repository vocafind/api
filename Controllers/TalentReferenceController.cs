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
    public class TalentReferenceController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public TalentReferenceController(
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



        // ✅ GET: Ambil semua reference berdasarkan TalentId
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var references = await _context.TalentReferences
                .Where(r => r.TalentId == talentId)
                .ProjectTo<TalentReferenceGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(references);
        }

        // ✅ POST: Tambah reference
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TalentReferencePostDTO dto)
        {
            var reference = _mapper.Map<TalentReference>(dto);
            reference.ReferenceId = Guid.NewGuid().ToString();
            reference.CreatedAt = DateTime.Now;
            reference.UpdatedAt = DateTime.Now;

            _context.TalentReferences.Add(reference);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Referensi berhasil ditambahkan" });
        }

        // 🛠 PUT: Update full reference
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] TalentReferencePutDTO dto)
        {
            var reference = await _context.TalentReferences.FindAsync(id);
            if (reference == null) return NotFound();

            _mapper.Map(dto, reference);
            reference.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Referensi berhasil diperbarui" });
        }

        // ❌ DELETE: Hapus referensi
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var reference = await _context.TalentReferences.FindAsync(id);
            if (reference == null) return NotFound();

            _context.TalentReferences.Remove(reference);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Referensi berhasil dihapus" });
        }
    }
}
