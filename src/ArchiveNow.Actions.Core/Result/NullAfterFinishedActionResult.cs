using System;

namespace ArchiveNow.Actions.Core.Result
{
    public sealed class NullAfterFinishedActionResult : IAfterFinishedActionResult
    {
        private static readonly Lazy<IAfterFinishedActionResult> _instance =
            new Lazy<IAfterFinishedActionResult>(() => new NullAfterFinishedActionResult());

        public static IAfterFinishedActionResult Instance => _instance.Value;

        public bool IsSuccess => true;

        public string Message => string.Empty;

        public string OutputPath => string.Empty;
    }
}
