# PowerShell script to set up the software database

Write-Host "Setting up PostgreSQL database for Software Center..." -ForegroundColor Cyan

# Navigate to docker-compose directory
$dockerPath = Join-Path $PSScriptRoot "SoftwareCenter.Api\local-dev-environment"

if (-not (Test-Path $dockerPath)) {
    Write-Host "Error: Cannot find docker-compose directory at $dockerPath" -ForegroundColor Red
    exit 1
}

Set-Location $dockerPath

# Check if Docker is running
try {
    docker ps | Out-Null
} catch {
    Write-Host "Error: Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

Write-Host "`n1. Stopping existing containers..." -ForegroundColor Yellow
docker-compose down

Write-Host "`n2. Removing old volumes (fresh start)..." -ForegroundColor Yellow
docker-compose down -v

Write-Host "`n3. Starting PostgreSQL with 'software' database..." -ForegroundColor Yellow
docker-compose up -d

Write-Host "`n4. Waiting for database to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Verify database was created
Write-Host "`n5. Verifying 'software' database exists..." -ForegroundColor Yellow
$databases = docker-compose exec -T db psql -U postgres -t -c "SELECT datname FROM pg_database WHERE datname='software';"

if ($databases -match "software") {
    Write-Host "`n? SUCCESS! Database 'software' is ready!" -ForegroundColor Green
    Write-Host "`nConnection Details:" -ForegroundColor Cyan
    Write-Host "  Host: localhost" -ForegroundColor White
    Write-Host "  Port: 5432" -ForegroundColor White
    Write-Host "  Database: software" -ForegroundColor White
    Write-Host "  Username: postgres" -ForegroundColor White
    Write-Host "  Password: password" -ForegroundColor White
    
    Write-Host "`nYou can now:" -ForegroundColor Cyan
    Write-Host "  - Run the API: dotnet run --project SoftwareCenter.Api" -ForegroundColor White
    Write-Host "  - Run tests: dotnet test" -ForegroundColor White
} else {
    Write-Host "`n??  Warning: Could not verify database creation" -ForegroundColor Yellow
    Write-Host "Try manually: docker-compose exec db psql -U postgres -c '\l'" -ForegroundColor White
}

Write-Host ""
