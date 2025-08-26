# ArchiveNow

A configurable folder archiver supporting profiles. It allows the selection of the archiving algorithm provider, the destination (e.g., placing in the cloud like Google Drive), defining a blacklist of excluded files/folders, and additional actions (such as encryption, setting the output file name, etc.).

## Available Archive Providers

- SevenZip (Default)
- SharpZip
- SystemZip
- LZ4
- LiteDb
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

## Available File Name Builders

- Random - generates a random file name
- AddDateTime - appends the current date and time to the file name
- LeaveOriginal - keeps the original file name
- AddVersion - adds a version number stored in preferences
- Rename(<name>) - renames the file to the provided name
- Default - same as LeaveOriginal

