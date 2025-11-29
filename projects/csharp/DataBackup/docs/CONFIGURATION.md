# Data Backup Configuration Guide

## Environment Variables

Set these before running the application:

```powershell
# AWS Credentials
$env:AWS_ACCESS_KEY_ID = "your-access-key-id"
$env:AWS_SECRET_ACCESS_KEY = "your-secret-access-key"
$env:AWS_DEFAULT_REGION = "us-east-1"
```

Or use the setup script:
```powershell
.\setup-aws.ps1 -AccessKeyId "your-key" -SecretAccessKey "your-secret" -Region "us-east-1"
```

## appsettings.json Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AwsConfig": {
    "AwsRegion": "us-east-1"
  }
}
```

### Logging Levels
- **Trace**: Very detailed diagnostic information
- **Debug**: Detailed diagnostic information
- **Information**: General informational messages (default)
- **Warning**: Warning messages for potential issues
- **Error**: Error messages
- **Fatal**: Critical failures

## AWS S3 Setup

### Create a bucket (AWS CLI):
```bash
aws s3 mb s3://my-backup-bucket --region us-east-1
```

### Enable versioning (optional but recommended):
```bash
aws s3api put-bucket-versioning \
  --bucket my-backup-bucket \
  --versioning-configuration Status=Enabled
```

### Create a backup folder structure:
Backups will be stored under `backups/` prefix automatically

### Set lifecycle policy (optional):
```json
{
  "Rules": [
    {
      "Id": "ArchiveOldBackups",
      "Status": "Enabled",
      "Transitions": [
        {
          "Days": 90,
          "StorageClass": "GLACIER"
        }
      ]
    }
  ]
}
```

## AWS Glacier Setup

### Create a vault (AWS CLI):
```bash
aws glacier create-vault --vault-name my-backup-vault --region us-east-1
```

### List vaults:
```bash
aws glacier list-vaults --region us-east-1
```

## IAM Policy Examples

### S3 Backup Policy
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "S3BackupAccess",
      "Effect": "Allow",
      "Action": [
        "s3:PutObject",
        "s3:PutObjectAcl",
        "s3:GetObject",
        "s3:ListBucket"
      ],
      "Resource": [
        "arn:aws:s3:::my-backup-bucket",
        "arn:aws:s3:::my-backup-bucket/*"
      ]
    }
  ]
}
```

### Glacier Backup Policy
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "GlacierBackupAccess",
      "Effect": "Allow",
      "Action": [
        "glacier:UploadArchive",
        "glacier:InitiateJob",
        "glacier:ListJobs",
        "glacier:GetJobOutput"
      ],
      "Resource": "arn:aws:glacier:us-east-1:123456789012:vaults/my-backup-vault"
    }
  ]
}
```

### Combined Policy (S3 + Glacier)
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "BackupAccess",
      "Effect": "Allow",
      "Action": [
        "s3:PutObject",
        "s3:GetObject",
        "s3:ListBucket",
        "glacier:UploadArchive",
        "glacier:InitiateJob"
      ],
      "Resource": [
        "arn:aws:s3:::my-backup-bucket",
        "arn:aws:s3:::my-backup-bucket/*",
        "arn:aws:glacier:us-east-1:123456789012:vaults/my-backup-vault"
      ]
    }
  ]
}
```

## Troubleshooting

### AWS Credentials Not Recognized

**Solution**: Ensure credentials are set as environment variables:
```powershell
$env:AWS_ACCESS_KEY_ID
$env:AWS_SECRET_ACCESS_KEY
```

Or check AWS credentials file at `~/.aws/credentials`

### S3 Access Denied

**Solution**: Verify IAM policy includes `s3:PutObject` permission

### Glacier Service Unavailable

**Solution**: 
- Verify vault exists in the specified region
- Check IAM policy includes `glacier:UploadArchive`

### Out of Memory with Large Files

**Solution**: 
- S3 and Glacier handle streaming uploads
- Increase system RAM or process files in batches

## Performance Tips

1. **Network**: Use an AWS region closest to your location
2. **Compression**: Zip compression level can be adjusted in `ZipService.cs`
3. **Parallelization**: Run multiple backups in separate processes for different folders
4. **Scheduling**: Use Windows Task Scheduler or cron for automated backups

## Backup Naming

Zip files are automatically named with timestamp:
```
backup_20250129_143025.zip
```

Format: `backup_YYYYMMDD_HHMMSS.zip`

## Additional Resources

- [AWS S3 Documentation](https://docs.aws.amazon.com/s3/)
- [AWS Glacier Documentation](https://docs.aws.amazon.com/glacier/)
- [AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/)
- [Serilog Documentation](https://github.com/serilog/serilog/wiki)
