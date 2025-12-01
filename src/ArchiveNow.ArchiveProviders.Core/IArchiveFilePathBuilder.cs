namespace ArchiveNow.Providers.Core
{
    public interface IArchiveFilePathBuilder
    {
        string Build(string fileExtension);
    }
}
