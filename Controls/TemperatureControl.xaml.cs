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

namespace Hardware_Monitor.Controls
{
    /// <summary>
    /// TemperatureControl.xaml 的交互逻辑
    /// </summary>
    public partial class TemperatureControl : UserControl
    {
        public static readonly DependencyProperty LeftTopTextProperty =
        DependencyProperty.Register(
            nameof(LeftTopText),
            typeof(string),
            typeof(TemperatureControl),
            new PropertyMetadata(string.Empty)
        );

        public string LeftTopText {
            get => (string)GetValue(LeftTopTextProperty);
            set => SetValue(LeftTopTextProperty, value);
        }

        public static readonly DependencyProperty TemperatureProperty =
        DependencyProperty.Register(
            nameof(Temperature),
            typeof(string),
            typeof(TemperatureControl),
            new PropertyMetadata("35") // 初始值
        );

        public string Temperature {
            get => (string)GetValue(TemperatureProperty);
            set => SetValue(TemperatureProperty, value);
        }

        public TemperatureControl()
        {
            InitializeComponent();
        }
    }
}
