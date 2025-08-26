# ArchiveNow

A configurable folder archiver supporting profiles. It allows the selection of the archiving algorithm provider, the destination (e.g., placing in the cloud like Google Drive), defining a blacklist of excluded files/folders, and additional actions (such as encryption, setting the output file name, etc.).

## Available Archive Providers

- SevenZip (Default)
- SharpZip
- SystemZip
- LiteDb
- LZ4
- Msi
- Listing
  
## Available After-Finished Actions

- SendToMailbox
- SendToArchiveNow
- SendToGoogleDrive
- SetClipboard
- MoveToDirectory
- Delete
- Encrypt

