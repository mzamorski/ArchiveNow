using System;

namespace ArchiveNow.Service.SearchFileProvider
{
    [Flags]
    public enum OutputType
    {
        FilePath = 0,
        DirectoryPath = 1,

        Default = FilePath,
        Full = FilePath | DirectoryPath
    }
}