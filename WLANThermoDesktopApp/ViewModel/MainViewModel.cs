using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using WLANThermoDesktopApp.Model;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace WLANThermoDesktopApp.ViewModel
{
    class MainViewModel : ObservableObject
        {

     
        private static readonly HttpClient _client = new HttpClient();
        private string _ip;  
        private bool _thermometerConnected = false;
        private Timer _timer;
        private WLANThermoData _thermoData;
        private WLANThermoSettings _thermoSettings;
        private float _temp ;
        private readonly int _timerIntervall = 1000;
        private List<string> _pidProfiles = new List<string>();
        private string _selectedPIDProfile;
        private float _kp;
        private float _ki;
        private float _kd; 
        private float _kp_a;
        private float _ki_a;
        private float _kd_a;
        private float _dcmmin;
        private float _dcmmax;
        private ObservableCollection<PitmasterStep> _pitmasterSteps = new ObservableCollection<PitmasterStep>();
        private PitmasterStep _selectedPitmasterStep;
        private PitmasterStep _currentPitmasterStep;
        private int _elapsedTime;
        private bool _pitmasterRunning = false;
        private bool _DataGridIsReadOnly = false;
        private Logger _logger;
        private int _connectionTimeoutCount = 0;


        #region Properties
        public bool DataGridIsReadOnly
        {
            get {
                return _DataGridIsReadOnly;
            }
            set {
                _DataGridIsReadOnly = value;
                OnPropertyChanged();
            }
        }
        public bool PitmasterRunning{
            get {
                return _pitmasterRunning;
            }
                set {
                _pitmasterRunning = value;
                DataGridIsReadOnly = _pitmasterRunning;
                OnPropertyChanged();
            }
        }
        public PitmasterStep CurrentPitmasterStep
        {
            get => _currentPitmasterStep;
            set {
                _currentPitmasterStep = value;
                PitmasterSteps[PitmasterSteps.IndexOf(_currentPitmasterStep)].TimeLeft = _currentPitmasterStep.Time;
                if (_currentPitmasterStep.Temperature > Temp) {
                    PitmasterSteps[PitmasterSteps.IndexOf(_currentPitmasterStep)].Status = Status.HeatingUp;
                }
                else {
                    PitmasterSteps[PitmasterSteps.IndexOf(_currentPitmasterStep)].Status = Status.CoolingDown;
                }
                PitmasterSteps[PitmasterSteps.IndexOf(_currentPitmasterStep)].HeatingTime = _currentPitmasterStep.HeatingTime;
                _currentPitmasterStep = PitmasterSteps[PitmasterSteps.IndexOf(_currentPitmasterStep)];
                OnPropertyChanged();
            }
        }
        public float TempTolerance { get; set; }
        public int ElapsedTime {
            get => _elapsedTime;
            set {
                _elapsedTime = value;
                OnPropertyChanged();
            }
        }
        public PitmasterStep SelectedPitmasterStep
        {
            get => _selectedPitmasterStep;
            
            set {
                _selectedPitmasterStep = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<PitmasterStep> PitmasterSteps
        {
            get =>  _pitmasterSteps;
            
            set {
                _pitmasterSteps = value;
                OnPropertyChanged();
            }
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SelectedPIDProfile {
            get => _selectedPIDProfile;
           
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
            get =>  _kp;
            
            set {
                _kp = value;
                OnPropertyChanged();
            }
        }
        public float Ki
        {
            get => _ki;
            set {
                _ki = value;
                OnPropertyChanged();
            }
        }
        public float Kd
        {
            get => _kd;
            
            set {
                _kd = value;
                OnPropertyChanged();
            }
        }
        public float Kp_a
        {
            get => _kp_a;
            
            set {
                _kp_a = value;
                OnPropertyChanged();
            }
        }
        public float Ki_a
        {
            get => _ki_a;
            
            set {
                _ki_a = value;
                OnPropertyChanged();
            }
        }
        public float Kd_a
        {
            get => _kd_a;
            
            set {
                _kd_a = value;
                OnPropertyChanged();
            }
        }
        public float DCmmin
        {
            get => _dcmmin;
            set {
                _dcmmin = value;
                OnPropertyChanged();
            }
        }
        public float DCmmax
        {
            get => _dcmmax;
            set {
                _dcmmax = value;
                OnPropertyChanged();
            }
        }
        public List<string> PIDProfiles {
            get => _pidProfiles; 
            set {
                _pidProfiles = value;
                OnPropertyChanged();
            }
        }
        public WLANThermoSettings ThermoSettings
        {
            get =>  _thermoSettings;
            set {
                if (ThermoData != null) {
                    _thermoSettings = value;
                    List<string> temp = new List<string>();
                    foreach (var item in _thermoSettings.pid) {
                        temp.Add(item.name);
                    }
                    PIDProfiles = temp;
                    SelectedPIDProfile = temp.ToArray()[_thermoData.pitmaster.First().pid];
                    OnPropertyChanged();
                }
            }
        }
        public WLANThermoData ThermoData
        {
            get => _thermoData;
            set {
                _thermoData = value;
                OnPropertyChanged();
            }
        }

        public string IP
        {
            get => _ip;
            set {
                if (string.IsNullOrEmpty(value)) {
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
            get => _temp;
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
            _logger = new Logger(@".\logs\");
           

            IP = "192.168.0.101";
            Username = "admin";
            Password = "admin";
            var temp = new PitmasterStep();
            temp.Time = 100;
            temp.Temperature = 20;
            PitmasterSteps.Add(temp);
            temp = new PitmasterStep();
            temp.Time = 100;
            temp.Temperature = 30;
            PitmasterSteps.Add(temp);
  
            //ConnectThermometer();
            //getData();
            //getSettings();

        }

        public void ConnectThermometer() => ConnectThermometerAsync();
        public async void ConnectThermometerAsync()
         {
            var ipPattern = "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            var rgx = new Regex(ipPattern);
            if (String.IsNullOrEmpty(IP)) {
                MessageBox.Show("Please enter a IP-Address!");
                return;
            }
            else if (rgx.Matches(IP).Count ==0) {
                MessageBox.Show("IP-Address not entered correctly");
                return;
            }
            try {
                var temp = await GetData("/");
                if (!string.IsNullOrEmpty(temp)) {
                    _thermometerConnected = true;
                    await GetThermoData();
                    await GetSettings();
                    await SetPIDProfile();
                    MessageBox.Show("Connection established!");
                    _logger.Log("Connection established!");
                    _timer.Enabled = true;

                }
            }
            catch (OperationCanceledException e) {
                Console.WriteLine(e.Message);
                MessageBox.Show("Connection timed out! Check IP-Address!");
                
            }
            catch(HttpRequestException e) {
                MessageBox.Show("Check Credentials.");
                _thermometerConnected = false;
            }
        }

        public void DisconnectThermometer()
        {
            _thermometerConnected = false;
            _timer.Stop();
        }
        public async Task GetThermoData()
        {
            if (_thermometerConnected) 
                { 
                var jsonString = await GetData("/data");
                if (!string.IsNullOrEmpty(jsonString)) {
                    try {
                        ThermoData = JsonConvert.DeserializeObject<WLANThermoData>(jsonString);
                    }
                    catch (Exception e) {
                        MessageBox.Show("JSON couldn't be parsed.\n Contact Administrator!");
                        _logger.Log(jsonString + "\n JSON couldn't be parsed.\n Contact Administrator!");
                    }
                }
            }
            else {
                MessageBox.Show("Thermometer not connected!");
            }

}
        public void WaitForGetSettings()
        {
            GetSettings();
            //TODO: This call is not waiting for the completion of the HTTP Request. Look for a better version.
        }


        public async Task GetSettings()
        {
            if (_thermometerConnected) {
                var jsonString = await GetData("/settings");
                if (!string.IsNullOrEmpty(jsonString)) {
                    try {
                        ThermoSettings = JsonConvert.DeserializeObject<WLANThermoSettings>(jsonString);
                    }
                    catch (Exception e) {
                        MessageBox.Show("JSON couldn't be parsed.\n Contact Administrator!");
                        _logger.Log(jsonString + "\n JSON couldn't be parsed.\n Contact Administrator!");
                    }
                }
            }
            else {
                MessageBox.Show("Thermometer not connected!");
            }
        }
        public void WaitOnSetPIDProfile() => SetPIDProfile();
        public async Task SetPIDProfile()
        {
            if (!_thermometerConnected) {
                MessageBox.Show("Thermometer is not Connected!");
                return;
            }
            if (ThermoSettings != null) {
                PIDSettings pid;
                //TODO: Check for a better call for this, or just if it gets the correct data.
                pid = ThermoSettings.pid.ToArray()[PIDProfiles.IndexOf(SelectedPIDProfile)];
                pid.Kd = Kd;
                pid.Kd_a = Kd_a;
                pid.Ki = Ki;
                pid.Ki_a = Ki_a;
                pid.Kp = Kp;
                pid.Kp_a = Kp_a;
                pid.DCmmax = DCmmax;
                pid.DCmmin = DCmmin;
                var temp = ThermoSettings.pid.ToArray();
                temp[PIDProfiles.IndexOf(SelectedPIDProfile)] = pid;


                //TODO: Finish with rest of the Settings.
                await SetData("/setpid", JsonConvert.SerializeObject(temp.ToList<PIDSettings>()));
            }
        }

        public async Task<string> GetData(string service)
        {
            try {
                var response = await _client.GetStringAsync("http://" + IP + service);
                _connectionTimeoutCount = 0;
                return response;
            }
            catch(OperationCanceledException e) {
                _connectionTimeoutCount++;
                if (_connectionTimeoutCount == 5) {
                    DisconnectThermometer();
                    PitmasterRunning = false;
                    _logger.Log("Connection was interrupted! Pitmaster stopped.");
                    MessageBox.Show("Connection timed out! ");
                }
                _logger.Log("Connection timed out!");
                return null;
            }
        }
        public void StartStopPitmaster()
        {
            if (PitmasterRunning) {
                StopPitmaster();
                ElapsedTime = 0;
                PitmasterRunning = false;
                _logger.Log("PitmasterStopped!");
                
            }
            else {
                if (_thermometerConnected) {
                    try { 
                        CurrentPitmasterStep = PitmasterSteps.First();
                        PitmasterRunning = true;
                        _logger.Log("Pitmaster Starting.");
                        _logger.Log("Temp" + ";" + "Seconds" + ";" + "HeatingTime" + ";" + "TimeLeft" + ";" + "CurrentTemp" + ";\n");
                        _logger.Log(CurrentPitmasterStep.Temperature + ";" + CurrentPitmasterStep.Time + ";" + CurrentPitmasterStep.HeatingTime + ";" + CurrentPitmasterStep.TimeLeft + ";" + Temp + ";\n");
                        SetPitmaster();
                        MessageBox.Show("Pitmaster Started.");
 
                    }
                    catch(InvalidOperationException e) {
                        MessageBox.Show("Add entries first.");
                    }
                }
                else {
                    MessageBox.Show("Device is not connected!");
                }
            }
        }
        public async Task StopPitmaster()
        {
            var temp = _thermoData.pitmaster.First();
            _thermoData.pitmaster.First().typ = "off";
            await SetData("/setpitmaster", JsonConvert.SerializeObject(_thermoData.pitmaster));
        }
        public async Task SetPitmaster()
        {
            ElapsedTime = 0;
            var temp = _thermoData.pitmaster.First();
            _thermoData.pitmaster.First().pid = PIDProfiles.IndexOf(SelectedPIDProfile);
            _thermoData.pitmaster.First().typ = "auto";
            _thermoData.pitmaster.First().set = CurrentPitmasterStep.Temperature;
            await SetData("/setpitmaster", JsonConvert.SerializeObject(_thermoData.pitmaster));
        }
        public async Task SetData(string service, string data)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
        "Basic",
        Convert.ToBase64String(
            ASCIIEncoding.ASCII.GetBytes( Username + ":"+ Password)));
            var response = await _client.PostAsync("http://" + IP + service , new StringContent(data,Encoding.Default,"applicatioin/json"));
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new HttpRequestException("Something went wrong.");
            }
        }

        private void NewPitmasterStep ()=>PitmasterSteps.Add(new PitmasterStep());

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
            if (selectedIndex > 0) {
                PitmasterSteps.Move(selectedIndex, selectedIndex - 1);
            }
        }

        private void MoveDownPitmasterStep()
        {
            var selectedIndex = PitmasterSteps.IndexOf(SelectedPitmasterStep);
            if (selectedIndex < (PitmasterSteps.Count -1)) {
                PitmasterSteps.Move(selectedIndex, selectedIndex + 1);
            }
        }

        #region EventHandlers
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if(_timer.Enabled == false) {
                return;
            }
            //TODO:Disable all other requests when timer is running.
            GetThermoData().Wait();
            if(ThermoData == null) {
                return;
            }
            var channel = _thermoData.channel.Find(x => x.number == _thermoData.pitmaster.First().channel);
            _temp = channel.temp;
            Temp = _temp;
            if (CurrentPitmasterStep != null && PitmasterRunning) {
                if ((Temp >= CurrentPitmasterStep.Temperature && CurrentPitmasterStep.Status == Status.HeatingUp)
                    ||(Temp <= CurrentPitmasterStep.Temperature && CurrentPitmasterStep.Status == Status.CoolingDown)
                    || (CurrentPitmasterStep.TimeLeft < CurrentPitmasterStep.Time)) {
                    CurrentPitmasterStep.TimeLeft--;
                    CurrentPitmasterStep.Status = Status.HoldingTemp;
                    _logger.Log( CurrentPitmasterStep.Temperature + ";" + CurrentPitmasterStep.Time + ";" + CurrentPitmasterStep.HeatingTime + ";" + CurrentPitmasterStep.TimeLeft + ";" + Temp + ";\n");

                }
                else if(CurrentPitmasterStep.Status == Status.HeatingUp) {
                    CurrentPitmasterStep.HeatingTime++;
                    _logger.Log(CurrentPitmasterStep.Temperature + ";" + CurrentPitmasterStep.Time + ";" + CurrentPitmasterStep.HeatingTime + ";" + CurrentPitmasterStep.TimeLeft + ";" + Temp + ";\n");
                }
                else if(CurrentPitmasterStep.Status == Status.CoolingDown) {
                    CurrentPitmasterStep.HeatingTime--;
                    _logger.Log(CurrentPitmasterStep.Temperature + ";" + CurrentPitmasterStep.Time + ";" + CurrentPitmasterStep.HeatingTime + ";" + CurrentPitmasterStep.TimeLeft + ";" + Temp + ";\n");

                }

                if (CurrentPitmasterStep.TimeLeft == 0) {
                    PitmasterSteps[PitmasterSteps.IndexOf(CurrentPitmasterStep)].Status = Status.Done;
                    if(PitmasterSteps.Last().Equals(CurrentPitmasterStep)) {
                        StartStopPitmaster();
                        MessageBox.Show("Pitmaster finished!");
                        _logger.Log("Pitmaster finished!");
                    }
                    else {
                        
                        CurrentPitmasterStep = PitmasterSteps.ElementAt(PitmasterSteps.IndexOf(CurrentPitmasterStep)+1);
                        SetPitmaster();
                    }
                }
            }
          
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
        public ICommand LoadSettingsFromDeviceClicked => new DelegateCommand(WaitForGetSettings);
        public ICommand SetSettingsToDeviceClicked => new DelegateCommand(WaitOnSetPIDProfile);
        public ICommand NewEntryClicked => new DelegateCommand(NewPitmasterStep);
        public ICommand DeleteEntryClicked => new DelegateCommand(DeletePitmasterStep);
        public ICommand MoveUpEntryClicked => new DelegateCommand(MoveUpPitmasterStep);
        public ICommand MoveDownEntryClicked => new DelegateCommand(MoveDownPitmasterStep);
        public ICommand StartPitmasterClicked => new DelegateCommand(StartStopPitmaster);
        #endregion
    }
}
