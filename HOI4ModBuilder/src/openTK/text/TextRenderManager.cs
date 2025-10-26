using QuickFont.Configuration;
using QuickFont;
using System.Collections.Generic;
using HOI4ModBuilder.src.openTK.text;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.openTK
{
    public class TextRenderManager
    {
        public static readonly TextRenderManager Instance = new TextRenderManager();

        private FontData _fondData24;
        private FontData _fondData32;
        private FontData _fondData48;
        private FontData _fondData64;
        private FontData _fondData72;
        public FontData FontData24 { get => _fondData24; private set => _fondData24 = value; }
        public FontData FontData32 { get => _fondData32; private set => _fondData32 = value; }
        public FontData FontData48 { get => _fondData48; private set => _fondData48 = value; }
        public FontData FontData64 { get => _fondData64; private set => _fondData64 = value; }
        public FontData FontData72 { get => _fondData72; private set => _fondData72 = value; }
        private QFontDrawing _drawing;

        private Dictionary<object, QFontDrawingPimitive> _primitiveCache = new Dictionary<object, QFontDrawingPimitive>(256);

        public TextRenderManager() { }

        public static bool IsLoaded { get; private set; } = false;

        public void Load()
        {
            if (IsLoaded)
                return;

            _drawing = new QFontDrawing();

            var actions = new LocalizedAction[] {
                //FontData24 = new FontData(24, new QFont("data/fonts/previewer_arial.ttf", 24, new QFontBuilderConfiguration(true)));
                //FontData32 = new FontData(32, new QFont("data/fonts/previewer_arial.ttf", 32, new QFontBuilderConfiguration(true)));
                //FontData48 = new FontData(48, new QFont("data/fonts/previewer_arial.ttf", 48, new QFontBuilderConfiguration(true)));
                new LocalizedAction(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_TEXT_FONTS, () => LoadFont(ref _fondData64, 64, "data/fonts/previewer_arial.ttf"))
                //FontData72 = new FontData(72, new QFont("data/fonts/previewer_arial.ttf", 72, new QFontBuilderConfiguration(true)));
            };

            MainForm.ExecuteActions(actions);

            IsLoaded = true;
        }

        private void LoadFont(ref FontData fontData, int size, string path)
        => fontData = new FontData(size, new QFont(path, size, new QFontBuilderConfiguration(true)));
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
