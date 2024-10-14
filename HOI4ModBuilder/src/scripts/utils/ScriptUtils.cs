using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;
using System.Drawing;
using System.IO.IsolatedStorage;
using System.Text;

namespace HOI4ModBuilder.src.scripts.utils
{
    public class ScriptUtils
    {
        public static void GetImplements(StringBuilder sb, object obj)
        {
            if (obj is IScriptObject) sb.Append("ISCRIPTOBJECT ");

            if (obj is ICollectionObject) sb.Append("ICOLLECTION ");
            if (obj is IFileObject) sb.Append("IFILE ");
            if (obj is IListObject) sb.Append("ILIST ");
            if (obj is ILogicalObject) sb.Append("ILOGICAL ");
            if (obj is IMapObject) sb.Append("IMAP ");
            if (obj is INumberObject) sb.Append("INUMBER ");
            if (obj is IRelativeObject) sb.Append("IRELATIVE ");
            if (obj is IStringObject) sb.Append("ISTRING ");

            if (obj is IAddObject) sb.Append("IADD ");
            if (obj is IAddRangeObject) sb.Append("IADDRANGE ");
            if (obj is IAndObject) sb.Append("IAND ");
            if (obj is IAppendObject) sb.Append("IAPPEND ");
            if (obj is IClearObject) sb.Append("ICLEAR ");
            if (obj is IDivideObject) sb.Append("IDIVIDE ");
            if (obj is IGetObject) sb.Append("IGET ");
            if (obj is IGetSizeObject) sb.Append("IGETSIZE ");
            if (obj is IInsertObject) sb.Append("IINSERT ");
            if (obj is IModuloObject) sb.Append("IMODULO ");
            if (obj is IMultiplyObject) sb.Append("IMULTIPLY ");
            if (obj is INotObject) sb.Append("INOT ");
            if (obj is IOrObject) sb.Append("IOR ");
            if (obj is IPrimitiveObject) sb.Append("IPRIMITIVE ");
            if (obj is IPutObject) sb.Append("IPUT ");
            if (obj is IRemoveAtObject) sb.Append("IREMOVEAT ");
            if (obj is IRemoveObject) sb.Append("IREMOVE ");
            if (obj is ISetObject) sb.Append("ISET ");
            if (obj is ISetSizeObject) sb.Append("ISETSIZE ");
            if (obj is ISubtractObject) sb.Append("ISUBTRACT ");
            if (obj is ITrimObject) sb.Append("ITRIM ");
            if (obj is IWriteObject) sb.Append("IWRITE ");
            if (obj is IXorObject) sb.Append("IXOR ");
        }

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

            int ldX = 0;
            int ldY = bitmap.Height - 1;
            var ldC = bitmap.GetPixel(ldX, ldY);

            int rdX = bitmap.Width - 1;
            int rdY = bitmap.Height - 1;
            var rdC = bitmap.GetPixel(rdX, rdY);

            int ruX = bitmap.Width - 1;
            int ruY = 0;
            var ruC = bitmap.GetPixel(ruX, ruY);

            int luX = 0;
            int luY = 0;
            var luC = bitmap.GetPixel(luX, luY);



            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var cA = Interpolate2D(
                        x, y,
                        ldX, ldY, ldC.A,
                        rdX, rdY, rdC.A,
                        ruX, ruY, ruC.A,
                        luX, luY, luC.A

                    );
                    var cR = Interpolate2D(
                        x, y,
                        ldX, ldY, ldC.R,
                        rdX, rdY, rdC.R,
                        ruX, ruY, ruC.R,
                        luX, luY, luC.R

                    );
                    var cG = Interpolate2D(
                        x, y,
                        ldX, ldY, ldC.G,
                        rdX, rdY, rdC.G,
                        ruX, ruY, ruC.G,
                        luX, luY, luC.G

                    );
                    var cB = Interpolate2D(
                        x, y,
                        ldX, ldY, ldC.B,
                        rdX, rdY, rdC.B,
                        ruX, ruY, ruC.B,
                        luX, luY, luC.B

                    );
                    var p = Color.FromArgb((byte)Math.Round(cA), (byte)Math.Round(cR), (byte)Math.Round(cG), (byte)Math.Round(cB));

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
            (double u, double v) = MapToUnitSquare(x1, y1, x2, y2, x3, y3, x4, y4, x, y);

            // Perform bilinear interpolation using the mapped coordinates (u, v)
            return (1 - u) * (1 - v) * Q1 +
                u * (1 - v) * Q2 +
                u * v * Q3 +
                (1 - u) * v * Q4;
        }

        // Function to map the point (x, y) to the unit square coordinates (u, v)
        private static (double, double) MapToUnitSquare(
            double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4,
            double x, double y)
        {
            // Solve for u and v using bilinear coordinate mapping
            double denominator = (x2 - x1 + x3 - x4) * (y4 - y1) - (y2 - y1 + y3 - y4) * (x4 - x1);

            double uNumerator = (x - x1) * (y4 - y1) - (y - y1) * (x4 - x1);
            double vNumerator = (x2 - x1 + x3 - x4) * (y - y1) - (y2 - y1 + y3 - y4) * (x - x1);

            double u = uNumerator / denominator;
            double v = vNumerator / denominator;

            return (u, v);
        }
    }
}
