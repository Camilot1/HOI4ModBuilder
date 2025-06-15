
namespace HOI4ModBuilder.src.utils
{
    public class MathUtils
    {

        public static bool InboundsPositiveBox(double x, double y, double width, double height)
        {
            return x >= 0 && x <= width && y >= 0 && y <= height;
        }

        public static bool Inbounds(double x, double y, double minX, double minY, double maxX, double maxY)
        {
            return x >= minX && x <= maxX && y >= minY && y <= maxY;
        }

        public static bool InboundsPositiveBox(int x, int y, int width, int height)
        {
            return x >= 0 && x <= width && y >= 0 && y <= height;
        }

        public static bool Inbounds(int x, int y, int minX, int minY, int maxX, int maxY)
        {
            return x >= minX && x <= maxX && y >= minY && y <= maxY;
        }

    }
}
