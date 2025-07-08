using System;

namespace ArchiveNow.Service
{
    public class ArchiveNowProgress : Progress<ArchiveNowProgressReport>, IArchiveNowProgress
    {
        public ArchiveNowProgress(Action<ArchiveNowProgressReport> reportAction)
            : base(reportAction)
        { }

        public bool IsIndeterminate { get; set; }

        public void Report(ArchiveNowProgressReport value)
        {
            value.IsIndeterminate = IsIndeterminate;

            base.OnReport(value);
        }
    }

    public interface IArchiveNowProgress : IProgress<ArchiveNowProgressReport>
    {
        bool IsIndeterminate { get; set; }
    }
}