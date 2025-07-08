using System.IO;

namespace ArchiveNow.Service
{
    public class ArchiveNowProgressReport
    {
        public ArchiveNowProgressReport(int totalCount)
        {
            TotalEntriesCount = totalCount;
            ProcessedEntriesCount = 0;
        }

        public bool IsIndeterminate { get; set; }

        public int TotalEntriesCount { get; }

        public int ProcessedEntriesCount { get; set; }

        public FileSystemInfo CurrentEntry { get; set; }

        public decimal PercentComplete => (ProcessedEntriesCount / TotalEntriesCount) * 100m;

        public void Clear()
        {
            ProcessedEntriesCount = 0;
        }

        public void Step()
        {
            ProcessedEntriesCount++;
        }

        public void Done()
        {
            ProcessedEntriesCount = TotalEntriesCount;
        }
    }
}