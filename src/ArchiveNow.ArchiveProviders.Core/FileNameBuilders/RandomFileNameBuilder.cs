using System;
using System.IO;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class RandomFileNameBuilder : IFileNameBuilder
    {
        private static readonly Lazy<RandomFileNameBuilder> _instance = new Lazy<RandomFileNameBuilder>(() => new RandomFileNameBuilder());

        public static RandomFileNameBuilder Instance => _instance.Value;

        public string Build()
        {
            return Build(EmptyFileNameBuilderContext.Instance);
        }

        public string Build(IFileNameBuilderContext context)
        {
            return Path.GetRandomFileName().Replace(".", String.Empty);
        }
    }
}
