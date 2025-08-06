using OpenTK;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HOI4ModBuilder.src.openTK.text
{
    public class FontRenderRegion : IDisposable
    {
        private readonly QFontDrawing _drawing;
        private bool _isDisposed;

        private Dictionary<object, QFontDrawingPimitive> _primitiveCache = new Dictionary<object, QFontDrawingPimitive>(128);
        private readonly Queue<Action<FontRenderRegion>> _actionQueue = new Queue<Action<FontRenderRegion>>(128);

        public FontRenderRegion()
        {
            _drawing = new QFontDrawing();
            _drawing.RefreshBuffers_Step1_InitVAO();
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _drawing?.Dispose();
            _primitiveCache.Clear();
        }

        public bool ClearAllMulti()
        {
            _primitiveCache.Clear();
            int count = _drawing.DrawingPimitiveses.Count;
            _drawing.DrawingPimitiveses.Clear();
            return count > 0;
        }

        public void PushAction(Action<FontRenderRegion> action) => _actionQueue.Enqueue(action);
        public void RefreshBuffers() => _drawing?.RefreshBuffers();
        public void LoadVAO() => _drawing.RefreshBuffers_Step3_LoadVAO();

        public void ExecuteActions()
        {
            while (_actionQueue.Count > 0)
            {
                var action = _actionQueue.Dequeue();
                action(this);
            }
            _drawing.RefreshBuffers_Step2_AddVertexes();
        }

        public SizeF SetText(object id, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
        {
            var size = SetTextMulti(id, fontData, scale, text, pos, aligment, color, dropShadows);
            _drawing.RefreshBuffers();
            return size;
        }
        public SizeF SetTextMulti(object id, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
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

        public void Render(Matrix4 proj)
        {
            if (_drawing.DrawingPimitiveses.Count == 0 || _drawing._vertexArrayObject == null)
                return;

            _drawing.ProjectionMatrix = proj;
            _drawing.Draw();
            _drawing.DisableShader();
        }
    }
}
