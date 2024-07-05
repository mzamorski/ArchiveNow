using System;
using System.Collections.Generic;

namespace ArchiveNow.Profiles
{
    public class EmptyArchiveNowProfile : IArchiveNowProfile
    {
        private static readonly Lazy<EmptyArchiveNowProfile> _instance =
            new Lazy<EmptyArchiveNowProfile>(() => new EmptyArchiveNowProfile());

        public static EmptyArchiveNowProfile Instance => _instance.Value;

        private EmptyArchiveNowProfile()
        { }

        public string Name => "Default (empty)";

        public ISet<string> IgnoredDirectories => new HashSet<string>();

        public ISet<string> IgnoredFiles => new HashSet<string>();

        public IFileNameBuilder FileNameBuilder => EmptyFileNameBuilder.Instance;
    }
}