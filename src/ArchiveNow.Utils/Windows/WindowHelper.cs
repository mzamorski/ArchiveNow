using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ArchiveNow.Utils.Windows
{
    public class WindowHelper
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetActiveWindow();

        public static bool ShowDialog(Window dialog, Window owner)
        {
            // Sometimes it doesn't work in WPF.
            //dialog.Owner = owner;

            var helper = new WindowInteropHelper(dialog)
            {
                Owner = owner != null
                    ? new WindowInteropHelper(owner).Handle
                    : GetActiveWindow()
            };

            bool? dialogResult = dialog.ShowDialog();

            return dialogResult ?? false;
        }

        public static bool Launch(Window window, bool modal = true)
        {
            if (window == null)
            {
                return false;
            }

            if (modal)
            {
                bool? retVal = window.ShowDialog();
                if (retVal.HasValue)
                {
                    return retVal.Value;
                }
            }
            else
            {
                window.Show();
            }

            return false;
        }
    }
}
