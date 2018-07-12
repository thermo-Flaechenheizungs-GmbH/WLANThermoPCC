using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WLANThermoDesktopApp.Model;

namespace WLANThermoDesktopApp
{
    
    public partial class MainWindow : Window
    {
        private static readonly HttpClient _client = new HttpClient();
        private static string _ip = "";
        private static bool _thermometerConnected = false;

        public MainWindow()
        {
            _ip = "192.168.0.105"; 
            _client.Timeout = TimeSpan.FromSeconds(5);
            InitializeComponent();
            ConnectThermometer();
            //getData();
            getSettings();
            
        }            

        public static async Task ConnectThermometer()
        {
            try {
                if (await testConnection()) {
                    _thermometerConnected = true;
                    Console.WriteLine("Connection established!");
                }
            }
            catch(OperationCanceledException e) {
                Console.WriteLine(e.Message);
                Console.WriteLine("Connection timed out!");
                Console.WriteLine("Check IP-Address!");
            }
        }

        public static void  DisconnectThermometer()
        {
            _thermometerConnected = false;
        }
        
            
        public static async Task<Boolean> testConnection()
        {

            var response = await _client.GetStringAsync("http://" + _ip + "/");

            return !String.IsNullOrEmpty(response);


        }
        public static async Task getData()
        {
            var jsonString = await getJSONData("/data");
            WLANThermoData json =  JsonConvert.DeserializeObject<WLANThermoData>(jsonString);

            Console.WriteLine("DATA: " + json);
        }
        public static async Task getSettings()
        {
            var jsonString = await getJSONData("/settings");
            WLANThermoSettings json = JsonConvert.DeserializeObject<WLANThermoSettings>(jsonString);

            Console.WriteLine("Settings: " + json);
        }
        public static async Task<String> getJSONData(string service)
        {
            var response =  await _client.GetStringAsync("http://" + _ip + service );
            return  response; 
        }

        
        
        
      
    }
}
