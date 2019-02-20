using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp.Model
{
    class IoT
    {
        public string PMQhost { get; set; }
        public int PMQport { get; set; }
        public string PMQuser { get; set; }
        public string PMQpass { get; set; }
        public int PMQqos { get; set; }
        public bool PMQon { get; set; }
        public int PMQint { get; set; }
        public bool CLon { get; set; }
        public string CLtoken { get; set; }
        public int CLint { get; set; }
    }
}
