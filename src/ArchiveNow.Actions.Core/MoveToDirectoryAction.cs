using System;
using System.IO;
using ArchiveNow.Actions.Core.Result;
using ArchiveNow.Utils;

namespace ArchiveNow.Actions.Core
{
    public class MoveToDirectoryAction : AfterFinishedActionBase
    {
        private readonly string _destinationPath;

        public MoveToDirectoryAction(string destinationPath)
            : base(precedence: 9)
        {
            _destinationPath = destinationPath;
        }

        public override string Description => "Moving to the directory...";

        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
            var hasError = false;
            var message = string.Empty;
            var outputPath = context.InputPath;

            try
            {
                var fileName = Path.GetFileName(context.InputPath);
                if (fileName == null)
                {
                    throw new NullReferenceException("fileName");
                }

                outputPath = Path.Combine(_destinationPath, fileName);

                File.Move(context.InputPath, outputPath);
            }
            catch (Exception ex)
            {
                hasError = true;
                message = ex.Message;
            }

            return new AfterFinishedActionResult(hasError.Not(), outputPath, message);
        }
    }
}