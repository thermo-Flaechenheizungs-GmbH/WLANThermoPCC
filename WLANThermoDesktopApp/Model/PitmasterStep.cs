using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp.Model
{
    class PitmasterStep : ObservableObject
    {
        private int _timeLeft;
        private int _heatingTime;
        private Status _status;
        private string _delimiter = ";";
        #region Properties 
        public Status Status
        {
            get => _status;
            set {
                _status = value;
                OnPropertyChanged();
            }
        }
        public float Temperature { get; set; }
        public int Time { get; set; }
        public int HeatingTime
        {
            get => _heatingTime;
            set {
                _heatingTime = value;
                OnPropertyChanged();
            }
        }
        public int TimeLeft
        {
            get => _timeLeft;
            set {
                _timeLeft = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties
        #region Constructors
        public PitmasterStep() : this("") { }
        public PitmasterStep(string inputString)
        {
            ReadFromString(inputString);
        }

        #endregion Constructors
        public void ReadFromString(string inputString)
        {
            if (!string.IsNullOrEmpty(inputString) && inputString.IndexOf(_delimiter) > 0) {
                this.Temperature = int.Parse(inputString.Substring(0, inputString.IndexOf(_delimiter)));
                inputString = inputString.Substring(inputString.IndexOf(_delimiter)+1);
                this.Time = int.Parse(inputString.Substring(0, inputString.IndexOf(_delimiter) ));
                inputString = inputString.Substring(inputString.IndexOf(_delimiter));
            }
        }
        public string WriteToString()
        {
            return this.Temperature + _delimiter + this.Time + _delimiter;        
        }
    }
    enum Status {
        NotStarted,
        HeatingUp,
        CoolingDown,
        HoldingTemp,
        Done
    }
}
