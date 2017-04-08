using System;
using System.Net;
using System.Net.Sockets;


#pragma warning disable 0618

namespace SocketServer
{
    public class SocketClient
    {
        public static readonly int PORT = 11000;

        private const int NUM_CLIENTS = 5;
        private const byte END_STREAM = 255;

        public SocketClient()
        {
            StartListening();
        }

        private IPAddress getLocalIPAddress()
        {
            foreach ( IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()) )
            {
                if ( address.AddressFamily == AddressFamily.InterNetwork )
                {
                    return address;
                }
            }

            return null;
        }

        private void StartListening()
        {
            IPEndPoint localEndPoint = new IPEndPoint(getLocalIPAddress(), PORT);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(NUM_CLIENTS);

                while ( true )
                {
                    Console.WriteLine("Listening on {0}", listener.LocalEndPoint.ToString());
                    Console.WriteLine("Waiting for a connection...");

                    using ( Socket handler = listener.Accept() )
                    {
                        Console.WriteLine("\n({0}) Connected to {1}\n", DateTime.Now.ToString(), handler.RemoteEndPoint.ToString());

                        while ( !handleInput(handler) )
                        {

                        }

                        Console.Clear();
                    }
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <returns>Whether or not the packet END_STREAM has been encountered</returns>
        private bool handleInput( Socket handler )
        {
            byte[] buffer = new byte[1];
            byte packet;

            int numBytes = handler.Receive(buffer);

            if ( numBytes > 0 )
            {
                packet = buffer[0];

                String message = String.Empty;

                switch ( packet )
                {
                    #region Handle Input

                    case (byte) NetworkPacket.PacketType.SingleClick:
                        InputController.SingleClick();
                        message = "Single Click Received";
                        break;

                    case (byte) NetworkPacket.PacketType.DoubleClick:
                        InputController.DoubleClick();
                        message = "Double Click Received";
                        break;

                    case (byte) NetworkPacket.PacketType.Scroll:
                        HandleScroll(handler);
                        message = "Scroll Received";
                        break;

                    case (byte) NetworkPacket.PacketType.RightClick:
                        InputController.RightClick();
                        message = "Right Click";
                        break;

                    case (byte) NetworkPacket.PacketType.Disconnect:
                        message = "End Connection";
                        break;

                    case (byte) NetworkPacket.PacketType.Sleep:
                        ComputerPowerOptions.Sleep();
                        message = "Sleep";
                        break;

                    case (byte) NetworkPacket.PacketType.Shutdown:
                        ComputerPowerOptions.Shutdown();
                        message = "Shutdown";
                        break;

                    case (byte) NetworkPacket.PacketType.Restart:
                        ComputerPowerOptions.Restart();
                        message = "Restart";
                        break;

                    default:
                        message = "Unknown Data Received " + packet;
                        break;

                    #endregion
                }

#if DEBUG
                Console.WriteLine(message);
#endif

                return packet == (byte) NetworkPacket.PacketType.Disconnect;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="handler"></param>
        private void HandleScroll( Socket handler )
        {
            //Float is 4 bytes
            byte[] buffer = new byte[4];

            float[] motion = new float[2];

            for ( int i = 0; i < motion.Length; i++ )
            {
                if ( handler.Receive(buffer) == 0 )
                    return;

                //Network byte order is in big endian. Ensure that we recieve the data in the same way
                if ( BitConverter.IsLittleEndian )
                    Array.Reverse(buffer);

                motion[i] = System.BitConverter.ToSingle(buffer, 0);
            }

            InputController.MoveMouse(motion[0], motion[1]);
        }
    }
}
