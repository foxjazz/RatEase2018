using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace RatEaseW
{
    /// <summary>
    /// Interaction logic for GreenScreenW.xaml
    /// </summary>
    public partial class GreenScreenW : Window
    {
        public GreenScreenW()
        {
            InitializeComponent();
            //var g = new System.Drawing.Graphics()
            cd = CurrentData.Instance;
            
        }
        CurrentData cd;
        private bool IsRecSelectOn { get; set; }
        private System.Windows.Point StartPoint { get; set; }
        private System.Windows.Point EndPoint { get; set; }
        
        private int recTop;
        private int recHeight;
        private int recLeft;
        private Rectangle recStart { get; set; }
        //public System.Windows.Shapes.Rectangle rec  { get; set; }
        //System.Drawing.Graphics graphics { get; set; }
        private bool RecSettingEnable { get; set; }
        private int width { get; set; }
        private int height { get; set; }
        public bool UseTitle { get; set; }
        public bool mouseDown { get; set; }

        private System.Windows.Point downPosition;
        private System.Windows.Point upPosition;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static System.Windows.Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new System.Windows.Point(w32Mouse.X, w32Mouse.Y);
            
        }

        //public static Point GetMousePositionWindowsForms()
        //{
        //    System.Drawing.Point point = Control.MousePosition;
        //    return new Point(point.X, point.Y);
        //}

        //private Point GetMousePosition()
        //{
        //    // Position of the mouse relative to the window
        //    var position = Mouse.GetPosition(Window);

        //    // Add the window position
        //    return new Point(position.X + Window.Left, position.Y + Window.Top);
        //}
    //    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    //    {
    //        if (e.RightButton == MouseButtonState.Pressed)
    //        {
    //            this.Hide();
    //            return;
    //        }
            
    //        StartPoint = GreenScreenW.GetMousePosition();
    //        //cd.redPixel.X, cd.redPixel.Y, 6, 6
    //        //rec = new System.Windows.Shapes.Rectangle();
    //        rec.HorizontalAlignment = HorizontalAlignment.Left;
    //        rec.VerticalAlignment = VerticalAlignment.Top;
    //        rec.Height = 200;
    //        rec.Width = 100;
            
    //        //grnCanvas.Children.Add(rec);

    //    }

    //    private void Window_MouseUp(object sender, MouseButtonEventArgs e)
    //    {
    //        mouseDown = false;
    //        var pos = e.GetPosition(this);

            

    //        if (UseTitle)
    //        {
    //            cd.pBottomRight = new System.Drawing.Point((int)pos.X, (int)pos.Y);
    //            cd.SetTitleImage(false);
    //        }
    //        if (cd.SelectedDraw == "SetVertical")
    //        {
    //            cd.pBottomRight = new System.Drawing.Point((int)pos.X, (int)pos.Y);
    //            cd.SetVerticalImage(false);
    //            cd.recselect = false;
    //            cd.foundRed = false;
    //        }


    //        if (cd.SelectedDraw == "foundRed")
    //        {
    //            //rec = new System.Drawing.Rectangle(cd.redPixel.X, cd.redPixel.Y, 6, 6);
    //            //graphics.DrawRectangle(System.Drawing.Pens.DarkRed, rec);
    //        }
    //        if (cd.SelectedDraw == "recselect")
    //        {
    //            width = cd.pBottomRight.X - cd.pTopleft.X; height = cd.pBottomRight.Y - cd.pTopleft.Y;
    //            if (width > 1 && height > 1)
    //            {
    //                //rec = new System.Drawing.Rectangle(cd.pTopleft.X, cd.pTopleft.Y, width, height);
    //                //graphics.DrawRectangle(System.Drawing.Pens.Black, rec);
    //            }
    //        }
            
    //        EndPoint = GreenScreenW.GetMousePosition();
    //    }

    //    private void Window_MouseMove(object sender, MouseEventArgs e)
    //    {
    //        if (rec == null)
    //            return;
    //        upPosition = e.GetPosition(this);
    //        //grnCanvas.
    //        rec.StrokeThickness = 2;
    //        double test = upPosition.X - StartPoint.X;
    //        if (test > 0)
    //            rec.Width = test;
    //        test = upPosition.Y - StartPoint.Y;
    //        if (test > 200)
    //            rec.Height = test;
    //        // graphics.DrawRectangle(System.Drawing.Pens.DarkRed, rec);
    //    }
    }
}
