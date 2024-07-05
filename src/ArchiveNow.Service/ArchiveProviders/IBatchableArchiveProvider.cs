using System;
using System.Collections.Generic;

namespace ArchiveNow.Service.ArchiveProviders
{
    /// <summary>
    /// Some providers don't provide an API that allows you to add files/folders to the archive individually.
    /// </summary>
    public interface IBatchableArchiveProvider : IDisposable
    {
        void Add(ICollection<string> paths);

        event EventHandler<string> FileAdded;

        event EventHandler<string> DirectoryAdded;

        event EventHandler<string> Initialized;

        event EventHandler Finished;
    }
}