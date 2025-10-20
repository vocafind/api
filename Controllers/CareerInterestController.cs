using AutoMapper;
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
    public class CareerInterestController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;
        private readonly JwtService _jwtService;

        public CareerInterestController(
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


        // ✅ GET: Ambil semua minat karir talent
        [HttpGet("{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var careerInterests = await _context.CareerInterests
                .Where(s => s.TalentId == talentId)
                .Select(s => new CareerInterestGetDTO
                {
                    CareerinterestId = s.CareerinterestId,      // ✅ Ditambahkan
                    TalentId = s.TalentId,
                    TingkatKetertarikan = s.TingkatKetertarikan,
                    Alasan = s.Alasan,
                    BidangKetertarikan = s.BidangKetertarikan,
                })
                .ToListAsync();

            return Ok(careerInterests);
        }



        // ✅ POST: Tambah minat karir
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CareerInterestPostDTO dto)
        {
            var careerInterest = new CareerInterest
            {
                CareerinterestId = Guid.NewGuid().ToString(),
                TalentId = dto.TalentId,
                TingkatKetertarikan = dto.TingkatKetertarikan,
                Alasan = dto.Alasan,
                BidangKetertarikan = dto.BidangKetertarikan,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _context.CareerInterests.Add(careerInterest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Minat karir berhasil ditambahkan" });
        }

        // 🛠 PATCH: Update minat karir
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CareerInterestPutDTO dto)
        {
            var careerInterest = await _context.CareerInterests.FindAsync(id);
            if (careerInterest == null) return NotFound();

            // Karena PUT = wajib ganti semua field
            careerInterest.TingkatKetertarikan = dto.TingkatKetertarikan;
            careerInterest.Alasan = dto.Alasan;
            careerInterest.BidangKetertarikan = dto.BidangKetertarikan;
            careerInterest.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Minat karir berhasil diperbarui" });
        }



        // ❌ DELETE: Hapus Minat karir
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var careerInterest = await _context.CareerInterests.FindAsync(id);
            if (careerInterest == null) return NotFound();

            _context.CareerInterests.Remove(careerInterest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Minat karir berhasil dihapus" });
        }
    }
}
