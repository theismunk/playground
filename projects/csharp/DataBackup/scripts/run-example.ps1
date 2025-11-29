#!/usr/bin/env pwsh
<#
    .SYNOPSIS
    Example usage script for the Data Backup Service
    
    .DESCRIPTION
    This script demonstrates how to use the data-backup application
#>

# Colors for output
$green = "Green"
$yellow = "Yellow"
$cyan = "Cyan"

Write-Host "Data Backup Service - Usage Examples" -ForegroundColor $cyan
Write-Host ""

# Check if dotnet is installed
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: dotnet CLI is not installed" -ForegroundColor Red
    exit 1
}

# Check if project can be built
Write-Host "Building project..." -ForegroundColor $yellow
dotnet build -q

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Build successful" -ForegroundColor $green
Write-Host ""

# Show examples
Write-Host "Usage Examples:" -ForegroundColor $cyan
Write-Host ""

Write-Host "1. Backup to S3 only:" -ForegroundColor $yellow
Write-Host "   dotnet run -- `"C:\path\to\folder`" `"my-bucket`" --s3" -ForegroundColor $green
Write-Host ""

Write-Host "2. Backup to Glacier only:" -ForegroundColor $yellow
Write-Host "   dotnet run -- `"C:\path\to\folder`" `"my-vault`" --glacier" -ForegroundColor $green
Write-Host ""

Write-Host "3. Backup to both S3 and Glacier:" -ForegroundColor $yellow
Write-Host "   dotnet run -- `"C:\path\to\folder`" `"my-destination`" --both" -ForegroundColor $green
Write-Host "   or simply:" -ForegroundColor $green
Write-Host "   dotnet run -- `"C:\path\to\folder`" `"my-destination`"" -ForegroundColor $green
Write-Host ""

Write-Host "Before running, make sure to:" -ForegroundColor $cyan
Write-Host "  1. Set up AWS credentials using: .\setup-aws.ps1 -AccessKeyId <key> -SecretAccessKey <secret>" -ForegroundColor $yellow
Write-Host "  2. Create an S3 bucket or Glacier vault in your AWS account" -ForegroundColor $yellow
Write-Host "  3. Have the appropriate IAM permissions" -ForegroundColor $yellow
