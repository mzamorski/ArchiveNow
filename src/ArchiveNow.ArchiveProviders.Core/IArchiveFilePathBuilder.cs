using ArchiveNow.Providers.Core.FileNameBuilders;

namespace ArchiveNow.Providers.Core
{
    public interface IArchiveFilePathBuilder
    {
        IFileNameBuilderContext Context { get; }

        string Build(string fileExtension);
    }
}
