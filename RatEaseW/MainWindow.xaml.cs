using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
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
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Brush = System.Drawing.Brush;
using Brushes = System.Windows.Media.Brushes;
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
        
        public MainWindow()
        {
            InitializeComponent();
            gcwShowing = false;
            DataContext = CurrentData.Instance;
            dtimer = new DispatcherTimer();
            dtimer.Interval = new TimeSpan(0,0,0,0,250);
            dtimer.Tick += Dtimer_Tick;

            isSettingRect = false;
            msg = CurrentData.Instance.RedData;
            openFileDialog1 = new OpenFileDialog();
            player = new System.Media.SoundPlayer();
            gcwLocal = new GreenScreenW();
            gcwSystem = new GreenScreenW();
            //VRec = new System.Drawing.Rectangle();
            left = Properties.Settings.Default.left;
            top = Properties.Settings.Default.top;
            width = Properties.Settings.Default.width;
            height = Properties.Settings.Default.height;
            outFolder.Text =Properties.Settings.Default.outFolder;
            //resultFolder.Text = Properties.Settings.Default.resultFolder;

            //System region Setting
            sLeft = Properties.Settings.Default.sLeft;
            sTop = Properties.Settings.Default.sTop;
            sWidth = Properties.Settings.Default.sWidth;
            sHeight = Properties.Settings.Default.sHeight;

            sc = new ScreenCapture();
            RedInSystem = false;
            reds = 0;
            populateResult = true;
            RedCheckStart = DateTime.Now.AddSeconds(-5);
            waitASecond = false;
            waitIterations = 0;
            setRectangle = true;
            GreenGrid.Visibility = Visibility.Visible;
            checkSystemCounter = 0;
            
            isOutPathValid = TestOutPath();
        }

        private bool isOutPathValid;
        public GreenScreenW gcwLocal { get; set; }
        public GreenScreenW gcwSystem { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int top;
        public int left;


        private enum GreenScreen {Local, System};

        private GreenScreen greenType;
        private int sWidth;
        private int sHeight;
        private int sTop;
        private int sLeft;

        public ScreenCapture sc { get; set; }
        
        Message msg;
        FileDialog openFileDialog1;

        private bool setRectangle;
        byte red = 130;
        private int redV;
        private List<int> RedStartList;
        private List<RedTopHeight> listRedTopHeight;
        public int cnt { get; set; }
        private bool RedInSystem;
        private bool gcwShowing;
        private bool waitASecond;
        private int reds;
        private int redsPrev;
        private DateTime RedCheckStart;
        private bool populateResult;

        private int waitIterations;
        
        private int eveSystemBid;

        private int checkSystemCounter;

        private bool isSystemDifferent;
        //public System.Drawing.Point pTopleft { get; set; }
        //public System.Drawing.Point pBottomRight { get; set; }
        public int iteration { get; set; }
        private void Dtimer_Tick(object sender, EventArgs e)
        {
            dtimer.Stop();
            if (checkSystemCounter > 6)
            {
                isSystemDifferent = CheckEveSystem();
                checkSystemCounter = 0;  // only want to check every few seconds.
                if (isSystemDifferent)
                    checkSystemCounter = -20;  //set it back to allow time to change again.
            }
            checkSystemCounter++;
            if (waitIterations > 0)
            {
                waitIterations--;
                dtimer.Stop();
            }
            if (waitASecond)
            {
                waitIterations = 5;
                waitASecond = false;

            }

            if (populateResult)
            {
                populateRedLine();
                populateResult = false;
            }
            if ((DateTime.Now - RedCheckStart).TotalSeconds > 6)
            {
                
                reds = CheckRed();
            }
            if (reds != redsPrev)
            {
                redCount.Text = "Reds:" + reds;
            }
            redsPrev = reds;
            if (reds > 0 && RedInSystem == false)
            {
                RedInSystem = true;
                RedCheckStart = DateTime.Now;
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
        private System.Drawing.Bitmap bm2;
        private double fRed;
        public bool foundRed { get; set; }

        private bool CheckEveSystem()
        {
            if (curImage == null)
                return false;
            var sp = new System.Drawing.Point();
            var dp = new System.Drawing.Point();
            sp.X = sLeft;
            sp.Y = sTop;
            dp.X = sWidth + sLeft;
            dp.Y = sHeight + sTop;
            curImage = sc.Capture(sp, dp);
            int currentEveSystemBid = 0;
            curBitmap = (Bitmap)curImage;

            for (int x = 0; x < sWidth; x++)
            {
                for (int y = 0; y < sHeight; y++)
                {
                    var pixel = curBitmap.GetPixel(x, y);
                    if (pixel.R == 0xFF && pixel.B == 0xFF && pixel.G == 0xFF)
                    {
                        currentEveSystemBid++;
                    }
                }
            }
            if (eveSystemBid != currentEveSystemBid)
            {
                eveSystemBid = currentEveSystemBid;
                if (isOutPathValid)
                    SaveEveSystemBMP();  //If EveSystem bitmap white bits are different, we are likely in a new system.
                
                return true;
            }
            return false;
        }
        private int CheckRed()
        {
            if (listRedTopHeight == null)
                listRedTopHeight = new List<RedTopHeight>();
            RedStartList = new List<int>();
            bool inRed = false;
            RedCount = 0;
            fRed = 0;
            int redHeight = 7;
            int ySectionStart = 0;
            //listRedTopHeight.Clear();
            var sp = new System.Drawing.Point(left, top);
            var dp = new System.Drawing.Point(left + width, top + height);
            
            curImage = sc.Capture(sp, dp);
            
            curBitmap = (Bitmap) curImage;
            //var img = GetImage(curBitmap);
            //images.Children.Clear();
            //images.Children.Add(img);
            RedStartList.Clear();
            int xRedPosition = -1;

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

                            xRedPosition = x;
                            if (xRedPosition > 0)
                            { //This will set the position of starting red
                                left = left + x;
                                width = 1;
                            }
                            // The - one on the y axis if to allow font kerning to get the whole name;
                            var capImage = (Bitmap) sc.Capture(new System.Drawing.Point(left + x + 5, (top + y) - 2), new System.Drawing.Point(capW + left + x,capY + top + y));
                            //var bitmap = pushBitmap(left, top, ref curBitmap);
                            var img2 = GetImage(capImage);
                            //images.Children.Add(img2);
                            Bitmap resized = new Bitmap(capImage, new System.Drawing.Size(capImage.Width * 4, capImage.Height * 4));
                            string fname = outFolder.Text + "\\t" + RedCount + ".bmp";
                           
                            try
                            {
                                if (isOutPathValid)
                                {
                                    File.Delete(fname);
                                    resized.Save(fname, ImageFormat.Bmp);
                                }
                            }
                            catch 
                            {
                            }
                            scaleImage(capImage);
                            y += redHeight;
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
                    File.CreateText(outFolder.Text + "\\newset").Dispose();
                    populateResult = true;
                    RedCheckStart = DateTime.Now;
                    return RedCount;
                }
            }
            return 0;
        }

        private void scaleImage(Bitmap image)
        {
            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            // uncomment for higher quality output
            //graph.InterpolationMode = InterpolationMode.High;
            //graph.CompositingQuality = CompositingQuality.HighQuality;
            //graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(image.Width * 4);
            var scaleHeight = (int)(image.Height * 4);
            var brush = new SolidBrush(System.Drawing.Color.Black);

            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);
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

        private bool isSettingRect;
        DispatcherTimer dtimer, rtimer;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " could not find sound file:" + Properties.Settings.Default.AlertSoundFile);
            }

        }

     
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            populateResult = true;
            if (gcwLocal.Width > 4)
            {
                gcwLocal.Width = 4;
            }
                
                height = (int) gcwLocal.Height;
                width = (int) gcwLocal.Width;
                top = (int) gcwLocal.Top;
                left = (int) gcwLocal.Left;
                Properties.Settings.Default.left = left;
                Properties.Settings.Default.top = top;
                Properties.Settings.Default.width = width;
                Properties.Settings.Default.height = height;
                Properties.Settings.Default.Save();
       
            coord.Text = $"L:{(int)left}, T:{(int)top} W:{(int)width} H:{(int)height}";
            
            BtnStart.Content = "Started";
            if (gcwShowing)
            {
                gcwLocal.Hide();
                gcwSystem.Hide();   
            }
            GreenGrid.Visibility = Visibility.Collapsed;
            Status.Background = Brushes.LightSeaGreen;
            Status.Content = "Started";
            eveSystemBid = 0;
            dtimer.Start();

        }

        private void SetAlertSound_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog1.Title = "Pick sound alert file";
            if (Properties.Settings.Default.AlertSoundFile.Length > 0)
            {
                string f = Properties.Settings.Default.AlertSoundFile;
                openFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(f);
            }

            openFileDialog1.ShowDialog();
            Properties.Settings.Default.AlertSoundFile = openFileDialog1.FileName;
            Properties.Settings.Default.Save();
            player.SoundLocation = Properties.Settings.Default.AlertSoundFile;
            player.Load();
            player.Play();
        }

        private void SetClearSound_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog1.Title = "Pick sound all clear file";
            if (Properties.Settings.Default.AllClearSoundFile.Length > 0)
            {
                string f = Properties.Settings.Default.AllClearSoundFile;
                openFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(f);
            }
            openFileDialog1.ShowDialog();
            Properties.Settings.Default.AllClearSoundFile = openFileDialog1.FileName;
            Properties.Settings.Default.Save();
            player.SoundLocation = Properties.Settings.Default.AllClearSoundFile;
            player.Load();
            player.Play();
        }

        private void SetRectangle(object sender, RoutedEventArgs e)
        {
            dtimer.Stop();
            BtnStart.Content = "Start";
            isSettingRect = true;
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
            if (width > 10)
                width = 4;
            gcwLocal.Top = top;
            gcwLocal.Left = left;
            gcwLocal.Width = width;
            gcwLocal.Height = height;
            gcwShowing = true;
            gcwLocal.Show();

            sLeft = Properties.Settings.Default.sLeft;
            sTop = Properties.Settings.Default.sTop;
            sWidth = Properties.Settings.Default.sWidth;
            sHeight = Properties.Settings.Default.sHeight;
            if (sLeft < 0)
                sLeft = 40;
            if (sHeight < 5 || sHeight > 100)
                sHeight = 20;
            if (sWidth < 20)
                sWidth = 60;
            if (sTop< 5)
                sTop = 10;

            gcwSystem.Top = sTop;
            gcwSystem.Left = sLeft;
            gcwSystem.Width = sWidth;
            gcwSystem.Height = sHeight;
            
            gcwSystem.Show();
            greenType = GreenScreen.Local;
            GreenControlMode.Content = "G Mode: Local";
            GreenGrid.Visibility = Visibility.Visible;
            Status.Background = Brushes.LightCoral;
            Status.Content = "Stopped";

        }

       

       

        private void thinner(object sender, RoutedEventArgs e)
        {
            int test=0;

            if (greenType == GreenScreen.Local)
            {
                if (gcwLocal.Width > 1)
                    test = (int) gcwLocal.Width - (int) Interval.Value;
                if (test > 0)
                    gcwLocal.Width = test;
            }
            else
            {
                if (gcwSystem.Width > 1)
                    test = (int)gcwSystem.Width - (int)Interval.Value;
                if (test > 0)
                    gcwSystem.Width = test;
            }

        }
        private void addWidth_Click(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                gcwLocal.Width += (int) Interval.Value;
            }
            else
            {
                gcwSystem.Width += (int)Interval.Value;
            }
        }
        private void subHeigth_Click(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                gcwLocal.Height -= (int)Interval.Value;
            }
            else
            {
                gcwSystem.Height -= (int)Interval.Value;
            }
        }
        private void MoveRight(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                gcwLocal.Left += Interval.Value;
            }
            else
            {
                gcwSystem.Left += Interval.Value;
            }
        }

      

        private void extend(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                gcwLocal.Height += Interval.Value;
            }
            else
            {
                gcwSystem.Height += Interval.Value;
            }
        }

        private void Interval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Interval.Value = (int) e.NewValue;
            txtInterval.Text = ((int) e.NewValue).ToString();
        }

        private void LeftArrow_Click(object sender, RoutedEventArgs e)
        {
            
            if (greenType == GreenScreen.Local)
            {
                double test = gcwLocal.Left -= Interval.Value;
                if (test > 0)
                    gcwLocal.Left -= Interval.Value;
            }
            else
            {
                double test = gcwSystem.Left -= Interval.Value;
                if (test > 0)
                    gcwSystem.Left -= Interval.Value;
            }
           
            
        }

        private void MoveDown(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                gcwLocal.Top += Interval.Value;
            }
            else
            {
                gcwSystem.Top += Interval.Value;
            }
        }

        private void BtnStart_Unloaded(object sender, RoutedEventArgs e)
        {
            gcwLocal.Close();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                if ((gcwLocal.Top - Interval.Value) > 0)
                    gcwLocal.Top -= Interval.Value;
            }
            else
            {
                if ((gcwSystem.Top - Interval.Value) > 0)
                    gcwSystem.Top -= Interval.Value;
            }
        }

        private void pickOutput_Click(object sender, RoutedEventArgs e)
        {
            
         

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Set outFolder";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = outFolder.Text;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = outFolder.Text;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                outFolder.Text = dlg.FileName;
                // Do something with selected folder string
            }
            Properties.Settings.Default.outFolder = outFolder.Text;
            Properties.Settings.Default.Save();
            dlg.Dispose();

        }


        private void SaveEveSystemBMP()
        {
            sHeight = (int)gcwSystem.Height;
            sWidth = (int)gcwSystem.Width;
            sTop = (int)gcwSystem.Top;
            sLeft = (int)gcwSystem.Left;
            Properties.Settings.Default.sLeft = sLeft;
            Properties.Settings.Default.sTop = sTop;
            Properties.Settings.Default.sWidth = sWidth;
            Properties.Settings.Default.sHeight = sHeight;
            Properties.Settings.Default.Save();
            var sp = new System.Drawing.Point(sLeft, sTop);
            var dp = new System.Drawing.Point(sLeft + sWidth, sTop + sHeight);

            curImage = sc.Capture(sp, dp);

            curBitmap = (Bitmap)curImage;
            Bitmap resized = new Bitmap(curBitmap, new System.Drawing.Size(curBitmap.Width * 2, curBitmap.Height * 2));
            string fname = outFolder.Text + "\\system.bmp";
            if(isOutPathValid)
                resized.Save(fname);
            resized.Dispose();
            if (gcwShowing)
            {
                gcwLocal.Hide();
                gcwSystem.Hide();
            }
            if (File.Exists(outFolder.Text + "\\system.txt"))
                File.Delete(outFolder.Text + "\\system.txt");
        }

        private void FinishSystemRectanglee_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void SetSystemRectanglee_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            gcwLocal.Top = 50;
            gcwLocal.Left = 50;
            gcwLocal.Width = 80;
            gcwLocal.Height = 8;
            gcwShowing = true;
            gcwLocal.Show();
        }

        private string readText(string fn)
        {
            string data = "";
            if (File.Exists(fn))
            {
                var fs = new FileStream(fn, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);
                data = sr.ReadToEnd();
                fs.Dispose();
                return data;
            }
            else
            {
                return data;
            }
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(redline.Text);
        }

        private void GreenControlMode_Click(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                GreenControlMode.Content = "G Mode: System";
                greenType = GreenScreen.System;
            }
            else
            {
                GreenControlMode.Content = "G Mode: Local";
                greenType = GreenScreen.Local;
            }
        }

      

        private void populateRedLine()
        {
            string data;
            string sysfile = outFolder.Text + "\\system.txt";
            redline.Text = readText(sysfile).Replace("\n","") + ": ";
            var list = Directory.GetFiles(outFolder.Text, "t*.txt");

            foreach (var file in list)
            {
                try
                {
                    waitASecond = true;
                    data = readText(file).Replace("\n","");
                    
                    if(data.Length > 0)
                    redline.Text += data + "; ";
                }
                catch
                {
                }
                
            }
            redline.Text += " nv";
            waitASecond = false;
        }

        private bool TestOutPath()
        {
            try
            {
                string sysfile = outFolder.Text + "\\test.log";
                File.WriteAllText(sysfile, "test");
                File.Delete(sysfile);

            }
            catch
            {
                return false;
            }
            return true;
        }
       
    }
}
