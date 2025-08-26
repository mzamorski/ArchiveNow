using System.Collections.Generic;

namespace ArchiveNow.Actions.Core
{
    public interface IAfterFinishedActionContext
    {
        string InputPath { get; }

        ICollection<string> AdditionalPaths { get; }
    }
}

