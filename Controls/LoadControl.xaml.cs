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
    /// LoadControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoadControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(LoadControl),
                new PropertyMetadata(string.Empty));

        public string Title {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty LoadValueProperty =
            DependencyProperty.Register(
                nameof(LoadValue),
                typeof(double),
                typeof(LoadControl),
                new PropertyMetadata(0.0, OnLoadValueChanged));

        public double LoadValue {
            get => (double)GetValue(LoadValueProperty);
            set => SetValue(LoadValueProperty, value);
        }

        public static readonly DependencyProperty ExtraInfoProperty =
            DependencyProperty.Register(
                nameof(ExtraInfo),
                typeof(string),
                typeof(LoadControl),
                new PropertyMetadata("0", OnExtraInfoChanged));

        public string ExtraInfo {
            get => (string)GetValue(ExtraInfoProperty);
            set => SetValue(ExtraInfoProperty, value);
        }

        public LoadControl() {
            InitializeComponent();
        }

        private static void OnLoadValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is LoadControl control) {
                // 可选：在这里做一些内部逻辑
                // 例如动态改变颜色、动画等
            }
        }

        private static void OnExtraInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is LoadControl control) {
                // 可选：在这里做一些内部逻辑
                // 例如动态改变颜色、动画等
            }
        }
    }
}
