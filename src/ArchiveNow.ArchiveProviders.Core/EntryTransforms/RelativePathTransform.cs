using System;
using System.IO;

namespace ArchiveNow.Providers.Core.EntryTransforms
{
    public class RelativePathTransform : IArchiveEntryTransform
    {
        public string RootPath { get; }

        public RelativePathTransform(string sourcePath)
        {
            RootPath = sourcePath;
        }

        public string Transform(string path)
        {
            //return path.Replace(RootPath, string.Empty);

            var pathUri = new Uri(path);
            var directoryPath = RootPath;

            // Folders must end in a slash
            if (!directoryPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                directoryPath += Path.DirectorySeparatorChar;
            }
            var directoryUri = new Uri(directoryPath);

            return Uri.UnescapeDataString(directoryUri.MakeRelativeUri(pathUri).ToString()
                .Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
