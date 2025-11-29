# Quick Start Guide

## 5-Minute Setup

### Step 1: Prerequisites
- Install [.NET 8.0 or later](https://dotnet.microsoft.com/download)
- Have an [AWS Account](https://aws.amazon.com/)

### Step 2: Create AWS Resources

#### Create S3 Bucket (Optional, if backing up to S3)
```powershell
aws s3 mb s3://my-backup-bucket --region us-east-1
```

#### Create Glacier Vault (Optional, if backing up to Glacier)
```powershell
aws glacier create-vault --vault-name my-backup-vault --region us-east-1
```

### Step 3: Set Up AWS Credentials

**Option A: Using the setup script (Recommended)**
```powershell
cd "c:\Users\tmunk\Documents\git\playground\projects\data-backup"
.\setup-aws.ps1 -AccessKeyId "your-access-key" -SecretAccessKey "your-secret-key" -Region "us-east-1"
```

**Option B: Manual environment variables**
```powershell
$env:AWS_ACCESS_KEY_ID = "your-access-key"
$env:AWS_SECRET_ACCESS_KEY = "your-secret-key"
$env:AWS_DEFAULT_REGION = "us-east-1"
```

### Step 4: Build the Project
```powershell
cd "c:\Users\tmunk\Documents\git\playground\projects\data-backup"
dotnet build
```

### Step 5: Run Your First Backup

**To S3:**
```powershell
dotnet run -- "C:\MyDocuments" "my-backup-bucket"
```

**To Glacier:**
```powershell
dotnet run -- "C:\MyDocuments" "my-backup-vault" --glacier
```

**To Both:**
```powershell
dotnet run -- "C:\MyDocuments" "my-backup-bucket" --both
```

## Command Line Reference

```powershell
dotnet run -- <source-directory> <destination> [--s3 | --glacier | --both]
```

### Parameters
- `<source-directory>` - Path to folder to backup (e.g., `C:\Documents`)
- `<destination>` - S3 bucket name or Glacier vault name
- `[--s3]` - Backup to S3 only (optional)
- `[--glacier]` - Backup to Glacier only (optional)
- `[--both]` - Backup to both S3 and Glacier (optional, default)

### Examples

```powershell
# Backup Documents folder to S3
dotnet run -- "C:\Users\Username\Documents" "company-backups"

# Backup with Glacier only
dotnet run -- "D:\Data" "archive-vault" --glacier

# Backup to both destinations
dotnet run -- "E:\Projects" "my-backup-destination" --both
```

## What Happens During Backup

1. **Compression**: Directory is zipped with optimal compression
   - File name: `backup_YYYYMMDD_HHMMSS.zip`
   - Temporary location: System temp folder

2. **Upload to S3** (if selected):
   - Destination: `s3://bucket-name/backups/backup_YYYYMMDD_HHMMSS.zip`
   - Encryption: AES256 (server-side)

3. **Upload to Glacier** (if selected):
   - Destination: Vault with archive ID and description
   - Description: `Backup: backup_YYYYMMDD_HHMMSS.zip - YYYY-MM-DD HH:MM:SS`

4. **Cleanup**: Temporary zip file is deleted

## Monitoring Progress

The application logs all operations to the console:
```
[INF] Data Backup Service starting...
[INF] Starting backup process. Source: C:\Documents, Destination: my-bucket
[INF] Step 1: Creating zip archive...
[INF] Creating zip archive from directory: C:\Documents
[INF] Zip archive created successfully. Size: 52428800 bytes
[INF] Step 2a: Uploading to S3 bucket: my-bucket
[INF] Uploading file to S3 bucket: my-bucket, Key: backups/backup_20250129_143025.zip
[INF] Successfully uploaded to S3. ETag: "abc123def456"
[INF] Cleaning up temporary files...
[INF] Temporary zip file deleted
[INF] Backup process completed successfully
```

## Troubleshooting

### "AWS credentials not found"
```powershell
# Verify credentials are set
echo $env:AWS_ACCESS_KEY_ID
echo $env:AWS_SECRET_ACCESS_KEY
```

If empty, run the setup script again:
```powershell
.\setup-aws.ps1 -AccessKeyId "key" -SecretAccessKey "secret"
```

### "Bucket not found"
```powershell
# List your S3 buckets
aws s3 ls

# Create bucket if needed
aws s3 mb s3://my-backup-bucket
```

### "Access Denied" Error
- Verify IAM user has `s3:PutObject` and `glacier:UploadArchive` permissions
- Check AWS credentials are correct

### "Directory not found"
- Verify the source directory path exists
- Use absolute paths: `C:\Full\Path\To\Directory`

## Next Steps

1. **Schedule Backups**: Use Windows Task Scheduler to run backups automatically
2. **Monitor Costs**: S3 and Glacier have different pricing; S3 is cheaper for frequent access
3. **Test Recovery**: Periodically test restoring from backups
4. **Implement Retention**: Set S3/Glacier lifecycle policies to manage storage costs
5. **Enable Versioning**: Enable S3 versioning for data protection

## Resources

- 📖 [Full Documentation](README.md)
- ⚙️ [Configuration Guide](CONFIGURATION.md)
- 📁 [Project Structure](PROJECT_STRUCTURE.md)
- 🔧 [AWS S3 Docs](https://docs.aws.amazon.com/s3/)
- ❄️ [AWS Glacier Docs](https://docs.aws.amazon.com/glacier/)

## Support

For issues or questions:
1. Check the [README.md](README.md) for detailed information
2. Review [CONFIGURATION.md](CONFIGURATION.md) for setup issues
3. Examine logs from the console output
4. Verify AWS credentials and permissions
