using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Timers;
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

namespace WLANThermoDesktopApp.ViewModel
{
    class MainViewModel : ObservableObject
        {


        private static readonly HttpClient _client = new HttpClient();
        private string _ip = "192.168.0.105";
        private bool _thermometerConnected = false;
        private Timer _timer;
        private WLANThermoData _thermoData;
        private WLANThermoSettings _thermoSettings;
        private float _temp = 12.9f;
        private readonly int _timerIntervall = 5000; 


        #region Properties
        public string IP
        {
            get {
                return _ip;
            }
            set {
                if (String.IsNullOrEmpty(value)) {
                    MessageBox.Show("IP-Address invalid!");
                }
                else {
                    _ip = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Temp
        {
            get {
                return _temp.ToString();
            }
            set {
                _temp = float.Parse(value);
                OnPropertyChanged();
            }
        }
        #endregion Properties

        public MainViewModel()
        {
            _client.Timeout = TimeSpan.FromSeconds(5);
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = _timerIntervall;


            //ConnectThermometer();
            //getData();
            //getSettings();

        }

        public void ConnectThermometer()
        {
            ConnectThermometerAsync();
        }
        public async void ConnectThermometerAsync()
         {
            try {
                var temp = await getData("/");
                if (!String.IsNullOrEmpty(temp)) {
                    _thermometerConnected = true;
                    MessageBox.Show("Connection established!");
                    getThermoData().Wait();
                    getSettings().Wait();
                    _timer.Enabled = true;

                }
            }
            catch (OperationCanceledException e) {
                Console.WriteLine(e.Message);
                MessageBox.Show("Connection timed out! Check IP-Address!");
            }
        }

        public void DisconnectThermometer()
        {
            _thermometerConnected = false;
        }
        public async Task getThermoData()
        {   
            var jsonString = await getData("/data");
            _thermoData = JsonConvert.DeserializeObject<WLANThermoData>(jsonString);
        }
        public async Task getSettings()
        {  
            var jsonString = await getData("/settings");
            _thermoSettings = JsonConvert.DeserializeObject<WLANThermoSettings>(jsonString);
        }
        public async Task<String> getData(string service)
        {
            var response = await _client.GetStringAsync("http://" + _ip + service);
            return response;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _timer.Stop();
            getThermoData().Wait();
            getSettings().Wait();
            var channel = _thermoData.channel.Find(x => x.number == _thermoData.pitmaster.channel);
            _temp = channel.temp;
            Temp = _temp.ToString();
            _timer.Start();
        }

        #region EventHandlers
        public ICommand ConnectedThermometerClicked
        {
            get {
                if (_thermometerConnected) {
                    return new DelegateCommand(DisconnectThermometer);
                }
                else {
                    return new DelegateCommand(ConnectThermometer);
                }
            }
        }
        #endregion
    }
}
