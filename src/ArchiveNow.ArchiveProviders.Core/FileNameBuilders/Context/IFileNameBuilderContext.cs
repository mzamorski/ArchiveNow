using System;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public interface IFileNameBuilderContext
    {
        string SourcePath { get; }

        DateTime Date { get; }
    }
}