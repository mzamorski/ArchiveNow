# Configuration
$Url = "http://localhost:5000"
$Secret = "ANOW-SEC-2026-UPLOAD" # Secret key needed for the server
$TempFileName = "large_test_file.bin"
$FileSizeMB = 105 # Slightly above the 100MB limit

# 1. Generate a dummy large file (approx 105MB)
Write-Host "Generating test file: $FileSizeMB MB..." -ForegroundColor Cyan
$fileStream = [System.IO.File]::Create("$PSScriptRoot/$TempFileName")
$fileStream.SetLength($FileSizeMB * 1024 * 1024)
$fileStream.Close()

# 2. Define headers
$headers = @{
    "X-Access-Secret" = $Secret
}

Write-Host "Sending large file to $Url..." -ForegroundColor Cyan

try {
	# Sending a POST request with payload and custom security header
	$response = Invoke-WebRequest -Uri $Url `
								  -Method Post `
								  -InFile "$PSScriptRoot/$TempFileName" `
								  -Headers $headers `
								  -ContentType "application/octet-stream" `
								  -ErrorAction Stop
    
    Write-Host "Status: $($response.StatusCode) - OK (File accepted unexpectedy)" -ForegroundColor Yellow
}
catch {
    if ($_.Exception.Response -ne $null) {
        $statusCode = [int]$_.Exception.Response.StatusCode
        $statusDescription = $_.Exception.Response.StatusDescription
        
        if ($statusCode -eq 413) { # RequestEntityTooLarge
            Write-Host "Status: $statusCode ($statusDescription) - BLOCKED (File blocked as expected)" -ForegroundColor Yellow
        }
        else {
            Write-Host "Status: $statusCode - ERROR: $statusDescription" -ForegroundColor Red
        }
    }
    else {
        Write-Host "Error: Connection failed. Is the server running?" -ForegroundColor DarkRed
    }
}
finally {
    # 3. Cleanup
    if (Test-Path "$PSScriptRoot/$TempFileName") {
        Remove-Item "$PSScriptRoot/$TempFileName"
        Write-Host "Temporary file removed." -ForegroundColor Gray
    }
}