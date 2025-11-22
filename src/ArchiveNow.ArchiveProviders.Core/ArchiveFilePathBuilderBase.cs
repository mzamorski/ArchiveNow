using System.IO;

using ArchiveNow.Utils;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public abstract class ArchiveFilePathBuilderBase : IArchiveFilePathBuilder
    {
        public abstract string Build(string fileExtension);

        protected string ChangeExtension(string path, string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                return path;
            }

            //string output = path.IsDirectory()
            //    ? $"{path}.{fileExtension}"
            //    : Path.ChangeExtension(path, fileExtension);

            //return output;

            return $"{path}.{fileExtension}";
        }
    }
}