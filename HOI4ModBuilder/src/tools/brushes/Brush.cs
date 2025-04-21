using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HOI4ModBuilder.src.tools.brushes
{
    public class Brush
    {
        public BrushInfo BrushInfo { get; private set; }
        public List<object> SortedVariantKeys { get; private set; }
        public Dictionary<string, BrushVariant> Variants { get; private set; }

        public Brush()
        {
            Variants = new Dictionary<string, BrushVariant>();
        }
        public Brush(BrushInfo brushInfo) : this()
        {
            BrushInfo = brushInfo;
            SortedVariantKeys = new List<object>();
        }

        public void LoadVariant(object name, Bitmap bitmap)
        {
            var variant = new BrushVariant();
            variant.Load(bitmap);
            Variants[name.ToString()] = variant;
            SortedVariantKeys.Add(name);
        }

        public void ForEachPixel(string variantName, Point2D center, Action<int, int> action)
            => ForEachPixel(variantName, center.x, center.y, action);

        public void ForEachPixel(string variantName, double x, double y, Action<int, int> action)
        {
            if (!Variants.TryGetValue(variantName, out var variant))
                return;

            variant.ForEachPixel(x, y, action);
        }

        public void ForEachLineStrip(string variantName, Point2D center, Action<List<Value2S>, double, double> action)
            => ForEachLineStrip(variantName, center.x, center.y, action);

        public void ForEachLineStrip(string variantName, double x, double y, Action<List<Value2S>, double, double> action)
        {
            if (!Variants.TryGetValue(variantName, out var variant))
                return;

            variant.ForEachLineStrip(x, y, action);
        }
    }
}
