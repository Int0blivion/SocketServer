using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#pragma warning disable 0618

namespace SocketServer
{
    public class SocketListener
    {
        public static readonly int PORT = 11000;

        private const int NUM_CLIENTS = 5;

        private Thread mClientThread;

        public SocketListener()
        {
            ListenForConnections();
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

        private void ListenForConnections()
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

                    Socket handler = listener.Accept();

                    Console.WriteLine("\n({0}) Connected to {1}\n", DateTime.Now.ToString(), handler.RemoteEndPoint.ToString());

                    if ( mClientThread != null && mClientThread.IsAlive )
                    {
                        mClientThread.Abort();
                        mClientThread = null;
                    }

                    mClientThread = new Thread(() => HandleClient(handler));
                    mClientThread.Start();
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        /// <summary>Method to run in seperate thread in order to handle a single client connection
        /// </summary>
        /// <param name="client"></param>
        private void HandleClient(Socket client)
        {
            try
            {
                while ( !HandleInput(client) )
                {

                }
            }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Disconnect(false);
                Console.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <returns>Whether or not the packet END_STREAM has been encountered</returns>
        private bool HandleInput(Socket handler)
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

                    case (byte) NetworkPacket.PacketType.BeginKeyboard:
                        HandleKeyboardInput(handler);
                        message = "Begin Keyboard Input";
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

        private void HandleKeyboardInput(Socket handler)
        {
            int received;
            byte[] sizeBuffer = new byte[4];
            byte[] buffer;

            received = handler.Receive(sizeBuffer);
            
            while(received > 0 || (received == 1 && sizeBuffer[0] != (byte) NetworkPacket.PacketType.EndKeyboard))
            {
                buffer = new byte[System.BitConverter.ToInt32(sizeBuffer, 0)];

                handler.Receive(buffer);

                //Network byte order is in big endian. Ensure that we recieve the data in the same way
                if ( BitConverter.IsLittleEndian )
                    Array.Reverse(buffer);

                InputController.InputMessage(BitConverter.ToString(buffer));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="handler"></param>
        private void HandleScroll(Socket handler)
        {
            //Float is 4 bytes
            byte[] buffer = new byte[4];

            float[] motion = new float[2];

            for ( int i = 0; i < motion.Length; i++)
            {
                if ( handler.Receive(buffer) == 0 )
                    return;

                //Network byte order is in big endian. Ensure that we recieve the data in the same way
                if(BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);

                motion[i] = System.BitConverter.ToSingle(buffer, 0);
            }

            InputController.MoveMouse(motion[0], motion[1]);
        }
    }
}
