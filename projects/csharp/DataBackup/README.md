# Data Backup Service

A C# console application for backing up files to AWS S3 and/or Glacier with automatic compression.

## Features

- **Zip Compression**: Automatically compresses directories into zip files
- **S3 Upload**: Upload backups to AWS S3 buckets
- **Glacier Upload**: Upload backups to AWS Glacier vaults
- **Dual Backup**: Support for simultaneous uploads to both S3 and Glacier
- **Logging**: Comprehensive logging using Serilog
- **Error Handling**: Robust error handling and recovery

## Prerequisites

- .NET 8.0 or later
- AWS Account with S3 and/or Glacier access
- AWS credentials configured (see AWS Configuration below)

## AWS Configuration

### Option 1: Environment Variables (Recommended for Local Development)
```powershell
$env:AWS_ACCESS_KEY_ID = "your-access-key"
$env:AWS_SECRET_ACCESS_KEY = "your-secret-key"
$env:AWS_DEFAULT_REGION = "us-east-1"
```

### Option 2: AWS Credentials File
Create `~/.aws/credentials` file:
```ini
[default]
aws_access_key_id = your-access-key
aws_secret_access_key = your-secret-key
```

And `~/.aws/config` file:
```ini
[default]
region = us-east-1
```

### Option 3: IAM Role (For EC2 Instances)
Attach an IAM role to your EC2 instance with S3 and Glacier permissions.

## Installation

1. Clone the repository
2. Navigate to the project directory
3. Restore NuGet packages:
```bash
dotnet restore
```

## Build

```bash
dotnet build
```

## Running the Application

### Basic Usage
```powershell
dotnet run -- <source-directory> <destination> [--s3 | --glacier | --both]
```

### Examples

**Backup to S3 only:**
```powershell
dotnet run -- "C:\MyDocuments" "my-backup-bucket" --s3
```

**Backup to Glacier only:**
```powershell
dotnet run -- "C:\MyDocuments" "my-vault-name" --glacier
```

**Backup to both S3 and Glacier (default):**
```powershell
dotnet run -- "C:\MyDocuments" "my-destination" --both
```

Or simply:
```powershell
dotnet run -- "C:\MyDocuments" "my-destination"
```

## Project Structure

```
src/
├── Program.cs                      # Application entry point
├── Services/
│   ├── IBackupServices.cs         # Service interfaces
│   ├── BackupService.cs           # Main orchestration service
│   ├── ZipService.cs              # Compression service
│   ├── S3Service.cs               # AWS S3 upload service
│   └── GlacierService.cs          # AWS Glacier upload service
├── Configuration/
│   └── AwsConfig.cs               # AWS configuration model
└── appsettings.json               # Application settings
```

## Configuration

Edit `appsettings.json` to adjust logging and AWS settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AwsConfig": {
    "AwsRegion": "us-east-1"
  }
}
```

## IAM Permissions Required

### For S3:
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:PutObject",
        "s3:GetObject"
      ],
      "Resource": "arn:aws:s3:::your-bucket-name/*"
    }
  ]
}
```

### For Glacier:
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "glacier:UploadArchive",
        "glacier:InitiateJob"
      ],
      "Resource": "arn:aws:glacier:region:account-id:vaults/vault-name"
    }
  ]
}
```

## Troubleshooting

### AWS Credentials Not Found
Ensure your AWS credentials are properly configured in one of the methods above and that environment variables are set correctly.

### S3 Bucket Not Found
Verify the bucket name is correct and that your AWS credentials have `s3:PutObject` permission.

### Glacier Vault Not Found
Verify the vault name is correct and that your AWS credentials have `glacier:UploadArchive` permission.

### Insufficient Permissions
Review your IAM policy and ensure it includes the required permissions for S3 and/or Glacier operations.

## Development

### Running in Debug Mode
```bash
dotnet run --configuration Debug -- "C:\test-folder" "test-bucket"
```

### Publishing
```bash
dotnet publish -c Release -o ./bin/Release/publish
```

## Notes

- Temporary zip files are automatically cleaned up after upload
- Large files may take considerable time to upload depending on network speed
- Consider adjusting compression level in `ZipService.cs` for different performance/size trade-offs

## License

This project is provided as-is for backup and archival purposes.