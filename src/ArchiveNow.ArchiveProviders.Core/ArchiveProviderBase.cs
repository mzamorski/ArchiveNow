
using System;
using System.Threading;

using ArchiveNow.Providers.Core.PasswordProviders;
using ArchiveNow.Utils;
using ArchiveNow.Utils.IO;

namespace ArchiveNow.Providers.Core
{
    public abstract class ArchiveProviderBase : IArchiveProvider
    {
        public virtual bool IsBatchOnly { get; protected set; } = false;

        public abstract string FileExtension { get; }

        public IPasswordProvider PasswordProvider { get; }

        public string Password => PasswordProvider.Password.ConvertToString();

        public string ArchiveFilePath { get; }

        public bool SimulateLatency { get; set; } = false;

        protected CancellationToken CancellationToken { get; set; }

        protected enum ProgressMode
        {
            Determinate,
            Indeterminate
        }

        private ProgressMode _currentProgressMode;

        protected ProgressMode CurrentProgressMode
        {
            get => _currentProgressMode;
            set
            {
                if (_currentProgressMode != value)
                {
                    _currentProgressMode = value;
                    OnIsProgressIndeterminateChanged(IsProgressIndeterminate);
                }
            }
        }

        public bool IsProgressIndeterminate => CurrentProgressMode == ProgressMode.Indeterminate;

        public event EventHandler FileCompressionStarted;
        public event EventHandler FileCompressed;
        public event EventHandler Finished;
        public event EventHandler<int> Starting;
        public event EventHandler<bool> IsProgressIndeterminateChanged;

        protected ArchiveProviderBase(
            IArchiveFilePathBuilder archiveFilePathBuilder,
            IPasswordProvider passwordProvider)
        {
            // Path builder is also responsible for validating the path.
            ArchiveFilePath = archiveFilePathBuilder.Build(FileExtension);
            PasswordProvider = passwordProvider ?? NullPasswordProvider.Instance;
        }

        protected ArchiveProviderBase(IArchiveFilePathBuilder archiveFilePathBuilder)
            : this(archiveFilePathBuilder, NullPasswordProvider.Instance)
        { }

        public abstract void AddDirectory(string path);

        public abstract void Add(string path);

        public abstract void BeginUpdate(string sourcePath);

        public abstract void CommitUpdate(CancellationToken cancellationToken = default);

        public virtual void AbortUpdate()
        {
            FileSystemExtensions.DeletePath(ArchiveFilePath);
        }

        protected void ApplySimulateLatency(int milliseconds = 100)
        {
            if (SimulateLatency)
            {
                Thread.Sleep(milliseconds);
            }
        }

        protected virtual void OnFileCompressed()
        {
            FileCompressed?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFileCompressionStarted()
        {
            FileCompressionStarted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFinished()
        {
            Finished?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStarting(int fileCount)
        {
            Starting?.Invoke(this, fileCount);
        }

        protected virtual void OnIsProgressIndeterminateChanged(bool newValue)
        {
            IsProgressIndeterminateChanged?.Invoke(this, newValue);
        }

        public virtual void Dispose()
        { }
    }
}
