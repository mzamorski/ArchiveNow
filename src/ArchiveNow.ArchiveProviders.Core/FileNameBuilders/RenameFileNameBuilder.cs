namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class RenameFileNameBuilder : IFileNameBuilder
    {
        public string Name { get; }

        public RenameFileNameBuilder(string name)
        {
            Name = name ?? string.Empty;
        }

        string IFileNameBuilder.Build(IFileNameBuilderContext context)
        {
            return Name;
        }
    }
}
