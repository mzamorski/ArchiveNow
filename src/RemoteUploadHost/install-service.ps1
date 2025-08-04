param(
    [string]$ServiceName = "ArchiveNowRemoteUploadHost",
    [string]$ExecutablePath = (Join-Path $PSScriptRoot 'RemoteUploadHost.exe')
)

if (-not (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue)) {
    New-Service -Name $ServiceName -BinaryPathName "`"$ExecutablePath`"" -DisplayName "ArchiveNow Remote Upload Host" -StartupType Automatic | Out-Null
    Write-Host "Service $ServiceName installed."
} else {
    Write-Host "Service $ServiceName already exists."
}
