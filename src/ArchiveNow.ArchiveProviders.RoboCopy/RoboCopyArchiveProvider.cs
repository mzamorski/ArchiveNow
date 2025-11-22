

using ArchiveNow.Providers.Core;

using RoboSharp;
using RoboSharp.Interfaces;
using System.Threading;

namespace ArchiveNow.Providers.RoboCopy
{
    public class RoboCopyArchiveProvider : ArchiveProviderBase
    {
        private readonly RoboCommand _backup;
        private readonly ManualResetEventSlim _finishedEvent = new ManualResetEventSlim(false);


        public RoboCopyArchiveProvider(IArchiveFilePathBuilder pathBuilder)
            : base(pathBuilder)
        {
            _backup = new RoboCommand();

            _backup.OnCommandError += OnCommandError;
            _backup.OnError += OnError;
            _backup.OnFileProcessed += OnFileProcessed;
            _backup.OnCommandCompleted += OnCommandCompleted;
            _backup.OnCopyProgressChanged += OnCopyProgressChanged;

            _backup.CopyOptions.CopySubdirectories = true;
            _backup.CopyOptions.Destination = ArchiveFilePath;
        }

        private void OnError(IRoboCommand sender, ErrorEventArgs e)
        { }

        private void OnCommandError(IRoboCommand sender, CommandErrorEventArgs e)
        { }

        private void OnCopyProgressChanged(object sender, CopyProgressEventArgs copyProgressEventArgs)
        { }

        private void OnCommandCompleted(object sender, RoboCommandCompletedEventArgs roboCommandCompletedEventArgs)
        {
            _finishedEvent.Set();
        }

        private void OnFileProcessed(object sender, FileProcessedEventArgs fileProcessedEventArgs)
        {
            OnFileCompressed();
        }

        public override string FileExtension => null;

        public override void AddDirectory(string fullName)
        { }

        public override void Add(string fullName)
        { }

        public override void BeginUpdate(string sourcePath)
        {
            _backup.CopyOptions.Source = sourcePath;
        }

        public override void CommitUpdate()
        {
            _backup.Start();
            _finishedEvent.Wait();
        }

        public override void AbortUpdate()
        {
            _backup.Stop();
        }
    }
}
