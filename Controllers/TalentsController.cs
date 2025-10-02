using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vocafind_api.DTO;
using vocafind_api.Models;

namespace vocafind_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TalentsController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;

        public TalentsController(TalentcerdasContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] TalentsRegisterDTO dto)
        {
            try
            {
                var talent = new Talent
                {
                    TalentId = Guid.NewGuid().ToString(), // 🔑 isi manual karena PK bukan auto increment
                    Nama = dto.Nama,
                    Usia = dto.Usia,
                    JenisKelamin = dto.JenisKelamin,
                    Alamat = "",
                    Email = dto.Email,
                    NomorTelepon = dto.NomorTelepon,
                    PreferensiGaji = 0,
                    LokasiKerjaDiinginkan = null,
                    StatusPekerjaanSaatIni = "",
                    StatusVerifikasi = "1", // default = belum verifikasi
                    PreferensiJamKerjaMulai = null,
                    PreferensiJamKerjaSelesai = null,
                    PreferensiPerjalananDinas = null,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    FotoProfil = "",
                    VerificationToken = "",
                    Nik = dto.Nik,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // simpan ke DB
                _context.Talents.Add(talent);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Registrasi berhasil. Silakan tunggu proses verifikasi KTP Anda (±1 hari kerja).",
                    talentId = talent.TalentId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }




        // ✅ GET: api/talents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Talent>>> GetAll()
        {
            var talents = await _context.Talents
                .Include(t => t.Hobbies)
                .Include(t => t.Educations)
                .ToListAsync();

            return Ok(talents);
        }


        // ✅ GET: api/talents/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Talent>> GetById(string id)
        {
            var talent = await _context.Talents.FindAsync(id);

            if (talent == null)
                return NotFound();

            return talent;
        }

        // ✅ POST: api/talents
        [HttpPost]
        public async Task<ActionResult<Talent>> Create([FromBody] Talent talent)
        {
            _context.Talents.Add(talent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = talent.TalentId }, talent);
        }

        // ✅ PUT: api/talents/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Talent updatedTalent)
        {
            if (id != updatedTalent.TalentId)
                return BadRequest();

            _context.Entry(updatedTalent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Talents.Any(t => t.TalentId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ DELETE: api/talents/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var talent = await _context.Talents.FindAsync(id);
            if (talent == null)
                return NotFound();

            _context.Talents.Remove(talent);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
