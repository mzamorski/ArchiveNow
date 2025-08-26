using System;
using System.IO;
using ArchiveNow.Actions.Core.Result;
using ArchiveNow.Utils;

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
                if (File.Exists(context.InputPath))
                {
                    File.Delete(context.InputPath);

                    foreach (var path in context.AdditionalPaths)
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
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

