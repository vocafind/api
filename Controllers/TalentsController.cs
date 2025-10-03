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
    public class TalentsController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<TalentsController> _logger;

        public TalentsController(
            TalentcerdasContext context,
            IWebHostEnvironment env,
            IMapper mapper,
            IEmailService emailService,
            ILogger<TalentsController> logger)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
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


        [HttpPost("verify")]
        public async Task<IActionResult> VerifyTalent([FromBody] TalentsVerifyDTO dto)
        {
            var talent = await _context.Talents
                .FirstOrDefaultAsync(t => t.TalentId == dto.TalentID);   // dto juga string
            if (talent == null)
                return NotFound(new { message = "Talent tidak ditemukan" });

            var allowed = new[] { "Belum Terverifikasi", "Sudah Terverifikasi", "Tidak Terverifikasi" };
            if (!allowed.Contains(dto.StatusAkun))
                return BadRequest(new { message = "Status akun tidak valid." });

            var talentData = new
            {
                Nama = talent.Nama,
                Email = talent.Email,
                TalentID = talent.TalentId
            };

            // Hapus file KTP kalau status Sudah/Tidak Terverifikasi
            if (dto.StatusAkun == "Sudah Terverifikasi" || dto.StatusAkun == "Tidak Terverifikasi")
            {
                var ktpExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                foreach (var ext in ktpExtensions)
                {
                    var filePath = Path.Combine("wwwroot/ktp", $"{talent.TalentId}{ext}");
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation($"File KTP dihapus: {filePath}");
                    }
                }
            }

            if (dto.StatusAkun == "Sudah Terverifikasi")
            {
                talent.StatusAkun = dto.StatusAkun;
                await _context.SaveChangesAsync();

                string subject = "Akun Talent Anda Sudah Diverifikasi";
                string body = "Selamat, akun Anda telah diverifikasi oleh admin. Silakan login untuk melanjutkan.";

                await _emailService.SendEmailAsync(talentData.Email, subject, body);


                _logger.LogInformation($"Talent {talentData.Email} diverifikasi oleh admin.");
                return Ok(new { message = "Akun talent berhasil diverifikasi." });
            }
            else if (dto.StatusAkun == "Tidak Terverifikasi")
            {
                _logger.LogInformation($"Menghapus akun talent ditolak: {talentData.Email} (ID: {talentData.TalentID})");

                // Hapus QR Codes dari registrasi acara (kalau ada)
                var registrations = _context.TalentAcaraRegistrations
                    .Where(r => r.TalentId == talent.TalentId)
                    .ToList();

                foreach (var reg in registrations)
                {
                    if (!string.IsNullOrEmpty(reg.QrCodePath) && System.IO.File.Exists(reg.QrCodePath))
                    {
                        System.IO.File.Delete(reg.QrCodePath);
                        _logger.LogInformation($"QR code dihapus: {reg.RegistrationCode}");
                    }
                }

                // Hapus akun talent
                _context.Talents.Remove(talent);
                await _context.SaveChangesAsync();

                string subject = "Akun Talent Anda Ditolak - Silakan Daftar Ulang";
                string body = $"Halo {talentData.Nama},<br/>Mohon maaf, akun Anda ditolak karena data tidak valid. Silakan daftar ulang dengan data yang benar.";

                await _emailService.SendEmailAsync(talentData.Email, subject, body);

                return Ok(new { message = "Akun talent ditolak, dihapus, dan email notifikasi dikirim." });
            }
            else
            {
                // Balik ke Belum Terverifikasi
                talent.StatusAkun = dto.StatusAkun;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Status akun talent diubah menjadi: {dto.StatusAkun}" });
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
