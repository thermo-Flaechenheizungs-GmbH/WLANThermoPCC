using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp.Model
{
    class SystemSettings
    {
        public long time { get; set; }
        public string ap { get; set; }
        public string host { get; set; }
        public string language { get; set; }
        public char unit { get; set; }
        public string version { get; set; }
        public string getupdate { get; set; }
        public bool autoupd { get; set; }
        public string hwversion { get; set; }
        public bool god { get; set; }
    }
}
