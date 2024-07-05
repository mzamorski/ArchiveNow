using System.Collections.Generic;

namespace ArchiveNow.Profiles
{
    public class CompositeArchiveNowProfile : IArchiveNowProfile
    {
        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public ISet<string> IgnoredDirectories
        {
            get { throw new System.NotImplementedException(); }
        }

        public ISet<string> IgnoredFiles
        {
            get { throw new System.NotImplementedException(); }
        }

        public IFileNameBuilder FileNameBuilder
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}