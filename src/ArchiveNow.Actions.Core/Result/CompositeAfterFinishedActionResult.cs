using System.Collections.Generic;
using System.Linq;

namespace ArchiveNow.Actions.Core.Result
{
    public class CompositeAfterFinishedActionResult : IAfterFinishedActionResult
    {
        private readonly IReadOnlyCollection<IAfterFinishedActionResult> _results;

        public bool IsSuccess => _results.All(r => r.IsSuccess);

        public string Message
        {
            get
            {
                return _results
                    .Select(r => r.Message)
                    .Aggregate((current, next) => current + ". " + next);
            }
        }
        public string OutputPath { get; }

        public CompositeAfterFinishedActionResult(params IAfterFinishedActionResult[] results)
        {
            _results = results ?? new[] { NullAfterFinishedActionResult.Instance };
        }
    }
}
