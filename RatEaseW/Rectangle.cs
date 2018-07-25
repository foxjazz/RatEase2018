using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace RatEaseW
{
    public class Rectangle
    {

        public Rectangle(Point p, Size s)
        {
            
            point = p;
            size = s;
        }
        public Rectangle(int X, int Y, int width, int height)
        {
            if (_point == null)
                _point = new Point(X, Y);
            else
            {
                _point.X = X;
                _point.Y = Y;
            }
            if (_size == null)
                _size = new Size(width, height);
            else
            {
                _size.Width = width;
                _size.Height = height;
            }
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private Point _point;
        public Point point
        {
            get { if (_point == null) _point = new Point(X, Y); return _point; }
            set
            {
                _point = value;
                X = _point.X;
                Y = _point.Y;
            }
        }
        private Size _size;
        public Size size { get { if (_size == null) _size = new Size(Width, Height); return _size; } set {_size = value; Width = _size.Width;Height = _size.Height; } }


        public class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Point(int X, int Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }

        public class Size
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public Size(int Width, int Height)
            {
                this.Width = Width;
                this.Height = Height;
            }
        }
    }
    
}
