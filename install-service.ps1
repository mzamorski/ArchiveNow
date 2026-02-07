$exe = "C:\Program Files (x86)\ArchiveNow\Server\ArchiveNow.RemoteUpload.Server.exe"
$config = "C:\Program Files (x86)\ArchiveNow\ArchiveNow.conf"

setx ARCHIVENOW_CONFIG "`"$config`"" /M

if (-not [System.Diagnostics.EventLog]::SourceExists("ArchiveNow.Server")) {
    New-EventLog -LogName Application -Source "ArchiveNow.Server"
}

New-Service `
  -Name "ArchiveNowServer" `
  -BinaryPathName "`"$exe`"" `
  -DisplayName "ArchiveNow Remote Upload Server" `
  -Description "Remote file upload service" `
  -StartupType Automatic `

Start-Service -Name "ArchiveNowServer"