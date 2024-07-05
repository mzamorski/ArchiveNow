using System;

using ArchiveNow.Providers.Core.PasswordProviders;
using ArchiveNow.Utils;

namespace ArchiveNow.Providers.Core
{
    public abstract class ArchiveProviderBase : IArchiveProvider
    {
        public abstract string FileExtension { get; }

        public IPasswordProvider PasswordProvider { get; }

        public string Password => PasswordProvider.Password.ConvertToString();

        public string ArchiveFilePath { get; }

        public event EventHandler FileCompressionStarted;
        public event EventHandler FileCompressed;
        public event EventHandler Finished;
        public event EventHandler<int> Starting;

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

        public abstract void BeginUpdate();

        public abstract void CommitUpdate();

        public abstract void AbortUpdate();

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

        public virtual void Dispose()
        { }
    }
}
