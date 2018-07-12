using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp.Model
{
    class IoT
    {
        public string TSwrite { get; set; }
        public string TShttp { get; set; }
        public string TSuser { get; set; }
        public string TSchID { get; set; }
        public bool TSshow8 { get; set; }
        public int TSint { get; set; }
        public bool TSon { get; set; } 
        public string PMQhost { get; set; }
        public int PMQport { get; set; }
        public string PMQuser { get; set; }
        public string PMQpass { get; set; }
        public int PMQqos { get; set; }
        public bool TGon { get; set; }
        public string TGtoken { get; set; }
        public string TGid { get; set; }
        public bool CLon { get; set; }
        public string CLtoken { get; set; }
        public int CLint { get; set; }
    }
}
