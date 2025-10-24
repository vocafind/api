using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vocafind_api.DTO;
using vocafind_api.Models;
using vocafind_api.Services;
using static vocafind_api.DTO.TalentsDTO;

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
        private readonly JwtService _jwtService;

        public TalentsController(
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



        //---------------------------------------------------AUTH----------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] TalentsLoginDTO dto)
        {
            var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Email == dto.Email);
            if (talent == null)
            {
                return Unauthorized(new { message = "Akun tidak ditemukan!" });
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, talent.Password))
            {
                return Unauthorized(new { message = "Password salah!" });
            }

            /* if (talent.StatusVerifikasi == "0")
             {
                 return BadRequest(new { message = "Silakan verifikasi email/identitas Anda terlebih dahulu." });
             }*/

            if (talent.StatusAkun == "Belum Terverifikasi")
            {
                return BadRequest(new { message = "Akun belum diverifikasi oleh Admin." });
            }

            if (talent.StatusAkun == "Tidak Terverifikasi")
            {
                return BadRequest(new { message = "Akun tidak terverifikasi. Hubungi Admin." });
            }

            // ✅ generate JWT token
            var token = _jwtService.GenerateToken(talent);

            return Ok(new
            {
                message = $"Login berhasil, Selamat datang {talent.Nama}",
                token = token,
                talentId = talent.TalentId
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] TalentsRegisterDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var talentId = Guid.NewGuid().ToString();

                string hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // Force prefix $2y$ (Laravel standard)
                if (hashed.StartsWith("$2a$"))
                {
                    hashed = "$2y$" + hashed.Substring(4);
                }



                var talent = new Talent
                {
                    TalentId = talentId,
                    Nama = dto.Nama,
                    Usia = dto.Usia,
                    JenisKelamin = dto.JenisKelamin,
                    Email = dto.Email,
                    NomorTelepon = dto.NomorTelepon,
                    Nik = dto.Nik,
                    Provinsi = Request.Form["provinsi"],
                    KabupatenKota = Request.Form["kabupaten_kota"],
                    ProvinsiId = null, // bisa diisi nanti kalau punya ID-nya
                    KabupatenKotaId = null,
                    StatusVerifikasi = "0",
                    StatusAkun = "Belum Terverifikasi",
                    Password = hashed,
                    FotoProfil = "",
                    VerificationToken = "",
                    Alamat = "",
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

                    var ext = Path.GetExtension(dto.Ktp.FileName);
                    if (string.IsNullOrEmpty(ext)) ext = ".jpg";

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
                    talentId = talent.TalentId,
                    data = new
                    {
                        talent.Nama,
                        talent.Provinsi,
                        talent.KabupatenKota,
                        talent.JenisKelamin,
                        talent.Usia
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }






        //---------------------------------------------------VERIFIKASI TALENT----------------------------------------------------

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




        /*[Authorize] // ini kunci, hanya bisa diakses dengan JWT valid
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Ambil data user dari token
            var talentId = User.FindFirst("TalentId")?.Value;
            if (talentId == null)
                return Unauthorized(new { message = "Token tidak valid" });

            var talent = await _context.Talents.FindAsync(talentId);
            if (talent == null)
                return NotFound(new { message = "Data talent tidak ditemukan" });

            return Ok(new
            {
                talent.TalentId,
                talent.Nama,
                talent.Email,
                talent.StatusAkun,
                talent.StatusVerifikasi,
                talent.CreatedAt
            });
        }*/







        //---------------------------------------------------DATA DIRI----------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Talent>>> GetAll()
        {
            var talents = await _context.Talents
                .Include(t => t.Hobbies)
                .Include(t => t.Educations)
                .ToListAsync();

            return Ok(talents);
        }


        [HttpGet("profil/data_diri/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var dataDiri = await _context.Talents
                .Where(t => t.TalentId == id)
                .ProjectTo<TalentsGetDataDiriDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (dataDiri == null)
                return NotFound(new { message = "Talent tidak ditemukan" });

            return Ok(dataDiri);
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTalent(string id, [FromForm] TalentsUpdateDTO updateDto)
        {
            var talent = await _context.Talents.FindAsync(id);
            if (talent == null)
                return NotFound();

            // 🔁 Mapping otomatis semua field teks
            _mapper.Map(updateDto, talent);

            // 📸 Handle upload foto manual
            if (updateDto.FotoProfil != null && updateDto.FotoProfil.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "foto");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var ext = Path.GetExtension(updateDto.FotoProfil.FileName);
                var fileName = $"{id}{ext}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateDto.FotoProfil.CopyToAsync(stream);
                }

                talent.FotoProfil = $"/uploads/foto/{fileName}";
            }

            // 🕒 update waktu terakhir
            talent.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil berhasil diperbarui" });
        }


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








        //---------------------------------------------------SOSMED----------------------------------------------------
        // ✅ GET: Ambil semua social media talent
        [HttpGet("profil/media_sosial/{talentId}")]
        public async Task<IActionResult> GetByTalent(string talentId)
        {
            var socials = await _context.Socials
                .Where(s => s.TalentId == talentId)
                .ProjectTo<SocialGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(socials);
        }



        // ✅ POST: Tambah akun sosial media
        [HttpPost("profil/media_sosial/")]
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
        [HttpPut("profil/media_sosial/{id}")]
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
        [HttpDelete("profil/media_sosial/{id}")]
        public async Task<IActionResult> DeleteSocial(string id)
        {
            var social = await _context.Socials.FindAsync(id);
            if (social == null) return NotFound();

            _context.Socials.Remove(social);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Social media berhasil dihapus" });
        }









        //---------------------------------------------------Minat karir----------------------------------------------------
        // ✅ GET: Ambil semua minat karir talent
        [HttpGet("profil/minat_karir/{talentId}")]
        public async Task<IActionResult> GetMinatByTalent(string talentId)
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
        [HttpPost("profil/minat_karir/")]
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
        [HttpPut("profil/minat_karir/{id}")]
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
        [HttpDelete("profil/minat_karir/{id}")]
        public async Task<IActionResult> DeleteMinat(string id)
        {
            var careerInterest = await _context.CareerInterests.FindAsync(id);
            if (careerInterest == null) return NotFound();

            _context.CareerInterests.Remove(careerInterest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Minat karir berhasil dihapus" });
        }










        //---------------------------------------------------Referensi----------------------------------------------------
        // ✅ GET: Ambil semua reference berdasarkan TalentId
        [HttpGet("profil/referensi/{talentId}")]
        public async Task<IActionResult> GetReferensiByTalent(string talentId)
        {
            var references = await _context.TalentReferences
                .Where(r => r.TalentId == talentId)
                .ProjectTo<TalentReferenceGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(references);
        }

        // ✅ POST: Tambah reference
        [HttpPost("profil/referensi/")]
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
        [HttpPut("profil/referensi/{id}")]
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
        [HttpDelete("profil/referensi/{id}")]
        public async Task<IActionResult> DeleteReferensi(string id)
        {
            var reference = await _context.TalentReferences.FindAsync(id);
            if (reference == null) return NotFound();

            _context.TalentReferences.Remove(reference);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Referensi berhasil dihapus" });
        }
    }
}
