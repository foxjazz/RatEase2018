using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatEaseW
{
    public sealed class CurrentData : INotifyPropertyChanged
    {
        private static CurrentData instance;
        internal Point pBottomRight;

        private CurrentData() { }

        public static CurrentData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CurrentData();
                    instance.CaptureMode = new List<string>();
                    instance.CaptureMode.Add("Set Local Vertical");
                    instance.CaptureMode.Add("Set System Image");
                    instance.CaptureMode.Add("Set Overview Image");

                    instance.RedData = new Message();


                }
                return instance;
            }
        }

        public void SaveData()
        {

        }
        public string SelectedDraw { get; set; }
        public bool recselect { get; internal set; }
        public bool foundRed { get; internal set; }
        public Point redPixel { get; internal set; }
        public Point pTopleft { get; internal set; }
        public ScreenCapture sc { get; set; }
        public Message RedData { get; set; }

        public List<string> CaptureMode { get; set; }
        internal void SetVerticalImage(bool v)
        {
            throw new NotImplementedException();
        }

        private Image _titleImage { get; set; }
        public Image TitleImage { get {return _titleImage; }
            set {_titleImage = value;  OnPropertyChanged("TitleImage"); } }

        private Image _vImg { get; set; }
        public Image VImg
        {
            get { return _vImg; }
            set { _vImg = value; OnPropertyChanged("VImg"); }
        }

        Rectangle TitleRec;
        //the virticle rectangle
        Rectangle VRec;
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetTitleImage(bool useSaved)
        {
            if (useSaved)
            {
                Point pnt = new Point(TitleRec.Location.X + TitleRec.Width, TitleRec.Y + TitleRec.Height);
                TitleImage = sc.Capture(TitleRec.Location, pnt);
            }
            else
                TitleImage = sc.Capture(pTopleft, pBottomRight);
            TitleRec = SetRec(pTopleft, pBottomRight);
            //this.pbMainSystem.Image = TitleImage;  should be bound by the surface

        }

        private Rectangle SetRec(Point a, Point b)
        {
            Size s = new Size { Width = b.X - a.X, Height = b.Y - a.Y };
            return new Rectangle { Location = a, Size = s };
        }
    }
}
