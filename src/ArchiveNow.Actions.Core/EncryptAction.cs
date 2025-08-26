using System;
using ArchiveNow.Actions.Core.Result;
using ArchiveNow.Utils;
using ArchiveNow.Utils.Security;

namespace ArchiveNow.Actions.Core
{
    public class EncryptAction : AfterFinishedActionBase
    {
        private readonly AsymmetricEncryptionService _service = new AsymmetricEncryptionService();
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
                context.AdditionalPaths.Add(context.InputPath);
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

