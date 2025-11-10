/*using AutoMapper;
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
    public class TrainingController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public TrainingController(
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


        // ✅ GET: Ambil semua pelatihan
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var training = await _context.Trainings
                .Where(s => s.TalentId == talentId)
                .ProjectTo<TrainingGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(training);
        }


        // ✅ POST: Tambah pelatihan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TrainingPostDTO dto)
        {
            var training = _mapper.Map<Training>(dto);
            training.TrainingId = Guid.NewGuid().ToString();
            training.CreatedAt = DateTime.Now;
            training.UpdatedAt = DateTime.Now;

            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pelatihan berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update training
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] TrainingPutDTO dto)
        {
            var training = await _context.Trainings.FindAsync(id);
            if (training == null) return NotFound();

            _mapper.Map(dto, training);  // Langsung timpa seluruh field DTO ke model
            training.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Pelatihan berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus pendidikan
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var training = await _context.Trainings.FindAsync(id);
            if (training == null) return NotFound();

            _context.Trainings.Remove(training);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Penghargaan berhasil dihapus" });
        }
    }
}
*/