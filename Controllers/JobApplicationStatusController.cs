using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using QRCoder;
using vocafind_api.Models;

namespace vocafind_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationStatusController : ControllerBase
    {
        private readonly TalentcerdasContext _context;
        private readonly ILogger<JobApplicationStatusController> _logger;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public JobApplicationStatusController(
            TalentcerdasContext context,
            ILogger<JobApplicationStatusController> logger,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpPut("update-status/{applyId}")]
        public async Task<IActionResult> UpdateApplicationStatus(string applyId, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                _logger.LogInformation($"=== MEMULAI UPDATE STATUS ===");
                _logger.LogInformation($"ApplyId: {applyId}, Status: {request?.Status}");

                // Validasi input
                if (request == null || string.IsNullOrEmpty(request.Status))
                {
                    return BadRequest(new { message = "Status tidak boleh kosong." });
                }

                // Cari lamaran dengan join ke Talent
                var applicationQuery = await (from ja in _context.JobApplies
                                              join t in _context.Talents on ja.TalentId equals t.TalentId
                                              join l in _context.JobVacancies on ja.LowonganId equals l.LowonganId
                                              join la in _context.LowonganAcaras on l.LowonganId equals la.LowonganId into laGroup
                                              from la in laGroup.DefaultIfEmpty()
                                              where ja.ApplyId == applyId
                                              select new
                                              {
                                                  JobApply = ja,
                                                  Talent = t,
                                                  Lowongan = l,
                                                  LowonganAcara = la
                                              })
                                        .FirstOrDefaultAsync();

                if (applicationQuery == null)
                {
                    _logger.LogWarning($"Lamaran tidak ditemukan: {applyId}");
                    return NotFound(new { message = "Lamaran tidak ditemukan." });
                }

                var application = applicationQuery.JobApply;
                var talent = applicationQuery.Talent;
                var lowongan = applicationQuery.Lowongan;
                var lowonganAcara = applicationQuery.LowonganAcara;

                _logger.LogInformation($"Application ditemukan: {application.ApplyId}");
                _logger.LogInformation($"Status saat ini: {application.Status}, Talent: {talent?.Nama}");
                _logger.LogInformation($"ApplicationCode: {application.ApplicationCode}");
                _logger.LogInformation($"LowonganId: {application.LowonganId}");
                _logger.LogInformation($"TalentId: {application.TalentId}");

                // Validasi status
                var validStatuses = new[] { "pending", "review", "interview", "accepted", "rejected" };
                if (!validStatuses.Contains(request.Status.ToLower()))
                {
                    return BadRequest(new { message = "Status tidak valid." });
                }

                // Simpan status lama untuk log
                var oldStatus = application.Status;

                // Update status
                application.Status = request.Status.ToLower();
                application.UpdatedAt = DateTime.Now;

                // Jika status diubah menjadi interview, set reviewed_at
                if (application.Status == "interview" && application.ReviewedAt == null)
                {
                    application.ReviewedAt = DateTime.Now;
                }

                _context.JobApplies.Update(application);

                // ✅ SAVE CHANGES DULU - Simpan perubahan status ke database
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Status berhasil disimpan ke database");

                // DEBUG: Log kondisi untuk generate QR code
                bool shouldGenerateQR = application.Status == "interview" && oldStatus != "interview";
                _logger.LogInformation($"Kondisi generate QR: Status={application.Status}, OldStatus={oldStatus}, ShouldGenerate={shouldGenerateQR}");

                string qrCodePath = null;

                // Jika status berubah menjadi interview, generate QR code
                if (shouldGenerateQR)
                {
                    _logger.LogInformation($"Memanggil GenerateInterviewQRCode...");
                    qrCodePath = await GenerateInterviewQRCode(application, talent, lowongan, lowonganAcara);
                    _logger.LogInformation($"Hasil GenerateInterviewQRCode: {qrCodePath ?? "NULL"}");
                }

                _logger.LogInformation($"=== UPDATE STATUS SELESAI ===");

                return Ok(new
                {
                    message = "Status lamaran berhasil diupdate.",
                    applicationId = application.ApplyId,
                    oldStatus = oldStatus,
                    newStatus = application.Status,
                    reviewedAt = application.ReviewedAt,
                    qrGenerated = shouldGenerateQR,
                    qrCodePath = qrCodePath
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Gagal update status lamaran: {applyId}");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengupdate status lamaran." });
            }
        }

        // Method GenerateInterviewQRCode yang mengembalikan path
        private async Task<string> GenerateInterviewQRCode(JobApply application, Talent talent, JobVacancy lowongan, LowonganAcara lowonganAcara)
        {
            try
            {
                _logger.LogInformation($"=== MEMULAI GENERATE QR CODE ===");
                _logger.LogInformation($"Application: {application.ApplyId}");

                // Jika lowonganAcara null, cari dari database
                if (lowonganAcara == null)
                {
                    _logger.LogInformation($"Mencari LowonganAcara untuk LowonganId: {application.LowonganId}");
                    lowonganAcara = await _context.LowonganAcaras
                        .Include(la => la.AcaraJobfair)
                        .FirstOrDefaultAsync(la => la.LowonganId == application.LowonganId);

                    if (lowonganAcara == null)
                    {
                        _logger.LogError($"Lowongan acara TIDAK ditemukan untuk lowongan: {application.LowonganId}");
                        return null;
                    }
                    _logger.LogInformation($"LowonganAcara ditemukan: {lowonganAcara.AcaraJobfair?.NamaAcara}");
                }
                else
                {
                    _logger.LogInformation($"LowonganAcara dari query: {lowonganAcara.AcaraJobfair?.NamaAcara}");
                }

                _logger.LogInformation($"LowonganAcara ditemukan: AcaraId={lowonganAcara.AcaraJobfairId}");

                // Cari registrasi talent di acara
                _logger.LogInformation($"Mencari registrasi untuk TalentId: {application.TalentId}, AcaraId: {lowonganAcara.AcaraJobfairId}");
                var registration = await _context.TalentAcaraRegistrations
                    .FirstOrDefaultAsync(r => r.TalentId == application.TalentId &&
                                            r.AcaraJobfairId == lowonganAcara.AcaraJobfairId);

                if (registration == null)
                {
                    _logger.LogWarning($"Registrasi tidak ditemukan, membuat baru...");

                    // Buat registrasi otomatis jika tidak ada
                    var newRegistrationCode = GenerateRegistrationCode();
                    registration = new TalentAcaraRegistration
                    {
                        TalentId = application.TalentId,
                        AcaraJobfairId = lowonganAcara.AcaraJobfairId,
                        RegistrationCode = newRegistrationCode,
                        Status = "registered",
                        CheckinStatus = "waiting",
                        RegisteredAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.TalentAcaraRegistrations.Add(registration);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Registrasi baru dibuat: {newRegistrationCode}");
                }
                else
                {
                    _logger.LogInformation($"Registrasi ditemukan: {registration.RegistrationCode}");
                }

                // Pastikan application code ada
                if (string.IsNullOrEmpty(application.ApplicationCode))
                {
                    _logger.LogWarning($"ApplicationCode kosong, generate baru...");
                    application.ApplicationCode = await GenerateApplicationCode();
                    _context.JobApplies.Update(application);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"ApplicationCode baru: {application.ApplicationCode}");
                }

                _logger.LogInformation($"ApplicationCode: {application.ApplicationCode}");

                // Data untuk QR code
                var qrData = new
                {
                    application_id = application.ApplyId,
                    application_code = application.ApplicationCode,
                    registration_code = registration.RegistrationCode,
                    talent_id = application.TalentId,
                    talent_name = talent?.Nama,
                    talent_email = talent?.Email,
                    lowongan_id = application.LowonganId,
                    lowongan_posisi = lowongan?.Posisi,
                    acara_id = lowonganAcara.AcaraJobfairId,
                    acara_name = lowonganAcara.AcaraJobfair?.NamaAcara,
                    status = "interview",
                    generated_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _logger.LogInformation($"QR Data prepared: ApplicationCode={application.ApplicationCode}, RegistrationCode={registration.RegistrationCode}");

                // Generate QR code
                var qrCodeImage = GenerateQRCodeImage(qrData);
                if (qrCodeImage == null)
                {
                    _logger.LogError($"Gagal generate QR code image");
                    return null;
                }

                _logger.LogInformation($"QR Code image berhasil di-generate");

                // Simpan QR code ke folder
                var fileName = $"qr_interview_{application.ApplicationCode}_{registration.RegistrationCode}.png";
                var folderPath = Path.Combine(_environment.WebRootPath, "upload", "qr");

                _logger.LogInformation($"Menyimpan ke: {folderPath}");
                _logger.LogInformation($"WebRootPath: {_environment.WebRootPath}");

                // Buat folder jika belum ada
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    _logger.LogInformation($"Folder dibuat: {folderPath}");
                }
                else
                {
                    _logger.LogInformation($"Folder sudah ada: {folderPath}");
                }

                var fullPath = Path.Combine(folderPath, fileName);
                _logger.LogInformation($"Full path: {fullPath}");

                try
                {
                    qrCodeImage.Save(fullPath, ImageFormat.Png);
                    _logger.LogInformation($"QR code berhasil disimpan: {fullPath}");

                    // Verifikasi file tersimpan
                    if (System.IO.File.Exists(fullPath))
                    {
                        var fileInfo = new FileInfo(fullPath);
                        _logger.LogInformation($"File verification: Exists={true}, Size={fileInfo.Length} bytes");
                    }
                    else
                    {
                        _logger.LogError($"File verification: File TIDAK ditemukan setelah save!");
                    }

                    // Simpan path QR code ke database
                    var savedPath = await SaveQRCodePathToDatabase(application, registration, fileName);
                    _logger.LogInformation($"Path QR code disimpan ke database: {savedPath}");

                    _logger.LogInformation($"=== GENERATE QR CODE SELESAI ===");
                    return savedPath;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Gagal menyimpan QR code ke file: {fullPath}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Gagal generate QR code untuk interview: {application.ApplyId}");
                return null;
            }
        }

        // Method GenerateQRCodeImage
        private Bitmap GenerateQRCodeImage(object data)
        {
            try
            {
                var jsonData = System.Text.Json.JsonSerializer.Serialize(data);
                _logger.LogInformation($"Generating QR code untuk data: {jsonData}");

                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(jsonData, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new QRCode(qrCodeData))
                    {
                        // Generate QR code dengan warna dan size
                        var qrCodeImage = qrCode.GetGraphic(20,
                            Color.Black,
                            Color.White,
                            true);

                        _logger.LogInformation($"QR code berhasil di-generate");
                        return qrCodeImage;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal generate QR code image");
                return null;
            }
        }

        // Method SaveQRCodePathToDatabase yang diperbaiki
        private async Task<string> SaveQRCodePathToDatabase(JobApply application, TalentAcaraRegistration registration, string fileName)
        {
            try
            {
                _logger.LogInformation($"=== MEMULAI SAVE QR PATH KE DATABASE ===");
                _logger.LogInformation($"ApplicationCode: {application.ApplicationCode}");
                _logger.LogInformation($"ApplyId: {application.ApplyId}");
                _logger.LogInformation($"RegistrationCode: {registration.RegistrationCode}");

                // ✅ VALIDASI: Pastikan ApplyId ada di job_apply
                var applicationExists = await _context.JobApplies
                    .AnyAsync(ja => ja.ApplyId == application.ApplyId);

                if (!applicationExists)
                {
                    _logger.LogError($"❌ ApplyId tidak ditemukan di job_apply: {application.ApplyId}");
                    return null;
                }
                _logger.LogInformation($"✅ ApplyId valid dan ada di job_apply");

                // Cek apakah sudah ada QR code untuk aplikasi ini
                var existingQr = await _context.AcaraQr
                    .FirstOrDefaultAsync(q => q.ApplicationCode == application.ApplicationCode);

                var qrPath = $"/uploads/qr/{fileName}";
                _logger.LogInformation($"QR Path yang akan disimpan: {qrPath}");

                if (existingQr != null)
                {
                    _logger.LogInformation($"QR code SUDAH ADA (ID: {existingQr.Id}), melakukan UPDATE...");
                    existingQr.QrCodePath = qrPath;
                    existingQr.ApplyId = application.ApplyId;
                    existingQr.RegistrationCode = registration.RegistrationCode;

                    _context.AcaraQr.Update(existingQr);
                }
                else
                {
                    _logger.LogInformation($"QR code BELUM ADA, membuat record BARU...");

                    // Buat baru
                    var acaraQr = new AcaraQr
                    {
                        ApplicationCode = application.ApplicationCode,
                        ApplyId = application.ApplyId,
                        RegistrationCode = registration.RegistrationCode,
                        QrCodePath = qrPath,
                    };

                    _logger.LogInformation($"Data QR yang akan disimpan:");
                    _logger.LogInformation($"  - ApplicationCode: {acaraQr.ApplicationCode}");
                    _logger.LogInformation($"  - ApplyId: {acaraQr.ApplyId}");
                    _logger.LogInformation($"  - RegistrationCode: {acaraQr.RegistrationCode}");
                    _logger.LogInformation($"  - QrCodePath: {acaraQr.QrCodePath}");

                    _context.AcaraQr.Add(acaraQr);
                    _logger.LogInformation($"Entity ditambahkan ke context");
                }

                // Save changes dengan error handling lebih detail
                _logger.LogInformation($"Memanggil SaveChangesAsync...");

                try
                {
                    var result = await _context.SaveChangesAsync();
                    _logger.LogInformation($"SaveChangesAsync selesai: {result} rows affected");

                    if (result == 0)
                    {
                        _logger.LogError($"⚠️ WARNING: SaveChangesAsync return 0 rows affected!");
                        _logger.LogError($"Kemungkinan tidak ada perubahan atau data sudah sama");
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError($"❌ DbUpdateException saat SaveChanges!");
                    _logger.LogError($"Message: {dbEx.Message}");

                    if (dbEx.InnerException != null)
                    {
                        _logger.LogError($"InnerException: {dbEx.InnerException.Message}");

                        // Cek apakah masalah foreign key
                        if (dbEx.InnerException.Message.Contains("foreign key") ||
                            dbEx.InnerException.Message.Contains("FOREIGN KEY"))
                        {
                            _logger.LogError($"❌ MASALAH FOREIGN KEY CONSTRAINT!");
                            _logger.LogError($"ApplyId yang dicoba: {application.ApplyId}");
                        }
                    }

                    return null;
                }

                // Verifikasi data tersimpan
                _logger.LogInformation($"Melakukan verifikasi...");
                var verifyQr = await _context.AcaraQr
                    .FirstOrDefaultAsync(q => q.ApplicationCode == application.ApplicationCode);

                if (verifyQr != null)
                {
                    _logger.LogInformation($"✅ VERIFIKASI BERHASIL!");
                    _logger.LogInformation($"  - ID: {verifyQr.Id}");
                    _logger.LogInformation($"  - ApplicationCode: {verifyQr.ApplicationCode}");
                    _logger.LogInformation($"  - ApplyId: {verifyQr.ApplyId}");
                    _logger.LogInformation($"  - QrCodePath: {verifyQr.QrCodePath}");

                    _logger.LogInformation($"=== SAVE QR PATH SELESAI ===");
                    return qrPath;
                }
                else
                {
                    _logger.LogError($"❌ VERIFIKASI GAGAL: Data tidak ditemukan setelah save!");

                    // Cek semua data di tabel
                    var allQrs = await _context.AcaraQr.ToListAsync();
                    _logger.LogError($"Total records di acara_qr: {allQrs.Count}");

                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ EXCEPTION saat menyimpan path QR code");
                _logger.LogError($"Exception Type: {ex.GetType().Name}");
                _logger.LogError($"Message: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"InnerException Type: {ex.InnerException.GetType().Name}");
                    _logger.LogError($"InnerException Message: {ex.InnerException.Message}");
                }

                return null;
            }
        }


        [HttpPost("test-insert-qr/{applyId}")]
        public async Task<IActionResult> TestInsertQR(string applyId)
        {
            try
            {
                _logger.LogInformation($"Testing insert QR for: {applyId}");

                var application = await _context.JobApplies
                    .FirstOrDefaultAsync(a => a.ApplyId == applyId);

                if (application == null)
                {
                    return NotFound(new { message = "Application not found" });
                }

                var testQr = new AcaraQr
                {
                    ApplicationCode = "TEST-" + DateTime.Now.Ticks,
                    ApplyId = applyId,
                    RegistrationCode = "TEST-REG",
                    QrCodePath = "/uploads/qr/test.png"
                };

                _logger.LogInformation($"Attempting to insert: {System.Text.Json.JsonSerializer.Serialize(testQr)}");

                _context.AcaraQr.Add(testQr);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation($"Insert result: {result} rows");

                return Ok(new { message = "Test insert success", rows = result, id = testQr.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test insert failed");
                return StatusCode(500, new
                {
                    message = "Test insert failed",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }


        // Helper method untuk generate registration code
        private string GenerateRegistrationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return "REG-" + new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Helper method untuk generate application code
        private async Task<string> GenerateApplicationCode()
        {
            string code;
            bool exists;

            do
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var randomString = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                code = $"APP-{randomString}";

                exists = await _context.JobApplies
                    .AnyAsync(ja => ja.ApplicationCode == code);

            } while (exists);

            return code;
        }

        [HttpGet("qr-code/{applyId}")]
        public async Task<IActionResult> GetQRCodeByApplication(string applyId)
        {
            try
            {
                _logger.LogInformation($"Mencari QR code untuk applyId: {applyId}");

                // Cari data QR code
                var qrData = await _context.AcaraQr
                    .FirstOrDefaultAsync(q => q.ApplyId == applyId);

                if (qrData == null)
                {
                    _logger.LogWarning($"QR code tidak ditemukan di database untuk applyId: {applyId}");
                    return NotFound(new { message = "QR code tidak ditemukan untuk lamaran ini." });
                }

                if (string.IsNullOrEmpty(qrData.QrCodePath))
                {
                    _logger.LogWarning($"QrCodePath kosong untuk applyId: {applyId}");
                    return NotFound(new { message = "Path QR code tidak valid." });
                }

                _logger.LogInformation($"QR code ditemukan: {qrData.QrCodePath}");

                var fullPath = Path.Combine(_environment.WebRootPath, qrData.QrCodePath.TrimStart('/'));
                _logger.LogInformation($"Full path file: {fullPath}");

                if (!System.IO.File.Exists(fullPath))
                {
                    _logger.LogWarning($"File QR code tidak ditemukan di path: {fullPath}");
                    return NotFound(new { message = "File QR code tidak ditemukan." });
                }

                var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                _logger.LogInformation($"QR code berhasil diambil, size: {imageBytes.Length} bytes");

                return File(imageBytes, "image/png", Path.GetFileName(fullPath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Gagal mengambil QR code untuk lamaran: {applyId}");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil QR code." });
            }
        }

        [HttpGet("check-qr-status/{applyId}")]
        public async Task<IActionResult> CheckQRCodeStatus(string applyId)
        {
            try
            {
                _logger.LogInformation($"Checking QR status untuk: {applyId}");

                // Cari application
                var application = await _context.JobApplies
                    .FirstOrDefaultAsync(a => a.ApplyId == applyId);

                if (application == null)
                {
                    return NotFound(new { message = "Lamaran tidak ditemukan." });
                }

                // Cari QR code
                var qrData = await _context.AcaraQr
                    .FirstOrDefaultAsync(q => q.ApplyId == applyId);

                var fullPath = "";
                var fileExists = false;

                if (qrData != null && !string.IsNullOrEmpty(qrData.QrCodePath))
                {
                    fullPath = Path.Combine(_environment.WebRootPath, qrData.QrCodePath.TrimStart('/'));
                    fileExists = System.IO.File.Exists(fullPath);
                }

                return Ok(new
                {
                    applicationId = applyId,
                    applicationStatus = application.Status,
                    hasQRCode = qrData != null,
                    qrCodePath = qrData?.QrCodePath,
                    fileExists = fileExists,
                    fullPath = fullPath
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Gagal check QR status: {applyId}");
                return StatusCode(500, new { message = "Terjadi kesalahan saat check QR status." });
            }
        }

        [HttpGet("debug-application/{applyId}")]
        public async Task<IActionResult> DebugApplication(string applyId)
        {
            try
            {
                _logger.LogInformation($"Debug application: {applyId}");

                var application = await _context.JobApplies
                    .FirstOrDefaultAsync(a => a.ApplyId == applyId);

                if (application == null)
                {
                    return NotFound(new { message = "Application not found" });
                }

                var qrCodes = await _context.AcaraQr
                    .Where(q => q.ApplyId == applyId)
                    .ToListAsync();

                return Ok(new
                {
                    application.ApplyId,
                    application.ApplicationCode,
                    application.Status,
                    application.TalentId,
                    application.LowonganId,
                    QrCodesCount = qrCodes.Count,
                    QrCodes = qrCodes.Select(q => new { q.ApplicationCode, q.QrCodePath })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Debug error: {applyId}");
                return StatusCode(500, new { message = "Debug error", error = ex.Message });
            }
        }

        [HttpGet("test-db-connection")]
        public async Task<IActionResult> TestDbConnection()
        {
            try
            {
                _logger.LogInformation("Testing database connection...");

                // Test JobApplies
                var jobAppliesCount = await _context.JobApplies.CountAsync();
                _logger.LogInformation($"JobApplies count: {jobAppliesCount}");

                // Test AcaraQr
                var acaraQrCount = await _context.AcaraQr.CountAsync();
                _logger.LogInformation($"AcaraQr count: {acaraQrCount}");

                // Test LowonganAcaras
                var lowonganAcarasCount = await _context.LowonganAcaras.CountAsync();
                _logger.LogInformation($"LowonganAcaras count: {lowonganAcarasCount}");

                // Test TalentAcaraRegistrations
                var registrationsCount = await _context.TalentAcaraRegistrations.CountAsync();
                _logger.LogInformation($"TalentAcaraRegistrations count: {registrationsCount}");

                return Ok(new
                {
                    jobAppliesCount,
                    acaraQrCount,
                    lowonganAcarasCount,
                    registrationsCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection test failed");
                return StatusCode(500, new { message = "Database test failed", error = ex.Message });
            }
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; }
        public string InterviewSlotId { get; set; }
        public string LocationInterview { get; set; }
    }
}