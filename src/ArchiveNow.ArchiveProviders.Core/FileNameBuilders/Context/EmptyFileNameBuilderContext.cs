using System;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public sealed class EmptyFileNameBuilderContext : IFileNameBuilderContext
    {
        private static readonly Lazy<EmptyFileNameBuilderContext> _instance =
            new Lazy<EmptyFileNameBuilderContext>(() => new EmptyFileNameBuilderContext());

        public static EmptyFileNameBuilderContext Instance => _instance.Value;

        public string SourcePath => string.Empty;
        public DateTime Date => DateTime.MinValue;
    }
}