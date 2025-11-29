# Project Structure and File Overview

## Directory Structure

```
data-backup/
├── .gitignore                 # Git ignore rules
├── CONFIGURATION.md           # Detailed configuration guide
├── README.md                  # Main documentation
├── appsettings.json          # Application settings
├── data-backup.csproj        # Project file with dependencies
├── data-backup.sln           # Solution file
├── run-example.ps1           # Example usage script
├── setup-aws.ps1             # AWS credential setup script
├── bin/                      # Compiled output
│   └── Debug/
│       └── net8.0/
└── src/                      # Source code
    ├── Program.cs            # Application entry point
    ├── Configuration/
    │   └── AwsConfig.cs      # AWS configuration model
    └── Services/
        ├── BackupService.cs          # Main orchestration service
        ├── GlacierService.cs         # AWS Glacier upload service
        ├── IBackupServices.cs        # Service interfaces
        ├── S3Service.cs              # AWS S3 upload service
        └── ZipService.cs             # File compression service
```

## Key Files

### Configuration Files
- **appsettings.json** - Application configuration (logging, AWS region)
- **data-backup.csproj** - NuGet package dependencies and project settings
- **CONFIGURATION.md** - Detailed setup and configuration guide

### Scripts
- **setup-aws.ps1** - Configures AWS credentials as environment variables
- **run-example.ps1** - Shows usage examples and builds the project

### Source Code

#### Program.cs
- Entry point for the application
- Configures dependency injection
- Sets up logging with Serilog
- Parses command-line arguments
- Orchestrates the backup workflow

#### Services/IBackupServices.cs
**Interfaces:**
- `IZipService` - Compresses directories
- `IS3Service` - Uploads to S3
- `IGlacierService` - Uploads to Glacier
- `IBackupService` - Orchestrates backup process

#### Services/BackupService.cs
- **Purpose**: Main orchestration service
- **Workflow**:
  1. Creates zip archive from source directory
  2. Uploads to S3 (if requested)
  3. Uploads to Glacier (if requested)
  4. Cleans up temporary files

#### Services/ZipService.cs
- **Purpose**: Handles file compression
- **Features**:
  - Async compression with `System.IO.Compression`
  - Optimal compression level
  - Timestamped filenames
  - Error logging

#### Services/S3Service.cs
- **Purpose**: AWS S3 operations
- **Features**:
  - Server-side encryption (AES256)
  - Automatic credential discovery
  - Organized backups under "backups/" prefix
  - Error handling for S3-specific exceptions

#### Services/GlacierService.cs
- **Purpose**: AWS Glacier operations
- **Features**:
  - Streaming upload for large files
  - Archive descriptions with metadata
  - Automatic credential discovery
  - Error handling for Glacier-specific exceptions

#### Configuration/AwsConfig.cs
- **Purpose**: AWS configuration model
- **Properties**:
  - `AwsAccessKeyId`
  - `AwsSecretAccessKey`
  - `AwsRegion`

## Dependencies

### NuGet Packages
- **AWSSDK.S3** (v3.7.300) - AWS S3 client library
- **AWSSDK.Glacier** (v3.7.300) - AWS Glacier client library
- **Microsoft.Extensions.Configuration** (v8.0.0) - Configuration framework
- **Microsoft.Extensions.Configuration.Json** (v8.0.0) - JSON configuration
- **Microsoft.Extensions.DependencyInjection** (v8.0.0) - Dependency injection
- **Serilog** (v3.1.1) - Logging framework
- **Serilog.Sinks.Console** (v5.0.0) - Console output for logs

## Build Information

- **Target Framework**: .NET 8.0
- **Language Version**: Latest
- **Nullable**: Enabled
- **Output Type**: Console Executable

## Workflow

```
1. User runs application with command line args
   ↓
2. Program.cs parses arguments
   ↓
3. DI container creates service instances
   ↓
4. BackupService orchestrates process
   ├─→ ZipService.CreateZipAsync()
   │   └─→ Creates backup_YYYYMMDD_HHMMSS.zip
   │
   ├─→ S3Service.UploadToS3Async() [if requested]
   │   └─→ Uploads to S3 bucket under backups/ prefix
   │
   ├─→ GlacierService.UploadToGlacierAsync() [if requested]
   │   └─→ Uploads to Glacier vault
   │
   └─→ Cleanup temporary files
       ↓
5. Application completes with success/error
```

## Usage Examples

### Quick Start
```powershell
# Set up AWS credentials
.\setup-aws.ps1 -AccessKeyId "YOUR_KEY" -SecretAccessKey "YOUR_SECRET"

# Backup to S3
dotnet run -- "C:\MyDocuments" "my-bucket"

# Backup to Glacier
dotnet run -- "C:\MyDocuments" "my-vault" --glacier

# Backup to both
dotnet run -- "C:\MyDocuments" "my-destination" --both
```

## Build & Run Commands

```powershell
# Restore dependencies
dotnet restore

# Build project
dotnet build

# Build Release version
dotnet build -c Release

# Run application
dotnet run -- "C:\source" "destination"

# Publish standalone executable
dotnet publish -c Release -o ./bin/Release/publish
```

## Error Handling

- **File Not Found**: Checks source directory and file existence before processing
- **AWS Exceptions**: Catches and logs S3-specific and Glacier-specific errors
- **General Exceptions**: Comprehensive try-catch with detailed error logging

## Logging

All operations are logged via Serilog with the following information:
- Backup start/completion
- File compression details (size, path)
- S3 upload ETag confirmation
- Glacier archive ID confirmation
- Detailed error messages with stack traces
