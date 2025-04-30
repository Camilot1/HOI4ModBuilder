using System;

namespace HOI4ModBuilder.src.utils.structs
{
    public struct LogScaleData
    {
        public double logMin;
        public double logMax;

        public LogScaleData(double min, double max)
        {
            min += 1;
            max += 1;
            if (min < 1)
                min = 1;
            if (max < 1)
                max = 1;

            if (max > min)
                (min, max) = (max, min);

            logMin = Math.Log(min);
            logMax = Math.Log(max);
        }

        public void SetMin(double value)
        {
            value += 1;
            if (value < 1)
                value = 1;
            logMin = Math.Log(value);
        }
        public void SetMax(double value)
        {
            value += 1;
            if (value < 1)
                value = 1;
            logMax = Math.Log(value);
        }

        public double Calculate(double value, double normalizeFactor)
        {
            value += 1;
            if (value < 1)
                value = 1;
            var logValue = Math.Log(value);
            var normalized = (logValue - logMin) / (logMax - logMin);
            var result = (normalized * normalizeFactor);
            return result;
        }
        public double CalculateInverted(double value, double normalizeFactor)
        {
            value += 1;
            if (value < 1)
                value = 1;
            var logValue = Math.Log(value);
            var normalized = (logValue - logMin) / (logMax - logMin);
            var result = normalizeFactor - (normalized * normalizeFactor);
            return result;
        }
    }
}
