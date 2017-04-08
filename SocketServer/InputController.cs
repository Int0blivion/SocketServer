using System;
using System.Runtime.InteropServices;
using System.Windows;
using WindowsInput;

namespace SocketServer
{
    public class InputController
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point( POINT point )
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        static extern bool SetCursorPos( int x, int y );

        [DllImport("user32.dll")]
        public static extern void mouse_event( int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo );

        [DllImport("user32.dll")]
        static extern bool GetCursorPos( out Point lpPoint );

        [DllImport("user32.dll")]
        static extern bool GetCursorPos( out POINT lpPoint );

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey( uint uCode, uint uMapType );

        public static void SingleClick()
        {
            Utils.TrackExecutionTime(() =>
            {
                POINT cursorPos;

                GetCursorPos(out cursorPos);

                mouse_event((int) (MouseEventFlags.LeftDown | MouseEventFlags.LeftUp), cursorPos.X, cursorPos.Y, 0, 0);
            }, "Single Click");
        }

        public static void DoubleClick()
        {
            Utils.TrackExecutionTime(() =>
            {
                POINT cursorPos;

                GetCursorPos(out cursorPos);

                mouse_event((int) (MouseEventFlags.LeftDown | MouseEventFlags.LeftUp), cursorPos.X, cursorPos.Y, 0, 0);
                mouse_event((int) (MouseEventFlags.LeftDown | MouseEventFlags.LeftUp), cursorPos.X, cursorPos.Y, 0, 0);
            }, "Double Click");
        }

        public static void RightClick()
        {
            Utils.TrackExecutionTime(() =>
            {
                POINT cursorPos;

                GetCursorPos(out cursorPos);

                mouse_event((int) (MouseEventFlags.RightDown | MouseEventFlags.RightUp), cursorPos.X, cursorPos.Y, 0, 0);
            }, "Right Click");
        }

        public static void MoveMouse(float velocityX, float velocityY)
        {
            Utils.TrackExecutionTime(() =>
            {
                POINT cursorPos;
                GetCursorPos(out cursorPos);

                const float SCALE = 1f;

                int finalX = (int) (cursorPos.X + (SCALE * velocityX));
                int finalY = (int) (cursorPos.Y + (SCALE * velocityY));

                TransitionMouseTo(cursorPos, finalX, finalY);
            }, "Move Cursor");
        }

        /// <summary>Helper function to 'smoothly' move the mouse to the desired point
        /// </summary>
        /// <param name="cursorPos"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private static void TransitionMouseTo( Point cursorPos, int x, int y)
        {
            int frames = 1000;

            Point vector = new Point();

            vector.X = (x - cursorPos.X) / frames;
            vector.Y = (y - cursorPos.Y) / frames;

            for ( int i = 0; i < frames; i++ )
            {
                cursorPos.X += vector.X;
                cursorPos.Y += vector.Y;

                SetCursorPos((int) cursorPos.X, (int) cursorPos.Y);
            }
        }

        public static void InputMessage(String message)
        {
            InputSimulator.SimulateTextEntry(message);
        }
    }
}
