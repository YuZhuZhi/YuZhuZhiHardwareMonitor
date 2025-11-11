using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace Hardware_Monitor.ViewModel
{
    public class MainVM
    {
        public SensorDataVM Sensors { get; }
        public DigitalClockVM Clock { get; }

        private readonly DispatcherTimer _timer;

        public MainVM() {
            Sensors = new SensorDataVM();
            Clock = new DigitalClockVM();

            _timer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(2)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e) {
            Clock.UpdateTime();
            Sensors.Refresh();
        }
    }
}
