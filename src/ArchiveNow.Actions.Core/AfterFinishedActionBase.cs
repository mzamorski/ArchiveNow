using System;

namespace ArchiveNow.Actions.Core
{
    public abstract class AfterFinishedActionBase : IAfterFinishedAction
    {
        public string Name => GetType().Name;

        public abstract string Description { get; }

        public int Precedence { get; }

        public bool BreakIfError { get; set; }

        public IProgress<AfterFinishedActionProgress> Progress { get; set; }

        protected AfterFinishedActionBase(int precedence = 0)
        {
            this.Precedence = precedence;
        }

        public abstract IAfterFinishedActionResult Execute(IAfterFinishedActionContext context);
    }
}
