using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hardware_Monitor.ViewModel
{
    public class DigitalClockVM : INotifyPropertyChanged
    {
        private string _hourMinute;
        public string HourMinute {
            get => _hourMinute;
            private set { _hourMinute = value; OnPropertyChanged(); }
        }

        private string _yearMonthDay;
        public string YearMonthDay {
            get => _yearMonthDay;
            private set { _yearMonthDay = value; OnPropertyChanged(); }
        }

        public DigitalClockVM() {
            UpdateTime();
        }

        public void UpdateTime() {
            var now = DateTime.Now;
            HourMinute = now.ToString("HH:mm");
            YearMonthDay = now.ToString("yyyy-MM-dd");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
