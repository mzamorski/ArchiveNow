using System.Collections.Generic;

namespace ArchiveNow.Configuration.Profiles
{
    public interface IArchiveNowProfileRepository
    {
        IEnumerable<IArchiveNowProfile> LoadAll();
        IArchiveNowProfile Find(string name, IArchiveNowProfile defaultProfile = null);
        IArchiveNowProfile Open(string filePath);
        void Save(IArchiveNowProfile profile);
    }
}