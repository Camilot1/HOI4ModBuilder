using QuickFont.Configuration;
using QuickFont;
using OpenTK.Graphics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System;

namespace HOI4ModBuilder.src.openTK
{
    public class FontData : IDisposable
    {
        public int Size { get; private set; }
        public QFont Font { get; private set; }
        public bool IsDisposed { get; private set; }

        public FontData(int size, QFont font)
        {
            Size = size;
            Font = font;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            Font?.Dispose();
            IsDisposed = true;
        }
    }

    public class TextRenderManager
    {
        public static readonly TextRenderManager Instance = new TextRenderManager();

        public FontData FontData { get; private set; }
        private QFontDrawing _drawing;

        private Dictionary<object, QFontDrawingPimitive> _primitiveCache = new Dictionary<object, QFontDrawingPimitive>(256);

        public void OnLoad()
        {
            FontData = new FontData(64, new QFont("data/fonts/previewer_arial.ttf", 64, new QFontBuilderConfiguration(true)));
            _drawing = new QFontDrawing();
        }

        public void RefreshBuffers()
        {
            _drawing.RefreshBuffers();
            _drawing.DisableShader(); //Fixes display blinking
        }

        public SizeF SetText(object id, FontData fontData, string text, Vector3 pos, float scale, QFontAlignment aligment, Color color, bool dropShadows)
        {
            var size = SetTextMulti(id, fontData, text, pos, scale, aligment, color, dropShadows);
            _drawing.RefreshBuffers();
            return size;
        }
        public SizeF SetTextMulti(object id, FontData fontData, string text, Vector3 pos, float scale, QFontAlignment aligment, Color color, bool dropShadows)
        {
            if (_primitiveCache.TryGetValue(id, out var dp))
            {
                _drawing.DrawingPimitiveses.Remove(dp);
                _primitiveCache.Remove(id);
            }

            pos.X = pos.X / scale;
            pos.Y = fontData.Size / 3f * 2f + pos.Y / scale;

            dp = new QFontDrawingPimitive(fontData.Font, new QFontRenderOptions
            {
                Colour = color,
                DropShadowActive = dropShadows,
            });
            _primitiveCache.Add(id, dp);

            var size = dp.Print(text, pos, aligment);
            _drawing.DrawingPimitiveses.Add(dp);

            return size;
        }
        public bool RemoveText(object id)
        {
            var result = RemoveTextMulti(id);
            if (result)
                RefreshBuffers();
            return result;
        }
        public bool RemoveTextMulti(object id)
        {
            if (!_primitiveCache.TryGetValue(id, out var dp))
                return false;

            _primitiveCache.Remove(id);
            _drawing.DrawingPimitiveses.Remove(dp);
            return true;
        }

        public void ClearAll()
        {
            var result = _drawing.DrawingPimitiveses.Count != 0;
            ClearAllMulti();

            if (result)
                RefreshBuffers();
        }

        public void ClearAllMulti()
        {
            _primitiveCache.Clear();
            _drawing.DrawingPimitiveses.Clear();
        }

        public void Render(Matrix4 proj, double zoomFactor)
        {
            if (_drawing == null || _drawing.DrawingPimitiveses.Count == 0 || _drawing._vertexArrayObject == null)
            {
                if (_drawing?._vertexArrayObject != null)
                    _drawing.DisableShader();
                return;
            }

            _drawing.ProjectionMatrix = proj;
            _drawing.Draw();
            _drawing.DisableShader();
        }

        public void Dispose()
        {
            _drawing?.Dispose();
            _drawing = null;

            FontData?.Dispose();
            FontData = null;
        }
    }
}
