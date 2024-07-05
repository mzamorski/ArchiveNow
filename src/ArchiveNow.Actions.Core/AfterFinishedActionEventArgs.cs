using System;

namespace ArchiveNow.Actions.Core
{
    public class AfterFinishedActionEventArgs : EventArgs
    {
        public IAfterFinishedAction Action { get; }

        public IAfterFinishedActionResult Result { get; }

        public bool Break { get; set; }

        public AfterFinishedActionEventArgs(IAfterFinishedAction action, IAfterFinishedActionResult result = null)
        {
            Action = action;
            Result = result;
        }
    }
}
