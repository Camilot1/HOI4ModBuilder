using HOI4ModBuilder;
using HOI4ModBuilder.src.managers.settings;
using System;
using System.Collections.Generic;

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
            {
                H *= 360.0;
            }

            while (H < 0) { H += 360; }
            while (H >= 360) { H -= 360; }

            double R;
            double G;
            double B;

            if (value <= 0)
            {
                R = G = B = 0;
            }
            else if (saturation <= 0)
            {
                R = G = B = value;
            }
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
                    case 0:
                        R = value;
                        G = tv;
                        B = pv;
                        break;
                    case 1:
                        R = qv;
                        G = value;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = value;
                        B = tv;
                        break;
                    case 3:
                        R = pv;
                        G = qv;
                        B = value;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = value;
                        break;
                    case 5:
                        R = value;
                        G = pv;
                        B = qv;
                        break;
                    case 6:
                        R = value;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = value;
                        G = pv;
                        B = qv;
                        break;
                    default:
                        R = G = B = value;
                        break;
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

        public static int GenerateDistinctColor(Random random, List<int> existingColors, HSVRanges hsvRanges, Func<int, bool> isColorValid)
        {
            if (hsvRanges == null)
                throw new ArgumentNullException(nameof(hsvRanges));

            hsvRanges.Validate();

            if (existingColors == null)
                throw new ArgumentNullException(nameof(existingColors));

            var palette = new List<HsvColor>(existingColors.Count);
            foreach (var color in existingColors)
                palette.Add(ToHsvColor(color));

            var testedColors = new HashSet<int>(existingColors.Count + 32);
            foreach (var color in existingColors)
                testedColors.Add(color);

            if (palette.Count == 0)
            {
                if (TryRandomCandidates(random, palette, hsvRanges, isColorValid, testedColors, out int randomColor))
                    return randomColor;

                return GenerateColor(random, hsvRanges, isColorValid);
            }

            double step = DetermineSamplingStep(hsvRanges.minDif);

            var hueSamples = BuildHueSamples(step, hsvRanges.minH, hsvRanges.maxH);
            var saturationSamples = BuildRangeSamples(hsvRanges.minS, hsvRanges.maxS, step);
            var valueSamples = BuildRangeSamples(hsvRanges.minV, hsvRanges.maxV, step);

            if (TryEvaluateCandidates(
                EnumerateGridCandidates(hueSamples, saturationSamples, valueSamples),
                palette,
                hsvRanges,
                isColorValid,
                testedColors,
                out int deterministicColor))
                return deterministicColor;

            if (TryEvaluateCandidates(
                BuildNeighbourCandidates(palette, hsvRanges),
                palette, hsvRanges, isColorValid, testedColors, out int neighbourColor))
                return neighbourColor;

            if (TryRandomCandidates(random, palette, hsvRanges, isColorValid, testedColors, out int fallbackColor))
                return fallbackColor;

            return GenerateColor(random, hsvRanges, isColorValid);
        }

        public static int GenerateColor(Random random, HSVRanges hsvRanges, Func<int, bool> isColorValid)
        {
            if (hsvRanges == null)
                throw new ArgumentNullException(nameof(hsvRanges));
            hsvRanges.Validate();

            const int maxAttempts = 10000;
            double spanH = Math.Max(0.0, hsvRanges.maxH - hsvRanges.minH);
            double spanS = Math.Max(0.0, hsvRanges.maxS - hsvRanges.minS);
            double spanV = Math.Max(0.0, hsvRanges.maxV - hsvRanges.minV);

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var candidate = new HsvColor
                {
                    H = spanH <= 0.0 ? hsvRanges.minH : hsvRanges.minH + (random.NextDouble() * spanH),
                    S = spanS <= 0.0 ? hsvRanges.minS : hsvRanges.minS + (random.NextDouble() * spanS),
                    V = spanV <= 0.0 ? hsvRanges.minV : hsvRanges.minV + (random.NextDouble() * spanV)
                };

                int argb = HsvToArgb(candidate);
                if (isColorValid == null || isColorValid(argb))
                    return argb;
            }

            throw new InvalidOperationException(
                GuiLocManager.GetLoc(
                    EnumLocKey.UNABLE_TO_GENERATE_COLOR_WITHIN_PROVIDED_CONSTRAINTS,
                    new Dictionary<string, string> { { "{constraints}", hsvRanges.ToString() } }
                )
            );
        }

        // Weighted HSV distance keeps the check inexpensive while roughly reflecting visual deltas.
        private static double CalculateColorDifference(HsvColor colorA, HsvColor colorB)
        {
            double hueDiff = Math.Abs(colorA.H - colorB.H);
            hueDiff = Math.Min(hueDiff, 1.0 - hueDiff);

            double satDiff = Math.Abs(colorA.S - colorB.S);
            double valDiff = Math.Abs(colorA.V - colorB.V);

            const double hueWeight = 2.0;
            const double saturationWeight = 1.0;
            const double valueWeight = 1.0;

            return Math.Sqrt(
                (hueDiff * hueWeight) * (hueDiff * hueWeight) +
                (satDiff * saturationWeight) * (satDiff * saturationWeight) +
                (valDiff * valueWeight) * (valDiff * valueWeight));
        }

        private static bool TryEvaluateCandidates(
            IEnumerable<HsvColor> candidates,
            List<HsvColor> palette,
            HSVRanges hsvRanges,
            Func<int, bool> isColorValid,
            HashSet<int> testedColors,
            out int argb)
        {
            foreach (var candidate in candidates)
            {
                if (!IsWithinRange(candidate, hsvRanges))
                    continue;

                int generatedColor = HsvToArgb(candidate);
                if (!testedColors.Add(generatedColor))
                    continue;

                if (isColorValid != null && !isColorValid(generatedColor))
                    continue;

                if (IsWithinDifference(candidate, palette, hsvRanges.minDif, hsvRanges.maxDif))
                {
                    argb = generatedColor;
                    return true;
                }
            }

            argb = default;
            return false;
        }

        private static bool TryRandomCandidates(
            Random random,
            List<HsvColor> palette,
            HSVRanges hsvRanges,
            Func<int, bool> isColorValid,
            HashSet<int> testedColors,
            out int argb)
        {
            const int maxAttempts = 10000;
            double spanH = Math.Max(0.0, hsvRanges.maxH - hsvRanges.minH);
            double spanS = Math.Max(0.0, hsvRanges.maxS - hsvRanges.minS);
            double spanV = Math.Max(0.0, hsvRanges.maxV - hsvRanges.minV);

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var candidate = new HsvColor
                {
                    H = spanH <= 0.0 ? hsvRanges.minH : hsvRanges.minH + (random.NextDouble() * spanH),
                    S = spanS <= 0.0 ? hsvRanges.minS : hsvRanges.minS + (random.NextDouble() * spanS),
                    V = spanV <= 0.0 ? hsvRanges.minV : hsvRanges.minV + (random.NextDouble() * spanV)
                };

                int generatedColor = HsvToArgb(candidate);
                if (!testedColors.Add(generatedColor))
                    continue;

                if (isColorValid != null && !isColorValid(generatedColor))
                    continue;

                if (!IsWithinRange(candidate, hsvRanges))
                    continue;

                if (IsWithinDifference(candidate, palette, hsvRanges.minDif, hsvRanges.maxDif))
                {
                    argb = generatedColor;
                    return true;
                }
            }

            argb = default;
            return false;
        }

        private static IEnumerable<HsvColor> EnumerateGridCandidates(List<double> hueSamples, List<double> saturationSamples, List<double> valueSamples)
        {
            foreach (var saturation in saturationSamples)
            {
                foreach (var value in valueSamples)
                {
                    foreach (var hue in hueSamples)
                    {
                        yield return new HsvColor { H = hue, S = saturation, V = value };
                    }
                }
            }
        }

        private static IEnumerable<HsvColor> BuildNeighbourCandidates(List<HsvColor> palette, HSVRanges hsvRanges)
        {
            if (palette.Count == 0)
                yield break;

            double target = double.IsPositiveInfinity(hsvRanges.maxDif)
                ? Math.Max(hsvRanges.minDif, 0.15)
                : (hsvRanges.minDif + hsvRanges.maxDif) * 0.5;

            double hueShift = Math.Min(0.5, Math.Max(0.02, target / 2.2));
            double satShift = Math.Min(0.4, Math.Max(0.02, target * 0.5));
            double valueShift = Math.Min(0.4, Math.Max(0.02, target * 0.5));

            double[] hueOffsets = { hueShift, -hueShift, hueShift * 2.0, -hueShift * 2.0 };
            double[] saturationOffsets = { 0.0, satShift, -satShift };
            double[] valueOffsets = { 0.0, valueShift, -valueShift };

            foreach (var color in palette)
            {
                foreach (var hueOffset in hueOffsets)
                {
                    double hue = color.H + hueOffset;
                    hue = hue - Math.Floor(hue);
                    hue = MathUtils.Clamp(hue, hsvRanges.minH, hsvRanges.maxH);

                    foreach (var saturationOffset in saturationOffsets)
                    {
                        double saturation = MathUtils.Clamp(color.S + saturationOffset, hsvRanges.minS, hsvRanges.maxS);

                        foreach (var valueOffset in valueOffsets)
                        {
                            double value = MathUtils.Clamp(color.V + valueOffset, hsvRanges.minV, hsvRanges.maxV);
                            yield return new HsvColor { H = hue, S = saturation, V = value };
                        }
                    }
                }
            }
        }

        private static List<double> BuildHueSamples(double step, double minH, double maxH)
        {
            double span = Math.Max(0.0, maxH - minH);
            if (span <= 0.0)
                return new List<double> { MathUtils.Clamp(minH, 0.0, 1.0) };

            if (step <= 0.0)
                step = 0.05;

            int count = (int)Math.Ceiling(span / step) + 1;
            if (count < 12)
                count = 12;
            if (count > 360)
                count = 360;

            var samples = new List<double>(count);
            for (int i = 0; i < count; i++)
            {
                double t = count == 1 ? 0.0 : (double)i / (count - 1);
                double value = minH + (span * t);
                samples.Add(MathUtils.Clamp(value, 0.0, 1.0));
            }

            return samples;
        }

        private static List<double> BuildRangeSamples(double minValue, double maxValue, double step)
        {
            double span = Math.Max(0.0, maxValue - minValue);
            if (span <= 0.0)
                return new List<double> { MathUtils.Clamp(minValue, 0.0, 1.0) };

            if (step <= 0.0)
                step = 0.1;

            int steps = (int)Math.Ceiling(span / step);
            if (steps < 1)
                steps = 1;
            if (steps > 12)
                steps = 12;

            var samples = new List<double>(steps + 1);
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double value = minValue + (span * t);
                samples.Add(MathUtils.Clamp(value, 0.0, 1.0));
            }

            return samples;
        }

        private static double DetermineSamplingStep(double minDifference)
        {
            if (minDifference <= 0.0)
                return 0.08;

            double step = minDifference * 0.4;
            if (step < 0.04)
                step = 0.04;
            if (step > 0.2)
                step = 0.2;

            return step;
        }

        public static void ValidateHueRange(ref double minH, ref double maxH)
        {
            if (double.IsNaN(minH) || double.IsInfinity(minH) || double.IsNaN(maxH) || double.IsInfinity(maxH))
                throw new ArgumentOutOfRangeException("Hue bounds must be finite numbers.");

            minH = NormalizeHue(minH, preserveUpperBound: false);
            maxH = NormalizeHue(maxH, preserveUpperBound: true);

            minH = MathUtils.Clamp(minH, 0.0, 1.0);
            maxH = MathUtils.Clamp(maxH, 0.0, 1.0);

            if (maxH >= 1.0 && maxH > minH)
                maxH = NextDown(1.0);
        }

        private static double NormalizeHue(double value, bool preserveUpperBound)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return value;

            double normalized = value % 1.0;
            if (normalized < 0.0)
                normalized += 1.0;

            if (preserveUpperBound && Math.Abs(normalized) <= 1e-12 && value > 0.0)
                normalized = 1.0;

            return normalized;
        }

        private static double NextDown(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return value;
            if (value == 0.0)
                return -double.Epsilon;

            long bits = BitConverter.DoubleToInt64Bits(value);
            if (value > 0.0)
                bits--;
            else
                bits++;

            return BitConverter.Int64BitsToDouble(bits);
        }

        private static bool IsWithinRange(HsvColor candidate, HSVRanges hsvRanges)
        {
            return candidate.H >= hsvRanges.minH && candidate.H <= hsvRanges.maxH &&
                   candidate.S >= hsvRanges.minS && candidate.S <= hsvRanges.maxS &&
                   candidate.V >= hsvRanges.minV && candidate.V <= hsvRanges.maxV;
        }

        private static bool IsWithinDifference(HsvColor candidate, List<HsvColor> palette, double minDifference, double maxDifference)
        {
            foreach (var color in palette)
            {
                double diff = CalculateColorDifference(candidate, color);
                if (diff < minDifference)
                    return false;
                if (!double.IsPositiveInfinity(maxDifference) && diff > maxDifference)
                    return false;
            }

            return true;
        }

        private static HsvColor ToHsvColor(int argb)
        {
            Utils.IntToRgb(argb, out byte r, out byte g, out byte b);
            RgbToHsv(r, g, b, out double h, out double s, out double v);
            return new HsvColor { H = h, S = s, V = v };
        }

        private static int HsvToArgb(HsvColor hsv)
        {
            HsvToRgb(hsv.H, hsv.S, hsv.V, out byte r, out byte g, out byte b);
            return Utils.ArgbToInt(255, r, g, b);
        }

        private struct HsvColor
        {
            public double H;
            public double S;
            public double V;
        }
    }
}
