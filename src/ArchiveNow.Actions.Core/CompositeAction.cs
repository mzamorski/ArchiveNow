using System.Collections.Generic;
using System.Linq;
using ArchiveNow.Actions.Core.Result;

namespace ArchiveNow.Actions.Core
{
    public class CompositeAction : AfterFinishedActionBase
    {
        private readonly IEnumerable<IAfterFinishedAction> _actions;

        public CompositeAction(IEnumerable<IAfterFinishedAction> actions)
        {
            _actions = actions;
        }

        public override string Description => string.Empty;

        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext result)
        {
            IList<IAfterFinishedActionResult> list = new List<IAfterFinishedActionResult>(_actions.Count());

            foreach (var job in _actions)
            {
                var actionResult = job.Execute(result);
                list.Add(actionResult);
            }

            return new CompositeAfterFinishedActionResult(list.ToArray());
        }
    }
}
