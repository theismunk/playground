using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Serilog;

namespace DataBackup.Services;

public class ZipService : IZipService
{
    private readonly ILogger _logger;

    public ZipService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<string> CreateZipAsync(string sourceDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
        {
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");
        }

        _logger.Information("Creating zip archive from directory: {SourceDirectory}", sourceDirectory);

        string zipFileName = $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip";
        string zipPath = Path.Combine(Path.GetTempPath(), zipFileName);

        try
        {
            // Create zip file asynchronously
            await Task.Run(() =>
            {
                ZipFile.CreateFromDirectory(sourceDirectory, zipPath, CompressionLevel.Optimal, false);
            });

            var fileInfo = new FileInfo(zipPath);
            _logger.Information("Zip archive created successfully. Size: {Size} bytes", fileInfo.Length);

            return zipPath;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create zip archive");
            throw;
        }
    }
}
