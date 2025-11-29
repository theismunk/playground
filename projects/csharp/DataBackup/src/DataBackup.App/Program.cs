using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using DataBackup.Services;
using DataBackup.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Data Backup Service starting...");

    var services = new ServiceCollection();
    services.AddSingleton(configuration);
    services.AddSingleton(Log.Logger);
    services.AddScoped<IBackupService, BackupService>();
    services.AddScoped<IZipService, ZipService>();
    services.AddScoped<IS3Service, S3Service>();
    services.AddScoped<IGlacierService, GlacierService>();

    var serviceProvider = services.BuildServiceProvider();
    var backupService = serviceProvider.GetRequiredService<IBackupService>();

    // Example usage
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: data-backup <source-directory> <destination> [--s3 | --glacier | --both]");
        Console.WriteLine("  destination: S3 bucket name or Glacier vault name");
        Console.WriteLine("  --s3: Backup to S3 only");
        Console.WriteLine("  --glacier: Backup to Glacier only");
        Console.WriteLine("  --both: Backup to both S3 and Glacier (default)");
        return;
    }

    string sourceDirectory = args[0];
    string destination = args[1];
    string target = args.Length > 2 ? args[2] : "--both";

    bool backupToS3 = target == "--s3" || target == "--both";
    bool backupToGlacier = target == "--glacier" || target == "--both";

    if (!Directory.Exists(sourceDirectory))
    {
        Log.Error("Source directory does not exist: {SourceDirectory}", sourceDirectory);
        return;
    }

    await backupService.BackupAsync(sourceDirectory, destination, backupToS3, backupToGlacier);
    Log.Information("Backup completed successfully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Backup operation failed");
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}
