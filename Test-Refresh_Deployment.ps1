# -----------------------------------------------------------------------------
# Script: Deploy-ArchiveNow.ps1
# Description: Deploys binaries to AppFolder, handles Server subfolder, 
#              restarts Explorer shell, and restores opened windows.
# -----------------------------------------------------------------------------

# 1. Save the current location to restore it later
$originalLocation = Get-Location

# Parameters
$projectBinFolder = "E:\__PROJECTS\MY\ArchiveNow\ArchiveNow\bin\Debug"
$serverBinFolder  = Join-Path $projectBinFolder "Server"

$appFolder        = "C:\Program Files (x86)\ArchiveNow"
$serverDestFolder = Join-Path $appFolder "Server"
$srmPath          = Join-Path $appFolder "srm.exe"

# Function to copy binaries from a source folder to a destination folder
function Copy-Binaries {
    param (
        [string]$SourcePath,
        [string]$DestinationPath
    )
    
    if (Test-Path $SourcePath) {
        # Ensure destination directory exists
        if (-not (Test-Path $DestinationPath)) {
            Write-Host "Creating directory: $DestinationPath" -ForegroundColor DarkCyan
            New-Item -ItemType Directory -Path $DestinationPath -Force | Out-Null
        }

        Write-Host "Copying binaries from: $SourcePath -> $DestinationPath" -ForegroundColor Cyan
        
        # Append "\*" to SourcePath so -Include works on the directory content
        $searchPath = Join-Path $SourcePath "*"
        
        # FIX: Added *.json, *.config (required for .NET apps) and *.pdb (for debugging)
        Get-ChildItem -Path $searchPath -Include *.dll, *.exe, *.json, *.config, *.pdb -File | 
            Copy-Item -Destination $DestinationPath -Force
    } else {
        Write-Warning "Source directory not found: $SourcePath"
    }
}

# 2. Capture currently opened Explorer windows
Write-Host "Capturing active Explorer windows..." -ForegroundColor Yellow
$explorerApp = New-Object -ComObject Shell.Application
$openedFolders = $explorerApp.Windows() | Where-Object { $_.Name -eq "Explorer" } | ForEach-Object { $_.LocationURL }

# 3. Stop Explorer and uninstall shell extension
if (Test-Path $appFolder) {
    # Change location to AppFolder to run local tools if needed
    Set-Location $appFolder
    
    # Run srm.exe to unregister the shell extension if present
    if (Test-Path $srmPath) {
        Write-Host "Uninstalling shell extension..." -ForegroundColor Magenta
        Start-Process $srmPath -ArgumentList "uninstall ArchiveNow.Shell.dll" -Wait
    }
    
    Write-Host "Stopping Explorer process..." -ForegroundColor Red
    Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
} else {
    Write-Error "The directory $appFolder does not exist."
    Set-Location $originalLocation
    exit
}

# 4. Cleanup existing files in AppFolder (preserve config and srm.exe)
Write-Host "Cleaning up old files in $appFolder..."
Get-ChildItem -Path $appFolder -File | Where-Object { 
    $_.Name -ne "srm.exe" -and $_.Name -ne "ArchiveNow.conf" 
} | Remove-Item -Force

# 5. Copy binaries
# Task A: Copy main Debug files to App Root
Copy-Binaries -SourcePath $projectBinFolder -DestinationPath $appFolder

# Task B: Copy Server files to App/Server subfolder
Copy-Binaries -SourcePath $serverBinFolder -DestinationPath $serverDestFolder

# 6. Reinstall shell extension
if (Test-Path $srmPath) {
    Write-Host "Installing shell extension..." -ForegroundColor Magenta
    Start-Process $srmPath -ArgumentList "install ArchiveNow.Shell.dll -codebase" -Wait
}

# 7. Restart Explorer shell
Write-Host "Restarting Explorer shell..." -ForegroundColor Green
Start-Process explorer.exe
Start-Sleep -Seconds 2

# 8. Restore previously opened folders
if ($openedFolders) {
    Write-Host "Restoring previously opened folders..." -ForegroundColor Yellow
    foreach ($url in $openedFolders) {
        $path = [uri]::UnescapeDataString($url).Replace("file:///", "").Replace("/", "\")
        if (Test-Path $path) {
            Start-Process explorer.exe -ArgumentList "`"$path`""
        }
    }
}

# 9. Return to the original directory
Set-Location $originalLocation
Write-Host "Returned to: $originalLocation"
Write-Host "Deployment completed successfully!" -ForegroundColor Green