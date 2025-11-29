using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Glacier;
using Amazon.Glacier.Model;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DataBackup.Services;

public class GlacierService : IGlacierService
{
    private readonly IAmazonGlacier _glacierClient;
    private readonly ILogger _logger;

    public GlacierService(IConfiguration configuration, ILogger logger)
    {
        _logger = logger;

        // AWS SDK will automatically pick up credentials from:
        // 1. Environment variables (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY)
        // 2. AWS credentials file (~/.aws/credentials)
        // 3. IAM role (if running on EC2)
        _glacierClient = new AmazonGlacierClient();
    }

    public async Task UploadToGlacierAsync(string filePath, string vaultName)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        try
        {
            var fileInfo = new FileInfo(filePath);
            var fileName = fileInfo.Name;

            _logger.Information("Uploading file to Glacier vault: {Vault}, File: {FileName}, Size: {Size} bytes",
                vaultName, fileName, fileInfo.Length);

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var uploadRequest = new UploadArchiveRequest
                {
                    VaultName = vaultName,
                    ArchiveDescription = $"Backup: {fileName} - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                    Body = fileStream
                };

                var response = await _glacierClient.UploadArchiveAsync(uploadRequest);

                _logger.Information("Successfully uploaded to Glacier. Archive ID: {ArchiveId}", response.ArchiveId);
            }
        }
        catch (AmazonGlacierException ex)
        {
            _logger.Error(ex, "AWS Glacier error: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to upload to Glacier");
            throw;
        }
    }
}
