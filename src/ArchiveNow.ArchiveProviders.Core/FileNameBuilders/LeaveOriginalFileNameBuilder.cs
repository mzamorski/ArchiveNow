namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class LeaveOriginalFileNameBuilder : IFileNameBuilder
    {
        public string Build(IFileNameBuilderContext context)
        {
            return context.SourcePath;
        }
    }
}