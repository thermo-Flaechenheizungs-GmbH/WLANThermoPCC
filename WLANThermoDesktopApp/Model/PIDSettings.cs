using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp.Model
{
    class PIDSettings
    {
        public string name { get; set; }
        public int id { get; set; }
        public int aktor { get; set; }
        public float Kp { get; set; }
        public float Ki { get; set; }
        public float Kd { get; set; }
        public float Kp_a { get; set; }
        public float Ki_a { get; set; }
        public float Kd_a { get; set; }
        public float DCmmin { get; set; }
        public float DCmmax { get; set; }
    }
}
