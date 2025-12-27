using System;

namespace ArchiveNow.Service
{
    public interface IArchiveResult
    {
        bool IsSuccess { get; }

        bool IsAborted { get; }

        string Message { get; }

        string ArchivePath { get; }

        TimeSpan Duration { get; }
    }
}