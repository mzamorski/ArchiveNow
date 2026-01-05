using System.Collections.Generic;
using System.Text;
using ArchiveNow.Configuration.Profiles;

namespace ArchiveNow.Configuration
{
    public class ArchiveNowConfiguration : IArchiveNowConfiguration
    {
        private IArchiveNowProfile _defaultProfile;

        public IDictionary<string, IArchiveNowProfile> DirectoryProfileMap { get; } =
            new Dictionary<string, IArchiveNowProfile>();

        /// <summary>
        /// Default user profile.
        /// </summary>
        public IArchiveNowProfile DefaultProfile
        {
            get => _defaultProfile ?? (_defaultProfile = NullArchiveNowProfile.Instance);
            set => _defaultProfile = value;
        }

        public bool ShowSummary { get; set; }

        public bool CloseWindowOnSuccess { get; set; } = false;

        public bool HasDefaultProfile => !DefaultProfile.IsEmpty;

        public RemoteUploadServerConfiguration RemoteUploadServer { get; set; } = new RemoteUploadServerConfiguration();

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"ShowSummary: {ShowSummary}\nDefaultProfile: {DefaultProfile}");
            builder.AppendLine("Maps:");

            foreach (var item in DirectoryProfileMap)
            {
                builder.AppendLine($"[{item.Key}] -> {{{item.Value}}}");
            }

            builder.AppendLine($"RemoteUploadServer: {RemoteUploadServer}");

            return builder.ToString();
        }
    }
}