using System.Collections.Generic;

namespace ArchiveNow.Actions.Core
{
    public class AfterFinishedActionContext : IAfterFinishedActionContext
    {
        public string InputPath { get; }

        public ICollection<string> AdditionalPaths { get; }

        public AfterFinishedActionContext(string inputPath, ICollection<string> additionalPaths = null)
        {
            InputPath = inputPath;
            AdditionalPaths = additionalPaths ?? new List<string>();
        }
    }
}

