using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLANThermoDesktopApp.Model
{
    class PitmasterStep:ObservableObject
    {
        private int _timeLeft;
        private int _heatingTime;
        private Status _status;

        public Status Status{
            get => _status;
            set {
                _status = value;
                OnPropertyChanged();
            }
        }
        public float Temperature { get; set; }
        public int Time { get; set; }
        public int HeatingTime {
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
    }
    enum Status {NotStarted,InProgress,Done }
}
