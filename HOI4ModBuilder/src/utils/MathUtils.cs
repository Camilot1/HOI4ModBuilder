
namespace HOI4ModBuilder.src.utils
{
    public class MathUtils
    {

        public static bool InboundsPositiveBox(double x, double y, double width, double height)
            => x >= 0 && x <= width && y >= 0 && y <= height;

        public static bool Inbounds(double x, double y, double minX, double minY, double maxX, double maxY)
            => x >= minX && x <= maxX && y >= minY && y <= maxY;

        public static bool InboundsPositiveBox(int x, int y, int width, int height)
            => x >= 0 && x <= width && y >= 0 && y <= height;

        public static bool Inbounds(int x, int y, int minX, int minY, int maxX, int maxY)
            => x >= minX && x <= maxX && y >= minY && y <= maxY;


        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        public static int Clamp(int i, int min, int max)
        {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
        public static float Clamp(float i, float min, float max)
        {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
        public static double Clamp(double i, double min, double max)
        {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
    }
}
