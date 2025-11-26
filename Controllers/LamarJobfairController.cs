using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vocafind_api.Models;

namespace vocafind_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LamarJobfairController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly ILogger<LamarJobfairController> _logger;
        private readonly IMapper _mapper;

        public LamarJobfairController(TalentcerdasContext context, ILogger<LamarJobfairController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        //---------------------------------------------------LAMAR PEKERJAAN DI ACARA----------------------------------------------------
        [Authorize(Roles = "Talent")]
        [HttpPost("lokerAcara/{lowonganId}")]
        public async Task<IActionResult> LamarLokerAcara(string lowonganId)
        {
            string talentId = null;

            try
            {
                // Get authenticated talent
                talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Unauthorized access." });

                _logger.LogInformation($"Memproses lamaran acara - Talent: {talentId}, Lowongan: {lowonganId}");

                // Validasi: Apakah lowongan dengan ID ini ada di acara?
                var lowonganAcara = await _context.LowonganAcaras
                    .Include(la => la.Lowongan)
                        .ThenInclude(l => l.Company)
                    .Include(la => la.AcaraJobfair)
                    .FirstOrDefaultAsync(la => la.LowonganId == lowonganId);

                if (lowonganAcara == null)
                {
                    _logger.LogWarning($"Lowongan tidak ditemukan dalam acara: {lowonganId}");
                    return NotFound(new { message = "Lowongan tidak ditemukan dalam acara ini." });
                }

                // Validasi null references
                if (lowonganAcara.Lowongan == null)
                {
                    _logger.LogError($"Lowongan null untuk LowonganAcara: {lowonganId}");
                    return StatusCode(500, new { message = "Data lowongan tidak valid." });
                }

                if (lowonganAcara.AcaraJobfair == null)
                {
                    _logger.LogError($"AcaraJobfair null untuk LowonganAcara: {lowonganId}");
                    return StatusCode(500, new { message = "Data acara tidak valid." });
                }

                var lowongan = lowonganAcara.Lowongan;
                var acara = lowonganAcara.AcaraJobfair;

                _logger.LogInformation($"Lowongan: {lowongan.Posisi}, Acara: {acara.NamaAcara}");

                // Cek status acara - harus dalam masa pendaftaran
                if (acara.Status != "masa pendaftaran")
                {
                    _logger.LogWarning($"Acara tidak dalam masa pendaftaran. Status: {acara.Status}");
                    return BadRequest(new { message = "Acara tidak dalam periode pendaftaran." });
                }

                // Get talent data dengan semua relasi yang dibutuhkan
                var talent = await _context.Talents
                    .Include(t => t.Socials)
                    .Include(t => t.CareerInterests)
                    .Include(t => t.Educations)
                    .Include(t => t.SoftSkills)
                    .Include(t => t.Experiences)
                    .Include(t => t.Portofolios)
                    .FirstOrDefaultAsync(t => t.TalentId == talentId);

                if (talent == null)
                {
                    _logger.LogWarning($"Talent tidak ditemukan: {talentId}");
                    return NotFound(new { message = "Talent tidak ditemukan." });
                }

                // Cek statusAkun, hanya boleh jika Sudah Terverifikasi atau Alumni
                if (talent.StatusAkun != "Sudah Terverifikasi" && talent.StatusAkun != "Alumni")
                {
                    _logger.LogWarning($"Status akun tidak memenuhi syarat: {talent.StatusAkun}");
                    return BadRequest(new { message = "Akun anda belum diverifikasi oleh administrator." });
                }

                // Validasi kelengkapan data talent berdasarkan pendidikan
                var missing = new List<string>();

                // Data dasar wajib untuk semua level pendidikan
                if (string.IsNullOrEmpty(talent.Nama) || string.IsNullOrEmpty(talent.Email) ||
                    string.IsNullOrEmpty(talent.NomorTelepon) || talent.Usia == 0 ||
                    string.IsNullOrEmpty(talent.JenisKelamin) || string.IsNullOrEmpty(talent.Alamat) ||
                    string.IsNullOrEmpty(talent.KabupatenKota) || string.IsNullOrEmpty(talent.Provinsi) ||
                    talent.PreferensiGaji == 0 ||
                    string.IsNullOrEmpty(talent.LokasiKerjaDiinginkan) ||
                    string.IsNullOrEmpty(talent.StatusPekerjaanSaatIni) ||
                    talent.PreferensiJamKerjaMulai == null || talent.PreferensiJamKerjaSelesai == null ||
                    string.IsNullOrEmpty(talent.PreferensiPerjalananDinas))
                {
                    missing.Add("Data Diri");
                }

                if (!talent.Socials.Any())
                {
                    missing.Add("Media Sosial");
                }
                if (!talent.CareerInterests.Any())
                {
                    missing.Add("Minat Karir");
                }

                // Validasi berdasarkan tingkat pendidikan
                var education = talent.Educations
                    .OrderByDescending(e => e.Jenjang)
                    .FirstOrDefault();

                if (education == null)
                {
                    missing.Add("Pendidikan");
                }
                else
                {
                    var educationLevel = education.Jenjang?.ToUpper();

                    // SMA/SMK/D1/D2 hanya perlu mengisi soft skills
                    if (new[] { "SMA", "SMK", "D1", "D2" }.Contains(educationLevel))
                    {
                        if (!talent.SoftSkills.Any())
                        {
                            missing.Add("Soft Skills");
                        }
                    }
                    // D3/D4/S1 wajib memiliki soft skills
                    else if (new[] { "S1", "D3", "D4" }.Contains(educationLevel))
                    {
                        if (!talent.SoftSkills.Any())
                        {
                            missing.Add("Soft Skills");
                        }
                    }
                    // S2/S3 wajib mengisi soft skills dan portofolio
                    else if (new[] { "S2", "S3" }.Contains(educationLevel))
                    {
                        if (!talent.SoftSkills.Any())
                        {
                            missing.Add("Soft Skills");
                        }
                        if (!talent.Portofolios.Any())
                        {
                            missing.Add("Portofolio");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Tingkat pendidikan tidak dikenali: {educationLevel}");
                        return BadRequest(new { message = $"Tingkat pendidikan tidak dikenali: {educationLevel}" });
                    }
                }

                if (missing.Any())
                {
                    _logger.LogWarning($"Data talent tidak lengkap. Missing: {string.Join(", ", missing)}");
                    return BadRequest(new { message = $"Anda harus melengkapi data {string.Join(", ", missing)} sebelum melamar pekerjaan" });
                }

                // Check education level requirement
                if (!string.IsNullOrEmpty(lowongan.MinimalLulusan))
                {
                    var talentEducation = talent.Educations
                        .OrderByDescending(e => e.Jenjang)
                        .FirstOrDefault();

                    if (talentEducation == null)
                    {
                        return BadRequest(new { message = "Anda belum menambahkan data pendidikan." });
                    }

                    if (!CanTalentApply(lowongan.MinimalLulusan, talentEducation.Jenjang))
                    {
                        var eligibleLevels = GetEligibleEducationLevels(lowongan.MinimalLulusan);
                        _logger.LogWarning($"Pendidikan tidak memenuhi syarat. Minimal: {lowongan.MinimalLulusan}, Talent: {talentEducation.Jenjang}");
                        return BadRequest(new { message = $"Maaf, lowongan ini membutuhkan minimal lulusan {lowongan.MinimalLulusan}. Tingkat pendidikan yang memenuhi syarat: {string.Join(", ", eligibleLevels)}." });
                    }
                }

                // Check if job is still active
                if (lowongan.Status != "aktif")
                {
                    _logger.LogWarning($"Lowongan tidak aktif. Status: {lowongan.Status}");
                    return BadRequest(new { message = "Lowongan ini sudah tidak aktif." });
                }

                // Check batas pelamar
                if (lowongan.BatasPelamar != 0 && lowongan.JumlahPelamar >= lowongan.BatasPelamar)
                {
                    _logger.LogWarning($"Batas pelamar tercapai. Jumlah: {lowongan.JumlahPelamar}, Batas: {lowongan.BatasPelamar}");
                    return BadRequest(new { message = "Maaf, jumlah pelamar sudah mencapai batas maksimal." });
                }

                // Cek apakah sudah melamar lowongan yang sama
                var sudahMelamarLowonganIni = await _context.JobApplies
                    .AnyAsync(ja => ja.TalentId == talentId && ja.LowonganId == lowonganId);

                if (sudahMelamarLowonganIni)
                {
                    _logger.LogWarning($"Talent sudah melamar lowongan ini: {lowonganId}");
                    return BadRequest(new { message = "Anda sudah melamar lowongan ini." });
                }

                // Cek jumlah lamaran di acara ini (maksimal 3)
                var lowonganIdsInAcara = await _context.LowonganAcaras
                    .Where(la => la.AcaraJobfairId == acara.Id)
                    .Select(la => la.LowonganId)
                    .ToListAsync();

                var jumlahLamaranDiAcara = await _context.JobApplies
                    .CountAsync(ja => ja.TalentId == talentId && lowonganIdsInAcara.Contains(ja.LowonganId));

                if (jumlahLamaranDiAcara >= 3)
                {
                    _logger.LogWarning($"Batas lamaran acara tercapai. Jumlah: {jumlahLamaranDiAcara}");
                    return BadRequest(new { message = "Anda sudah mencapai batas maksimal 3 aplikasi untuk acara ini." });
                }

                _logger.LogInformation($"Memulai transaction untuk lamaran...");

                // Begin transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Jika ini lamaran pertama di acara ini, daftarkan talent ke acara
                    var registration = await _context.TalentAcaraRegistrations
                        .FirstOrDefaultAsync(tar => tar.TalentId == talentId && tar.AcaraJobfairId == acara.Id);

                    if (registration == null && jumlahLamaranDiAcara == 0)
                    {
                        _logger.LogInformation($"Membuat registrasi baru untuk acara...");

                        // Cek kapasitas acara
                        if (acara.CurrentCapacity >= acara.MaxCapacity)
                        {
                            _logger.LogWarning($"Kapasitas acara penuh. Current: {acara.CurrentCapacity}, Max: {acara.MaxCapacity}");
                            return BadRequest(new { message = "Maaf, kapasitas acara sudah penuh." });
                        }

                        // Buat registrasi baru
                        registration = new TalentAcaraRegistration
                        {
                            TalentId = talentId,
                            AcaraJobfairId = acara.Id,
                            RegistrationCode = GenerateRegistrationCode(),
                            Status = "registered",
                            CheckinStatus = "waiting",
                            RegisteredAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        _context.TalentAcaraRegistrations.Add(registration);

                        // Increment current capacity
                        acara.CurrentCapacity++;
                        _context.AcaraJobfairs.Update(acara);

                        _logger.LogInformation($"Registrasi acara dibuat: {registration.RegistrationCode}");
                    }

                    // Create job application
                    var applicationCode = await GenerateApplicationCode();
                    var application = new JobApply
                    {
                        ApplyId = Guid.NewGuid().ToString(),
                        TalentId = talentId,
                        LowonganId = lowonganId,
                        Status = "pending",
                        ApplicationCode = applicationCode,
                        AppliedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.JobApplies.Add(application);

                    // Increment jumlah pelamar
                    lowongan.JumlahPelamar++;
                    _context.JobVacancies.Update(lowongan);

                    _logger.LogInformation($"Menyimpan perubahan ke database...");

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"Lamaran berhasil disimpan. ApplicationCode: {applicationCode}");

                    var message = jumlahLamaranDiAcara == 0
                        ? "Pendaftaran acara dan lamaran berhasil dikirim! Anda telah terdaftar di acara ini."
                        : "Lamaran berhasil dikirim!";

                    return Ok(new
                    {
                        message = message,
                        isFirstApplication = jumlahLamaranDiAcara == 0,
                        registrationCode = registration?.RegistrationCode,
                        applicationCode = application.ApplicationCode,
                        applicationCount = jumlahLamaranDiAcara + 1
                    });
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(dbEx, "Database update error dalam transaction");
                    _logger.LogError($"Inner exception: {dbEx.InnerException?.Message}");

                    return StatusCode(500, new
                    {
                        message = "Terjadi kesalahan database saat menyimpan lamaran.",
                        detail = dbEx.InnerException?.Message
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Transaction error - Gagal menyimpan lamaran acara");
                    _logger.LogError($"Exception type: {ex.GetType().Name}, Message: {ex.Message}");

                    return StatusCode(500, new
                    {
                        message = "Gagal memproses lamaran. Data telah dikembalikan ke state semula.",
                        detail = ex.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error - Gagal menyimpan lamaran acara");
                _logger.LogError($"Talent: {talentId}, Lowongan: {lowonganId}");
                _logger.LogError($"Exception type: {ex.GetType().Name}, Message: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }

                // Return error yang lebih informatif
                var errorResponse = new
                {
                    message = "Terjadi kesalahan saat mengirim lamaran. Silakan coba lagi.",
                    error = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }




        //---------------------------------------------------GET LAMARAN ACARA SAYA----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpGet("lamaran-acara-saya/{acaraId}")]
        public async Task<IActionResult> GetLamaranAcaraSaya(ulong acaraId)
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                // Ambil semua lowongan yang ada di acara ini
                var lowonganIdsInAcara = await _context.LowonganAcaras
                    .Where(la => la.AcaraJobfairId == acaraId)
                    .Select(la => la.LowonganId)
                    .ToListAsync();

                // Ambil lamaran talent untuk lowongan-lowongan tersebut
                var lamaran = await (from apply in _context.JobApplies
                                     join lowongan in _context.JobVacancies on apply.LowonganId equals lowongan.LowonganId
                                     join company in _context.Companies on lowongan.CompanyId equals company.CompanyId
                                     where apply.TalentId == talentId && lowonganIdsInAcara.Contains(apply.LowonganId)
                                     orderby apply.CreatedAt descending
                                     select new
                                     {
                                         apply.ApplyId,
                                         apply.LowonganId,
                                         apply.Status,
                                         apply.ApplicationCode,
                                         apply.Interview,
                                         apply.Location_interview,
                                         apply.CreatedAt,
                                         apply.AppliedAt,
                                         apply.ReviewedAt,
                                         Lowongan = new
                                         {
                                             lowongan.Posisi,
                                             lowongan.DeskripsiPekerjaan,
                                             lowongan.Lokasi,
                                             lowongan.Gaji,
                                             lowongan.OpsiKerjaRemote,
                                             Company = new
                                             {
                                                 company.NamaPerusahaan,
                                                 company.Logo
                                             }
                                         }
                                     }).ToListAsync();

                return Ok(lamaran);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengambil data lamaran acara");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data lamaran." });
            }
        }

        //---------------------------------------------------GET SEMUA LAMARAN ACARA SAYA----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpGet("semua-lamaran-acara")]
        public async Task<IActionResult> GetSemuaLamaranAcara()
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                _logger.LogInformation($"Fetching semua lamaran acara for talent: {talentId}");

                // Ambil semua lamaran talent yang lowongannya terdaftar di acara
                var lamaran = await (from apply in _context.JobApplies
                                     join lowongan in _context.JobVacancies on apply.LowonganId equals lowongan.LowonganId
                                     join company in _context.Companies on lowongan.CompanyId equals company.CompanyId
                                     join lowonganAcara in _context.LowonganAcaras on lowongan.LowonganId equals lowonganAcara.LowonganId
                                     join acara in _context.AcaraJobfairs on lowonganAcara.AcaraJobfairId equals acara.Id
                                     where apply.TalentId == talentId
                                     orderby apply.CreatedAt descending
                                     select new
                                     {
                                         apply.ApplyId,
                                         apply.LowonganId,
                                         apply.Status,
                                         apply.ApplicationCode,
                                         apply.Interview,
                                         apply.Location_interview,
                                         apply.CreatedAt,
                                         apply.AppliedAt,
                                         apply.ReviewedAt,
                                         Acara = new
                                         {
                                             acara.Id,
                                             acara.NamaAcara,
                                             acara.TanggalMulaiAcara,
                                             acara.TanggalSelesaiAcara,
                                             acara.Status,
                                             acara.Lokasi
                                         },
                                         Lowongan = new
                                         {
                                             lowongan.Posisi,
                                             lowongan.DeskripsiPekerjaan,
                                             lowongan.Lokasi,
                                             lowongan.Gaji,
                                             lowongan.OpsiKerjaRemote,
                                             Company = new
                                             {
                                                 company.NamaPerusahaan,
                                                 company.Logo
                                             }
                                         }
                                     }).ToListAsync();

                _logger.LogInformation($"Found {lamaran.Count} lamaran records");
                return Ok(lamaran);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Gagal mengambil data semua lamaran acara. Error: {ex.Message}");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data lamaran.", detail = ex.Message });
            }
        }



        //---------------------------------------------------BATAL LAMARAN ACARA----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpDelete("batal/{applyId}")]
        public async Task<IActionResult> BatalkanLamaranAcara(string applyId)
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                var application = await _context.JobApplies
                    .Include(ja => ja.ApplyAcaras)
                    .FirstOrDefaultAsync(ja => ja.ApplyId == applyId && ja.TalentId == talentId);

                if (application == null)
                    return NotFound(new { message = "Lamaran tidak ditemukan." });

                // Cek apakah ini lamaran untuk acara (ada di ApplyAcara)
                var applyAcara = application.ApplyAcaras.FirstOrDefault();
                if (applyAcara == null)
                    return BadRequest(new { message = "Lamaran ini bukan untuk acara jobfair." });

                // Hanya bisa membatalkan jika status masih pending
                if (application.Status != "pending")
                    return BadRequest(new { message = "Lamaran tidak dapat dibatalkan karena sudah diproses." });

                var acara = await _context.AcaraJobfairs.FindAsync(applyAcara.AcaraJobfairId);
                if (acara == null)
                    return NotFound(new { message = "Acara tidak ditemukan." });

                // Cek apakah acara sudah dimulai
                if (acara.Status == "aktif")
                    return BadRequest(new { message = "Lamaran tidak dapat dibatalkan karena acara sudah dimulai." });

                var lowongan = await _context.JobVacancies
                    .FirstOrDefaultAsync(l => l.LowonganId == application.LowonganId);

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Kurangi jumlah pelamar di lowongan
                    if (lowongan != null && lowongan.JumlahPelamar > 0)
                    {
                        lowongan.JumlahPelamar--;
                        _context.JobVacancies.Update(lowongan);
                    }

                    // Hapus dari ApplyAcara
                    _context.ApplyAcaras.Remove(applyAcara);

                    // Hapus lamaran
                    _context.JobApplies.Remove(application);

                    // Cek apakah masih ada lamaran lain di acara ini
                    var lowonganIdsInAcara = await _context.LowonganAcaras
                        .Where(la => la.AcaraJobfairId == applyAcara.AcaraJobfairId)
                        .Select(la => la.LowonganId)
                        .ToListAsync();

                    var sisaLamaran = await _context.JobApplies
                        .CountAsync(ja => ja.TalentId == talentId &&
                                         lowonganIdsInAcara.Contains(ja.LowonganId) &&
                                         ja.ApplyId != applyId);

                    // Jika tidak ada lamaran lagi, hapus registrasi acara
                    if (sisaLamaran == 0)
                    {
                        var registration = await _context.TalentAcaraRegistrations
                            .FirstOrDefaultAsync(r => r.TalentId == talentId &&
                                                     r.AcaraJobfairId == applyAcara.AcaraJobfairId);

                        if (registration != null)
                        {
                            _context.TalentAcaraRegistrations.Remove(registration);

                            // Kurangi current capacity
                            if (acara.CurrentCapacity > 0)
                            {
                                acara.CurrentCapacity--;
                                _context.AcaraJobfairs.Update(acara);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var message = sisaLamaran == 0
                        ? "Lamaran berhasil dibatalkan. Anda telah dihapus dari pendaftaran acara ini."
                        : "Lamaran berhasil dibatalkan.";

                    return Ok(new { message = message, removedFromEvent = sisaLamaran == 0 });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Gagal membatalkan lamaran acara");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal membatalkan lamaran acara");
                return StatusCode(500, new { message = "Terjadi kesalahan saat membatalkan lamaran." });
            }
        }

        //---------------------------------------------------GET STATUS REGISTRASI ACARA----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpGet("status-registrasi/{acaraId}")]
        public async Task<IActionResult> GetStatusRegistrasiAcara(ulong acaraId)
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                var registration = await _context.TalentAcaraRegistrations
                    .FirstOrDefaultAsync(tar => tar.TalentId == talentId && tar.AcaraJobfairId == acaraId);

                // Hitung jumlah lamaran di acara ini
                var lowonganIdsInAcara = await _context.LowonganAcaras
                    .Where(la => la.AcaraJobfairId == acaraId)
                    .Select(la => la.LowonganId)
                    .ToListAsync();

                var jumlahLamaran = await _context.JobApplies
                    .CountAsync(ja => ja.TalentId == talentId && lowonganIdsInAcara.Contains(ja.LowonganId));

                return Ok(new
                {
                    isRegistered = registration != null,
                    registrationCode = registration?.RegistrationCode,
                    applicationCount = jumlahLamaran,
                    canApplyMore = jumlahLamaran < 3
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengambil status registrasi acara");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil status registrasi." });
            }
        }

        //---------------------------------------------------HELPER METHODS----------------------------------------------------

        private bool CanTalentApply(string minimalLulusan, string talentEducation)
        {
            var educationHierarchy = new Dictionary<string, int>
            {
                { "SMA", 1 }, { "SMK", 1 }, { "D1", 2 }, { "D2", 3 },
                { "D3", 4 }, { "D4", 5 }, { "S1", 5 }, { "S2", 6 }, { "S3", 7 }
            };

            if (educationHierarchy.ContainsKey(minimalLulusan.ToUpper()) &&
                educationHierarchy.ContainsKey(talentEducation.ToUpper()))
            {
                return educationHierarchy[talentEducation.ToUpper()] >= educationHierarchy[minimalLulusan.ToUpper()];
            }

            return false;
        }

        private List<string> GetEligibleEducationLevels(string minimalLulusan)
        {
            var educationHierarchy = new Dictionary<string, int>
            {
                { "SMA", 1 }, { "SMK", 1 }, { "D1", 2 }, { "D2", 3 },
                { "D3", 4 }, { "D4", 5 }, { "S1", 5 }, { "S2", 6 }, { "S3", 7 }
            };

            if (educationHierarchy.ContainsKey(minimalLulusan.ToUpper()))
            {
                var minLevel = educationHierarchy[minimalLulusan.ToUpper()];
                return educationHierarchy
                    .Where(x => x.Value >= minLevel)
                    .Select(x => x.Key)
                    .ToList();
            }

            return new List<string> { minimalLulusan };
        }

        private string GenerateRegistrationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return "REG-" + new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task<string> GenerateApplicationCode()
        {
            string code;
            bool exists;

            do
            {
                // Generate random 8 character code
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var randomString = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                code = $"APP-{randomString}";

                // Check if code already exists in database
                exists = await _context.JobApplies
                    .AnyAsync(ja => ja.ApplicationCode == code);

            } while (exists);

            return code;
        }



    }
}