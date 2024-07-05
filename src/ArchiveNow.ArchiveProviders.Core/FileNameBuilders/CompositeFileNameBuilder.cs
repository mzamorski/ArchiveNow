using ArchiveNow.Utils;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class CompositeFileNameBuilder : CompositeBase<IFileNameBuilder>, IFileNameBuilder
    {
        public CompositeFileNameBuilder(params IFileNameBuilder[] builders)
            : base(builders)
        { }

        public string Build(IFileNameBuilderContext context)
        {
            var path = context.SourcePath;

            foreach (var builder in List)
            {
                path = builder.Build(new FileNameBuilderContext(path, context.Date));
            }

            return path;
        }
    }
}