using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp.Model
{
    class WLANThermoSettings
    {
        public SystemSettings system { get; set; }
        public List<string> sensors { get; set; }
        public List<PIDSettings> pid { get; set; }
        public List<string> aktor { get; set; }
        public IoT iot { get; set; }
        public List<string> hardware { get; set; }
        public API api { get; set; }
    }
}
