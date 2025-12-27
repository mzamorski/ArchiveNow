using System.Threading;

using ArchiveNow.Providers.Core;

using RoboSharp;
using RoboSharp.Interfaces;


namespace ArchiveNow.Providers.RoboCopy
{
    public class RoboCopyArchiveProvider : ArchiveProviderBase
    {
        private readonly RoboCommand _backup;
        private readonly ManualResetEventSlim _finishedEvent = new ManualResetEventSlim(false);


        public RoboCopyArchiveProvider(IArchiveFilePathBuilder pathBuilder)
            : base(pathBuilder)
        {
            SimulateLatency = true;

            _backup = new RoboCommand();

            _backup.OnCommandError += OnCommandError;
            _backup.OnError += OnError;
            _backup.OnFileProcessed += OnFileProcessed;
            _backup.OnCommandCompleted += OnCommandCompleted;
            _backup.OnCopyProgressChanged += OnCopyProgressChanged;

            _backup.CopyOptions.CopySubdirectoriesIncludingEmpty = true;
            _backup.CopyOptions.MultiThreadedCopiesCount = 32;
            _backup.RetryOptions.RetryCount = 0;
            _backup.RetryOptions.RetryWaitTime = 0;
            _backup.LoggingOptions.NoProgress = true;
            _backup.LoggingOptions.NoFileList = true;
            _backup.LoggingOptions.NoDirectoryList = true;
            _backup.CopyOptions.UseUnbufferedIo = false;

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
            CurrentProgressMode = ProgressMode.Determinate;

            _backup.CopyOptions.Source = sourcePath;
        }

        public override void CommitUpdate()
        {
            CurrentProgressMode = ProgressMode.Indeterminate;

            _backup.Start();
            _finishedEvent.Wait();
        }

        public override void AbortUpdate()
        {
            _backup.Stop();

            base.AbortUpdate();
        }
    }
}
