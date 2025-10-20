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
    public class WorkHistoryController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public WorkHistoryController(
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


        // ✅ GET: Ambil semua WorkHistory
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var workhistory = await _context.WorkHistories
                .Where(s => s.TalentId == talentId)
                .ProjectTo<WorkHistoryGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(workhistory);
        }


        // ✅ POST: Tambah penghargaan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkHistoryPostDTO dto)
        {
            var workhistory = _mapper.Map<WorkHistory>(dto);
            workhistory.WorkhistoryId = Guid.NewGuid().ToString();
            workhistory.CreatedAt = DateTime.Now;
            workhistory.UpdatedAt = DateTime.Now;

            _context.WorkHistories.Add(workhistory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Riwayat Kerja berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update Riwayat Kerja
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] WorkHistoryPutDTO dto)
        {
            var workhistory = await _context.WorkHistories.FindAsync(id);
            if (workhistory == null) return NotFound();

            _mapper.Map(dto, workhistory);  // Langsung timpa seluruh field DTO ke model
            workhistory.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Riwayat Kerja berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus pendidikan
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var workhistory = await _context.WorkHistories.FindAsync(id);
            if (workhistory == null) return NotFound();

            _context.WorkHistories.Remove(workhistory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Riwayat kerja berhasil dihapus" });
        }
    }
}
