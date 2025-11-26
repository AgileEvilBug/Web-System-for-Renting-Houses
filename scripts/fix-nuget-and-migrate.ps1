<#
Run this script from an Administrator PowerShell to fix NuGet package permission issues,
restore packages, build the project, and (optionally) apply EF migrations to MySQL.

Usage (run as Administrator):
  1. Right-click PowerShell -> Run as Administrator
  2. cd to the project folder (where .csproj lives):
     cd 'C:\Users\User\Documents\ASP.Net\Term Project\Web System for Renting Houses\RentifyApi\RentifyApi'
  3. Run:
     .\scripts\fix-nuget-and-migrate.ps1

The script:
  - verifies admin privileges
  - takes ownership of %USERPROFILE%\.nuget\packages and grants the current user full control
  - deletes the packages folder to force a fresh restore
  - clears NuGet caches, restores, and builds
  - sets EF_DESIGN_USE_MYSQL=true and attempts `dotnet ef database update`
  - runs the app (optional, prompts before running)

Warning: deleting the NuGet packages folder will force redownload of packages.
#>

function Write-Info { param($m) Write-Host "[INFO] $m" -ForegroundColor Cyan }
function Write-ErrorAndExit { param($m) Write-Host "[ERROR] $m" -ForegroundColor Red; exit 1 }

# Check admin
$IsAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $IsAdmin) {
    Write-Host "This script must be run as Administrator. Right-click PowerShell and choose 'Run as Administrator'." -ForegroundColor Yellow
    exit 1
}

$nugetPackages = Join-Path $env:USERPROFILE ".nuget\packages"
Write-Info "NuGet packages folder: $nugetPackages"

if (Test-Path $nugetPackages) {
    Write-Info "Taking ownership of the packages folder (this may take a moment)..."
    cmd /c "takeown /F `"$nugetPackages`" /R /D Y" | Out-Null

    Write-Info "Granting current user full control via icacls..."
    try {
        icacls "$nugetPackages" /grant "$($env:USERNAME):F" /T | Out-Null
    } catch {
        Write-Info "icacls failed in PowerShell; attempting cmd form..."
        cmd /c "icacls `"$nugetPackages`" /grant %USERNAME%:F /T" | Out-Null
    }

    Write-Host "About to delete the packages folder to force a fresh restore. This will redownload packages." -ForegroundColor Yellow
    $confirm = Read-Host "Type YES to delete and continue"
    if ($confirm -ne 'YES') {
        Write-Host "Aborting per user input." -ForegroundColor Yellow
        exit 1
    }

    Write-Info "Removing packages folder..."
    try {
        Remove-Item -Recurse -Force $nugetPackages -ErrorAction Stop
    } catch {
        Write-ErrorAndExit "Failed to delete packages folder: $_"
    }
} else {
    Write-Info "Packages folder does not exist; skipping ownership and delete steps."
}

Write-Info "Clearing NuGet local caches..."
dotnet nuget locals all --clear

Write-Info "Restoring packages..."
$restore = dotnet restore
if ($LASTEXITCODE -ne 0) { Write-Host $restore; Write-ErrorAndExit "dotnet restore failed." }

Write-Info "Building project..."
$build = dotnet build -c Debug
if ($LASTEXITCODE -ne 0) { Write-Host $build; Write-ErrorAndExit "dotnet build failed." }

Write-Info "Attempting EF database update against MySQL (design-time factory will use MySQL when EF_DESIGN_USE_MYSQL=true)."
$Env:EF_DESIGN_USE_MYSQL = 'true'
$ef = dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host $ef
    Write-Host "EF database update failed. If this is due to MySQL credentials or server not running, fix MySQL and re-run the migration commands manually." -ForegroundColor Yellow
} else {
    Write-Info "EF database update succeeded against MySQL."
}

$runNow = Read-Host "Run the app now? (y/N)"
if ($runNow -match '^(y|Y)') {
    Write-Info "Starting dotnet run..."
    dotnet run
} else {
    Write-Info "Finished. You can run the app later with: dotnet run"
}
