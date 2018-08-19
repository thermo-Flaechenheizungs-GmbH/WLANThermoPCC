using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp
{
    class Logger
    {
        
        public string Path { get; set; }
        public Logger(String root)
        {
            Path = root + Logger.GetTimestamp(DateTime.Now) + "log.txt";
            if (!Directory.Exists(root)) {
                Directory.CreateDirectory(root);
            }
        }
        public void Log(String lines)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@Path, true);
            file.WriteLine(GetTimestamp(DateTime.Now) +" ; "+lines);
            file.Close();
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmss");
        }
    }
}
