using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils
{

    public class ImageUtils
    {
        public static Bitmap Reduce8bppIndexedImageSize(Bitmap originalImage)
        {
            int newWidth = originalImage.Width / 2;
            int newHeight = originalImage.Height / 2;
            var reducedImage = new Bitmap(newWidth, newHeight, PixelFormat.Format8bppIndexed);
            reducedImage.Palette = originalImage.Palette;
            var originalData = originalImage.LockBits(new Rectangle(0, 0, originalImage.Width, originalImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            var reducedData = reducedImage.LockBits(new Rectangle(0, 0, reducedImage.Width, reducedImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            unsafe
            {
                byte* originalScan0 = (byte*)originalData.Scan0;
                byte* reducedScan0 = (byte*)reducedData.Scan0;

                int originalStride = originalData.Stride;
                int reducedStride = reducedData.Stride;

                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++)
                    {
                        int originalX = x * 2;
                        int originalY = y * 2;

                        byte p1 = originalScan0[originalY * originalStride + originalX];
                        byte p2 = originalScan0[originalY * originalStride + originalX + 1];
                        byte p3 = originalScan0[(originalY + 1) * originalStride + originalX];
                        byte p4 = originalScan0[(originalY + 1) * originalStride + originalX + 1];

                        byte average = (byte)((p1 + p2 + p3 + p4) / 4);

                        reducedScan0[y * reducedStride + x] = average;
                    }
                }
            }

            originalImage.UnlockBits(originalData);
            reducedImage.UnlockBits(reducedData);

            return reducedImage;
        }

        public static Bitmap GenerateNormalMap(Bitmap heightMap, double strength)
        {
            int width = heightMap.Width;
            int height = heightMap.Height;

            var normalMap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            var heightData = heightMap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            var normalData = normalMap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = heightData.Stride;
            var heightScan0 = heightData.Scan0;
            var normalScan0 = normalData.Scan0;

            unsafe
            {
                byte* heightPtr = (byte*)heightScan0;
                byte* normalPtr = (byte*)normalScan0;

                for (int y = 0; y < height; y++)
                {
                    byte* heightRow = heightPtr + (y * stride);
                    byte* normalRow = normalPtr + (y * stride * 3);

                    for (int x = 0; x < width; x++)
                    {
                        int x0 = (x == 0) ? 0 : x - 1;
                        int x2 = (x == width - 1) ? x : x + 1;
                        int y0 = (y == 0) ? 0 : y - 1;
                        int y2 = (y == height - 1) ? y : y + 1;

                        byte* topLeft = heightRow + x0;
                        byte* top = heightRow + x;
                        byte* topRight = heightRow + x2;
                        byte* left = heightPtr + (y0 * stride) + x0;
                        byte* right = heightPtr + (y0 * stride) + x2;
                        byte* bottomLeft = heightPtr + (y2 * stride) + x0;
                        byte* bottom = heightPtr + (y2 * stride) + x;
                        byte* bottomRight = heightPtr + (y2 * stride) + x2;

                        float gx = topLeft[0] + 2 * left[0] + bottomLeft[0] - topRight[0] - 2 * right[0] - bottomRight[0];
                        float gy = topLeft[0] + 2 * top[0] + topRight[0] - bottomLeft[0] - 2 * bottom[0] - bottomRight[0];

                        float normalX = gx / 255f;
                        float normalY = -gy / 255f;

                        int r = (int)((normalX + 1f) * 0.5f * 255f);
                        int g = (int)((normalY + 1f) * 0.5f * 255f);

                        r += (int)((r - 127) * strength);
                        g += (int)((g - 127) * strength);

                        if (r < 0) r = 0;
                        else if (r > 255) r = 255;

                        if (g < 0) g = 0;
                        else if (g > 255) g = 255;

                        normalRow[x * 3] = 255;
                        normalRow[x * 3 + 1] = (byte)g;
                        normalRow[x * 3 + 2] = (byte)r;
                    }
                }
            }

            heightMap.UnlockBits(heightData);
            normalMap.UnlockBits(normalData);

            return normalMap;
        }
    }
}
