using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Windows.Media;

namespace Hardware_Monitor
{
    public struct HSV
    {
        public double H;
        public double S;
        public double V;
    }

    public static class ColorUtils
    {
        // RGB -> HSV
        public static HSV RGBtoHSV(Color color) {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h = 0;
            if (delta != 0) {
                if (max == r) h = 60 * (((g - b) / delta) % 6);
                else if (max == g) h = 60 * (((b - r) / delta) + 2);
                else h = 60 * (((r - g) / delta) + 4);
            }
            if (h < 0) h += 360;

            double s = max == 0 ? 0 : delta / max;
            double v = max;

            return new HSV { H = h, S = s, V = v };
        }

        // HSV -> RGB
        public static Color HSVtoRGB(HSV hsv) {
            double h = hsv.H;
            double s = hsv.S;
            double v = hsv.V;

            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double r1 = 0, g1 = 0, b1 = 0;
            if (0 <= h && h < 60) { r1 = c; g1 = x; b1 = 0; }
            else if (60 <= h && h < 120) { r1 = x; g1 = c; b1 = 0; }
            else if (120 <= h && h < 180) { r1 = 0; g1 = c; b1 = x; }
            else if (180 <= h && h < 240) { r1 = 0; g1 = x; b1 = c; }
            else if (240 <= h && h < 300) { r1 = x; g1 = 0; b1 = c; }
            else { r1 = c; g1 = 0; b1 = x; }

            byte r = (byte)Math.Round((r1 + m) * 255);
            byte g = (byte)Math.Round((g1 + m) * 255);
            byte b = (byte)Math.Round((b1 + m) * 255);

            return Color.FromRgb(r, g, b);
        }

        // HSV 插值函数
        public static Color InterpolateColor(Color from, Color to, double t) {
            t = Math.Clamp(t, 0.0, 1.0);

            HSV hsvFrom = RGBtoHSV(from);
            HSV hsvTo = RGBtoHSV(to);

            // Hue 插值（处理环绕 360 度）
            double dh = hsvTo.H - hsvFrom.H;
            if (dh > 180) dh -= 360;
            if (dh < -180) dh += 360;

            HSV hsvResult = new HSV {
                H = (hsvFrom.H + dh * t) % 360,
                S = hsvFrom.S + (hsvTo.S - hsvFrom.S) * t,
                V = hsvFrom.V + (hsvTo.V - hsvFrom.V) * t
            };

            return HSVtoRGB(hsvResult);
        }
    }
}
