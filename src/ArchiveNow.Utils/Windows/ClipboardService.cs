using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace ArchiveNow.Utils.Windows
{
    public class ClipboardService : IClipboardService
    {
        private const int DefaultNumberOfTries = 3;

        private static readonly object ThreadSyncObject = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="numberOfTries"></param>
        /// <returns></returns>
        [STAThread]
        public static bool TrySetClipboard(string text, int numberOfTries = DefaultNumberOfTries)
        {
            var hasError = false;

            var thread = new Thread(() =>
                {
                    try
                    {
                        Clipboard.SetText(text);
                    }
                    catch (ExternalException)
                    {
                        hasError = true;
                    }
                }
            );
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return hasError;
        }

        public void SetText(string text)
        {
            TrySetClipboard(text);
        }
    }
}
