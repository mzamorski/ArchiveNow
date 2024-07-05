using System.Collections.Generic;
using System.IO;

namespace ArchiveNow.Profiles
{
    public class ArchiveNowProfileRepository
    {
        private const string DefaultProfileFileNameExtension = "profile";

        private readonly string _directoryPath;

        public ArchiveNowProfileRepository(string directoryPath)
        {
            this._directoryPath = directoryPath;
        }

        public IEnumerable<ArchiveNowProfile> Load()
        {
            var files = Directory.EnumerateFiles(this._directoryPath, "*.profile");

            return files.Select(ArchiveNowProfileReader.Read);
        }

        public void Save(ArchiveNowProfile profile)
        {
            var profileFileName = Path.ChangeExtension(profile.Name, DefaultProfileFileNameExtension);
            var profileFilePath = Path.Combine(this._directoryPath, profileFileName);

            var writer = new ArchiveNowProfileReader(profileFilePath);
            writer.Write(profile);
        }

        
    }
}
