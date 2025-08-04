using QuickFont.Configuration;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using SharpFont.TrueType;
using System.Drawing;
using OpenTK;

namespace HOI4ModBuilder.src.openTK
{
    public class TextManager
    {
        public static readonly TextManager Instance = new TextManager();

        private QFont _font;
        private QFontDrawing _drawing;

        public void OnLoad()
        {
            //_myFont = new QFont("data/fonts/HappySans.ttf", 72, new QFontBuilderConfiguration(true));
            _font = new QFont("data/fonts/HappySans.ttf", 72, new QFontBuilderConfiguration(true));
            _drawing = new QFontDrawing();
        }

        public void Update()
        {
            _drawing.DrawingPrimitives.Clear();
            // draw with options
            var textOpts = new QFontRenderOptions()
            {
                Colour = Color.FromArgb(new Color4(0.8f, 0.1f, 0.1f, 1.0f).ToArgb()),
                DropShadowActive = true
            };
            SizeF size = _drawing.Print(_font, "text1", new Vector3(), QFontAlignment.Left, textOpts);

            // after all changes do update buffer data and extend it's size if needed.
            _drawing.RefreshBuffers();

        }

        public void Render(Matrix4 proj)
        {
            _drawing.ProjectionMatrix = proj;
            _drawing.Draw();
            MainForm.Instance.glControl.SwapBuffers();
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
