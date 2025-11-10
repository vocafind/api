using Microsoft.AspNetCore.Mvc;
using YourNamespace.Services;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<BackupController> _logger;
        private readonly string _backupFolder;

        public BackupController(
            IBackupService backupService,
            ILogger<BackupController> logger)
        {
            _backupService = backupService;
            _logger = logger;
            _backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "Backups");

            // Create backup folder if not exists
            if (!Directory.Exists(_backupFolder))
            {
                Directory.CreateDirectory(_backupFolder);
            }
        }

        /// <summary>
        /// Create a database backup
        /// </summary>
        [HttpPost("backup")]
        public async Task<IActionResult> CreateBackup()
        {
            try
            {
                var filePath = await _backupService.BackupDatabaseAsync(_backupFolder);
                var fileName = Path.GetFileName(filePath);

                return Ok(new
                {
                    success = true,
                    message = "Backup berhasil dibuat",
                    fileName = fileName,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating backup");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Gagal membuat backup",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Restore database from backup file
        /// </summary>
        [HttpPost("restore")]
        public async Task<IActionResult> RestoreBackup([FromBody] RestoreRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FileName))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Nama file harus diisi"
                    });
                }

                var filePath = Path.Combine(_backupFolder, request.FileName);
                var result = await _backupService.RestoreDatabaseAsync(filePath);

                return Ok(new
                {
                    success = result,
                    message = "Database berhasil di-restore",
                    fileName = request.FileName,
                    timestamp = DateTime.Now
                });
            }
            catch (FileNotFoundException)
            {
                return NotFound(new
                {
                    success = false,
                    message = "File backup tidak ditemukan"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring backup");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Gagal restore database",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get list of available backup files
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetBackupFiles()
        {
            try
            {
                var files = await _backupService.GetBackupFilesAsync(_backupFolder);

                var fileInfos = files.Select(f => new
                {
                    fileName = Path.GetFileName(f),
                    size = new FileInfo(f).Length,
                    sizeFormatted = FormatFileSize(new FileInfo(f).Length),
                    createdDate = System.IO.File.GetCreationTime(f)
                }).ToList();

                return Ok(new
                {
                    success = true,
                    count = fileInfos.Count,
                    files = fileInfos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting backup files");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Gagal mengambil daftar backup",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Download a backup file
        /// </summary>
        [HttpGet("download/{fileName}")]
        public IActionResult DownloadBackup(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_backupFolder, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "File backup tidak ditemukan"
                    });
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading backup file");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Gagal download backup",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete a backup file
        /// </summary>
        [HttpDelete("delete/{fileName}")]
        public IActionResult DeleteBackup(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_backupFolder, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "File backup tidak ditemukan"
                    });
                }

                System.IO.File.Delete(filePath);

                return Ok(new
                {
                    success = true,
                    message = "File backup berhasil dihapus",
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting backup file");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Gagal menghapus backup",
                    error = ex.Message
                });
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    public class RestoreRequest
    {
        public string FileName { get; set; } = string.Empty;
    }
}