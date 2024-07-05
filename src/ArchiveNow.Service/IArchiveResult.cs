namespace ArchiveNow.Service
{
    public interface IArchiveResult
    {
        bool IsSuccess { get; }

        string Message { get; }

        string ArchivePath { get; }
    }
}