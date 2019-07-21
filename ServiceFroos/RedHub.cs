using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Drawing;
using System.IO;

namespace ServiceFroos
{
    public class RedHub : Hub
    {

        public void SendBmp(string user, BitMapMessage message)
        {
            //todo transform image to InfoMessage
            var myUniqueFileName = $@"{Guid.NewGuid()}";
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(Startup.path + "\\" + myUniqueFileName + ".nam"))
            {
                file.WriteLine(user);
            }
            message.eveSystem.Save(Startup.path + "\\" + myUniqueFileName + ".bmp");
            int i = 0;
            foreach (Bitmap bp in message.redPlayers)
            {
                i++;
                string fn = Startup.path + "\\" + myUniqueFileName + i + ".bmp";
                bp.Save(fn);
            }

            //write two files user and bmp for further processing.

            //await Clients.Caller.SendAsync("Acknowlege", user, message);
        }

        public Task SendMessageToCaller(InfoMessage message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "RatEase Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "RatEase Users");
            await base.OnDisconnectedAsync(exception);
        }
        public Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "RatEase Users" };
            return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        }


    }

    public class BitMapMessage
    {
        public Bitmap eveSystem { get; set; }
        public Bitmap[] redPlayers { get; set; }
        

    }

    public class InfoMessage
    {
        public string[] redPlayers { get; set; }
        public string eveSystem { get; set; }
    }
}
