using System;

namespace ArchiveNow.Actions.Core
{
    public interface IAfterFinishedAction
    {
        int Precedence { get; }
        string Name { get; }
        string Description { get; }
        bool BreakIfError { get; set; }
        IProgress<AfterFinishedActionProgress> Progress { get; set; }

        IAfterFinishedActionResult Execute(IAfterFinishedActionContext context);
    }
}
