using ArchiveNow.Providers.Core.FileNameBuilders.Formatters;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class AddDateTimeFileNameBuilder : IFileNameBuilder
    {
        public string Build(IFileNameBuilderContext context)
        {
            var formatter = new DateTimeFileNameFormatter(context.Date);

            return formatter.Format(context.SourcePath);
        }
    }
}