namespace ArchiveNow.Actions.Core
{
    public class AfterFinishedActionProgress
    {
        public bool IsCompleted { get; set; }

        public bool IsFailed { get; set; }

        public decimal Percentage { get; set; }
    }
}