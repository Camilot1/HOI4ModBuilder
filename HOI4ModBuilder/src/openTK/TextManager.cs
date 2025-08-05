using QuickFont.Configuration;
using QuickFont;
using OpenTK.Graphics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.openTK
{
    public class TextManager
    {
        public static readonly TextManager Instance = new TextManager();

        private QFont _font;
        private QFontDrawing _drawing;

        private Dictionary<object, QFontDrawingPimitive> _primitiveCache = new Dictionary<object, QFontDrawingPimitive>(256);

        public void OnLoad()
        {
            //_myFont = new QFont("data/fonts/HappySans.ttf", 72, new QFontBuilderConfiguration(true));
            _font = new QFont("data/fonts/HappySans.ttf", 72, new QFontBuilderConfiguration(true));
            _drawing = new QFontDrawing();

            Update();
        }

        public void Update()
        {
            _drawing.DrawingPimitiveses.Clear();
            // draw with options
            var textOpts = new QFontRenderOptions()
            {
                Colour = Color.FromArgb(new Color4(0.8f, 0.1f, 0.1f, 1.0f).ToArgb()),
                DropShadowActive = true
            };
            SizeF size = _drawing.Print(_font, "text1", new Vector3(0, 0, 0), QFontAlignment.Left, textOpts);


            var dp = new QFontDrawingPimitive(_font);

            size = dp.Print("text2", new Vector3(100, 72, 0), new SizeF(300, float.MaxValue), QFontAlignment.Centre);
            _drawing.DrawingPimitiveses.Add(dp);

            // after all changes do update buffer data and extend it's size if needed.
            _drawing.RefreshBuffers();

        }

        public void Render(Matrix4 proj)
        {
            if (_drawing == null)
                return;

            Update();

            _drawing.ProjectionMatrix = proj;
            _drawing.Draw();
            _drawing.DisableShader();
        }

        public void Dispose()
        {
            _drawing?.Dispose();
            _drawing = null;

            _font?.Dispose();
            _font = null;
        }
    }
}
