using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public TalentsController(TalentcerdasContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
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




        [HttpGet("unverified")]
        public async Task<ActionResult<IEnumerable<TalentsUnverifiedDTO>>> GetUnverified()
        {
            var result = await _context.Talents
                .Where(t => t.StatusVerifikasi != "guest" && t.StatusAkun == "Belum Terverifikasi")
                .OrderBy(t => t.UpdatedAt)
                .ProjectTo<TalentsUnverifiedDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(result);
        }


        [HttpGet("unverified/{id}")]
        public async Task<ActionResult<TalentsUnverifiedDTO>> GetUnverifiedById(string id)
        {
            var talent = await _context.Talents
                .Where(t => t.TalentId == id && t.StatusVerifikasi != "guest" && t.StatusAkun == "Belum Terverifikasi")
                .ProjectTo<TalentsUnverifiedDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (talent == null)
            {
                return NotFound(new { message = "Talent tidak ditemukan atau sudah terverifikasi" });
            }

            return Ok(talent);
        }






        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] TalentsRegisterDTO dto)
        {
            // Validasi sederhana bisa ditambah di DTO dengan [Required], [StringLength], [EmailAddress], dsb.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var talentId = Guid.NewGuid().ToString();

                var talent = new Talent
                {
                    TalentId = talentId,
                    Nama = dto.Nama,
                    Usia = dto.Usia,
                    JenisKelamin = dto.JenisKelamin,
                    Alamat = "",
                    Email = dto.Email,
                    NomorTelepon = dto.NomorTelepon,
                    PreferensiGaji = 0,
                    LokasiKerjaDiinginkan = null,
                    StatusPekerjaanSaatIni = "",
                    StatusVerifikasi = "0", // Belum diverifikasi KTP
                    StatusAkun = "Belum Terverifikasi", // mirror Laravel
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    FotoProfil = "",
                    VerificationToken = "",
                    Nik = dto.Nik,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };


                _context.Talents.Add(talent);
                await _context.SaveChangesAsync();

                // Simpan file KTP
                if (dto.Ktp != null && dto.Ktp.Length > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "ktp");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    // Ambil ekstensi file
                    var ext = Path.GetExtension(dto.Ktp.FileName);
                    if (string.IsNullOrEmpty(ext))
                    {
                        // fallback default jika ekstensi hilang
                        ext = ".jpg";
                    }

                    var fileName = $"{talentId}{ext}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.Ktp.CopyToAsync(stream);
                    }
                }


                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Registrasi berhasil. Silakan tunggu verifikasi admin.",
                    talentId = talent.TalentId
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var inner = ex.InnerException?.Message;
                return BadRequest(new { error = ex.Message, innerError = inner });
            }
        }











        // ✅ GET: api/talents/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Talent>> GetById(string id)
        {
            var talent = await _context.Talents
                .Include(t => t.Hobbies)
                .Include(t => t.Educations)
                .FirstOrDefaultAsync(t => t.TalentId == id);

            if (talent == null)
                return NotFound();

            return Ok(talent);
        }


        


        /*// ✅ GET: api/talents/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Talent>> GetById(string id)
        {
            var talent = await _context.Talents.FindAsync(id);

            if (talent == null)
                return NotFound();

            return talent;
        }*/























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
