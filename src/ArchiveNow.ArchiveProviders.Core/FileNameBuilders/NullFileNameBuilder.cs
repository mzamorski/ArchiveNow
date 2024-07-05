using System;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class NullFileNameBuilder : IFileNameBuilder
    {
        private static readonly Lazy<IFileNameBuilder> _instance = new Lazy<IFileNameBuilder>(() => new NullFileNameBuilder());

        public static IFileNameBuilder Instance => _instance.Value;

        private NullFileNameBuilder()
        { }

        public string Build(IFileNameBuilderContext context)
        {
            return string.Empty;
        }
    }
}
