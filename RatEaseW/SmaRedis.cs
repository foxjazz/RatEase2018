using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using StackExchange.Redis;
using System.Drawing;
using Newtonsoft.Json;

namespace RatEaseW
{
    public class SmaRedis
    {
        ConnectionMultiplexer redisconn;
        public ISubscriber sub;

        public SmaRedis()
        {
            redisconn = ConnectionMultiplexer.Connect(Properties.Settings.Default.RedisConn);

            sub = redisconn.GetSubscriber();

            linfo = new List<Info>();
            ts15 = new TimeSpan(0, -15, 0);
        }
        public void init(string txt)
        {
            var db = redisconn.GetDatabase(1);
            db.StringSet(txt, txt + " " + DateTime.Now);
        }
        TimeSpan ts15;
        public List<Info> linfo;
        public List<Info> ListInfo { get; set; }
        public bool Connected { get { return redisconn.IsConnected; } }

        private void refreshInfo(string json)
        {
            //first translate message to object from json
            Message msg = JsonConvert.DeserializeObject<Message>(json);
            //First clear list

            for (int i = 0; i < linfo.Count; i++)
            {
                if (linfo[i].time < DateTime.Now.Add(ts15))
                {
                    linfo.RemoveAt(i);
                    i--;
                }
            }
            linfo.Add(new Info { time = DateTime.Now, Msg = msg });

            ListInfo = linfo;

        }

        public List<Info> ReadList()
        {
            var ep = redisconn.GetEndPoints(true);
            var server = redisconn.GetServer(ep.FirstOrDefault());
            foreach (var key in server.Keys())
            {
                linfo.Add(new Info { key = key, time = DateTime.Now });
            }
            return linfo;
        }
        public void Subscribe(string s, CurrentData cdata)
        {
            sub.Subscribe(s, (chanel, msg) =>
            {
                refreshInfo(msg);
                //EventArgs e = new EventArgs();
                //mainForm.MainFormAddDelegate.Invoke(this, e);
            });

        }
        public void publish(Message msg)
        {

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
            sub.Publish("fadeData", json);

        }

        public void publish(string txt)
        {
            sub.Publish("fade", txt);
            var db = redisconn.GetDatabase(1);
            db.StringSet(txt, txt);


        }


        //IDatabase cache = connection.GetDatabase();
        //cache.StringSet("key1", "value");
        //cache.StringSet("key2", 25);
    }

}
