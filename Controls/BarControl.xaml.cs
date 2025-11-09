using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hardware_Monitor.Controls
{
    /// <summary>
    /// BarControl.xaml 的交互逻辑
    /// </summary>
    public partial class BarControl : UserControl
    {
        private const double AnimationDurationSeconds = 0.5;
        private static readonly Color COLD_COLOR = (Color)ColorConverter.ConvertFromString("#4A90E2"); // 蓝
        private static readonly Color MID_COLOR = (Color)ColorConverter.ConvertFromString("#FFD54F"); // 
        private static readonly Color HOT_COLOR = (Color)ColorConverter.ConvertFromString("#FF3B30"); // 橙

        public BarControl() {
            InitializeComponent();
            Loaded += (s, e) => {
                if (this.ActualWidth > 0)
                    UpdateBar(animated: false);
            };
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(BarControl),
                new PropertyMetadata(0.0, OnValueChanged));

        public double Value {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is BarControl control) {
                control.UpdateBar(animated: true);
            }
        }

        private void UpdateBar(bool animated) {
            if (ForegroundBar == null || ForegroundBrush == null) return;
            if (this.ActualWidth <= 0) return;

            double percent = Math.Max(0, Math.Min(100, Value)) / 100.0;
            double targetWidth = this.ActualWidth * percent;

            Color targetColor = GetColorForValue(Value);

            if (animated) {
                // 宽度动画
                var widthAnim = new DoubleAnimation {
                    To = targetWidth,
                    Duration = TimeSpan.FromSeconds(AnimationDurationSeconds),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                ForegroundBar.BeginAnimation(WidthProperty, widthAnim);

                // 颜色动画
                var colorAnim = new ColorAnimation {
                    To = targetColor,
                    Duration = TimeSpan.FromSeconds(AnimationDurationSeconds)
                };
                ForegroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            }
            else {
                ForegroundBar.Width = targetWidth;
                ForegroundBrush.Color = targetColor;
            }
        }

        private Color GetColorForValue(double value) {
            if (value <= 40)
                return COLD_COLOR;
            else if (value <= 65) {
                double t = (value - 40) / (65 - 40);
                return ColorUtils.InterpolateColor(COLD_COLOR, MID_COLOR, t);
            }
            else if (value <= 85) {
                double t = (value - 65) / (85 - 65);
                return ColorUtils.InterpolateColor(MID_COLOR, HOT_COLOR, t);
            }
            else
                return HOT_COLOR;
        }

        private Color InterpolateColor(Color from, Color to, double t) {
            byte r = (byte)(from.R + (to.R - from.R) * t);
            byte g = (byte)(from.G + (to.G - from.G) * t);
            byte b = (byte)(from.B + (to.B - from.B) * t);
            return Color.FromRgb(r, g, b);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateBar(animated: false); // 控件大小改变时即时更新
        }
    }
}

