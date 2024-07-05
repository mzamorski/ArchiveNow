using System.Collections.Generic;

namespace ArchiveNow
{
    public class ArchiveNowArguments
    {
        public static readonly ArchiveNowArguments Empty = new ArchiveNowArguments();

        private string _profileName;
        private string _profileFilePath;

        public ArchiveNowArguments()
        {
            Mode = ArchiveNowMode.Unknown;
            Paths = new List<string>(0);
        }

        public ArchiveNowMode Mode { get; set; }

        public ICollection<string> Paths { get; set; }

        public bool IsEmpty => ReferenceEquals(this, Empty);

        public string ProfileName
        {
            get
            {
                return _profileName;
            }

            set
            {
                _profileName = value;
                if (!string.IsNullOrWhiteSpace(_profileName))
                {
                    UseProfile = true;
                }
            }
        }

        public string ProfileFilePath
        {
            get
            {
                return _profileFilePath;
            }

            set
            {
                _profileFilePath = value;
                if (!string.IsNullOrWhiteSpace(_profileFilePath))
                {
                    UseProfile = true;
                }
            }
        }

        public bool UseProfile { get; set; }
    }
}