using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public class Utils
    {
        public delegate void DelegateMethod();

        public static void TrackExecutionTime (DelegateMethod delegateMethod, String action)
        {
            Stopwatch temp = Stopwatch.StartNew();

            delegateMethod();

            temp.Stop();

#if DEBUG
            Console.WriteLine(String.Format("({0}) Time Elapsed: {1}ms", action, temp.ElapsedMilliseconds));
#endif
        }
    }
}
