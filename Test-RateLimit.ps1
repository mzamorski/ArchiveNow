# Configuration
$Url = "http://localhost:5000"
$Iterations = 70
$Payload = "ArchiveNow Stress Test Data: Simulate file content upload."
$Secret = "__SECRET" # Secret key needed for the server

Write-Host "Starting POST stress test on $Url with Auth..." -ForegroundColor Cyan

# Define headers
$headers = @{
    "X-Access-Secret" = $Secret
}

for ($i = 1; $i -le $Iterations; $i++) {
    try {
        # Sending a POST request with payload and custom security header
        $response = Invoke-WebRequest -Uri $Url `
                                      -Method Post `
                                      -Body $Payload `
                                      -Headers $headers `
                                      -ContentType "text/plain" `
                                      -ErrorAction Stop
        
        Write-Host "[$i] Status: $($response.StatusCode) - OK" -ForegroundColor Green
    }
    catch {
        if ($_.Exception.Response -ne $null) {
            $statusCode = [int]$_.Exception.Response.StatusCode
            
            if ($statusCode -eq 429) {
                Write-Host "[$i] Status: $statusCode - BLOCKED (RATE LIMIT)" -ForegroundColor Red
            }
            elseif ($statusCode -eq 401) {
                Write-Host "[$i] Status: $statusCode - UNAUTHORIZED (Check Secret)" -ForegroundColor Magenta
            }
            else {
                Write-Host "[$i] Status: $statusCode - OTHER ERROR" -ForegroundColor Yellow
            }
        }
        else {
            Write-Host "[$i] Error: Connection failed" -ForegroundColor DarkRed
        }
    }
}

Write-Host "Test finished." -ForegroundColor Cyan