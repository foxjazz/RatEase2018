using System;

using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Specialized;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Diagnostics;

namespace RatEaseW
{
    public class DiscordSend
    {
        string _hook;
        WebClient wc;
        
        public string host { get; set; }
        public string un { get; set; }
        public string pw { get; set; }
        private string lastFilename;
        public string urlpic { get; set; }
        public DiscordSend(string hook)
        {
            _hook = hook;
            wc = new WebClient();
            lastFilename = "";
        }

        public void SendMessage(string system)
        {
            var nvc = new NameValueCollection();
            nvc.Add("username", "Intel");
            nvc.Add("content", $"Reds / Neuts reported in {system}");
            wc.UploadValues(_hook, nvc);
        }
        public void SendImage(Bitmap image)
        {
            var stream = new MemoryStream();
            // bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            image.Save(stream, ImageFormat.Bmp);
            stream.Flush();
            string filename = DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".png";
            string currentFolder = Directory.GetCurrentDirectory();
            string fully = currentFolder + "\\" + filename;
            image.Save(fully, ImageFormat.Png);
            // long len = stream.Length;
            if (filename == lastFilename)
                return;

            lastFilename = filename;
            
            using (var client = new SftpClient(host, 22, un, pw))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    //    client.BufferSize = (uint)stream.Length; // bypass Payload error large files
                    //client.BufferSize = 4 * 1024;
                    //client.UploadFile(stream, filename);
                    using (var fileStream = new FileStream(fully, FileMode.Open))
                    {

                        client.BufferSize = 4 * 1024; // bypass Payload error large files
                        client.UploadFile(stream, filename);
                        client.UploadFile(fileStream, Path.GetFileName(fully));

                    }
                }
                else
                {
                    Debug.WriteLine("I couldn't connect");
                }
            }
            File.Delete(fully);
           
            var nvc = new NameValueCollection();
            nvc.Add("username", "Intel");
            nvc.Add($"content", urlpic + filename);
            
            wc.UploadValues(_hook, nvc);

        }

    }
    
}

