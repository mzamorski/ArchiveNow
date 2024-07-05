using System;

namespace ArchiveNow.Providers.Core
{
    public interface IArchiveProvider : IDisposable
    {
        string FileExtension { get; }

        string Password { get; }

        string ArchiveFilePath { get; }

        void AddDirectory(string path);

        void Add(string path);

        void BeginUpdate();

        void CommitUpdate();

        void AbortUpdate();

        event EventHandler FileCompressionStarted;
        event EventHandler FileCompressed;
        event EventHandler Finished;
        event EventHandler<int> Starting;
    }

}
