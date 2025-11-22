using System;
using ArchiveNow.Actions.Core.Result;
using ArchiveNow.Utils;
using ArchiveNow.Utils.Windows;

namespace ArchiveNow.Actions.Core
{
    public class SetClipboardAction : AfterFinishedActionBase
    {
        private readonly IClipboardService _clipboardService;

        public override string Description => "Setting path in the clipboard...";

        public SetClipboardAction(IClipboardService clipboardService)
            : base(precedence: 10)
        {
            _clipboardService = clipboardService;
        }

        public override IAfterFinishedActionResult Execute(IAfterFinishedActionContext context)
        {
            var hasError = false;
            var message = string.Empty;

            try
            {
                _clipboardService.SetText(context.InputPath);
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