using System;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Text;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Win32;
using Brush = System.Drawing.Brush;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace RatEaseW
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            gcwShowing = false;
            state = false;
            DataContext = CurrentData.Instance;
            dtimer = new DispatcherTimer();
            dtimer.Interval = new TimeSpan(0,0,0,0,250);
            dtimer.Tick += Dtimer_Tick;
            secondText = new DispatcherTimer();
            secondText.Interval = new TimeSpan(0,0,0,2);
            colorTextOn = false;
            secondText.Tick += secondText_Tick;
            secondText.Start();
            
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
            if (left < 0) {left = 120;}
            if (top < 0) { top = 420; }

            if (width < 0)
            {
                width = 5;
            }

            if (height < 0)
            {
                height = 100;

            }
            outFolder.Text =Properties.Settings.Default.outFolder;
            discordHook.Text = Properties.Settings.Default.discord;
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
            keyDownTimer = DateTime.Now;
            copyWidth.Text = Properties.Settings.Default.cpyWidth;
            populateNotificationData();
            this.dcList = new ColorData();
            this.ColorList.BringIntoView();
            this.calibrate();
            
            
        }

        private ColorData dcList;
        private void populateNotificationData()
        {
            
            var d = Directory.GetCurrentDirectory();
            if (File.Exists($@"{d}\config\config.txt"))
            {
                StreamReader sr = new StreamReader(@"config\config.txt");
                while (true)
                {
                    var read = sr.ReadLine();
                    if (read == null)
                        break;
                    string[] strarray = read.Split(',');
                    switch (strarray[0]) {
                        case "sftphost":
                            sftphost.Text = strarray[1];
                            break;
                        case "sftpun":
                            sftpun.Text = strarray[1];
                            break;
                        case "sftppw":
                            sftppw.Password = strarray[1];
                            break;
                        case "baseurl":
                            urlpic.Text = strarray[1];
                            break;
                        case "webhook":
                            discordHook.Text = strarray[1];
                            break;
                    }
                }
                sr.Dispose();

            }
            else
            {
                sftphost.Text = Properties.Settings.Default.sftphost;
                sftppw.Password = Properties.Settings.Default.sftppw;
                sftpun.Text = Properties.Settings.Default.sftpun;
                urlpic.Text = Properties.Settings.Default.discordUrl;
            }
            
            
            setAbs();
        }
        DiscordSend ds;
        private bool isOutPathValid;
        private bool setRectangle;
        private Message msg;
        public GreenScreenW gcwLocal { get; set; }
        public GreenScreenW gcwSystem { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int top;
        public int left;

        private enum GreenScreen {Local, System};
        private DateTime keyDownTimer;
        private GreenScreen greenType;
        private int sWidth;
        private int sHeight;
        private int sTop;
        private int sLeft;
        public ScreenCapture sc { get; set; }
        FileDialog openFileDialog1;
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
        private int cpyWidth = 300;
        private int waitIterations;
        private int eveSystemBid;
        private int checkSystemCounter;
        private bool isSystemDifferent;

        private bool colorTextOn;

        private bool state;
        //public System.Drawing.Point pTopleft { get; set; }
        //public System.Drawing.Point pBottomRight { get; set; }

        private void secondText_Tick(object sender, EventArgs e)
        {
            colorTextOn = true;
            
            state = showGreenScreen(!state);
        }
        public int iteration { get; set; }

        private void calibrate()
        {
            int xx;
            var sp = new System.Drawing.Point(left, top);
            if (left < 50)
            {
                xx = 0;
            }
            else
            {
                xx = left - 50;
            }

            var dp = new System.Drawing.Point(left + width, top + height);
            curBitmap = (Bitmap)sc.Capture(sp, dp);
            for (int x = xx; x < width + 50; x++)
            {
                for (int y = 0; y < 200; y++)
                {
                    var pixel = curBitmap.GetPixel(x, y);
                    if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {
                        pixel = curBitmap.GetPixel(x, y + 1);
                        if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                        {
                            left += y;
                            return;
                        }
                    }
                }
            }
            
        }
        private void Dtimer_Tick(object sender, EventArgs e)
        {
            dtimer.Stop();
            if (keyDownTimer.AddMinutes(10) < DateTime.Now)
            {
               
            }
            //if (checkSystemCounter > 6)
            //{
            //    isSystemDifferent = CheckEveSystem();
            //    checkSystemCounter = 0;  // only want to check every few seconds.
            //    if (isSystemDifferent)
            //        checkSystemCounter = -20;  //set it back to allow time to change again.
            //}
            //checkSystemCounter++;
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
                colorTextOn = false;
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
            cnt = 0;
            iteration = 0;
            RedCheck = 0;
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
        private int RedCheck;
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
            RedCheck++;
            Bitmap bmap;
            if (listRedTopHeight == null)
                listRedTopHeight = new List<RedTopHeight>();
            if(RedStartList == null)
                RedStartList = new List<int>();
            bool inRed = false;
            RedCount = 0;
            fRed = 0;
            int redHeight = 7;
            int ySectionStart = 0;
            var sp = new System.Drawing.Point(left, top);
            var dp = new System.Drawing.Point(left + width, top + height);
            curBitmap = (Bitmap)sc.Capture(sp, dp);
            if (RedCheck == 1 || (RedCheck % 50 == 0))
                StickImage.Source = ImageHelper.ImageSourceForBitmap(curBitmap);
     
                RedStartList.Clear();
           

            int xRedPosition = -1;

            int capW, capY;
            capW = 100;
            capY = 18;
            var d = Directory.GetCurrentDirectory();
            if (colorTextOn)
            {
                dcList.clear();
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = curBitmap.GetPixel(x, y);
                    if (colorTextOn && y < 300)
                    {   
                        dcList.setD(pixel.R, pixel.G, pixel.B, y);
                    }
                    populateCL();
                    if (pixel.R > red && pixel.B < 16 && pixel.G < 15) // defined as red.
                    {
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
                            string fname = outFolder.Text + "\\t" + RedCount + ".bmp";
                           
                            try
                            {
                                if (isOutPathValid)
                                {
                                    File.Delete(fname);
                                }
                            }
                            catch 
                            {
                            }
                            //scaleImage(capImage);
                            y += redHeight;
                        }
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
                    if(outFolder.Text != null && outFolder.Text.Length > 1)
                        File.CreateText(outFolder.Text + "\\newset").Dispose();
                    populateResult = true;
                    RedCheckStart = DateTime.Now;
                    return RedCount;
                }
            }

            
            return 0;
        }
        #region Bitmap_stuff

        private void populateCL()
        {
            var data = dcList.get();
            StringBuilder sb = new StringBuilder();
            foreach (var v in data)
            {
                sb.Append(v + "\r\n");
            }

            colorText.Text = sb.ToString();
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
     
        #endregion
        private bool isSettingRect;
        DispatcherTimer dtimer, secondText;

        System.Media.SoundPlayer player;
        public bool IsClear { get; set; }
        // SmaRedis subred;
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
                    copyForDiscord();
                    player.SoundLocation = Properties.Settings.Default.AlertSoundFile;

                }
                if (player.SoundLocation == null || player.SoundLocation.Length < 2)
                    return;
                if (!File.Exists(player.SoundLocation))
                    return;
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
        private void copyForDiscord()
        {
            // Send to discord
            try
            {
                var capImage = (Bitmap)sc.Capture(new Point(left, (top)), new Point(left + cpyWidth, top + height));
                // var img2 = GetImage(capImage);
                ds.SendMessage(esystem.Text);
                if (sftphost.Text.Length > 4 && sftppw.Password.Length > 4)
                {
                    ds.pw = sftppw.Password;
                    ds.host = sftphost.Text;
                    ds.un = sftpun.Text;
                    ds.urlpic = urlpic.Text;
                    ds.SendImage(capImage);
                }

            }
            catch
            {

            }
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            ds = new DiscordSend(discordHook.Text);
            populateResult = true;
            if (width > 3)
            {
                width = 3;
            }
                Properties.Settings.Default.left = left;
                Properties.Settings.Default.top = top;
                Properties.Settings.Default.width = width;
                Properties.Settings.Default.height = height;
                Properties.Settings.Default.discord = discordHook.Text;
                Properties.Settings.Default.sftphost = sftphost.Text;
            Properties.Settings.Default.sftppw = sftppw.Password;
            Properties.Settings.Default.sftpun = sftpun.Text;
            Properties.Settings.Default.discordUrl = urlpic.Text;


            int testWidth;  
                if (Int32.TryParse(copyWidth.Text, out testWidth))
                {
                    //Only save valid option to settings
                    cpyWidth = testWidth;
                    Properties.Settings.Default.cpyWidth = cpyWidth.ToString();
                }
                Properties.Settings.Default.Save();
       
            coord.Text = $"L:{(int)left}, T:{(int)top} W:{(int)width} H:{(int)height}";
            
            BtnStart.Content = "Started";
            if (gcwShowing)
            {
                gcwLocal.Close();
                gcwSystem.Close();   
            }
            GreenGrid.Visibility = Visibility.Collapsed;
            Status.Background = Brushes.LightSeaGreen;
            Status.Content = "Started";
            eveSystemBid = 0;
            dtimer.Start();
            width = 3;
            setAbs();

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
        private void setAbs()
        {
            
            VImage.Source = ImageHelper.ScreenShotImageSource(left, top, width, height);
            coord.Text = $"L:{(int)left}, T:{(int)top} W:{(int)width} H:{(int)height}";
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
                left = 150;
            if (top < 0)
                top = 120;
            if (width < 0)
                width = 4;
            if (height < 0)
                height = 150;
            width = 500;
            GreenControlMode.Content = "G Mode: Local";
            GreenGrid.Visibility = Visibility.Visible;
            Status.Background = Brushes.LightCoral;
            Status.Content = "Stopped";
            setAbs();

        }
        private void addWidth_Click(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                width += (int) Interval.Value;
            }
            else
            {
                gcwSystem.Width += (int)Interval.Value;
            }
            setAbs();
        }
        private void subHeigth_Click(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                height -= (int)Interval.Value;
            }
            else
            {
                gcwSystem.Height -= (int)Interval.Value;
            }
            setAbs();
        }
        private void extend(object sender, RoutedEventArgs e)
        {
            if (greenType == GreenScreen.Local)
            {
                height += (int)Interval.Value;
            }
            else
            {
                gcwSystem.Height += Interval.Value;
            }
            setAbs();
        }

        private void Interval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Interval.Value = (int) e.NewValue;
            txtInterval.Text = ((int) e.NewValue).ToString();
        }
        private void BtnStart_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                gcwLocal.Close();
            }
            catch { }
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
            if (outFolder.Text == null || outFolder.Text.Length < 2)
                return;
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
            try
            {
                string sysfile = outFolder.Text + "\\system.txt";
                redline.Text = readText(sysfile).Replace("\n", "") + ": ";
                var list = Directory.GetFiles(outFolder.Text, "t*.txt");

                foreach (var file in list)
                {
                    try
                    {
                        waitASecond = true;
                        data = readText(file).Replace("\n", "");

                        if (data.Length > 0)
                            redline.Text += data + "; ";
                    }
                    catch
                    {
                    }

                }
                redline.Text += " nv";
                waitASecond = false;
            }
            catch { }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dtimer.Stop();
            gcwSystem.Close();
            //gcwLocal.Close();
            //gcwLocal = null;
            gcwSystem = null;
        }



        private void CopyWidth_TextInput(object sender, TextCompositionEventArgs e)
        {
            Console.WriteLine(e.Text);
            int i;
            if (!Int32.TryParse((string)e.Source, out i))
            {
                copyWidth.Text = "300"; //Default width of image to send to discord
            }

                
        }

        private void Testdiscord_Click(object sender, RoutedEventArgs e)
        {
            var capImage = (Bitmap)sc.Capture(new System.Drawing.Point(left, (top)), new System.Drawing.Point(left + cpyWidth, top + height));
            if (ds == null)
                ds = new DiscordSend(discordHook.Text);
            if (sftphost.Text.Length > 4 && sftppw.Password.Length > 4)
            {
                ds.pw = sftppw.Password;
                ds.host = sftphost.Text;
                ds.un = sftpun.Text;
                ds.urlpic = urlpic.Text;
                ds.SendImage(capImage);
            }
            ds.SendMessage("testing discord message");
        }

        private void RedCount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private bool showGreenScreen(bool show)
        {
            gcwLocal.Left = 0;
            gcwLocal.Top = 0;
            gcwLocal.Width = SystemParameters.PrimaryScreenWidth;
            gcwLocal.Height = SystemParameters.PrimaryScreenHeight;
            if (show)
            {
                gcwLocal.Show();
                gcwLocal.BringIntoView();

            }
            else
            {
                gcwLocal.Hide();
            }

            return show;
        }
        private Rectangle Bounds;
        private void WindowScreenshotWithoutClass(String filename)
        {
            
            System.Drawing.Rectangle bounds = new System.Drawing.Rectangle((int)gcwLocal.Left, (int) gcwLocal.Top, (int) gcwLocal.Width,(int) gcwLocal.Height);
            left = bounds.Left;
            top = bounds.Top;
            width = bounds.Width;
            height = bounds.Height;


            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    
                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }


                if (File.Exists(filename))
                        File.Delete(filename);
                    bitmap.Save(filename, ImageFormat.Bmp);
            }
        }

        private bool inMove;
        private System.Windows.Point firstPosition;
        private System.Windows.Point lastPosition;
        private int leftStart;
        private int topStart;
        private void VImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!inMove) return;
            lastPosition = ImageHelper.GetMousePosition();
            double x = firstPosition.X - lastPosition.X;
            double y = firstPosition.Y - lastPosition.Y;
            left = leftStart + (int) x;
            top = topStart + (int) y;
            setAbs();
        }

        private void VImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            firstPosition = ImageHelper.GetMousePosition();
            inMove = true;
            leftStart = left;
            topStart = top;
        }

        private void VImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void VImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            inMove = false;
        }

        private void Reset_Width_Click(object sender, RoutedEventArgs e)
        {
            width = 4;
            setAbs();
        }

        private void VImage_MouseLeave(object sender, MouseEventArgs e)
        {
            inMove = false;
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
