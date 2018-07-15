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
using System.Net.Http.Headers;
using System.Collections.ObjectModel;

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
        private List<string> _pidProfiles = new List<string>();
        private string _selectedPIDProfile;
        private float _kp;
        private float _ki;
        private float _kd;
        private float _kp_a;
        private float _ki_a;
        private float _kd_a;
        private int _dcmmin;
        private int _dcmmax;
        private ObservableCollection<PitmasterStep> _pitmasterSteps = new ObservableCollection<PitmasterStep>();
        private PitmasterStep _selectedPitmasterStep;


        #region Properties
        public PitmasterStep SelectedPitmasterStep
        {
            get {
                return _selectedPitmasterStep;
            }
            set {
                _selectedPitmasterStep = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<PitmasterStep> PitmasterSteps
        {
            get {
                return _pitmasterSteps;
            }
            set {
                _pitmasterSteps = value;
                OnPropertyChanged();
            }
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SelectedPIDProfile {
            get {
                return _selectedPIDProfile;
            }
            set {
                _selectedPIDProfile = value;
                //TODO: Check for a better call for this, or just if it gets the correct data.
                var temp = ThermoSettings.pid.ToArray()[PIDProfiles.IndexOf(SelectedPIDProfile)];
                Kp = temp.Kp;
                Ki = temp.Ki;
                Kd = temp.Kd;
                Kp_a = temp.Kp_a;
                Ki_a = temp.Ki_a;
                Kd_a = temp.Kd_a;
                DCmmin = temp.DCmmin;
                DCmmax = temp.DCmmax;
                OnPropertyChanged();
            }
        }
        public float Kp
        {
            get {
                return _kp;
            }
            set {
                _kp = value;
                OnPropertyChanged();
            }
        }
        public float Ki
        {
            get {
                return _ki;
            }
            set {
                _ki = value;
                OnPropertyChanged();
            }
        }
        public float Kd
        {
            get {
                return _kd;
            }
            set {
                _kd = value;
                OnPropertyChanged();
            }
        }
        public float Kp_a
        {
            get {
                return _kp_a;
            }
            set {
                _kp_a = value;
                OnPropertyChanged();
            }
        }
        public float Ki_a
        {
            get {
                return _ki_a;
            }
            set {
                _ki_a = value;
                OnPropertyChanged();
            }
        }
        public float Kd_a
        {
            get {
                return _kd_a;
            }
            set {
                _kd_a = value;
                OnPropertyChanged();
            }
        }
        public int DCmmin
        {
            get {
                return _dcmmin;
            }
            set {
                _dcmmin = value;
                OnPropertyChanged();
            }
        }
        public int DCmmax
        {
            get {
                return _dcmmax;
            }
            set {
                _dcmmax = value;
                OnPropertyChanged();
            }
        }
        public List<string> PIDProfiles {
            get { return _pidProfiles; }
            set {
                _pidProfiles = value;
                OnPropertyChanged();
            }
        }
        public WLANThermoSettings ThermoSettings
        {
            get {
                return _thermoSettings;
            }
            set {
                _thermoSettings = value;
                List<string>temp = new List<string>();
                foreach (var item in _thermoSettings.pid) {
                    temp.Add(item.name);
                }
                PIDProfiles = temp;
                SelectedPIDProfile = temp.ToArray()[_thermoData.pitmaster.pid];
                OnPropertyChanged();
            }
        }

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
        public float Temp
        {
            get {
                return _temp;
            }
            set {
                _temp = value;
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
            _pitmasterSteps.Add(new PitmasterStep());
            _pitmasterSteps.Add(new PitmasterStep());
            _pitmasterSteps.Add(new PitmasterStep());



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
                    await getThermoData();
                    //await getSettings();
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
        public void WaitForGetSettings()
        {
            getSettings();
            //TODO: This call is not waiting for the completion of the HTTP Request. Look for a better version.
        }


        public async Task getSettings()
        {
            if (_thermometerConnected) {
                var jsonString = await getData("/settings");
                ThermoSettings = JsonConvert.DeserializeObject<WLANThermoSettings>(jsonString);
            }
            else {
                MessageBox.Show("Thermometer not connected!");
            }
        }
        public void waitOnSetPIDProfile()
        {
            setPIDProfile();
            
        }
        public async Task setPIDProfile()
        {
            PIDSettings pid;

            //TODO: Check for a better call for this, or just if it gets the correct data.
            pid = ThermoSettings.pid.ToArray()[PIDProfiles.IndexOf(SelectedPIDProfile)];
            pid.Kd = Kd;
            pid.Kd_a = Kd_a;
            pid.Ki = Ki;
            pid.Ki_a = Ki_a;
            pid.Kd = Kd;
            pid.Kd_a = Kd_a;
            pid.DCmmax = DCmmax;
            pid.DCmmin = DCmmin;
            var temp = ThermoSettings.pid.ToArray();
            temp[PIDProfiles.IndexOf(SelectedPIDProfile)] = pid;
            

            //TODO: Finish with rest of the Settings.
            await setData("/setpid", JsonConvert.SerializeObject(temp.ToList<PIDSettings>()));
        }
        public async Task<String> getData(string service)
        {
            var response = await _client.GetStringAsync("http://" + IP + service);
            return response;
        }
        public async Task<bool> setData(string service, string data)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
        "Basic",
        Convert.ToBase64String(
            ASCIIEncoding.ASCII.GetBytes( Username + ":"+ Password)));
            var response = await _client.PostAsync("http://" + IP + service , new StringContent(data,Encoding.Default,"applicatioin/json"));
            return response.IsSuccessStatusCode;
        }
        private void NewPitmasterStep (){
            PitmasterSteps.Add(new PitmasterStep());
        }
        private void DeletePitmasterStep()
        {
            PitmasterSteps.Remove(SelectedPitmasterStep);
            if (PitmasterSteps.Count != 0) {
                SelectedPitmasterStep = PitmasterSteps.First();
            }
            else {
                SelectedPitmasterStep = null;
            }
        }
        private void MoveUpPitmasterStep()
        {
            var selectedIndex = PitmasterSteps.IndexOf(SelectedPitmasterStep);
            PitmasterSteps.Move(selectedIndex, selectedIndex - 1);
        }
        private void MoveDownPitmasterStep()
        {
            var selectedIndex = PitmasterSteps.IndexOf(SelectedPitmasterStep);
            PitmasterSteps.Move(selectedIndex, selectedIndex + 1);
        }
        #region EventHandlers
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _timer.Stop();
            //TODO:Disable all other requests when timer is running.
            //getThermoData().Wait();
            var channel = _thermoData.channel.Find(x => x.number == _thermoData.pitmaster.channel);
            _temp = channel.temp;
            Temp = _temp;
            _timer.Start();
        }

        
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
        public ICommand LoadSettingsFromDeviceClicked
        {
            get {
                return new DelegateCommand(WaitForGetSettings);
            }
        }
        public ICommand SetSettingsToDeviceClicked
        {
            get{
                return new DelegateCommand(waitOnSetPIDProfile);
            }
        }
        public ICommand NewEntryClicked
        {
            get {
                return new DelegateCommand(NewPitmasterStep);
            }
        }
        public ICommand DeleteEntryClicked
        {
            get {
                return new DelegateCommand(DeletePitmasterStep);
            }
        }
        public ICommand MoveUpEntryClicked
        {
            get {
                return new DelegateCommand(MoveUpPitmasterStep);
            }
        }
        public ICommand MoveDownEntryClicked
        {
            get{
                return new DelegateCommand(MoveDownPitmasterStep);
            }
        }
        #endregion
    }
}
