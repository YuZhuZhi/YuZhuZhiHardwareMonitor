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
    /// DashBoardControl.xaml 的交互逻辑
    /// </summary>
    public partial class DashBoardControl : UserControl
    {
        private static readonly Color START_COLOR = (Color)ColorConverter.ConvertFromString("#4A90E2"); // 蓝
        private static readonly Color MID_COLOR = (Color)ColorConverter.ConvertFromString("#FFD54F"); // 
        private static readonly Color END_COLOR = (Color)ColorConverter.ConvertFromString("#FF3B30"); // 橙

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(DashBoardControl),
                new PropertyMetadata(0.0, OnValueChanged));

        public double Value {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(DashBoardControl), new PropertyMetadata(0.0));

        public double MinValue {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(DashBoardControl), new PropertyMetadata(100.0));

        public double MaxValue {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty PointerColorProperty =
            DependencyProperty.Register(nameof(PointerColor), typeof(Brush), typeof(DashBoardControl), new PropertyMetadata(Brushes.Red));

        public Brush PointerColor {
            get => (Brush)GetValue(PointerColorProperty);
            set => SetValue(PointerColorProperty, value);
        }

        //private Line pointer;

        public DashBoardControl() {
            InitializeComponent();
            SizeChanged += (s, e) => DrawDashboard();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is DashBoardControl control)
                control.AnimatePointer();
        }

        private void DrawDashboard() {
            DashboardCanvas.Children.Clear();
            double width = ActualWidth;
            double height = ActualHeight;
            double radius = Math.Min(width, height) / 2 - 10;
            Point center = new(width / 2, height / 2);

            // 绘制刻度
            double startAngle = 225; // 左下
            double sweepAngle = 270; // 顺时针270度
            double startRad = startAngle * Math.PI / 180;
            double sweepRad = sweepAngle * Math.PI / 180;

            for (int i = 0; i <= 100; i += 2) {
                double angle = startRad - (i / 100.0) * sweepRad;
                double tickLength = (i % 10 == 0) ? 15 : 7;

                double x1 = center.X + radius * Math.Cos(angle);
                double y1 = center.Y - radius * Math.Sin(angle);
                double x2 = center.X + (radius - tickLength) * Math.Cos(angle);
                double y2 = center.Y - (radius - tickLength) * Math.Sin(angle);

                Line tick = new() {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = (i % 10 == 0) ? Brushes.WhiteSmoke : Brushes.Gray,
                    StrokeThickness = (i % 10 == 0) ? 2 : 1
                };
                DashboardCanvas.Children.Add(tick);

                if (i % 20 == 0) {
                    TextBlock tb = new() {
                        Text = i.ToString(),
                        Style = (Style)FindResource("OrbitronStyle"),
                        FontSize = Math.Max(8, radius * 0.05), // 可选：字体随表盘大小缩放
                        TextAlignment = TextAlignment.Center,
                        SnapsToDevicePixels = true
                    };

                    // 先测量，使 DesiredSize 准确
                    tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size ts = tb.DesiredSize;

                    // 文本中心点（在弧线上），注意 Canvas 坐标系 Y 向下
                    double textRadius = radius - 22;
                    double textX = center.X + textRadius * Math.Cos(angle);
                    double textY = center.Y - textRadius * Math.Sin(angle);

                    // 将文本中心对齐到刻度点（用测量出来的宽高除以2）
                    Canvas.SetLeft(tb, Math.Round(textX - ts.Width / 2));  // 用 Math.Round 做像素对齐
                    Canvas.SetTop(tb, Math.Round(textY - ts.Height / 2));

                    // 可选：保持文字竖直（不随表盘旋转）
                    // 如果你想文字沿弧旋转，取消下面注释并调整角度（通常不需要）
                    //double deg = angle * 180 / Math.PI;
                    //tb.RenderTransform = new RotateTransform(-deg, ts.Width / 2, ts.Height / 2);

                    DashboardCanvas.Children.Add(tb);
                }
            }

            // 绘制外圈圆弧
            double arcStart = 225;
            double arcEnd = -45;
            Point startPoint = new(
                center.X + radius * Math.Cos(arcStart * Math.PI / 180),
                center.Y - radius * Math.Sin(arcStart * Math.PI / 180));
            Point endPoint = new(
                center.X + radius * Math.Cos(arcEnd * Math.PI / 180),
                center.Y - radius * Math.Sin(arcEnd * Math.PI / 180));

            PathFigure arcFigure = new(startPoint, new[] {
                new ArcSegment(endPoint, new Size(radius, radius), 0, true, SweepDirection.Clockwise, true)
            }, false);

            PathGeometry arcGeometry = new(new[] { arcFigure });

            Path arcPath = new() {
                Stroke = Brushes.WhiteSmoke,
                StrokeThickness = 3,
                Opacity = 0.8,
                Data = arcGeometry
            };
            DashboardCanvas.Children.Add(arcPath);

            // 指针
            double pointerLength = radius - 20;
            pointer.X1 = 0;
            pointer.Y1 = 0;
            pointer.X2 = 0;
            pointer.Y2 = -pointerLength;
            Canvas.SetLeft(pointer, center.X);
            Canvas.SetTop(pointer, center.Y);
            pointer.RenderTransformOrigin = new Point(0.5, 1);
            DashboardCanvas.Children.Add(pointer);

            // 数值显示
            //Canvas.SetLeft(ValueText, center.X - ValueText.ActualWidth / 2);
            //Canvas.SetTop(ValueText, center.Y - ValueText.ActualHeight / 2);

            AnimatePointer();
        }

        private void AnimatePointer() {
            if (pointerRotate == null) return;

            double normalized = Math.Max(MinValue, Math.Min(MaxValue, Value));
            double t = (normalized - MinValue) / (MaxValue - MinValue);

            // 指针初始向上，Value 最小对应左下（-135°），最大对应右下（135°）
            double targetAngle = -135 + t * 270;

            var angleAnim = new DoubleAnimation {
                To = targetAngle,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            pointerRotate.BeginAnimation(RotateTransform.AngleProperty, angleAnim);

            // 更新指针颜色
            pointer.Stroke = new SolidColorBrush(GetColorForValue(Value));

            // 更新数值显示
            //ValueText.Text = Math.Round(Value).ToString();
        }

        private Color GetColorForValue(double value) {
            if (value <= 20)
                return START_COLOR;
            else if (value <= 50) {
                double t = (value - 20) / (50 - 20);
                return ColorUtils.InterpolateColor(START_COLOR, MID_COLOR, t);
            }
            else if (value <= 90) {
                double t = (value - 50) / (90 - 50);
                return ColorUtils.InterpolateColor(MID_COLOR, END_COLOR, t);
            }
            else
                return END_COLOR;
        }
    }
}
