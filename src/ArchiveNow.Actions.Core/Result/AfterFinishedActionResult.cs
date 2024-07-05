namespace ArchiveNow.Actions.Core.Result
{
    public class AfterFinishedActionResult : IAfterFinishedActionResult
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public string OutputPath { get; }

        public AfterFinishedActionResult(bool isSuccess, string outputPath, string message = null)
        {
            IsSuccess = isSuccess;
            OutputPath = outputPath;
            Message = message;
        }
    }
}
