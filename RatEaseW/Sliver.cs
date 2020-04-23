using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatEaseW
{
    [Serializable]
    public class Sliver
    {
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool calibrated { get; set; }
        public bool hasRed { get; set; }
        public string alertSoundFilename { get; set; }
        public int redsPrev { get; set; }
        public int reds { get; set; }

    }
}
