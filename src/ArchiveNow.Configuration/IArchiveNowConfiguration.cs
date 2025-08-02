using System.Collections.Generic;

using ArchiveNow.Configuration.Profiles;

namespace ArchiveNow.Configuration
{
    public interface IArchiveNowConfiguration
    {
        IArchiveNowProfile DefaultProfile { get; }

        IDictionary<string, IArchiveNowProfile> DirectoryProfileMap { get; }

        bool HasDefaultProfile { get; }

        bool ShowSummary { get; set; }

        int RemoteUploadPort { get; set; }
    }
}