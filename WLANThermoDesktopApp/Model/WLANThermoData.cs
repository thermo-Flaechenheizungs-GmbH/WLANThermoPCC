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
        public SystemData system { get; set; }
        public List<Channel> channel { get; set; }
        public PitmasterCollection pitmaster { get; set; }

    }
}
