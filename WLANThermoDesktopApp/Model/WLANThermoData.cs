using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLANThermoDesktopApp.Model;

namespace WLANThermoDesktopApp.Model
{
    class WLANThermoData 
    {
        public System system { get; set; }
        public List<Channel> channel { get; set; }
        public Pitmaster pitmaster { get; set; }

    }
}
