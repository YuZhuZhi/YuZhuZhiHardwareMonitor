using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Hardware_Monitor.Controls
{
    /// <summary>
    /// DigitalClockControl.xaml 的交互逻辑
    /// </summary>
    public partial class DigitalClockControl : UserControl
    {
        public static readonly DependencyProperty HourMinuteTextProperty =
            DependencyProperty.Register(
                nameof(HourMinuteText),
                typeof(string),
                typeof(DigitalClockControl),
                new PropertyMetadata(DateTime.Now.ToString("HH:mm"))
            );

        public static readonly DependencyProperty YearMonthDayTextProperty =
            DependencyProperty.Register(
                nameof(YearMonthDayText),
                typeof(string),
                typeof(DigitalClockControl),
                new PropertyMetadata(DateTime.Now.ToString("yyyy-MM-dd"))
            );
        public string HourMinuteText {
            get => (string)GetValue(HourMinuteTextProperty);
            set => SetValue(HourMinuteTextProperty, value);
        }

        public string YearMonthDayText {
            get => (string)GetValue(YearMonthDayTextProperty);
            set => SetValue(YearMonthDayTextProperty, value);
        }

        public DigitalClockControl()
        {
            InitializeComponent();
            //_timer = new DispatcherTimer {
            //    Interval = TimeSpan.FromSeconds(5)
            //};
            //_timer.Tick += Timer_Tick;
            //_timer.Start();

            //this.DataContext = this;
        }

        //private void Timer_Tick(object sender, EventArgs e) {
        //    HourMinuteText = DateTime.Now.ToString("HH:mm");
        //    YearMonthDayText = DateTime.Now.ToString("yyyy-MM-dd");
        //}
    }
}
