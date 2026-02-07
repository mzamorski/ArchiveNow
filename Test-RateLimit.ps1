# Configuration
$Url = "http://localhost:5000"
$Iterations = 70
$Payload = "ArchiveNow Stress Test Data: Simulate file content upload."

Write-Host "Starting POST stress test on $Url..." -ForegroundColor Cyan

for ($i = 1; $i -le $Iterations; $i++) {
    try {
        # Sending a POST request with a text body
        # -Body sends the string as the input stream
        # -ContentType specifies we are sending raw text
        $response = Invoke-WebRequest -Uri $Url `
                                      -Method Post `
                                      -Body $Payload `
                                      -ContentType "text/plain" `
                                      -ErrorAction Stop
        
        Write-Host "[$i] Status: $($response.StatusCode) - OK" -ForegroundColor Green
    }
    catch {
        # Handle exceptions for non-200 status codes
        if ($_.Exception.Response -ne $null) {
            $statusCode = [int]$_.Exception.Response.StatusCode
            
            if ($statusCode -eq 429) {
                Write-Host "[$i] Status: $statusCode - BLOCKED (SUCCESS)" -ForegroundColor Red
            }
            else {
                # Other errors (e.g., 500 if server crashes processing the stream)
                Write-Host "[$i] Status: $statusCode - ERROR" -ForegroundColor Yellow
            }
        }
        else {
            Write-Host "[$i] Error: Connection failed (Server down?)" -ForegroundColor DarkRed
        }
    }
}

Write-Host "Test finished." -ForegroundColor Cyan