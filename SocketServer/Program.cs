using System;

using System.Runtime.InteropServices;
using System.Threading;

namespace SocketServer
{
    public class Program
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        private const int SW_HIDE = 0;

        private static IntPtr handle;

        public static void Main( string[] args )
        {

            //System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;

#if !DEBUG
            handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
#endif

            Thread listener = new Thread(new ThreadStart(StartListener));

            listener.Start();
        }

        private static void StartListener()
        {
            SocketListener socketListener = new SocketListener();
            //AsyncSocketListener listener = new AsyncSocketListener();
        }
    }
}
