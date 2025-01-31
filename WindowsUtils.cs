using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace quicklaunch
{
    internal class WindowsUtils
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        private const int SW_MINIMIZE = 6;

        public static void Minimize()
        {
            IntPtr windowHandle = GetActiveWindow();
            if (windowHandle != IntPtr.Zero)
            {
                ShowWindow(windowHandle, SW_MINIMIZE);
            }
            else
            {
                Plugin.Log.LogError("Could not find the active window.");
            }
        }
    }
}
