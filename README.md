# ArchiveNow

A configurable folder archiver supporting profiles. It allows the selection of the archiving algorithm provider, the destination (e.g., placing in the cloud like Google Drive), defining a blacklist of excluded files/folders, and additional actions (such as encryption, setting the output file name, etc.).

## Available Archive Providers

- `SevenZip` (Default)
- `SharpZip`
- `SystemZip`
- `LZ4`
- `LiteDb`
- `Msi`
- `Listing`
  
## Available After-Finished Actions

- `SendToMailbox`
- `SendToArchiveNow`
- `SendToGoogleDrive`
- `SetClipboard`
- `MoveToDirectory`
- `Delete`
- `Encrypt`

## Available File Name Builders

- `Random` - Generates a random file name.
- `AddDateTime` - Appends the current date and time to the file name.
- `LeaveOriginal` - Keeps the original file name.
- `AddVersion` - Adds a version number stored in preferences.
- `Rename(<name>)` - Renames the output file name with the value provided in the parameter.
- `Default` - Equivalent to `LeaveOriginal`.

