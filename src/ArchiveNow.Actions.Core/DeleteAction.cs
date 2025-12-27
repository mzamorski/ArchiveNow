using System;

using ArchiveNow.Actions.Core.Result;
using ArchiveNow.Utils;
using ArchiveNow.Utils.IO;

namespace ArchiveNow.Actions.Core
{
    public class DeleteAction : AfterFinishedActionBase
    {
        public override string Description => "Deleting...";

        public DeleteAction()
            : base(precedence: 10)
        { }

        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
            var hasError = false;
            var message = string.Empty;

            try
            {
                // Attempt to delete the main input path.
                // Returns true if the path existed and was processed.
                if (FileSystemExtensions.DeletePath(context.InputPath))
                {
                    // If the main path existed, proceed to clear additional paths.
                    // We ignore return values here as we just want to ensure they are gone.
                    foreach (var path in context.AdditionalPaths)
                    {
                        FileSystemExtensions.DeletePath(path);
                    }

                    context.AdditionalPaths.Clear();
                }
                else
                {
                    hasError = true;
                    message = "Path not found!";
                }
            }
            catch (Exception ex)
            {
                hasError = true;
                message = ex.Message;
            }

            return new AfterFinishedActionResult(hasError.Not(), context.InputPath, message);
        }
    }
}

