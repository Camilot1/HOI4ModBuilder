using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils
{
    public static class NormalMapGenerator
    {
        // Добавляем небольшую константу для сравнения float
        private const float Epsilon = 1e-6f;

        /// <summary>
        /// Генерирует карту нормалей из заданной карты высот с опциональным размытием Гаусса.
        /// </summary>
        /// <param name="heightmap">Карта высот в формате Bitmap 8bppIndexed.</param>
        /// <param name="strength">Множитель "силы" карты нормалей.</param>
        /// <param name="blurSigma">Сила размытия Гаусса (стандартное отклонение). 0 - без размытия.
        /// Значения около 0.5-0.8 соответствуют легкому размытию, 1.0 и выше - более сильному.</param>
        /// <returns>Сгенерированная карта нормалей в формате Bitmap 24bppRgb, в два раза меньше карты высот.</returns>
        /// <exception cref="ArgumentNullException">...</exception>
        /// <exception cref="ArgumentException">...</exception>
        /// <exception cref="ArgumentOutOfRangeException">Вызывается, если blurSigma отрицательный.</exception>
        public static Bitmap GenerateNormalMap(Bitmap heightmap, float strength, float blurSigma)
        {
            if (heightmap == null) throw new ArgumentNullException(nameof(heightmap));
            if (heightmap.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_BITMAP_INVALID_PIXEL_FORMAT, new Dictionary<string, string> {
                    { "{name}", "heightmap.bmp" },
                    { "{currentFormat}", $"{heightmap.PixelFormat}" },
                    { "{validFormat}", $"{PixelFormat.Format8bppIndexed }" }
                }));
            // Сигма не может быть отрицательной
            if (blurSigma < 0.0f)
                throw new ArgumentException(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_INVALID_BLUR_SIGMA_VALUE, new Dictionary<string, string> {
                    { "{name}", "normals_map.bmp" },
                    { "{blurSigma}", $"{blurSigma}" },
                }));

            int hmWidth = heightmap.Width;
            int hmHeight = heightmap.Height;
            int nmWidth = hmWidth / 2;
            int nmHeight = hmHeight / 2;

            if (nmWidth < 1 || nmHeight < 1)
                throw new ArgumentException(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_BITMAP_IS_TOO_SMALL, new Dictionary<string, string> {
                    { "{name}", "heightmap.bmp" },
                    { "{width}", $"{hmWidth}" },
                    { "{height}", $"{hmHeight}" }
                }));

            Bitmap normalMap = new Bitmap(nmWidth, nmHeight, PixelFormat.Format24bppRgb);

            BitmapData hmData = null;
            BitmapData nmData = null;
            byte[] tempBlurBuffer = null;

            try
            {
                hmData = heightmap.LockBits(new Rectangle(0, 0, hmWidth, hmHeight), ImageLockMode.ReadOnly, heightmap.PixelFormat);
                nmData = normalMap.LockBits(new Rectangle(0, 0, nmWidth, nmHeight), ImageLockMode.ReadWrite, normalMap.PixelFormat);

                IntPtr hmScan0 = hmData.Scan0;
                IntPtr nmScan0 = nmData.Scan0;
                int hmStride = hmData.Stride;
                int nmStride = nmData.Stride;
                int nmBytesPerPixel = 3;
                long nmTotalBytes = (long)nmStride * nmHeight;

                unsafe
                {
                    byte* hmBase = (byte*)hmScan0.ToPointer();
                    byte* nmBase = (byte*)nmScan0.ToPointer();

                    // --- Шаг 1: Генерация базовой карты нормалей ---
                    Parallel.For(0, nmHeight, ny =>
                    {
                        byte* nmRow = nmBase + (ny * nmStride);
                        for (int nx = 0; nx < nmWidth; nx++)
                        {
                            int hx_center = nx * 2 + 1;
                            int hy_center = ny * 2 + 1;

                            float hL = GetHeightSafePointer(hmBase, hmStride, hx_center - 1, hy_center, hmWidth, hmHeight) / 255.0f;
                            float hR = GetHeightSafePointer(hmBase, hmStride, hx_center + 1, hy_center, hmWidth, hmHeight) / 255.0f;
                            float hB = GetHeightSafePointer(hmBase, hmStride, hx_center, hy_center - 1, hmWidth, hmHeight) / 255.0f;
                            float hT = GetHeightSafePointer(hmBase, hmStride, hx_center, hy_center + 1, hmWidth, hmHeight) / 255.0f;

                            float dX = (hR - hL);
                            float dY = (hT - hB);

                            float vecX = -dX * strength;
                            float vecY = dY * strength;

                            byte r = ClampToByte(vecX * 127.5f + 128.0f);
                            byte g = ClampToByte(vecY * 127.5f + 128.0f);
                            byte b = 255;

                            int nmOffset = nx * nmBytesPerPixel;
                            nmRow[nmOffset + 0] = b;
                            nmRow[nmOffset + 1] = g;
                            nmRow[nmOffset + 2] = r;
                        }
                    });

                    // --- Шаг 2: Применение размытия Гаусса, если blurSigma достаточно большой ---
                    // Используем Epsilon для сравнения с нулем
                    if (blurSigma > Epsilon)
                    {
                        tempBlurBuffer = new byte[nmTotalBytes];
                        // Вычисляем ядро на основе sigma
                        float[] kernel = CalculateGaussianKernel1D(blurSigma);
                        int kernelHalfWidth = kernel.Length / 2; // Целочисленный радиус ядра

                        // Копируем исходные данные в буфер
                        Marshal.Copy(nmScan0, tempBlurBuffer, 0, (int)nmTotalBytes);

                        // --- Проход 1: Горизонтальное размытие ---
                        // Читаем из tempBlurBuffer, пишем в nmBase
                        Parallel.For(0, nmHeight, y =>
                        {
                            int rowOffset = y * nmStride;
                            byte* nmRowWrite = nmBase + rowOffset;

                            for (int x = 0; x < nmWidth; x++)
                            {
                                float sumR = 0, sumG = 0;
                                int pixelBaseOffset = x * nmBytesPerPixel;

                                for (int k = -kernelHalfWidth; k <= kernelHalfWidth; k++)
                                {
                                    int sourceX = ClampInt(x + k, 0, nmWidth - 1);
                                    int sourceIndex = rowOffset + sourceX * nmBytesPerPixel;
                                    float weight = kernel[k + kernelHalfWidth];

                                    sumG += tempBlurBuffer[sourceIndex + 1] * weight;
                                    sumR += tempBlurBuffer[sourceIndex + 2] * weight;
                                }

                                nmRowWrite[pixelBaseOffset + 0] = 255; // B
                                nmRowWrite[pixelBaseOffset + 1] = ClampToByte(sumG); // G
                                nmRowWrite[pixelBaseOffset + 2] = ClampToByte(sumR); // R
                            }
                        });

                        // Копируем горизонтально размытые данные обратно в буфер
                        Marshal.Copy(nmScan0, tempBlurBuffer, 0, (int)nmTotalBytes);

                        // --- Проход 2: Вертикальное размытие ---
                        // Читаем из tempBlurBuffer, пишем обратно в nmBase
                        Parallel.For(0, nmHeight, y =>
                        {
                            byte* nmRowWrite = nmBase + (y * nmStride);

                            for (int x = 0; x < nmWidth; x++)
                            {
                                float sumR = 0, sumG = 0;
                                int pixelOffsetWrite = x * nmBytesPerPixel;

                                for (int k = -kernelHalfWidth; k <= kernelHalfWidth; k++)
                                {
                                    int sourceY = ClampInt(y + k, 0, nmHeight - 1);
                                    int sourceIndex = sourceY * nmStride + pixelOffsetWrite;
                                    float weight = kernel[k + kernelHalfWidth];

                                    sumG += tempBlurBuffer[sourceIndex + 1] * weight;
                                    sumR += tempBlurBuffer[sourceIndex + 2] * weight;
                                }

                                nmRowWrite[pixelOffsetWrite + 0] = 255; // B
                                nmRowWrite[pixelOffsetWrite + 1] = ClampToByte(sumG); // G
                                nmRowWrite[pixelOffsetWrite + 2] = ClampToByte(sumR); // R
                            }
                        });

                    } // Конец if (blurSigma > Epsilon)

                } // конец unsafe блока
            }
            finally
            {
                if (hmData != null) heightmap.UnlockBits(hmData);
                if (nmData != null) normalMap.UnlockBits(nmData);
            }

            return normalMap;
        }

        /// <summary>
        /// Безопасно получает значение высоты (0-255) из данных карты высот, используя указатель.
        /// </summary>
        private static unsafe byte GetHeightSafePointer(byte* basePtr, int stride, int x, int y, int width, int height)
        {
            x = ClampInt(x, 0, width - 1);
            y = ClampInt(y, 0, height - 1);
            return *(basePtr + y * stride + x);
        }

        /// <summary>
        /// Ограничивает значение float диапазоном [0, 255] и преобразует в byte.
        /// </summary>
        private static byte ClampToByte(float value)
        {
            if (value < 0.0f) return 0;
            if (value > 255.0f) return 255;
            return (byte)value;
        }

        /// <summary>
        /// Ограничивает целочисленное значение заданным диапазоном [min, max].
        /// </summary>
        private static int ClampInt(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Вычисляет нормализованное 1D ядро Гаусса на основе стандартного отклонения sigma.
        /// </summary>
        /// <param name="sigma">Стандартное отклонение (сила размытия).</param>
        /// <returns>Массив весов ядра.</returns>
        private static float[] CalculateGaussianKernel1D(float sigma)
        {
            // Определяем радиус ядра на основе sigma.
            // Ядро должно быть достаточно большим, чтобы охватить ~99.7% кривой (3 сигмы).
            // Радиус должен быть целым числом. Берем округление вверх.
            // Минимальный радиус = 1, чтобы ядро было хотя бы 3x1.
            int radius = Math.Max(1, (int)Math.Ceiling(3.0f * sigma));
            int kernelSize = 2 * radius + 1;
            float[] kernel = new float[kernelSize];

            // Предотвращаем деление на ноль для очень малых sigma
            if (sigma < Epsilon) sigma = Epsilon;

            float sigmaSq2 = 2 * sigma * sigma;
            float norm = 1.0f / (float)(Math.Sqrt(2.0 * Math.PI) * sigma);
            float sum = 0;

            for (int i = 0; i < kernelSize; i++)
            {
                int x = i - radius; // Координата относительно центра ядра [-radius, +radius]
                                    // Считаем вес по формуле Гаусса, используя sigma напрямую
                kernel[i] = norm * (float)Math.Exp(-(x * x) / sigmaSq2);
                sum += kernel[i];
            }

            // Нормализация, чтобы сумма весов была равна 1.0
            if (Math.Abs(sum) > Epsilon)
            {
                for (int i = 0; i < kernelSize; i++)
                {
                    kernel[i] /= sum;
                }
            }
            else if (kernelSize > 0)
            {
                // Маловероятный случай, если сумма почти ноль
                kernel[radius] = 1.0f;
            }

            return kernel;
        }
    }
}