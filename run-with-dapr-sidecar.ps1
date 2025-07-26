# Start Dapr Bank Service with Sidecar (PowerShell version)
# This script demonstrates how to run the BankService with Dapr sidecar manually

Write-Host "Starting Dapr Bank Service with Sidecar..." -ForegroundColor Green

# Check if Dapr CLI is installed
if (!(Get-Command dapr -ErrorAction SilentlyContinue)) {
    Write-Host "Error: Dapr CLI is not installed. Please install Dapr CLI first." -ForegroundColor Red
    Write-Host "Visit: https://docs.dapr.io/getting-started/install-dapr-cli/" -ForegroundColor Yellow
    exit 1
}

# Check if Redis is running
Write-Host "Checking Redis connection..." -ForegroundColor Cyan
try {
    $redisCheck = Invoke-Expression "redis-cli ping" -ErrorAction SilentlyContinue
    if ($redisCheck -ne "PONG") {
        Write-Host "Warning: Redis is not responding." -ForegroundColor Yellow
        Write-Host "Please start Redis with: docker run -d -p 6379:6379 redis:latest" -ForegroundColor Yellow
        Write-Host "Continuing anyway..." -ForegroundColor Yellow
    }
} catch {
    Write-Host "Warning: Could not check Redis status." -ForegroundColor Yellow
    Write-Host "Please ensure Redis is running: docker run -d -p 6379:6379 redis:latest" -ForegroundColor Yellow
}

# Navigate to BankService directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$scriptPath\src\BankService"

# Build the BankService
Write-Host "Building BankService..." -ForegroundColor Cyan
dotnet build

# Run with Dapr sidecar
Write-Host "Starting BankService with Dapr sidecar..." -ForegroundColor Green
Write-Host "Dashboard will be available at: http://localhost:8080" -ForegroundColor Yellow
Write-Host "BankService API will be available at: http://localhost:3500/v1.0/invoke/bankservice/method/" -ForegroundColor Yellow

dapr run `
    --app-id bankservice `
    --app-port 5000 `
    --dapr-http-port 3500 `
    --dapr-grpc-port 50001 `
    --components-path ../../components `
    --config ../../dapr-config.yaml `
    --log-level info `
    -- dotnet run --urls "http://localhost:5000"