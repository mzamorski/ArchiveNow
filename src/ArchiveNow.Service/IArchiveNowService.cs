using System;
using System.Threading;

using ArchiveNow.Actions.Core;
using ArchiveNow.Configuration;
using ArchiveNow.Configuration.Profiles;
using ArchiveNow.Utils.Threading;

namespace ArchiveNow.Service
{
    public interface IArchiveNowService
    {
        IArchiveNowConfiguration Configuration { get; }
        bool IsUsingProfile { get; }
        IArchiveNowProfile Profile { get; }

        event EventHandler<AfterFinishedActionEventArgs> ActionExecuted;
        event EventHandler<AfterFinishedActionEventArgs> ActionExecuting;
        event EventHandler<string> Commit;
        event EventHandler<string> DirectoryAdded;
        event EventHandler<string> DirectoryAdding;
        event EventHandler<string> FileAdded;
        event EventHandler<string> FileAdding;
        event EventHandler<IArchiveResult> Finished;
        event EventHandler<int> Initialized;

        void Archive(string sourcePath, IArchiveNowProgress progressIndicator, CancellationToken cancelToken, PauseToken pauseToken);
    }
}