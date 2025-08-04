# ArchiveNow

A configurable folder archiver supporting profiles. It allows the selection of the archiving algorithm provider, the destination (e.g., placing in the cloud like Google Drive), defining a blacklist of excluded files/folders, and additional actions (such as encryption, setting the output file name, etc.).

## RemoteUploadHost service

The `RemoteUploadHost` component can be installed as a Windows service to run in the background and handle remote uploads.

### Installation

Run the following PowerShell script to register the service and configure it to start automatically with Windows:

```powershell
./src/RemoteUploadHost/install-service.ps1
```

### Manual control

You can manually manage the service using the `sc` utility:

```cmd
sc start ArchiveNowRemoteUploadHost
sc stop ArchiveNowRemoteUploadHost
```

These commands allow you to start or stop the service without rebooting the system.

