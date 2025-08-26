$exe = "C:\Program Files (x86)\ArchiveNow\ArchiveNow.RemoteUpload.Server.exe"

New-Service `
  -Name "ArchiveNowServer" `
  -BinaryPathName "`"$exe`"" `
  -DisplayName "ArchiveNow Remote Upload Server" `
  -Description "Remote file upload service" `
  -StartupType Automatic `
