using System;
using System.IO;

namespace ArchiveNow.Actions.SendToGoogleDrive
{
    public class GoogleDriveContext
    {
        public string SecretKeyFilePath { get; set; }
        public string DestinationFolderId { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(SecretKeyFilePath))
                throw new ArgumentException("Secrets path cannot be empty.", nameof(SecretKeyFilePath));

            if (!File.Exists(SecretKeyFilePath))
                throw new FileNotFoundException($"Secrets file not found: {SecretKeyFilePath}");

            if (string.IsNullOrWhiteSpace(DestinationFolderId))
                throw new ArgumentException("Google Drive folder ID cannot be empty.", nameof(DestinationFolderId));
        }
    }
}
