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
    }
}
