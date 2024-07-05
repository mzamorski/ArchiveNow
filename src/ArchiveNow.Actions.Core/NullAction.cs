using System;
using ArchiveNow.Actions.Core.Result;

namespace ArchiveNow.Actions.Core
{
    public sealed class NullAction : AfterFinishedActionBase
    {
        private static readonly Lazy<NullAction> _instance = new Lazy<NullAction>(() => new NullAction());

        public override string Description => string.Empty;

        public static NullAction Instance => _instance.Value;

        private NullAction()
        { }

        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
            return NullAfterFinishedActionResult.Instance;
        }
    }
}