using HOI4ModBuilder.src.scripts.exceptions;
using System;
using System.Drawing;

namespace HOI4ModBuilder.src.scripts.utils
{
    public class ScriptUtils
    {
        private static int[] _daysInMonths = new int[]
        {
            31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
        };

        public static int GetDaysSinceYearStart(int lineIndex, string[] args, int dayIndex, int monthIndex)
        {
            if (monthIndex < 0 || monthIndex >= _daysInMonths.Length)
                throw new IndexOutOfRangeScriptException(lineIndex, args, monthIndex);

            int daysInMonth = _daysInMonths[monthIndex];

            if (dayIndex < 0 || dayIndex >= daysInMonth)
                throw new IndexOutOfRangeScriptException(lineIndex, args, dayIndex);

            int daysSinceYearStart = 0;
            for (int i = 0; i < monthIndex; i++)
                daysSinceYearStart += _daysInMonths[i];

            daysSinceYearStart += dayIndex;
            return daysSinceYearStart;
        }

        public static int GetWeeksSinceYearStart(int lineIndex, string[] args, int dayIndex, int monthIndex)
        {
            int daysSinceYearStart = GetDaysSinceYearStart(lineIndex, args, dayIndex, monthIndex);
            return GetWeeksSinceYearStart(daysSinceYearStart);
        }
        public static int GetWeeksSinceYearStart(int daysSinceYearStart)
            => daysSinceYearStart / 7;

        public static void TestInterpolation()
        {
            var bitmap = new Bitmap(@"data\images\test.bmp");

            var values = new double[]
            {
                0.1, 0.6,
                0.75, 1.55
            };

            int ldX = 0;
            int ldY = bitmap.Height - 1;
            double ldC = values[2];

            int rdX = bitmap.Width - 1;
            int rdY = bitmap.Height - 1;
            double rdC = values[3];

            int ruX = bitmap.Width - 1;
            int ruY = 0;
            var ruC = values[1];

            int luX = 0;
            int luY = 0;
            var luC = values[0];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    /*
                    var cA = Interpolate2D(
                        x, y,
                        ldX, ldY, ldC,
                        rdX, rdY, rdC,
                        ruX, ruY, ruC,
                        luX, luY, luC

                    );
                    var cR = Interpolate2D(
                        x, y,
                        ldX, ldY, ldC,
                        rdX, rdY, rdC,
                        ruX, ruY, ruC,
                        luX, luY, luC

                    );
                    var cG = Interpolate2D(
                        x, y,
                        ldX, ldY, ldC,
                        rdX, rdY, rdC,
                        ruX, ruY, ruC,
                        luX, luY, luC

                    );
                    var cB = Interpolate2D(
                        x, y,
                        ldX, ldY, ldC,
                        rdX, rdY, rdC,
                        ruX, ruY, ruC,
                        luX, luY, luC

                    );
                    var p = Color.FromArgb(
                        Utils.Clamp((int)Math.Round((1 - cA) * 100), 0, 50),
                        Utils.Clamp((int)Math.Round((1 - cR) * 100), 0, 50),
                        Utils.Clamp((int)Math.Round((1 - cG) * 100), 0, 50),
                        Utils.Clamp((int)Math.Round((1 - cB) * 100), 0, 50)
                    );
                    */
                    float v = (float)Interpolate2D(
                        x, y,
                        ldX, ldY, ldC,
                        rdX, rdY, rdC,
                        ruX, ruY, ruC,
                        luX, luY, luC

                    );

                    v = Utils.Clamp(v, 0.35f, 0.95f);


                    v = v * 100;
                    var p = Color.FromArgb(255, (int)v, (int)v, (int)v);

                    /*
                    Color p = default;
                    if (v >= 0.8) p = Color.Purple;
                    else if (v >= 0.55) p = Color.Blue;
                    else if (v > 0.3) p = Color.Yellow;
                    else p = Color.White;
                    */

                    bitmap.SetPixel(x, y, p);
                }
            }

            bitmap.Save(@"data\images\result.bmp");
        }

        public static double Interpolate1D(double p, double a1, double Q1, double a2, double Q2)
        {
            double dx = a1 - p;
            double da = a1 - a2;
            double dQ = Q1 - Q2;

            return Q1 + dx / da * dQ;
        }

        // Function to perform bilinear interpolation in a quadrilateral
        public static double Interpolate2D(
            double x, double y,
            double x1, double y1, double Q1,
            double x2, double y2, double Q2,
            double x3, double y3, double Q3,
            double x4, double y4, double Q4)
        {
            // Compute the bilinear coordinates (u, v) for the given point (x, y)
            (double u, double v) = MapToUnitSquareNewtonRaphson(x1, y1, x2, y2, x3, y3, x4, y4, x, y);

            // Perform bilinear interpolation using the mapped coordinates (u, v)
            return (1 - u) * (1 - v) * Q1 +
                   u * (1 - v) * Q2 +
                   u * v * Q3 +
                   (1 - u) * v * Q4;
        }

        // Function to map the point (x, y) to the unit square coordinates (u, v) using the Newton-Raphson method
        private static (double, double) MapToUnitSquareNewtonRaphson(
            double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4,
            double x, double y)
        {
            double u = 0.5, v = 0.5; // Initial guess
            double tolerance = 1e-6;
            int maxIterations = 100;

            for (int i = 0; i < maxIterations; i++)
            {
                // Compute the position (xp, yp) in the quadrilateral based on the current (u, v)
                double xp = (1 - u) * (1 - v) * x1 + u * (1 - v) * x2 + u * v * x3 + (1 - u) * v * x4;
                double yp = (1 - u) * (1 - v) * y1 + u * (1 - v) * y2 + u * v * y3 + (1 - u) * v * y4;

                // Differences between the target and current position
                double dx = x - xp;
                double dy = y - yp;

                if (Math.Abs(dx) < tolerance && Math.Abs(dy) < tolerance)
                {
                    break; // Converged to a solution
                }

                // Partial derivatives with respect to u and v
                double dxdU = -(1 - v) * x1 + (1 - v) * x2 + v * x3 - v * x4;
                double dxdV = -(1 - u) * x1 - u * x2 + u * x3 + (1 - u) * x4;
                double dydU = -(1 - v) * y1 + (1 - v) * y2 + v * y3 - v * y4;
                double dydV = -(1 - u) * y1 - u * y2 + u * y3 + (1 - u) * y4;

                // Determinant of the Jacobian matrix
                double det = dxdU * dydV - dxdV * dydU;

                if (Math.Abs(det) < tolerance)
                {
                    throw new InvalidOperationException("Jacobian determinant is too close to zero, unable to converge.");
                }

                // Update rules for u and v using the Newton-Raphson method
                double deltaU = (dx * dydV - dy * dxdV) / det;
                double deltaV = (dy * dxdU - dx * dydU) / det;

                u += deltaU;
                v += deltaV;
            }

            return (u, v);
        }
    }
}
