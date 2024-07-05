namespace ArchiveNow.Actions.Core
{
    public interface IAfterFinishedActionResult
    {
        bool IsSuccess { get; }

        string Message { get; }

        string OutputPath { get; }

        //bool CanContinue { get; }
    }
}