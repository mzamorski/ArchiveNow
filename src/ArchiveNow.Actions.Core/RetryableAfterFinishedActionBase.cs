using System;

namespace ArchiveNow.Actions.Core
{
    public abstract class RetryableAfterFinishedActionBase : AfterFinishedActionBase
    {
        private readonly Action _action;

        protected RetryableAfterFinishedActionBase(Action action)
        {
            _action = action;
        }

        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}