using System.Collections.Generic;

namespace ArchiveNow.Profiles
{
    public interface IArchiveNowProfile
    {
        string Name { get; }

        ISet<string> IgnoredDirectories { get; }

        ISet<string> IgnoredFiles { get; }

        IFileNameBuilder FileNameBuilder { get; }

        //bool IsEmpty { get; }

        //bool IsEmpty
        //{
        //    get
        //    {
        //        return ReferenceEquals(EmptyArchiveNowProfile.Instance, this);
        //    }
        //}
    }
}