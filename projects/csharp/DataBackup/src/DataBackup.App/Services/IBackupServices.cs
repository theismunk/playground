using System.Threading.Tasks;

namespace DataBackup.Services;

/// <summary>
/// Service for compressing files into zip archives
/// </summary>
public interface IZipService
{
    /// <summary>
    /// Creates a zip file from a directory
    /// </summary>
    Task<string> CreateZipAsync(string sourceDirectory);
}

/// <summary>
/// Service for uploading backups to AWS S3
/// </summary>
public interface IS3Service
{
    /// <summary>
    /// Uploads a file to S3
    /// </summary>
    Task UploadToS3Async(string filePath, string bucketName, string? keyPrefix = null);
}

/// <summary>
/// Service for uploading backups to AWS Glacier
/// </summary>
public interface IGlacierService
{
    /// <summary>
    /// Uploads a file to Glacier
    /// </summary>
    Task UploadToGlacierAsync(string filePath, string vaultName);
}

/// <summary>
/// Main backup orchestration service
/// </summary>
public interface IBackupService
{
    /// <summary>
    /// Performs backup of specified directory to S3 and/or Glacier
    /// </summary>
    Task BackupAsync(string sourceDirectory, string destination, bool backupToS3, bool backupToGlacier);
}
