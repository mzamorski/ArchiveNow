using System.IO;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class ParentDirectoryNameFileNameBuilder : IFileNameBuilder
    {
        public string Build(IFileNameBuilderContext context)
        {
            var directoryName = Directory.GetParent(context.SourcePath);
            
            return directoryName.Name;
        }
    }
}