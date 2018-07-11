using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp.Model
{
    class Channel
    {
        public int number { get; set; }
        public string name { get; set; }
        public int typ { get; set; }
        public float temp { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public bool alarm { get; set; }
        public string color { get; set; }
    }
}
