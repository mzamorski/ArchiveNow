using System;
using System.Threading;

using ArchiveNow.Actions.Core;
using ArchiveNow.Configuration;
using ArchiveNow.Configuration.Profiles;
using ArchiveNow.Core.Loggers;
using ArchiveNow.Utils.Threading;

namespace ArchiveNow.Service
{
    public class FakeArchiveNowService : IArchiveNowService
    {
        private readonly IArchiveNowLogger _logger;

        public IArchiveNowConfiguration Configuration { get; }
        public IArchiveNowProfile Profile { get; }
        public bool IsUsingProfile => !Profile.IsEmpty;

        public event EventHandler<AfterFinishedActionEventArgs> ActionExecuted;
        public event EventHandler<AfterFinishedActionEventArgs> ActionExecuting;
        public event EventHandler<string> Commit;
        public event EventHandler<string> DirectoryAdded;
        public event EventHandler<string> DirectoryAdding;
        public event EventHandler<string> FileAdded;
        public event EventHandler<string> FileAdding;
        public event EventHandler<IArchiveResult> Finished;
        public event EventHandler<int> Initialized;

        public FakeArchiveNowService(IArchiveNowConfiguration configuration, IArchiveNowProfile currentProfile, IArchiveNowLogger logger)
        {
            Configuration = configuration;
            Profile = currentProfile;

            _logger = logger;
        }

        public void Archive(string sourcePath, IArchiveNowProgress progressIndicator, CancellationToken cancelToken,
            PauseToken pauseToken)
        {
            Thread.Sleep(2000);
        }
    }
}