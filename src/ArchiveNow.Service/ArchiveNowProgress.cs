using System;

namespace ArchiveNow.Service
{
    public class ArchiveNowProgress : Progress<ArchiveNowProgressReport>, IArchiveNowProgress
    {
        public ArchiveNowProgress(Action<ArchiveNowProgressReport> reportAction)
            : base(reportAction)
        { }

        public void Report(ArchiveNowProgressReport value)
        {
            base.OnReport(value);
        }
    }

    public interface IArchiveNowProgress : IProgress<ArchiveNowProgressReport>
    {
        
    }
}