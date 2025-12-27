using System;
using System.Threading;

namespace ArchiveNow.Providers.Core
{
    public interface IArchiveProvider : IDisposable
    {
        bool IsBatchOnly { get; }

        string FileExtension { get; }

        string Password { get; }

        string ArchiveFilePath { get; }

        void AddDirectory(string path);

        void Add(string path);

        void BeginUpdate(string sourcePath);

        void CommitUpdate(CancellationToken cancellationToken = default);

        void AbortUpdate();

        event EventHandler FileCompressionStarted;
        event EventHandler FileCompressed;
        event EventHandler Finished;
        event EventHandler<int> Starting;
        event EventHandler<bool> IsProgressIndeterminateChanged;
    }

}
