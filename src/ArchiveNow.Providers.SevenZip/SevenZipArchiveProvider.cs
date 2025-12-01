using System;
using System.Collections.Generic;
using System.Linq;

using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.PasswordProviders;
using ArchiveNow.Utils;

using SevenZip;

namespace ArchiveNow.Service.ArchiveProviders
{
    public class SevenZipArchiveProvider : ArchiveProviderBase
    {
        private readonly SevenZipCompressor _compressor;
        private readonly IList<string> _paths = new List<string>();

        public override string FileExtension => "7z";

        public SevenZipArchiveProvider(IArchiveFilePathBuilder archiveFilePathBuilder, IPasswordProvider passwordProvider)
            : base(archiveFilePathBuilder, passwordProvider)
        {
            _compressor = new SevenZipCompressor
            {
                CompressionMethod = CompressionMethod.Deflate,
                CompressionLevel = CompressionLevel.Fast,
                CompressionMode = CompressionMode.Create,
                DirectoryStructure = true,
                PreserveDirectoryRoot = false,
                ArchiveFormat = OutArchiveFormat.SevenZip,
                IncludeEmptyDirectories = true,
                EncryptHeaders = true
            };

            _compressor.FileCompressionStarted += OnFileCompressionStarted;
            _compressor.FileCompressionFinished += OnFileCompressionFinished;
            _compressor.Compressing += OnCompressing;
            _compressor.CompressionFinished += OnCompressionFinished;
            _compressor.FilesFound += OnFilesFound;
        }

        private void OnFilesFound(object sender, IntEventArgs args)
        {
            OnStarting(args.Value);
        }

        private void OnCompressionFinished(object sender, EventArgs args)
        {
            OnFinished();
        }

        private void OnCompressing(object sender, ProgressEventArgs args)
        { }

        private void OnFileCompressionFinished(object sender, EventArgs args)
        {
            OnFileCompressed();
        }

        private void OnFileCompressionStarted(object sender, FileNameEventArgs args)
        { }

        public override void AddDirectory(string path)
        {
            _paths.Add(path);
        }

        public override void Add(string path)
        {
            _paths.Add(path);
        }

        public override void BeginUpdate(string sourcePath)
        { }

        public override void CommitUpdate()
        {
            if (Password.HasValue())
            {
                _compressor.CompressFilesEncrypted(ArchiveFilePath, Password, _paths.ToArray());
            }
            else
            {
                _compressor.CompressFiles(ArchiveFilePath, _paths.ToArray());
            }
        }

        public override void AbortUpdate()
        { }
    }
}
