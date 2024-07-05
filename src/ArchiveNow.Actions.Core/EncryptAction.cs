using System;
using ArchiveNow.Actions.Core.Result;
using ArchiveNow.Utils;
using ArchiveNow.Utils.Security;

namespace ArchiveNow.Actions.Core
{
    public class EncryptAction : AfterFinishedActionBase
    {
        private readonly EncryptionService _service = new EncryptionService();
        private readonly string _publicKeyFilePath;

        public EncryptAction(string publicKeyFilePath)
            : base(precedence: 0)
        {
            _publicKeyFilePath = publicKeyFilePath;
        }

        public override string Description => "Encrypting...";

        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
            var hasError = false;
            var message = string.Empty;

            string outputPath = string.Empty;

            try
            {
                outputPath = _service.EncryptFile(context.InputPath, _publicKeyFilePath);
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