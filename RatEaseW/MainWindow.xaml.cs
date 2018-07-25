using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Win32;
using Image = System.Windows.Controls.Image;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace RatEaseW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        //public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        //{
        //    using (var memory = new MemoryStream())
        //    {
        //        bitmap.Save(memory, ImageFormat.Png);
        //        memory.Position = 0;

        //        var bitmapImage = new BitmapImage();
        //        bitmapImage.BeginInit();
        //        bitmapImage.StreamSource = memory;
        //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapImage.EndInit();
        //        bitmapImage.Freeze();

        //        return bitmapImage;
        //    }
        //}
        public MainWindow()
        {
            InitializeComponent();
            gcwShowing = false;
            DataContext = CurrentData.Instance;
            dtimer = new DispatcherTimer();
            dtimer.Interval = new TimeSpan(0,0,0,0,250);
            dtimer.Tick += Dtimer_Tick;
            msg = CurrentData.Instance.RedData;
            openFileDialog1 = new OpenFileDialog();
            player = new System.Media.SoundPlayer();
            gcw = new GreenScreenW();
            //VRec = new System.Drawing.Rectangle();
            left = Properties.Settings.Default.left;
            top = Properties.Settings.Default.top;
            width = Properties.Settings.Default.width;
            height = Properties.Settings.Default.height;
            sc = new ScreenCapture();
            RedInSystem = false;
            reds = 0;
        }
        public GreenScreenW gcw { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int top;
        public int left;

        public ScreenCapture sc { get; set; }
        
        Message msg;
        FileDialog openFileDialog1;
        byte red = 130;
        private int redV;
        private List<int> RedStartList;
        private List<RedTopHeight> listRedTopHeight;
        public int cnt { get; set; }
        private bool RedInSystem;
        private bool gcwShowing;

        private int reds;
        private int redsPrev;
        //public System.Drawing.Point pTopleft { get; set; }
        //public System.Drawing.Point pBottomRight { get; set; }
        public int iteration { get; set; }
        private void Dtimer_Tick(object sender, EventArgs e)
        {
            dtimer.Stop();
            reds = CheckRed();
            if (reds != redsPrev)
            {
                redCount.Text = "Reds:" + reds;
            }
            redsPrev = reds;
            if (reds > 0 && RedInSystem == false)
            {
                RedInSystem = true;
                redCount.Text = "Reds:" + reds;
                PlaySound();
            }
            if (reds == 0 && RedInSystem )
            {
                redCount.Text = "Reds:" + reds;
                RedInSystem = false;
                PlaySound();
            }
            //if (subred == null)
            //    subred = new SmaRedis();
            //if (!subred.Connected)
            //{
            //    MessageBox.Show("Service is not active.");
            //    return;
            //}
            //subred.sublist = listAlerts;
            //subred.Subscribe("fade", CurrentData.Instance);
            //lblDetectedRed.Text = "Started";
            cnt = 0;
           

            //pBottomRight = new System.Drawing.Point { X = left + width, Y = top +height };
            iteration = 0;
            dtimer.Start();
        }
        public TimeSpan duration { get; set; }
        public DateTime ts { get; set; }
        public System.Drawing.Point redPixel { get; set; }
        System.Drawing.Rectangle VRec;
        System.Drawing.Image curImage;
        private System.Drawing.Bitmap curBitmap;
        private double fRed;
        public bool foundRed { get; set; }
        private int CheckRed()
        {
            if (listRedTopHeight == null)
                listRedTopHeight = new List<RedTopHeight>();
            RedStartList = new List<int>();
            bool inRed = false;
            RedCount = 0;
            fRed = 0;
            int ySectionStart = 0;
            //listRedTopHeight.Clear();
            var sp = new System.Drawing.Point(left, top);
            var dp = new System.Drawing.Point(left + width, top + height);
            
            curImage = sc.Capture(sp, dp);
            
            curBitmap = (Bitmap) curImage;
            //var img = GetImage(curBitmap);
            images.Children.Clear();
            //images.Children.Add(img);
            RedStartList.Clear();
            

            int capW, capY;
            capW = 100;
            capY = 18;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = curBitmap.GetPixel(x, y);
                    if (pixel.R > red && pixel.B < 16 && pixel.G < 15) // defined as red.
                    {
                        //dtimer.Stop();
                        //lblTopLeft.Text = x.ToString() + " - " + y.ToString();
                        redPixel = new System.Drawing.Point(left + x, top + y);
                        foundRed = true;
                        fRed++;
                        if (fRed == 1)
                        {
                            // The - one on the y axis if to allow font kerning to get the whole name;
                            var capImage = (Bitmap) sc.Capture(new System.Drawing.Point(left + x, (top + y) - 1), new System.Drawing.Point(capW + left + x,capY + top + y));
                            //var bitmap = pushBitmap(left, top, ref curBitmap);
                            var img2 = GetImage(capImage);
                            images.Children.Add(img2);
                        }
                        //lblDetectedRed.Text = "lblDetected Red";
                        IsClear = false;
                        if (inRed == false)
                        {
                            RedStartList.Add(y);
                            ySectionStart = y;
                            RedCount++;
                            redV = 1;
                            inRed = true;
                        }
                        else
                        {
                            redV++;
                        }

                    }
                    else {
                        if (inRed)
                        {
                            inRed = false;
                            fRed = 0;
                            listRedTopHeight.Add(new RedTopHeight { Top = ySectionStart, Height = redV + 1 });
                        }
                    }
                }
                duration = DateTime.Now.Subtract(ts);
                if (RedCount > 0)
                {
                    return RedCount;
                }
                //if (duration.Seconds > 1)
                //    lblDetectedRed.Text = "lagging scan by" + duration.ToString();
            }
            return 0;
        }
        private BitmapImage bmi(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        private Image GetImage(Bitmap bm)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage = bmi(bm);

            var img = new Image();

            img.Margin = new Thickness(0, 10, 0, 0);
            img.Source = bitmapImage;
            img.Stretch = Stretch.None;

            return img;
        }
        private Bitmap pushBitmap(int left, int top, ref Bitmap bm)
        {
            var cbm = new Bitmap(bm, 100, 14);
            int nleft=0, ntop=0;
            for (nleft = 0; nleft < 100; nleft++)
            {
                for (ntop = 0; ntop < 14; ntop++)
                {
                    cbm.SetPixel(nleft, ntop, bm.GetPixel(nleft + left, ntop + top));
                }
            }
            return cbm;
        }
        DispatcherTimer dtimer;
        System.Media.SoundPlayer player;
        public bool IsClear { get; set; }
        SmaRedis subred;
        public int RedCount { get; set; }
        private void PlaySound()
        {

            try
            {
                
                if (RedInSystem == false)
                {
                    player.SoundLocation = Properties.Settings.Default.AllClearSoundFile;
                    
                }
                else
                {

                    player.SoundLocation = Properties.Settings.Default.AlertSoundFile;

                }
                
                if (File.Exists(player.SoundLocation))
                {
                    player.Load();
                    player.Play();
                }
                //if (!IsClear)
                //{
                //    //subred.publish(tbSysName.Text + " vertical=" + redV);
                //    var msg = new Message();
                //    msg.Redcount = RedCount;
                //    msg.SystemText = tbSystemName.Text;
                //    //msg.System = TitleImage;
                //    var rth = listRedTopHeight.FirstOrDefault();
                //    if (rth != null)
                //        msg.FirstRed = sc.CaptureRed(VRec, rth);
                //    subred.publish(msg);

                //}
                //if (subred != null && IsClear)
                //    subred.publish(tbSystemName.Text + " Clear ");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " could not find sound file:" + Properties.Settings.Default.AlertSoundFile);
            }

        }

     
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            
            if (gcw == null)
                MessageBox.Show("Set the damn position");
            height = (int)gcw.Height;
            width = (int)gcw.Width;
            top = (int)gcw.Top;
            left = (int)gcw.Left;
            Properties.Settings.Default.left = left;
            Properties.Settings.Default.top = top;
            Properties.Settings.Default.width = width;
            Properties.Settings.Default.height = height;
            Properties.Settings.Default.Save();
            coord.Text = $"L:{(int)left}, T:{(int)top} W:{(int)width} H:{(int)height}";
            dtimer.Start();
            BtnStart.Content = "Started";
            if(gcwShowing)
                gcw.Hide();

        }

        private void SetAlertSound_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog1.Title = "Pick sound alert file";
            openFileDialog1.ShowDialog();
            Properties.Settings.Default.AlertSoundFile = openFileDialog1.FileName;
            Properties.Settings.Default.Save();
            bool hold = IsClear;
            IsClear = false;
            PlaySound();
            IsClear = hold;
        }

        private void SetClearSound_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog1.Title = "Pick sound all clear file";
            openFileDialog1.ShowDialog();
            Properties.Settings.Default.AllClearSoundFile = openFileDialog1.FileName;
            Properties.Settings.Default.Save();
            bool hold = IsClear;
            IsClear = true;
            PlaySound();
            IsClear = hold;
        }

        private void SetRectangle(object sender, RoutedEventArgs e)
        {
            left = Properties.Settings.Default.left;
            top = Properties.Settings.Default.top;
            width = Properties.Settings.Default.width;
            height = Properties.Settings.Default.height;
            if (left < 0)
                left = 100;
            if (top < 0)
                top = 100;
            if (width < 0)
                width = 4;
            if (height < 0)
                height = 500;
            if (width > 20)
                width = 4;
            gcw.Top = top;
            gcw.Left = left;
            gcw.Width = width;
            gcw.Height = height;
            gcwShowing = true;
            gcw.Show();

        }

       

       

        private void thinner(object sender, RoutedEventArgs e)
        {
            if(gcw.Width > 1)
                gcw.Width -= 1;
        }

        private void MoveRight(object sender, RoutedEventArgs e)
        {
            gcw.Left += Interval.Value;
        }

      

        private void extend(object sender, RoutedEventArgs e)
        {
            gcw.Height += Interval.Value;
        }

        private void Interval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtInterval.Text = e.NewValue.ToString();
        }

        private void LeftArrow_Click(object sender, RoutedEventArgs e)
        {
            double test = gcw.Left -= Interval.Value;
            if(test > 0)
                gcw.Left -= Interval.Value;
        }

        private void MoveDown(object sender, RoutedEventArgs e)
        {
            gcw.Top += Interval.Value;
        }

        private void BtnStart_Unloaded(object sender, RoutedEventArgs e)
        {
            gcw.Close();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if((gcw.Top - Interval.Value) > 0)
                gcw.Top -= Interval.Value;
        }
    }
}
