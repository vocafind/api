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
    public class AuthController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtService _jwtService;
        private readonly AesEncryptionHelper _aesHelper;

        public AuthController(
            TalentcerdasContext context,
            IWebHostEnvironment env,
            IMapper mapper,
            IEmailService emailService,
            ILogger<AuthController> logger,
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



        //---------------------------------------------------AUTH----------------------------------------------------
        /*[HttpPost("loginTalent")]
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

            if (talent.StatusAkun == "Belum Terverifikasi")
            {
                return BadRequest(new { message = "Akun belum diverifikasi oleh Admin." });
            }

            if (talent.StatusAkun == "Tidak Terverifikasi")
            {
                return BadRequest(new { message = "Akun tidak terverifikasi. Hubungi Admin." });
            }

            // ✅ Generate Access Token (expired 15 menit)
            var accessToken = _jwtService.GenerateAccessToken(talent);

            // ✅ Generate Refresh Token (expired 7 hari)
            var refreshToken = _jwtService.GenerateRefreshToken();


            var role = "Talent";

            // ✅ Simpan refresh token di database
            talent.RefreshToken = refreshToken;
            talent.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Login berhasil, Selamat datang {talent.Nama}",
                accessToken = accessToken,
                refreshToken = refreshToken,
                role = role,
                talentId = talent.TalentId
            });
        }*/


        [HttpPost("loginTalent")]
        public async Task<IActionResult> Login([FromForm] TalentsLoginDTO dto)
        {
            // Ambil informasi request
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            // 1️ CEK IP TERBLOKIR
            var blocked = await _context.BlockedIps
                .AsNoTracking()
                .FirstOrDefaultAsync(b =>
                    b.IpAddress == ipAddress &&
                    b.BlockedUntil > DateTime.Now
                );


            if (blocked != null)
            {
                _logger.LogWarning("🚫 Blocked IP attempted login: {IP}", ipAddress);
                return StatusCode(429, new
                {
                    message = $"Terlalu banyak percobaan gagal. Coba lagi setelah {blocked.BlockedUntil:HH:mm}."
                });
            }

            // 2️⃣ HITUNG FAILED ATTEMPTS (5 menit terakhir)
            var fiveMinutesAgo = DateTime.Now.AddMinutes(15);
            var recentFailedAttempts = await _context.LoginAttempts
                .Where(la =>
                    la.IpAddress == ipAddress &&
                    la.AttemptTime > fiveMinutesAgo &&
                    !la.IsSuccess
                )
                .CountAsync();

            // 3️⃣ BLOKIR JIKA >= 5 KALI GAGAL
            if (recentFailedAttempts >= 5)
            {
                await _context.BlockedIps.AddAsync(new BlockedIp
                {
                    IpAddress = ipAddress ?? "unknown",
                    Reason = "Brute force attack - 5+ failed login attempts",
                    BlockedAt = DateTime.Now,
                    BlockedUntil = DateTime.Now.AddMinutes(30)
                });
                await _context.SaveChangesAsync();

                _logger.LogWarning("🚨 IP blocked due to brute force: {IP}", ipAddress);

                return StatusCode(429, new
                {
                    message = "Terlalu banyak percobaan gagal. IP diblokir selama 30 menit."
                });
            }

            var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Email == dto.Email);

            if (talent == null)
            {
                // 4️⃣ CATAT FAILED ATTEMPT - Email tidak ditemukan
                await _context.LoginAttempts.AddAsync(new LoginAttempt
                {
                    Email = dto.Email,
                    IpAddress = ipAddress,
                    AttemptTime = DateTime.Now,
                    IsSuccess = false,
                    UserAgent = userAgent
                });
                await _context.SaveChangesAsync();

                _logger.LogWarning("❌ Failed login - Email not found: {Email} from {IP}", dto.Email, ipAddress);
                return Unauthorized(new { message = "Akun tidak ditemukan!" });
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, talent.Password))
            {
                // 5️⃣ CATAT FAILED ATTEMPT - Password salah
                await _context.LoginAttempts.AddAsync(new LoginAttempt
                {
                    Email = dto.Email,
                    IpAddress = ipAddress,
                    AttemptTime = DateTime.Now,
                    IsSuccess = false,
                    UserAgent = userAgent
                });
                await _context.SaveChangesAsync();

                _logger.LogWarning("❌ Failed login - Wrong password: {Email} from {IP}", dto.Email, ipAddress);
                return Unauthorized(new { message = "Password salah!" });
            }

            if (talent.StatusAkun == "Belum Terverifikasi")
                return BadRequest(new { message = "Akun belum diverifikasi oleh Admin." });

            if (talent.StatusAkun == "Tidak Terverifikasi")
                return BadRequest(new { message = "Akun tidak terverifikasi. Hubungi Admin." });

            // 6️⃣ CATAT SUCCESS ATTEMPT
            await _context.LoginAttempts.AddAsync(new LoginAttempt
            {
                Email = dto.Email,
                IpAddress = ipAddress,
                AttemptTime = DateTime.Now,
                IsSuccess = true,
                UserAgent = userAgent
            });
            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ Successful login: {Email} from {IP}", dto.Email, ipAddress);

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(talent);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var role = "Talent";

            talent.RefreshToken = refreshToken;
            talent.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Login berhasil, Selamat datang {talent.Nama}",
                accessToken,
                refreshToken,
                role,
                talentId = talent.TalentId
            });
        }






        [HttpPost("loginAdmin")]
        public async Task<IActionResult> LoginAdmin([FromForm] AdminLoginDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                return BadRequest(new { message = "Username/NIM dan password wajib diisi." });

            // ================= LOGIN ADMIN BIASA =================
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == dto.Username);

            if (admin != null && BCrypt.Net.BCrypt.Verify(dto.Password, admin.Password))
            {
                var accessToken = _jwtService.GenerateAccessTokenAdmin(admin);
                var refreshToken = _jwtService.GenerateRefreshToken();

                admin.RefreshToken = refreshToken;
                admin.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Admin login berhasil: {admin.Username} ({admin.HakAkses})");

                return Ok(new
                {
                    message = $"Login berhasil. Selamat datang {admin.HakAkses}!",
                    role = admin.HakAkses, // Disnaker / Perusahaan / Vokasi
                    adminId = admin.AdminId,
                    username = admin.Username,
                    accessToken,
                    refreshToken,
                    expiresIn = 900 // 15 menit (dalam detik)
                });
            }

            // ================= LOGIN ADMIN SECURITY =================
            var adminSecurity = await _context.AdminSecurities
                .FirstOrDefaultAsync(s => s.Nim == dto.Username && s.IsActive == true);

            if (adminSecurity != null && BCrypt.Net.BCrypt.Verify(dto.Password, adminSecurity.Password))
            {
                var accessToken = _jwtService.GenerateAccessTokenSecurity(adminSecurity);
                var refreshToken = _jwtService.GenerateRefreshToken();

                adminSecurity.RefreshToken = refreshToken;
                adminSecurity.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                adminSecurity.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Security login berhasil: {adminSecurity.Nim}");

                return Ok(new
                {
                    message = $"Login berhasil. Selamat datang, {adminSecurity.NamaLengkap}!",
                    role = "Security",
                    nim = adminSecurity.Nim,
                    accessToken,
                    refreshToken,
                    expiresIn = 900 // 15 menit
                });
            }

            // ================= LOGIN GAGAL =================
            _logger.LogWarning($"Login gagal untuk Username/NIM: {dto.Username}");
            return Unauthorized(new { message = "Username/NIM atau password salah." });
        }




        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
        {
            if (string.IsNullOrEmpty(dto.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var talent = await _context.Talents
                .FirstOrDefaultAsync(t => t.RefreshToken == dto.RefreshToken);

            if (talent == null)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            // ✅ Cek apakah refresh token sudah expired
            if (talent.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Refresh token expired" });
            }

            // ✅ Cek status akun
            if (talent.StatusAkun == "Belum Terverifikasi" || talent.StatusAkun == "Tidak Terverifikasi")
            {
                return Unauthorized(new { message = "Akun tidak aktif" });
            }

            // ✅ Generate token baru
            var newAccessToken = _jwtService.GenerateAccessToken(talent);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // ✅ Update refresh token di database
            talent.RefreshToken = newRefreshToken;
            talent.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }



        /// Logout - Hapus refresh token
        /*[HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var talentIdClaim = User.FindFirst("TalentId")?.Value;

                if (string.IsNullOrEmpty(talentIdClaim))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var talent = await _context.Talents
                    .FirstOrDefaultAsync(t => t.TalentId == talentIdClaim);

                if (talent != null)
                {
                    // ✅ Hapus refresh token dari database
                    talent.RefreshToken = null;
                    talent.RefreshTokenExpiryTime = null;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Logout berhasil" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Logout gagal", error = ex.Message });
            }
        }*/



        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var talentIdClaim = User.FindFirst("TalentId")?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(talentIdClaim))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var talent = await _context.Talents
                    .FirstOrDefaultAsync(t => t.TalentId == talentIdClaim);

                if (talent != null)
                {
                    // Hapus refresh token
                    talent.RefreshToken = null;
                    talent.RefreshTokenExpiryTime = null;
                    await _context.SaveChangesAsync();

                    // 📝 LOG LOGOUT
                    _logger.LogInformation("🚪 Logout: {Email} from {IP} at {Time}",
                        email, ipAddress, DateTime.UtcNow);
                }

                return Ok(new { message = "Logout berhasil" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Logout failed");
                return BadRequest(new { message = "Logout gagal", error = ex.Message });
            }
        }




        /// <summary>
        /// Validate Token - Cek apakah token masih valid
        /// </summary>
        [HttpGet("validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var talentId = User.FindFirst("TalentId")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var nama = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new
            {
                valid = true,
                talentId = talentId,
                email = email,
                nama = nama
            });
        }




        /*[HttpPost("registerTalent")]
        public async Task<IActionResult> Register([FromForm] TalentsRegisterDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // ✅ Validasi Password Level 2 (langsung di sini)
                if (string.IsNullOrEmpty(dto.Password) ||
                    dto.Password.Length < 8 ||
                    !dto.Password.Any(char.IsUpper) ||
                    !dto.Password.Any(char.IsLower) ||
                    !dto.Password.Any(char.IsDigit) ||
                    !dto.Password.Any(ch => !char.IsLetterOrDigit(ch)))
                {
                    return BadRequest(new
                    {
                        error = "Password harus minimal 8 karakter dan mengandung huruf besar, huruf kecil, angka, serta simbol."
                    });
                }

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
                    ProvinsiId = null,
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
        }*/

        [HttpPost("registerTalent")]
        public async Task<IActionResult> Register([FromForm] TalentsRegisterDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // ✅ Validasi Password Level 2
                if (string.IsNullOrEmpty(dto.Password) ||                   // 1️. Cek password tidak null atau kosong
                    dto.Password.Length < 8 ||                              // 2. Cek panjang minimal 8 karakter
                    !dto.Password.Any(char.IsUpper) ||                      // 3️. Cek minimal 1 huruf besar (A-Z)
                    !dto.Password.Any(char.IsLower) ||                      // 4️. Cek minimal 1 huruf kecil (a-z)
                    !dto.Password.Any(char.IsDigit) ||                      // 5️. Cek minimal 1 angka (0-9)
                    !dto.Password.Any(ch => !char.IsLetterOrDigit(ch)))     // 6️. Cek minimal 1 simbol khusus
                {
                    return BadRequest(new
                    {
                        error = "Password harus minimal 8 karakter dan mengandung huruf besar, huruf kecil, angka, serta simbol."
                    });
                }

                var talentId = Guid.NewGuid().ToString();
                string hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // Force prefix $2y$ (Laravel standard)
                if (hashed.StartsWith("$2a$"))
                {
                    hashed = "$2y$" + hashed.Substring(4);
                }

                // Enkripsi NIK sebelum disimpan (pakai instance _aesHelper)
                string encryptedNik = _aesHelper.Encrypt(dto.Nik);

                var talent = new Talent
                {
                    TalentId = talentId,
                    Nama = dto.Nama,
                    Usia = dto.Usia,
                    JenisKelamin = dto.JenisKelamin,
                    Email = dto.Email,
                    NomorTelepon = dto.NomorTelepon,
                    Nik = encryptedNik, // Simpan NIK yang sudah dienkripsi
                    Provinsi = Request.Form["provinsi"],
                    KabupatenKota = Request.Form["kabupaten_kota"],
                    ProvinsiId = null,
                    KabupatenKotaId = null,
                    StatusVerifikasi = "0",
                    StatusAkun = "Belum Terverifikasi",
                    Password = hashed, // Simpan PW Hashed
                    FotoProfil = "",
                    VerificationToken = "",
                    Alamat = "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Talents.Add(talent);
                await _context.SaveChangesAsync();

                // 🔐 Simpan dan enkripsi file KTP
                if (dto.Ktp != null && dto.Ktp.Length > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "ktp");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var ext = Path.GetExtension(dto.Ktp.FileName);
                    if (string.IsNullOrEmpty(ext)) ext = ".jpg";

                    var fileName = $"{talentId}{ext}";
                    var tempFilePath = Path.Combine(uploadPath, $"temp_{fileName}");
                    var encryptedFilePath = Path.Combine(uploadPath, fileName);

                    // Simpan file sementara
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await dto.Ktp.CopyToAsync(stream);
                    }

                    // Enkripsi file KTP (pakai instance _aesHelper)
                    await _aesHelper.EncryptFileAsync(tempFilePath, encryptedFilePath);

                    // Hapus file sementara
                    if (IOFile.Exists(tempFilePath))
                        IOFile.Delete(tempFilePath);
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




    }
}
