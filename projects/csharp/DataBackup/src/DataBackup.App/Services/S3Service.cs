using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DataBackup.Services;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger _logger;

    public S3Service(IConfiguration configuration, ILogger logger)
    {
        _logger = logger;

        // AWS SDK will automatically pick up credentials from:
        // 1. Environment variables (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY)
        // 2. AWS credentials file (~/.aws/credentials)
        // 3. IAM role (if running on EC2)
        _s3Client = new AmazonS3Client();
    }

    public async Task UploadToS3Async(string filePath, string bucketName, string? keyPrefix = null)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        try
        {
            var fileName = Path.GetFileName(filePath);
            var key = keyPrefix != null ? $"{keyPrefix}/{fileName}" : fileName;

            _logger.Information("Uploading file to S3 bucket: {Bucket}, Key: {Key}", bucketName, key);

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                FilePath = filePath,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };

            var response = await _s3Client.PutObjectAsync(putRequest);

            _logger.Information("Successfully uploaded to S3. ETag: {ETag}", response.ETag);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.Error(ex, "AWS S3 error: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to upload to S3");
            throw;
        }
    }
}
