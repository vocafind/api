using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using vocafind_api.DTO;
using vocafind_api.Models;
using vocafind_api.Services;
using IOFile = System.IO.File; // ✅ Alias untuk System.IO.File

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
        private readonly AesEncryptionHelper _aesHelper;

        public TalentsController(
            TalentcerdasContext context,
            IWebHostEnvironment env,
            IMapper mapper,
            IEmailService emailService,
            ILogger<TalentsController> logger,
            JwtService jwtService,
            AesEncryptionHelper aesHelper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
            _jwtService = jwtService;
            _aesHelper = aesHelper;
        }







        //---------------------------------------------------VERIFIKASI TALENT----------------------------------------------------
        [HttpGet("unverified")]
        public async Task<ActionResult<IEnumerable<TalentsUnverifiedDTO>>> GetUnverified()
        {
            var talents = await _context.Talents
                .Where(t => t.StatusVerifikasi != "guest" && t.StatusAkun == "Belum Terverifikasi")
                .OrderBy(t => t.UpdatedAt)
                .ToListAsync();

            // 🔄 Mapping manual agar bisa dekripsi NIK
            var result = talents.Select(t =>
            {
                // Dekripsi NIK
                string decryptedNik = null;
                try
                {
                    decryptedNik = _aesHelper.Decrypt(t.Nik);
                }
                catch
                {
                    decryptedNik = "[INVALID DATA]";
                }

                // Mapping ke DTO
                var dto = _mapper.Map<TalentsUnverifiedDTO>(t);
                dto.nik = decryptedNik; // override hasil mapping

                return dto;
            }).ToList();

            return Ok(result);
        }



        [HttpGet("unverified/{id}")]
        public async Task<IActionResult> GetUnverifiedById(string id)
        {
            var talent = await _context.Talents
                .Where(t => t.TalentId == id && t.StatusVerifikasi != "guest" && t.StatusAkun == "Belum Terverifikasi")
                .FirstOrDefaultAsync();

            if (talent == null)
                return NotFound(new { message = "Talent tidak ditemukan atau sudah terverifikasi" });

            // 🔓 Dekripsi NIK
            talent.Nik = _aesHelper.Decrypt(talent.Nik);

            // Map ke DTO
            var talentDto = _mapper.Map<TalentsUnverifiedDTO>(talent);

            // 🔓 Dekripsi KTP ke Base64
            string ktpBase64 = null;
            try
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "ktp");
                var encryptedFile = Directory.GetFiles(uploadPath, $"{talent.TalentId}.*").FirstOrDefault();

                if (encryptedFile != null && IOFile.Exists(encryptedFile))
                {
                    var ext = Path.GetExtension(encryptedFile);
                    var tempPath = Path.Combine(Path.GetTempPath(), $"ktp_{Guid.NewGuid()}{ext}");

                    await _aesHelper.DecryptFileAsync(encryptedFile, tempPath);
                    var fileBytes = await IOFile.ReadAllBytesAsync(tempPath);

                    var contentType = ext.ToLower() switch
                    {
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        _ => "image/jpeg"
                    };

                    ktpBase64 = $"data:{contentType};base64,{Convert.ToBase64String(fileBytes)}";
                    IOFile.Delete(tempPath);
                }
            }
            catch { }

            // 🔄 Gabungkan DTO dengan ktpImage di root
            var result = talentDto.GetType()
                .GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(talentDto, null));

            result["ktpImage"] = ktpBase64;

            return Ok(result);
        }



        [HttpPost("verify")]
        public async Task<IActionResult> VerifyTalent([FromBody] TalentsVerifyDTO dto)
        {
            var talent = await _context.Talents
                .FirstOrDefaultAsync(t => t.TalentId == dto.TalentID);
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

            // 🔥 Hapus file KTP jika sudah / tidak terverifikasi
            if (dto.StatusAkun == "Sudah Terverifikasi" || dto.StatusAkun == "Tidak Terverifikasi")
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "ktp");
                var ktpExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

                foreach (var ext in ktpExtensions)
                {
                    var filePath = Path.Combine(uploadPath, $"{talent.TalentId}{ext}");
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation($"✅ File KTP dihapus: {filePath}");
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
                return Ok(new { message = "Akun talent berhasil diverifikasi dan file KTP dihapus." });
            }
            else if (dto.StatusAkun == "Tidak Terverifikasi")
            {
                _logger.LogInformation($"Menghapus akun talent ditolak: {talentData.Email} (ID: {talentData.TalentID})");

                // 🔄 Hapus QR Code dari registrasi acara
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

                // 🚮 Hapus akun talent
                _context.Talents.Remove(talent);
                await _context.SaveChangesAsync();

                string subject = "Akun Talent Anda Ditolak - Silakan Daftar Ulang";
                string body = $"Halo {talentData.Nama},<br/>Mohon maaf, akun Anda ditolak karena data tidak valid. Silakan daftar ulang dengan data yang benar.";

                await _emailService.SendEmailAsync(talentData.Email, subject, body);

                return Ok(new { message = "Akun talent ditolak, dihapus, dan file KTP dihapus." });
            }
            else
            {
                // Kembalikan ke status "Belum Terverifikasi"
                talent.StatusAkun = dto.StatusAkun;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Status akun talent diubah menjadi: {dto.StatusAkun}" });
            }
        }



        [Authorize] // ini kunci, hanya bisa diakses dengan JWT valid
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
        }





        //---------------------------------------------------PROFIL-------------------------------------------------------------------------------------------------------PROFIL-------------------------------------------------------------------------------------------------------PROFIL----------------------------------------------------


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


        [Authorize]
        [HttpGet("profil/data_diri/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var talent = await _context.Talents
                .Where(t => t.TalentId == id)
                .FirstOrDefaultAsync();

            if (talent == null)
                return NotFound(new { message = "Talent tidak ditemukan" });

            // 🔓 Dekripsi NIK jika ada
            try
            {
                if (!string.IsNullOrEmpty(talent.Nik))
                {
                    talent.Nik = _aesHelper.Decrypt(talent.Nik);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mendekripsi NIK untuk Talent {TalentId}", id);
                // Supaya tetap bisa lanjut, kirimkan pesan error aman
                talent.Nik = "[Gagal dekripsi]";
            }

            // 🧩 Map ke DTO (gunakan AutoMapper)
            var dataDiri = _mapper.Map<TalentsGetDataDiriDTO>(talent);

            return Ok(dataDiri);
        }



        [Authorize(Roles = "Talent")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTalent(string id, [FromForm] TalentsUpdateDTO updateDto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });


            var talent = await _context.Talents.FindAsync(id);
            if (talent == null)
                return NotFound();

            // 🔁 Mapping otomatis semua field teks
            _mapper.Map(updateDto, talent);

            // 📸 Handle upload foto
            if (updateDto.FotoProfil != null && updateDto.FotoProfil.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "foto");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // 🧹 Hapus foto lama jika ada
                if (!string.IsNullOrEmpty(talent.FotoProfil))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", talent.FotoProfil.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // 💾 Simpan foto baru (overwrite pakai nama ID biar unik)
                var ext = Path.GetExtension(updateDto.FotoProfil.FileName);
                var fileName = $"{id}{ext}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateDto.FotoProfil.CopyToAsync(stream);
                }

                talent.FotoProfil = $"/uploads/foto/{fileName}";
            }

            // 🕒 Update waktu terakhir
            talent.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil berhasil diperbarui", foto = talent.FotoProfil });
        }


        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/media_sosial/")]
        public async Task<IActionResult> Create([FromBody] SocialPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var social = _mapper.Map<Social>(dto);
            social.SocialId = Guid.NewGuid().ToString();
            social.CreatedAt = DateTime.Now;
            social.UpdatedAt = DateTime.Now;

            // ✅ Cek kepemilikan
            if (social.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            _context.Socials.Add(social);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Social media berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update akun sosial
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/media_sosial/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SocialPutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var social = await _context.Socials.FindAsync(id);
            if (social == null)
                return NotFound(new { message = "Data sosial media tidak ditemukan." });

            // ✅ Cek kepemilikan
            if (social.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            _mapper.Map(dto, social);
            social.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Social media berhasil diperbarui" });
        }



        // ❌ DELETE: Hapus akun sosial
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/media_sosial/{id}")]
        public async Task<IActionResult> DeleteSocial(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var social = await _context.Socials.FindAsync(id);
            if (social == null) return NotFound();

            // ✅ Cek kepemilikan
            if (social.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

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
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/minat_karir/")]
        public async Task<IActionResult> Create([FromBody] CareerInterestPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

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

            // ✅ Cek kepemilikan
            if (careerInterest.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            _context.CareerInterests.Add(careerInterest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Minat karir berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update minat karir
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/minat_karir/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CareerInterestPutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var careerInterest = await _context.CareerInterests.FindAsync(id);
            if (careerInterest == null) return NotFound();

            // Karena PUT = wajib ganti semua field
            careerInterest.TingkatKetertarikan = dto.TingkatKetertarikan;
            careerInterest.Alasan = dto.Alasan;
            careerInterest.BidangKetertarikan = dto.BidangKetertarikan;
            careerInterest.UpdatedAt = DateTime.Now;

            // ✅ Cek kepemilikan
            if (careerInterest.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Minat karir berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus Minat karir
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/minat_karir/{id}")]
        public async Task<IActionResult> DeleteMinat(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });


            var careerInterest = await _context.CareerInterests.FindAsync(id);
            if (careerInterest == null) return NotFound();

            // ✅ Cek kepemilikan
            if (careerInterest.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

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
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/referensi/")]
        public async Task<IActionResult> Create([FromBody] TalentReferencePostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var reference = _mapper.Map<TalentReference>(dto);
            reference.ReferenceId = Guid.NewGuid().ToString();
            reference.CreatedAt = DateTime.Now;
            reference.UpdatedAt = DateTime.Now;

            // ✅ Cek kepemilikan
            if (reference.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            _context.TalentReferences.Add(reference);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Referensi berhasil ditambahkan" });
        }


        // 🛠 PUT: Update full reference
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/referensi/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] TalentReferencePutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var reference = await _context.TalentReferences.FindAsync(id);
            if (reference == null) return NotFound();

            _mapper.Map(dto, reference);
            reference.UpdatedAt = DateTime.Now;

            // ✅ Cek kepemilikan
            if (reference.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Referensi berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus referensi
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/referensi/{id}")]
        public async Task<IActionResult> DeleteReferensi(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var reference = await _context.TalentReferences.FindAsync(id);
            if (reference == null) return NotFound();

            // ✅ Cek kepemilikan
            if (reference.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            _context.TalentReferences.Remove(reference);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Referensi berhasil dihapus" });
        }












        //---------------------------------------------------AKADEMIK-------------------------------------------------------------------------------------------------------AKADEMIK-------------------------------------------------------------------------------------------------------AKADEMIK----------------------------------------------------

        //---------------------------------------------------Pendidikan----------------------------------------------------
        
        // ✅ GET: Ambil semua pendidikan talent
        [HttpGet("profil/pendidikan/{talentId}")]
        public async Task<IActionResult> GetPendidikanByTalent(string talentId)
        {
            var education = await _context.Educations
                .Where(s => s.TalentId == talentId)
                .ProjectTo<EducationGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(education);
        }


        // ✅ POST: Tambah pendidikan
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/pendidikan/")]
        public async Task<IActionResult> Create([FromBody] EducationPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var education = _mapper.Map<Education>(dto);
            education.EducationId = Guid.NewGuid().ToString();
            education.CreatedAt = DateTime.Now;
            education.UpdatedAt = DateTime.Now;

            // ✅ Cek kepemilikan
            if (education.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            _context.Educations.Add(education);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pendidikan berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update pendidikan
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/pendidikan/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] EducationPutDTO dto)
        {

            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var education = await _context.Educations.FindAsync(id);
            if (education == null) return NotFound();

            _mapper.Map(dto, education);  // Langsung timpa seluruh field DTO ke model
            education.UpdatedAt = DateTime.Now;

            // ✅ Cek kepemilikan
            if (education.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Pendidikan berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus pendidikan
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/pendidikan/{id}")]
        public async Task<IActionResult> DeletePendidikan(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var education = await _context.Educations.FindAsync(id);
            if (education == null) return NotFound();

            // ✅ Cek kepemilikan
            if (education.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            _context.Educations.Remove(education);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pendidikan berhasil dihapus" });
        }





        //---------------------------------------------------Bahasa----------------------------------------------------
        // ✅ GET: Ambil semua bahasa
        [HttpGet("profil/bahasa/{talentId}")]
        public async Task<IActionResult> GetBahasaByTalent(string talentId)
        {
            var language = await _context.Languages
                .Where(s => s.TalentId == talentId)
                .ProjectTo<LanguageGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(language);
        }


        // ✅ POST: Tambah bahasa
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/bahasa/")]
        public async Task<IActionResult> Create([FromBody] LanguagePostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var language = _mapper.Map<Language>(dto);
            language.LanguageId = Guid.NewGuid().ToString();
            language.CreatedAt = DateTime.Now;
            language.UpdatedAt = DateTime.Now;

            // ✅ Cek kepemilikan
            if (language.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            _context.Languages.Add(language);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bahasa berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update bahasa
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/bahasa/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] LanguagePutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var language = await _context.Languages.FindAsync(id);
            if (language == null) return NotFound();

            _mapper.Map(dto, language);  // Langsung timpa seluruh field DTO ke model
            language.UpdatedAt = DateTime.Now;

            // ✅ Cek kepemilikan
            if (language.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Bahasa berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus bahasa
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/bahasa/{id}")]
        public async Task<IActionResult> DeleteBahasa(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;
            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var language = await _context.Languages.FindAsync(id);
            if (language == null) return NotFound();

            // ✅ Cek kepemilikan
            if (language.TalentId != tokenTalentId)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Anda tidak diizinkan mengubah data talent lain." });

            _context.Languages.Remove(language);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bahasa berhasil dihapus" });
        }





        //---------------------------------------------------Penghargaan----------------------------------------------------
        // ✅ GET: Ambil semua penghargaan
        [HttpGet("profil/penghargaan/{talentId}")]
        public async Task<IActionResult> GetPenghargaanByTalent(string talentId)
        {
            var award = await _context.Awards
                .Where(s => s.TalentId == talentId)
                .ProjectTo<AwardGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(award);
        }


        // ✅ POST: Tambah penghargaan
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/penghargaan/")]
        public async Task<IActionResult> Create([FromBody] AwardPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var award = _mapper.Map<Award>(dto);
            award.AwardId = Guid.NewGuid().ToString();
            award.CreatedAt = DateTime.Now;
            award.UpdatedAt = DateTime.Now;

            _context.Awards.Add(award);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Penghargaan berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update penghargaan
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/penghargaan/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AwardPutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var award = await _context.Awards.FindAsync(id);
            if (award == null) return NotFound();

            _mapper.Map(dto, award);  // Langsung timpa seluruh field DTO ke model
            award.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Penghargaan berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus penghargaan
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/penghargaan/{id}")]
        public async Task<IActionResult> DeletePenghargaan(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var award = await _context.Awards.FindAsync(id);
            if (award == null) return NotFound();

            _context.Awards.Remove(award);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Penghargaan berhasil dihapus" });
        }






        //---------------------------------------------------KOMPETENSI-------------------------------------------------------------------------------------------------------KOMPETENSI-------------------------------------------------------------------------------------------------------KOMPETENSI----------------------------------------------------
        
        //---------------------------------------------------Sertifikasi----------------------------------------------------

        // ✅ GET: Ambil semua certification
        [HttpGet("profil/sertifikasi/{talentId}")]
        public async Task<IActionResult> GetSertifikasiByTalent(string talentId)
        {
            var certification = await _context.Certifications
                .Where(s => s.TalentId == talentId)
                .ProjectTo<CertificationGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(certification);
        }


        // ✅ POST: Tambah sertifikasi
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/sertifikasi/")]
        public async Task<IActionResult> Create([FromBody] CertificationPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var certification = _mapper.Map<Certification>(dto);
            certification.CertificationId = Guid.NewGuid().ToString();
            certification.CreatedAt = DateTime.Now;
            certification.UpdatedAt = DateTime.Now;

            _context.Certifications.Add(certification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sertifikasi berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update sertifikasi
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/sertifikasi{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CertificationPutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var certification = await _context.Certifications.FindAsync(id);
            if (certification == null) return NotFound();

            _mapper.Map(dto, certification);  // Langsung timpa seluruh field DTO ke model
            certification.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sertifikasi berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus sertifikasi
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/sertifikasi/{id}")]
        public async Task<IActionResult> DeleteSertifikasi(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var certification = await _context.Certifications.FindAsync(id);
            if (certification == null) return NotFound();

            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sertifikasi berhasil dihapus" });
        }






        //---------------------------------------------------Pelatihan----------------------------------------------------

        // ✅ GET: Ambil semua pelatihan
        [HttpGet("profil/pelatihan/{talentId}")]
        public async Task<IActionResult> GetPelatihanByTalent(string talentId)
        {
            var training = await _context.Trainings
                .Where(s => s.TalentId == talentId)
                .ProjectTo<TrainingGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(training);
        }


        // ✅ POST: Tambah pelatihan
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/pelatihan/")]
        public async Task<IActionResult> Create([FromBody] TrainingPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var training = _mapper.Map<Training>(dto);
            training.TrainingId = Guid.NewGuid().ToString();
            training.CreatedAt = DateTime.Now;
            training.UpdatedAt = DateTime.Now;

            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pelatihan berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update pelatihan
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/pelatihan/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] TrainingPutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var training = await _context.Trainings.FindAsync(id);
            if (training == null) return NotFound();

            _mapper.Map(dto, training);  // Langsung timpa seluruh field DTO ke model
            training.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Pelatihan berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus pelatihan
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/pelatihan/{id}")]
        public async Task<IActionResult> DeletePelatihan(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var training = await _context.Trainings.FindAsync(id);
            if (training == null) return NotFound();

            _context.Trainings.Remove(training);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Penghargaan berhasil dihapus" });
        }







        //---------------------------------------------------Soft SKill----------------------------------------------------


        // ✅ GET: Ambil semua soft skill
        [HttpGet("profil/softskill/{talentId}")]
        public async Task<IActionResult> GetSoftSkillByTalent(string talentId)
        {
            var softskill = await _context.SoftSkills
                .Where(s => s.TalentId == talentId)
                .ProjectTo<SoftSkillGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(softskill);
        }


        // ✅ POST: Tambah soft skill
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/softskill/")]
        public async Task<IActionResult> Create([FromBody] SoftSkillPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var softskill = _mapper.Map<SoftSkill>(dto);
            softskill.SoftskillsId = Guid.NewGuid().ToString();
            softskill.CreatedAt = DateTime.Now;
            softskill.UpdatedAt = DateTime.Now;

            _context.SoftSkills.Add(softskill);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Soft Skill berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update soft skill
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/softskill/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SoftSkillPutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var softskill = await _context.SoftSkills.FindAsync(id);
            if (softskill == null) return NotFound();

            _mapper.Map(dto, softskill);  // Langsung timpa seluruh field DTO ke model
            softskill.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Soft Skill berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus soft skill
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/softskill/{id}")]
        public async Task<IActionResult> DeleteSoftSkill(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var softskill = await _context.SoftSkills.FindAsync(id);
            if (softskill == null) return NotFound();

            _context.SoftSkills.Remove(softskill);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Soft Skill berhasil dihapus" });
        }







        //---------------------------------------------------PENGALAMAN-------------------------------------------------------------------------------------------------------PENGALAMAN-------------------------------------------------------------------------------------------------------PENGALAMAN----------------------------------------------------

        //---------------------------------------------------Riwayat Pekerjaan----------------------------------------------------

        // ✅ GET: Ambil semua WorkHistory
        [HttpGet("profil/riwayat_pekerjaan/{talentId}")]
        public async Task<IActionResult> GetRiwayatPekerjaanByTalent(string talentId)
        {
            var workhistory = await _context.WorkHistories
                .Where(s => s.TalentId == talentId)
                .ProjectTo<WorkHistoryGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(workhistory);
        }


        // ✅ POST: Tambah WorkHistory
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/riwayat_pekerjaan/")]
        public async Task<IActionResult> Create([FromBody] WorkHistoryPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var workhistory = _mapper.Map<WorkHistory>(dto);
            workhistory.WorkhistoryId = Guid.NewGuid().ToString();
            workhistory.CreatedAt = DateTime.Now;
            workhistory.UpdatedAt = DateTime.Now;

            _context.WorkHistories.Add(workhistory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Riwayat Kerja berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update WorkHistory
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/riwayat_pekerjaan/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] WorkHistoryPutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var workhistory = await _context.WorkHistories.FindAsync(id);
            if (workhistory == null) return NotFound();

            _mapper.Map(dto, workhistory);  // Langsung timpa seluruh field DTO ke model
            workhistory.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Riwayat Kerja berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus WorkHistory
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/riwayat_pekerjaan/{id}")]
        public async Task<IActionResult> DeleteRiwayatPekerjaan(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var workhistory = await _context.WorkHistories.FindAsync(id);
            if (workhistory == null) return NotFound();

            _context.WorkHistories.Remove(workhistory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Riwayat kerja berhasil dihapus" });
        }




        //---------------------------------------------------Proyek----------------------------------------------------

        // ✅ GET: Ambil semua Proyek
        [HttpGet("profil/proyek/{talentId}")]
        public async Task<IActionResult> GetProyekByTalent(string talentId)
        {
            var proyek = await _context.Projects
                .Where(s => s.TalentId == talentId)
                .ProjectTo<ProjectGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(proyek);
        }


        // ✅ POST: Tambah Proyek
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/proyek/")]
        public async Task<IActionResult> CreateProyek([FromBody] ProjectPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var proyek = _mapper.Map<Project>(dto);
            proyek.ProjectId = Guid.NewGuid().ToString();
            proyek.CreatedAt = DateTime.Now;
            proyek.UpdatedAt = DateTime.Now;

            _context.Projects.Add(proyek);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Proyek berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update Proyek
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/proyek/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ProjectPutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var proyek = await _context.Projects.FindAsync(id);
            if (proyek == null) return NotFound();

            _mapper.Map(dto, proyek);  // Langsung timpa seluruh field DTO ke model
            proyek.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Proyek berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus Proyek
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/proyek/{id}")]
        public async Task<IActionResult> DeleteProyek(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var proyek = await _context.Projects.FindAsync(id);
            if (proyek == null) return NotFound();

            _context.Projects.Remove(proyek);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Proyek berhasil dihapus" });
        }




        //---------------------------------------------------Portofolio----------------------------------------------------

        // ✅ GET: Ambil semua Portofolio
        [HttpGet("profil/portofolio/{talentId}")]
        public async Task<IActionResult> GetPortofolioByTalent(string talentId)
        {
            var portofolio = await _context.Portofolios
                .Where(s => s.TalentId == talentId)
                .ProjectTo<PortofolioGetDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(portofolio);
        }


        // ✅ POST: Tambah Portofolio
        [Authorize(Roles = "Talent")]
        [HttpPost("profil/portofolio/")]
        public async Task<IActionResult> CreatePortofolio([FromBody] PortofolioPostDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null)
                return Unauthorized(new { message = "Token tidak valid." });

            var portofolio = _mapper.Map<Portofolio>(dto);
            portofolio.PortfolioId = Guid.NewGuid().ToString();
            portofolio.CreatedAt = DateTime.Now;
            portofolio.UpdatedAt = DateTime.Now;

            _context.Portofolios.Add(portofolio);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Portofolio berhasil ditambahkan" });
        }


        // 🛠 PATCH: Update Portofolio
        [Authorize(Roles = "Talent")]
        [HttpPut("profil/portofolio/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PortofolioPutDTO dto)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var portofolio = await _context.Portofolios.FindAsync(id);
            if (portofolio == null) return NotFound();

            _mapper.Map(dto, portofolio);  // Langsung timpa seluruh field DTO ke model
            portofolio.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Portofolio berhasil diperbarui" });
        }


        // ❌ DELETE: Hapus Portofolio
        [Authorize(Roles = "Talent")]
        [HttpDelete("profil/portofolio/{id}")]
        public async Task<IActionResult> DeletePortofolio(string id)
        {
            var tokenTalentId = User.FindFirst("TalentId")?.Value;

            if (tokenTalentId == null || tokenTalentId != id)
                return Forbid("Anda tidak diizinkan mengubah data talent lain.");

            var portofolio = await _context.Portofolios.FindAsync(id);
            if (portofolio == null) return NotFound();

            _context.Portofolios.Remove(portofolio);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Portofolio berhasil dihapus" });
        }




    }
}
