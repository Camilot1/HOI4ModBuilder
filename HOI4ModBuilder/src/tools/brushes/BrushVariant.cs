using HOI4ModBuilder.src.utils.borders;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HOI4ModBuilder.src.tools.brushes
{
    public class BrushVariant
    {
        public Value2I[] pixels;
        public List<List<Value2S>> lineStrips = new List<List<Value2S>>();

        public int OriginalWidth { get; private set; }
        public int OriginalHeight { get; private set; }
        public int CenterOffsetX { get; private set; }
        public int CenterOffsetY { get; private set; }


        public void ForEachPixel(Point2D center, Action<int, int> action)
            => ForEachPixel(center.x, center.y, action);

        public void ForEachPixel(double x, double y, Action<int, int> action)
        {
            double snappedCenterX;
            double snappedCenterY;

            if (OriginalWidth % 2 != 0)
                snappedCenterX = Math.Floor(x);
            else
                snappedCenterX = Math.Round(x);

            if (OriginalHeight % 2 != 0)
                snappedCenterY = Math.Floor(y);
            else
                snappedCenterY = Math.Round(y);

            foreach (var point in pixels)
            {
                int targetX = (int)(point.x + snappedCenterX);
                int targetY = (int)(point.y + snappedCenterY);

                action(targetX, targetY);
            }
        }

        public void ForEachLineStrip(Point2D center, Action<List<Value2S>, double, double> action)
            => ForEachLineStrip(center.x, center.y, action);

        public void ForEachLineStrip(double x, double y, Action<List<Value2S>, double, double> action)
        {
            double snappedCenterX;
            double snappedCenterY;

            if (OriginalWidth % 2 != 0)
                snappedCenterX = Math.Floor(x);
            else
                snappedCenterX = Math.Round(x);

            if (OriginalHeight % 2 != 0)
                snappedCenterY = Math.Floor(y);
            else
                snappedCenterY = Math.Round(y);

            foreach (var line in lineStrips)
            {
                double xOffset = snappedCenterX + CenterOffsetX;
                double yOffset = snappedCenterY + CenterOffsetY;

                action(line, xOffset, yOffset);
            }
        }

        public void Load(Bitmap bitmap)
        {
            OriginalWidth = bitmap.Width;
            OriginalHeight = bitmap.Height;
            CenterOffsetX = -(bitmap.Width / 2);
            CenterOffsetY = -(bitmap.Height / 2);

            var centeredPixels = new List<Value2I>();
            int[,] usedPixel = new int[bitmap.Width + 2, bitmap.Height + 2];

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bool isOpaque = bitmap.GetPixel(x, y).A > 0;
                    if (isOpaque)
                    {
                        centeredPixels.Add(new Value2I() { x = x + CenterOffsetX, y = y + CenterOffsetY });
                        usedPixel[x + 1, y + 1] = 1;
                    }
                    else
                    {
                        usedPixel[x + 1, y + 1] = 0;
                    }
                }
            }
            pixels = centeredPixels.ToArray();
            var bordersAssembler = new BordersAssembler();

            for (int y = 0; y < bitmap.Height + 1; y++)
            {
                int y1 = y + 1;
                for (int x = 0; x < bitmap.Width + 1; x++)
                {
                    int x1 = x + 1;
                    bordersAssembler.AcceptBorderPixel(
                        x, y, // Координаты "верхнего левого" пикселя (оригинальные)
                        usedPixel[x, y],     // Значение NW в usedPixel (x, y)
                        usedPixel[x1, y],    // Значение NE в usedPixel (x+1, y)
                        usedPixel[x1, y1],   // Значение SE в usedPixel (x+1, y+1)
                        usedPixel[x, y1]     // Значение SW в usedPixel (x, y+1)
                     );
                }
            }

            lineStrips.Clear();
            foreach (var entry in bordersAssembler.BordersData)
            {
                foreach (var data in entry.Value)
                {
                    var pixelsLists = data.AssembleBorders((short)(bitmap.Width + 1));
                    if (pixelsLists != null)
                        lineStrips.AddRange(pixelsLists);
                }
            }

            bordersAssembler.Reset();
        }
    }
}
