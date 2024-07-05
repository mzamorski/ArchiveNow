using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchiveNow.Configuration.Profiles
{
    public sealed class NullArchiveNowProfileRepository : IArchiveNowProfileRepository
    {
        public static IArchiveNowProfileRepository Instance { get; } =
            new Lazy<IArchiveNowProfileRepository>(() => new NullArchiveNowProfileRepository()).Value;

        private NullArchiveNowProfileRepository() { }

        public IArchiveNowProfile Find(string name, IArchiveNowProfile defaultProfile = null) => NullArchiveNowProfile.Instance;

        public IEnumerable<IArchiveNowProfile> LoadAll() => Enumerable.Empty<IArchiveNowProfile>();

        public IArchiveNowProfile Open(string filePath) => NullArchiveNowProfile.Instance;

        public void Save(IArchiveNowProfile profile)
        { }
    }
}