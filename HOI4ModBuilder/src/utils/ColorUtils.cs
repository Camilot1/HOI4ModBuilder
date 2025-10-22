using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils
{
    public class ColorUtils
    {

        public static void HsvToRgb(double h, double S, double V, out byte r, out byte g, out byte b)
        {
            double saturation = MathUtils.Clamp(S, 0.0, 1.0);
            double value = MathUtils.Clamp(V, 0.0, 1.0);

            double H = h;
            if (H >= 0.0 && H <= 1.0 && saturation <= 1.0 && value <= 1.0)
                H *= 360.0;

            while (H < 0) { H += 360; }
            while (H >= 360) { H -= 360; }

            double R, G, B;
            if (value <= 0)
                R = G = B = 0;
            else if (saturation <= 0)
                R = G = B = value;
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = value * (1 - saturation);
                double qv = value * (1 - saturation * f);
                double tv = value * (1 - saturation * (1 - f));
                switch (i)
                {
                    case 0: R = value; G = tv; B = pv; break;
                    case 1: R = qv; G = value; B = pv; break;
                    case 2: R = pv; G = value; B = tv; break;
                    case 3: R = pv; G = qv; B = value; break;
                    case 4: R = tv; G = pv; B = value; break;
                    case 5: R = value; G = pv; B = qv; break;
                    case 6: R = value; G = tv; B = pv; break;
                    case -1: R = value; G = pv; B = qv; break;
                    default: R = G = B = value; break;
                }
            }

            r = (byte)MathUtils.Clamp((int)Math.Round(R * 255.0, MidpointRounding.AwayFromZero), 0, 255);
            g = (byte)MathUtils.Clamp((int)Math.Round(G * 255.0, MidpointRounding.AwayFromZero), 0, 255);
            b = (byte)MathUtils.Clamp((int)Math.Round(B * 255.0, MidpointRounding.AwayFromZero), 0, 255);
        }

        public static void RgbToHsv(byte r, byte g, byte b, out double h, out double s, out double v)
        {
            const double inv255 = 1.0 / 255.0;
            double rd = r * inv255;
            double gd = g * inv255;
            double bd = b * inv255;

            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            double delta = max - min;

            v = max;
            if (max <= 0.0)
            {
                s = 0.0;
                h = 0.0;
                return;
            }

            s = delta / max;

            if (delta <= 0.0)
            {
                h = 0.0;
                return;
            }

            if (Math.Abs(max - rd) < 1e-10)
                h = (gd - bd) / delta;
            else if (Math.Abs(max - gd) < 1e-10)
                h = 2.0 + (bd - rd) / delta;
            else
                h = 4.0 + (rd - gd) / delta;

            h /= 6.0;
            if (h < 0.0)
                h += 1.0;
            else if (h >= 1.0)
                h -= 1.0;
        }
    }
}
