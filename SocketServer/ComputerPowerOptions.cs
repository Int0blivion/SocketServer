using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SocketServer
{
    public class ComputerPowerOptions
    {
        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState( bool hiberate, bool forceCritical, bool disableWakeEvent );

        /// <summary>Sends the computer to sleep
        /// </summary>
        //[Conditional("Release")]
        public static void Sleep()
        {
            SetSuspendState(false, true, true);
        }

        /// <summary>Sends the computer to a hibernation state
        /// </summary>
        //[Conditional("RELEASE")]
        public static void Hibernate()
        {
            SetSuspendState(true, true, true);
        }

        /// <summary>Restart Computer
        /// </summary>
        //[Conditional("RELEASE")]
        public static void Restart()
        {
            Process.Start("shutdown","/r /t 0");
        }

        /// <summary>Shutdown Computer
        /// </summary>
        //[Conditional("RELEASE")]
        public static void Shutdown()
        {
            Process.Start("shutdown", "/s /t 0");
        }
    }
}
