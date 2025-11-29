#!/usr/bin/env pwsh
<#
    .SYNOPSIS
    Setup AWS credentials for the Data Backup Service
    
    .DESCRIPTION
    This script helps configure AWS credentials for the backup service
    
    .EXAMPLE
    .\setup-aws.ps1 -AccessKeyId "YOUR_KEY" -SecretAccessKey "YOUR_SECRET"
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$AccessKeyId,
    
    [Parameter(Mandatory=$true)]
    [string]$SecretAccessKey,
    
    [Parameter(Mandatory=$false)]
    [string]$Region = "us-east-1"
)

Write-Host "Setting up AWS credentials for Data Backup Service..." -ForegroundColor Green

# Set environment variables
$env:AWS_ACCESS_KEY_ID = $AccessKeyId
$env:AWS_SECRET_ACCESS_KEY = $SecretAccessKey
$env:AWS_DEFAULT_REGION = $Region

# Make them persistent for the current user
[Environment]::SetEnvironmentVariable("AWS_ACCESS_KEY_ID", $AccessKeyId, "User")
[Environment]::SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", $SecretAccessKey, "User")
[Environment]::SetEnvironmentVariable("AWS_DEFAULT_REGION", $Region, "User")

Write-Host "✓ AWS credentials have been configured successfully" -ForegroundColor Green
Write-Host ""
Write-Host "Configuration Summary:" -ForegroundColor Cyan
Write-Host "  AWS Region: $Region"
Write-Host "  Access Key ID: $($AccessKeyId.Substring(0,4))***" -ForegroundColor Yellow
Write-Host ""
Write-Host "You can now run the backup service with AWS credentials." -ForegroundColor Green
