namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public interface IFileNameBuilder
    {
        string Build(IFileNameBuilderContext context);
    }
}