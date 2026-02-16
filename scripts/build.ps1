# CoreBanking API - Build & Test Script (PowerShell)
# Usage: .\scripts\build.ps1 [-SkipTests] [-Configuration Release|Debug]

param(
    [switch]$SkipTests,
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$Solution = "Corebankingapp.sln"

Write-Host "=== CoreBanking API Build ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration"
Write-Host ""

# Step 1: Restore
Write-Host "--- Restoring dependencies ---" -ForegroundColor Yellow
dotnet restore $Solution
if ($LASTEXITCODE -ne 0) { throw "Restore failed" }

# Step 2: Build
Write-Host "--- Building solution ---" -ForegroundColor Yellow
dotnet build $Solution --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { throw "Build failed" }

# Step 3: Test
if (-not $SkipTests) {
    Write-Host "--- Running tests ---" -ForegroundColor Yellow
    dotnet test $Solution `
        --configuration $Configuration `
        --no-build `
        --verbosity normal `
        --logger "trx;LogFileName=test-results.trx" `
        --collect:"XPlat Code Coverage"
    if ($LASTEXITCODE -ne 0) { throw "Tests failed" }
    Write-Host "--- Tests passed ---" -ForegroundColor Green
} else {
    Write-Host "--- Tests skipped ---" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Build completed successfully ===" -ForegroundColor Green
