using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public class NetworkPacket
    {
        public enum PacketType : byte
        {
            SingleClick = 0,

            DoubleClick,

            Scroll,

            RightClick,

            BeginKeyboard,

            EndKeyboard,

            Sleep = 252,

            Restart,

            Shutdown,

            Disconnect
        }
    }
}
