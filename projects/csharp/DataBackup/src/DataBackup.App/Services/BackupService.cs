using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace DataBackup.Services;

public class BackupService : IBackupService
{
    private readonly IZipService _zipService;
    private readonly IS3Service _s3Service;
    private readonly IGlacierService _glacierService;
    private readonly ILogger _logger;

    public BackupService(IZipService zipService, IS3Service s3Service, IGlacierService glacierService, ILogger logger)
    {
        _zipService = zipService;
        _s3Service = s3Service;
        _glacierService = glacierService;
        _logger = logger;
    }

    public async Task BackupAsync(string sourceDirectory, string destination, bool backupToS3, bool backupToGlacier)
    {
        try
        {
            _logger.Information("Starting backup process. Source: {Source}, Destination: {Destination}", 
                sourceDirectory, destination);

            // Step 1: Create zip archive
            _logger.Information("Step 1: Creating zip archive...");
            var zipPath = await _zipService.CreateZipAsync(sourceDirectory);

            // Step 2: Upload to S3 if requested
            if (backupToS3)
            {
                _logger.Information("Step 2a: Uploading to S3 bucket: {Bucket}", destination);
                await _s3Service.UploadToS3Async(zipPath, destination, "backups");
            }

            // Step 3: Upload to Glacier if requested
            if (backupToGlacier)
            {
                _logger.Information("Step 2b: Uploading to Glacier vault: {Vault}", destination);
                await _glacierService.UploadToGlacierAsync(zipPath, destination);
            }

            // Cleanup
            _logger.Information("Cleaning up temporary files...");
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
                _logger.Information("Temporary zip file deleted");
            }

            _logger.Information("Backup process completed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Backup process failed");
            throw;
        }
    }
}
