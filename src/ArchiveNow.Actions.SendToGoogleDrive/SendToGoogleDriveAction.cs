using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ArchiveNow.Actions.Core;
using ArchiveNow.Actions.Core.Result;
using ArchiveNow.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;


namespace ArchiveNow.Actions.SendToGoogleDrive
{
    public class SendToGoogleDriveAction : AfterFinishedActionBase
    {
        private const string ContentMimeType = "application/unknown";

        private readonly GoogleDriveContext _context;

        public override string Description => "Uploading to the Google Drive...";

        private long ContentLength { get; set; }

        public SendToGoogleDriveAction(GoogleDriveContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.Validate();
        }

        private UserCredential CreateCredential()
        {
            UserCredential credential;

            var credentialFilePath = _context.SecretKeyFilePath;

            using (var stream = new FileStream(credentialFilePath, FileMode.Open, FileAccess.Read))
            {
                string personalFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string credentialPath = Path.Combine(personalFolderPath, "ArchiveNow.GoogleDrive.Credentials");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credentialPath, true)).Result;
            }

            return credential;
        }

        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
            var credential = CreateCredential();

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "ArchiveNow",
            });

            string folderId;
            if (_context.DestinationFolderId.IsNotEmpty())
            {
                folderId = _context.DestinationFolderId;
            }
            else
            {
                throw new NotImplementedException("Directory creation not supported!");
            }

            var fileMetadata = new File
            {
                Name = Path.GetFileName(context.InputPath),
                MimeType = ContentMimeType,
                Parents = new List<string> { folderId }
            };

            IUploadProgress progress;
            using (var stream = new FileStream(context.InputPath, FileMode.Open, FileAccess.Read))
            {
                ContentLength = stream.Length;

                var request = service.Files.Create(fileMetadata, stream, ContentMimeType);
                request.ChunkSize = ResumableUpload.MinimumChunkSize;
                request.ProgressChanged += RequestOnProgressChanged;
                request.Fields = "id";

                progress = request.Upload();
            }

            return new AfterFinishedActionResult(progress.Status == UploadStatus.Completed, context.InputPath,
                progress.Exception?.Message);
        }

        private bool FolderExists(DriveService service, string folderId)
        {
            try
            {
                var files = service.Files.List().Execute();
                foreach (var file in files.Files.Where(f => f.Kind == "file#drive"))
                {

                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void RequestOnProgressChanged(IUploadProgress uploadProgress)
        {
            base.Progress?.Report(new AfterFinishedActionProgress()
                {
                    IsCompleted = (uploadProgress.Status == UploadStatus.Completed),
                    IsFailed = (uploadProgress.Status == UploadStatus.Failed),
                    Percentage = uploadProgress.BytesSent.AsPercentOf(ContentLength)
                }
            );
        }

        private File CreateFolder(DriveService service, string folderPath)
        {
            //var folders = new Uri(folderPath, UriKind.Relative).Segments;
            var folders = folderPath.Split('/');

            File parentFolder = null;

            foreach (var folder in folders)
            {
                parentFolder = CreateFolderInternal(service, folder, parentFolder);
            }



            return parentFolder;
        }

        private static File CreateFolderInternal(DriveService service, string folderName, File parentFolder = null)
        {
            var fileMetadata = new File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
            };

            if (parentFolder != null)
            {
                fileMetadata.Parents = new List<string> { parentFolder.Id };
            }

            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";

            return request.Execute();
        }
    }
}
