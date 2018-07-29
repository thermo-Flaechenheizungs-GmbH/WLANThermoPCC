using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp
{
    class Logger
    {
        
        public string Path { get; set; }
        public void Log(String lines)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@Path, true);
            file.WriteLine(lines);
            file.Close();
        }
    }
}
