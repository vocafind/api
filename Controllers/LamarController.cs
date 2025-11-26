using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using vocafind_api.DTO;
using vocafind_api.Models;

namespace vocafind_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LamarController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly ILogger<LamarController> _logger;
        private readonly IMapper _mapper;

        public LamarController(TalentcerdasContext context, ILogger<LamarController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        //---------------------------------------------------LAMAR PEKERJAAN----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpPost("lokerUmum/{lowonganId}")]
        public async Task<IActionResult> Create(string lowonganId)
        {
            try
            {
                // Get authenticated talent
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Unauthorized access." });

                // Validasi: Apakah lowongan dengan ID ini ada?
                var lowongan = await _context.JobVacancies
                    .FirstOrDefaultAsync(j => j.LowonganId == lowonganId);

                if (lowongan == null)
                {
                    return NotFound(new { message = "Lowongan tidak ditemukan." });
                }

                // Get talent data for education level validation
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
                    return NotFound(new { message = "Talent tidak ditemukan." });
                }

                // Cek statusAkun, hanya boleh jika Sudah Terverifikasi atau Alumni
                if (talent.StatusAkun != "Sudah Terverifikasi" && talent.StatusAkun != "Alumni")
                {
                    return BadRequest(new { message = "Akun anda belum diverifikasi oleh administrator." });
                }

                // Validasi kelengkapan data talent berdasarkan pendidikan
                var missing = new List<string>();

                // Data dasar wajib untuk semua level pendidikan - PERBAIKAN: gunakan field yang sesuai
                if (string.IsNullOrEmpty(talent.Nama) || string.IsNullOrEmpty(talent.Email) ||
                    string.IsNullOrEmpty(talent.NomorTelepon) || talent.Usia == null ||
                    string.IsNullOrEmpty(talent.JenisKelamin) || string.IsNullOrEmpty(talent.Alamat) ||
                    string.IsNullOrEmpty(talent.KabupatenKota) || string.IsNullOrEmpty(talent.Provinsi) ||
                    talent.PreferensiGaji == null || // PERBAIKAN: PreferensiGaji adalah int?
                    string.IsNullOrEmpty(talent.LokasiKerjaDiinginkan) || string.IsNullOrEmpty(talent.StatusPekerjaanSaatIni) ||
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
                    // D3/D4/S1 wajib memiliki experience dan soft skills
                    else if (new[] { "S1", "D3", "D4" }.Contains(educationLevel))
                    {
                        if (!talent.SoftSkills.Any())
                        {
                            missing.Add("Soft Skills");
                        }
                    }
                    // S2/S3 wajib mengisi semuanya
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
                        return BadRequest(new { message = $"Tingkat pendidikan tidak dikenali: {educationLevel}" });
                    }
                }

                if (missing.Any())
                {
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
                        return BadRequest(new { message = $"Maaf, lowongan ini membutuhkan minimal lulusan {lowongan.MinimalLulusan}. Tingkat pendidikan yang memenuhi syarat: {string.Join(", ", eligibleLevels)}." });
                    }
                }

                // Check if job is still active
                if (lowongan.Status != "aktif")
                {
                    return BadRequest(new { message = "Lowongan ini sudah tidak aktif." });
                }

                // Cek apakah sudah melamar sebelumnya - PERBAIKAN: gunakan field yang sesuai
                var sudahMelamar = await _context.JobApplies
                    .AnyAsync(j => j.LowonganId == lowonganId && j.TalentId == talentId);

                if (sudahMelamar)
                {
                    return BadRequest(new { message = "Anda sudah melamar lowongan ini" });
                }

                // Begin transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Create application - PERBAIKAN: gunakan field yang sesuai dengan model
                    var application = new JobApply
                    {
                        ApplyId = Guid.NewGuid().ToString(),
                        LowonganId = lowonganId,
                        TalentId = talentId,
                        Status = "pending",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        AppliedAt = DateTime.Now
                    };

                    _context.JobApplies.Add(application);

                    // Atomically increment jumlahPelamar on the vacancy
                    lowongan.JumlahPelamar++;
                    _context.JobVacancies.Update(lowongan);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { message = "Lamaran berhasil dikirim (cek pada menu lamaran saya)." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Gagal menyimpan lamaran");
                    throw; // Will be caught by outer catch block
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal menyimpan lamaran");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengirim lamaran. Silakan coba lagi." });
            }
        }

        // Helper method to check if talent can apply based on education level
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

        // Helper method to get eligible education levels
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

        //---------------------------------------------------GET LAMARAN SAYA----------------------------------------------------
        [Authorize(Roles = "Talent")]
        [HttpGet("lamaran-saya")]
        public async Task<IActionResult> GetLamaranSaya()
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                // AMBIL HANYA LAMARAN UMUM (BUKAN JOBFAIR)
                var lamaran = await (from apply in _context.JobApplies
                                     join lowongan in _context.JobVacancies on apply.LowonganId equals lowongan.LowonganId
                                     join company in _context.Companies on lowongan.CompanyId equals company.CompanyId

                                     // LEFT JOIN ke LowonganAcaras untuk cek apakah ini jobfair
                                     join lowonganAcara in _context.LowonganAcaras
                                         on apply.LowonganId equals lowonganAcara.LowonganId into laGroup
                                     from lowonganAcara in laGroup.DefaultIfEmpty()

                                     where apply.TalentId == talentId
                                         && lowonganAcara == null // HANYA yang BUKAN jobfair (tidak ada di LowonganAcaras)
                                     orderby apply.CreatedAt descending
                                     select new
                                     {
                                         apply.ApplyId,
                                         apply.LowonganId,
                                         apply.Status,
                                         apply.Interview,
                                         apply.Location_interview,
                                         apply.CreatedAt,
                                         apply.AppliedAt,
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

                _logger.LogInformation($"Ambil lamaran umum: {lamaran.Count} lamaran untuk talent {talentId}");

                return Ok(lamaran);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengambil data lamaran");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data lamaran." });
            }
        }


        [Authorize(Roles = "Talent")]
        [HttpGet("lamaran-jobfair-saya")]
        public async Task<IActionResult> GetLamaranJobfairSaya()
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                // DEBUG: Cek data JobApply terlebih dahulu
                var jobApplies = await _context.JobApplies
                    .Where(j => j.TalentId == talentId)
                    .Select(j => new
                    {
                        j.ApplyId,
                        j.LowonganId,
                        j.Status,
                        j.InterviewSlot,
                        j.Interview,
                        j.Location_interview,
                        j.ApplicationCode // ✅ TAMBAHKAN INI
                    })
                    .ToListAsync();

                _logger.LogInformation($"DEBUG JobApplies: {JsonSerializer.Serialize(jobApplies)}");

                // AMBIL HANYA LAMARAN JOBFAIR dengan data interview yang benar + DATA QR CODE
                var lamaran = await (from apply in _context.JobApplies
                                     join lowongan in _context.JobVacancies on apply.LowonganId equals lowongan.LowonganId
                                     join company in _context.Companies on lowongan.CompanyId equals company.CompanyId

                                     // INNER JOIN ke LowonganAcaras untuk pastikan ini jobfair
                                     join lowonganAcara in _context.LowonganAcaras
                                         on apply.LowonganId equals lowonganAcara.LowonganId
                                     join acara in _context.AcaraJobfairs
                                         on lowonganAcara.AcaraJobfairId equals acara.Id

                                     // LEFT JOIN ke AcaraInterviewSlot untuk data interview
                                     join interviewSlot in _context.AcaraInterviewSlots
                                         on apply.InterviewSlot equals interviewSlot.Id into slotGroup
                                     from interviewSlot in slotGroup.DefaultIfEmpty()

                                         // ✅ PERBAIKI: LEFT JOIN ke AcaraQr menggunakan ApplyId
                                     join acaraQr in _context.AcaraQr
                                         on apply.ApplyId equals acaraQr.ApplyId into qrGroup
                                     from acaraQr in qrGroup.DefaultIfEmpty()

                                         // ✅ PERBAIKI: LEFT JOIN ke TalentAcaraRegistration 
                                     join talentReg in _context.TalentAcaraRegistrations
                                         on new { TalentId = apply.TalentId, AcaraId = acara.Id } equals
                                         new { TalentId = talentReg.TalentId, AcaraId = talentReg.AcaraJobfairId } into regGroup
                                     from talentReg in regGroup.DefaultIfEmpty()

                                     where apply.TalentId == talentId
                                     orderby apply.CreatedAt descending
                                     select new
                                     {
                                         apply.ApplyId,
                                         apply.LowonganId,
                                         apply.Status,
                                         apply.ApplicationCode, // ✅ TAMBAHKAN APPLICATION CODE
                                         Interview = interviewSlot != null ? interviewSlot.Slot : apply.Interview,
                                         LocationInterview = apply.Location_interview,
                                         apply.CreatedAt,
                                         apply.AppliedAt,
                                         InterviewSlotId = apply.InterviewSlot,

                                         // DEBUG: Tambahkan field untuk troubleshooting
                                         HasInterviewSlot = apply.InterviewSlot != null,
                                         InterviewSlotValue = apply.InterviewSlot,
                                         InterviewSlotTableId = interviewSlot != null ? interviewSlot.Id : (ulong?)null,
                                         InterviewSlotTableSlot = interviewSlot != null ? interviewSlot.Slot : null,

                                         // DATA ACARA JOBFAIR
                                         Acara = new
                                         {
                                             acara.Id,
                                             acara.NamaAcara,
                                             acara.Deskripsi,
                                             acara.Lokasi,
                                             acara.AlamatAcara,
                                             acara.Provinsi,
                                             acara.Kabupaten,
                                             acara.TanggalMulaiAcara,
                                             acara.TanggalSelesaiAcara,
                                             acara.TanggalAwalPendaftaranAcara,
                                             acara.TanggalAkhirPendaftaranAcara,
                                             acara.Status,
                                             acara.MaxCapacity,
                                             acara.CurrentCapacity
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
                                         },

                                         // DATA INTERVIEW SLOT JIKA ADA
                                         InterviewSlotData = interviewSlot != null ? new
                                         {
                                             interviewSlot.Id,
                                             interviewSlot.Slot,
                                             interviewSlot.CreatedAt
                                         } : null,

                                         // ✅ DATA QR CODE DARI TABEL ACARA_QR
                                         QrCodeData = acaraQr != null ? new
                                         {
                                             acaraQr.Id,
                                             acaraQr.ApplicationCode,
                                             acaraQr.ApplyId,
                                             acaraQr.RegistrationCode,
                                             acaraQr.QrCodePath,
                                             // Tambahkan URL lengkap jika perlu
                                             QrCodeUrl = !string.IsNullOrEmpty(acaraQr.QrCodePath)
                                                 ? $"{Request.Scheme}://{Request.Host}/{acaraQr.QrCodePath.TrimStart('/')}"
                                                 : null
                                         } : null,

                                         // ✅ DATA REGISTRASI ACARA
                                         TalentRegistration = talentReg != null ? new
                                         {
                                             talentReg.Id,
                                             talentReg.RegistrationCode,
                                             talentReg.QrCodePath,
                                             talentReg.Status,
                                             talentReg.CheckinStatus,
                                             talentReg.RegisteredAt,
                                             talentReg.AttendedAt,
                                             talentReg.CheckedInAt,
                                             talentReg.CheckedOutAt,
                                             // Tambahkan URL lengkap jika perlu
                                             QrCodeUrl = !string.IsNullOrEmpty(talentReg.QrCodePath)
                                                 ? $"{Request.Scheme}://{Request.Host}/{talentReg.QrCodePath.TrimStart('/')}"
                                                 : null
                                         } : null,

                                         // ✅ FLAG UNTUK FRONTEND
                                         IsJobfair = true,
                                         HasQrCode = acaraQr != null || talentReg != null,
                                         QrCodeSource = acaraQr != null ? "acara_qr" :
                                                       talentReg != null ? "talent_registration" : "none"
                                     }).ToListAsync();

                _logger.LogInformation($"Ambil lamaran jobfair: {lamaran.Count} lamaran untuk talent {talentId}");

                // DEBUG: Log sample data untuk inspection
                if (lamaran.Any())
                {
                    var sample = lamaran.First();
                    _logger.LogInformation($"SAMPLE DATA - ApplyId: {sample.ApplyId}, Status: {sample.Status}, " +
                                         $"HasQrCode: {sample.HasQrCode}, QrCodeSource: {sample.QrCodeSource}");

                    // ✅ DEBUG DETAIL: Cek apakah data QR code ada
                    if (sample.QrCodeData != null)
                    {
                        _logger.LogInformation($"QR CODE DATA FOUND - ApplyId: {sample.QrCodeData.ApplyId}, " +
                                             $"QR Path: {sample.QrCodeData.QrCodePath}");
                    }
                    else
                    {
                        _logger.LogInformation($"QR CODE DATA NOT FOUND for ApplyId: {sample.ApplyId}");

                        // ✅ DEBUG: Cek langsung dari database
                        var directQrCheck = await _context.AcaraQr
                            .Where(q => q.ApplyId == sample.ApplyId)
                            .FirstOrDefaultAsync();

                        _logger.LogInformation($"DIRECT QR CHECK - Found: {directQrCheck != null}, " +
                                             $"ApplyId: {directQrCheck?.ApplyId}, Path: {directQrCheck?.QrCodePath}");
                    }
                }

                return Ok(lamaran);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengambil data lamaran jobfair");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data lamaran jobfair." });
            }
        }




        //---------------------------------------------------BATAL LAMARAN----------------------------------------------------

        [Authorize(Roles = "Talent")]
        [HttpDelete("batal/{lamaranId}")]
        public async Task<IActionResult> BatalkanLamaran(string lamaranId)
        {
            try
            {
                var talentId = User.FindFirst("TalentId")?.Value;
                if (string.IsNullOrEmpty(talentId))
                    return Unauthorized(new { message = "Token tidak valid." });

                var lamaran = await _context.JobApplies
                    .FirstOrDefaultAsync(j => j.ApplyId == lamaranId && j.TalentId == talentId);

                if (lamaran == null)
                    return NotFound(new { message = "Lamaran tidak ditemukan." });

                // Hanya bisa membatalkan jika status masih pending
                if (lamaran.Status != "pending")
                    return BadRequest(new { message = "Lamaran tidak dapat dibatalkan karena sudah diproses." });

                // Dapatkan lowongan terkait
                var lowongan = await _context.JobVacancies
                    .FirstOrDefaultAsync(l => l.LowonganId == lamaran.LowonganId);

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Kurangi jumlah pelamar di lowongan
                    if (lowongan != null && lowongan.JumlahPelamar > 0)
                    {
                        lowongan.JumlahPelamar--;
                        _context.JobVacancies.Update(lowongan);
                    }

                    // Hapus lamaran
                    _context.JobApplies.Remove(lamaran);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { message = "Lamaran berhasil dibatalkan." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Gagal membatalkan lamaran");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal membatalkan lamaran");
                return StatusCode(500, new { message = "Terjadi kesalahan saat membatalkan lamaran." });
            }
        }
    }
}