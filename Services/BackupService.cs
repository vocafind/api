using MySqlConnector;
using System.Text;

namespace YourNamespace.Services
{
    public interface IBackupService
    {
        Task<string> BackupDatabaseAsync(string backupPath);
        Task<bool> RestoreDatabaseAsync(string backupFilePath);
        Task<List<string>> GetBackupFilesAsync(string backupDirectory);
    }

    public class BackupService : IBackupService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BackupService> _logger;
        private readonly string _connectionString;

        public BackupService(IConfiguration configuration, ILogger<BackupService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<string> BackupDatabaseAsync(string backupPath)
        {
            try
            {
                // Create backup directory if not exists
                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }

                // Generate filename with timestamp
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var backupFilePath = Path.Combine(backupPath, $"backup_{timestamp}.sql");

                var sb = new StringBuilder();

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var databaseName = connection.Database;

                    // Header
                    sb.AppendLine("-- MySQL Database Backup");
                    sb.AppendLine($"-- Database: {databaseName}");
                    sb.AppendLine($"-- Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sb.AppendLine("-- ------------------------------------------------------");
                    sb.AppendLine();
                    sb.AppendLine("SET FOREIGN_KEY_CHECKS=0;");
                    sb.AppendLine();

                    // Get all tables
                    var tables = new List<string>();
                    using (var cmd = new MySqlCommand("SHOW TABLES", connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }

                    // Backup each table
                    foreach (var table in tables)
                    {
                        sb.AppendLine($"-- Table structure for table `{table}`");
                        sb.AppendLine($"DROP TABLE IF EXISTS `{table}`;");

                        // Get CREATE TABLE statement
                        using (var cmd = new MySqlCommand($"SHOW CREATE TABLE `{table}`", connection))
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                sb.AppendLine(reader.GetString(1) + ";");
                            }
                        }

                        sb.AppendLine();
                        sb.AppendLine($"-- Dumping data for table `{table}`");

                        // Get table data
                        using (var cmd = new MySqlCommand($"SELECT * FROM `{table}`", connection))
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                var columnCount = reader.FieldCount;
                                var columnNames = new List<string>();

                                for (int i = 0; i < columnCount; i++)
                                {
                                    columnNames.Add($"`{reader.GetName(i)}`");
                                }

                                while (await reader.ReadAsync())
                                {
                                    var values = new List<string>();

                                    for (int i = 0; i < columnCount; i++)
                                    {
                                        if (reader.IsDBNull(i))
                                        {
                                            values.Add("NULL");
                                        }
                                        else
                                        {
                                            var value = reader.GetValue(i);

                                            if (value is byte[] byteArray)
                                            {
                                                values.Add($"0x{BitConverter.ToString(byteArray).Replace("-", "")}");
                                            }
                                            else if (value is DateTime dateTime)
                                            {
                                                values.Add($"'{dateTime:yyyy-MM-dd HH:mm:ss}'");
                                            }
                                            else if (value is bool boolValue)
                                            {
                                                values.Add(boolValue ? "1" : "0");
                                            }
                                            else if (IsNumericType(value))
                                            {
                                                values.Add(value.ToString()!);
                                            }
                                            else
                                            {
                                                var stringValue = value.ToString()!
                                                    .Replace("\\", "\\\\")
                                                    .Replace("'", "\\'")
                                                    .Replace("\n", "\\n")
                                                    .Replace("\r", "\\r");
                                                values.Add($"'{stringValue}'");
                                            }
                                        }
                                    }

                                    sb.AppendLine($"INSERT INTO `{table}` ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", values)});");
                                }
                            }
                        }

                        sb.AppendLine();
                    }

                    sb.AppendLine("SET FOREIGN_KEY_CHECKS=1;");
                }

                // Write to file
                await File.WriteAllTextAsync(backupFilePath, sb.ToString());

                _logger.LogInformation($"Database backup created successfully: {backupFilePath}");
                return backupFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database backup");
                throw;
            }
        }

        public async Task<bool> RestoreDatabaseAsync(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    throw new FileNotFoundException("Backup file not found", backupFilePath);
                }

                var sqlContent = await File.ReadAllTextAsync(backupFilePath);

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Disable foreign key checks
                    using (var cmd = new MySqlCommand("SET FOREIGN_KEY_CHECKS=0", connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Split SQL statements by semicolon
                    var lines = sqlContent.Split('\n');
                    var currentStatement = new StringBuilder();

                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();

                        // Skip comments and empty lines
                        if (string.IsNullOrWhiteSpace(trimmedLine) ||
                            trimmedLine.StartsWith("--") ||
                            trimmedLine.StartsWith("/*"))
                        {
                            continue;
                        }

                        currentStatement.AppendLine(line);

                        // Execute when we hit a semicolon at the end of line
                        if (trimmedLine.EndsWith(";"))
                        {
                            var statement = currentStatement.ToString().Trim();

                            if (!string.IsNullOrWhiteSpace(statement))
                            {
                                try
                                {
                                    using var cmd = new MySqlCommand(statement, connection);
                                    cmd.CommandTimeout = 300; // 5 minutes timeout
                                    await cmd.ExecuteNonQueryAsync();
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning($"Error executing statement: {ex.Message}");
                                    // Continue with next statement
                                }
                            }

                            currentStatement.Clear();
                        }
                    }

                    // Re-enable foreign key checks
                    using (var cmd = new MySqlCommand("SET FOREIGN_KEY_CHECKS=1", connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                _logger.LogInformation($"Database restored successfully from: {backupFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring database");
                throw;
            }
        }

        public async Task<List<string>> GetBackupFilesAsync(string backupDirectory)
        {
            try
            {
                if (!Directory.Exists(backupDirectory))
                {
                    return new List<string>();
                }

                var files = Directory.GetFiles(backupDirectory, "*.sql")
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .ToList();

                return await Task.FromResult(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting backup files");
                throw;
            }
        }


        private bool IsNumericType(object value)
        {
            return value is sbyte || value is byte || value is short || value is ushort ||
                   value is int || value is uint || value is long || value is ulong ||
                   value is float || value is double || value is decimal;
        }
    }
}