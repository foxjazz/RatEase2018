using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace RatEaseW
{
    public class ColorData
    {
        private List<string> list;

        public ColorData()
        {
            list = new List<string>();
        }
        public void setD(byte r, byte g, byte b, int row)
        {
            string d = $"({r}) ({g}) ({b})";
            list.Add(d);
        }

        public List<string> get()
        {
            return list;
        }
        public void clear()
        {
            list.Clear();
        }
        
    }
}
