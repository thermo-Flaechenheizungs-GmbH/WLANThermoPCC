using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp.Model
{
    class Pitmaster
    {
        public int id { get; set;  }
        public int channel { get; set; }
        public int pid { get; set; }
        public int value { get; set; }
        public float set { get; set; }
        public string typ { get; set; }
        public string set_color { get; set; }
        public string value_color { get; set; }
    }
}
