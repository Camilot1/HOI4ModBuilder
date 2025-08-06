using QuickFont.Configuration;
using QuickFont;
using System.Collections.Generic;
using HOI4ModBuilder.src.openTK.text;

namespace HOI4ModBuilder.src.openTK
{
    public class TextRenderManager
    {
        public static readonly TextRenderManager Instance = new TextRenderManager();

        public FontData FontData24 { get; private set; }
        public FontData FontData32 { get; private set; }
        public FontData FontData48 { get; private set; }
        public FontData FontData64 { get; private set; }
        public FontData FontData72 { get; private set; }
        private QFontDrawing _drawing;

        private Dictionary<object, QFontDrawingPimitive> _primitiveCache = new Dictionary<object, QFontDrawingPimitive>(256);

        public void OnLoad()
        {
            FontData24 = new FontData(24, new QFont("data/fonts/previewer_arial.ttf", 24, new QFontBuilderConfiguration(true)));
            FontData32 = new FontData(32, new QFont("data/fonts/previewer_arial.ttf", 32, new QFontBuilderConfiguration(true)));
            FontData48 = new FontData(48, new QFont("data/fonts/previewer_arial.ttf", 48, new QFontBuilderConfiguration(true)));
            FontData64 = new FontData(64, new QFont("data/fonts/previewer_arial.ttf", 64, new QFontBuilderConfiguration(true)));
            FontData72 = new FontData(72, new QFont("data/fonts/previewer_arial.ttf", 72, new QFontBuilderConfiguration(true)));
            _drawing = new QFontDrawing();
        }

        public void Dispose()
        {
            _drawing?.Dispose();
            _drawing = null;

            FontData24?.Dispose();
            FontData24 = null;

            FontData32?.Dispose();
            FontData32 = null;

            FontData48?.Dispose();
            FontData48 = null;

            FontData64?.Dispose();
            FontData64 = null;

            FontData72?.Dispose();
            FontData72 = null;
        }
    }
}
