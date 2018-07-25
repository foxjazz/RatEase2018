using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;


namespace RatEaseW
{
    public class ScreenCapture
    {
        public Image Capture(Rectangle rec)
        {
            // var rec = new Rectangle(SourcePoint.X, SourcePoint.Y, DestinationPoint.X - SourcePoint.X,
            //DestinationPoint.Y - SourcePoint.Y);


            LocalWidth = rec.Width;
            LocalHeight = rec.Height;
            LocalX = rec.X;
            LocalY = rec.Y;
            return CaptureScreen();

        }
        public Image Capture(Point SourcePoint, Point DestinationPoint)
        {
            var rec = new Rectangle(SourcePoint.X, SourcePoint.Y, DestinationPoint.X - SourcePoint.X,
           DestinationPoint.Y - SourcePoint.Y);


            LocalWidth = rec.Width;
            LocalHeight = rec.Height;
            LocalX = Convert.ToInt32(SourcePoint.X);
            LocalY = (Int32)SourcePoint.Y;
            return CaptureScreen();

        }
        public Image CapturePoint(Point SourcePoint)
        {
            var rec = new Rectangle(SourcePoint.X, SourcePoint.Y, 1, 1);



            LocalWidth = rec.Width;
            LocalHeight = rec.Height;
            LocalX = SourcePoint.X;
            LocalY = SourcePoint.Y;
            return CaptureScreen();

        }
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }
        public int LocalX { get; set; }
        public int LocalY { get; set; }
        public double LocalWidth { get; set; }
        public double LocalHeight { get; set; }
        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. 
        /// (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        /// 
        //        public Surface GetBackBuffer(
        // int swapChain,
        // int backBuffer,
        // BackBufferType backBufferType
        //            Surface backbuffer = device.GetBackBuffer(0, 0, BackBufferType.Mono);
        //        SurfaceLoader.Save("Screenshot.bmp", ImageFileFormat.Bmp, backbuffer);
        //backbuffer.Dispose();
        //);


        public Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);


            //int width = windowRect.right - windowRect.left;

            int width = Convert.ToInt32(LocalWidth);
            //int height = windowRect.bottom - windowRect.top;

            int height = Convert.ToInt32(LocalHeight);
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, LocalX, LocalY, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }
        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }
        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }

        internal Image CaptureRed(Rectangle vRec, RedTopHeight rth)
        {

            Point sp = new Point(vRec.X, vRec.Y + rth.Top);
            Point dp = new Point(vRec.X + 100, vRec.Y + rth.Top + rth.Height);

            return Capture(sp, dp);
        }
    }
}
