namespace ArchiveNow.Actions.Core
{
    public class AfterFinishedActionContext : IAfterFinishedActionContext
    {
        public string InputPath { get; }

        public AfterFinishedActionContext(string inputPath)
        {
            InputPath = inputPath;
        }
    }
}