using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
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
        public int ChacheCount => _primitiveCache.Count;
        public bool TryRemoveDrawingPrimitive(object id)
        {
            if (_primitiveCache.TryGetValue(id, out var dp))
            {
                _drawing.DrawingPimitiveses.Remove(dp);
                _primitiveCache.Remove(id);
                IsDirtyPost = true;
                return true;
            }
            return false;
        }

        private readonly Queue<Action<FontRenderRegion>> _actionQueue = new Queue<Action<FontRenderRegion>>(128);
        private readonly List<Action> _postActions = new List<Action>(16);

        public bool IsDirty { get; private set; }
        public bool IsDirtyPost { get; private set; }

        public readonly FontRenderController Controller;
        public readonly Value2S Index;
        public readonly Bounds4F Bounds;
        public bool IsIntersectsWith(Bounds4F other) => Bounds.IsIntersectsWith(other);

        public int loadedVertexCount = 0;

        public FontRenderRegion(FontRenderController controller, Value2S index, int regionSize)
        {
            Controller = controller;
            Index = index;
            _drawing = new QFontDrawing();
            _drawing.RefreshBuffers_Step1_InitVAO();

            Bounds = new Bounds4F
            {
                left = index.x * regionSize,
                top = index.y * regionSize,
                right = (index.x + 1) * regionSize,
                bottom = (index.y + 1) * regionSize,
            };
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
            IsDirty |= count != 0;
            _drawing.DrawingPimitiveses.Clear();
            return count > 0;
        }

        public void PushAction(Action<FontRenderRegion> action) => _actionQueue.Enqueue(action);
        public void RefreshBuffers()
        {
            _drawing?.RefreshBuffers();
            IsDirty = false;
        }
        public void LoadVAO()
        {
            if (IsDirty)
            {
                if (IsDirtyPost)
                {
                    _drawing.RefreshBuffers_Step2_AddVertexes();
                    IsDirtyPost = false;
                }
                _drawing.RefreshBuffers_Step3_LoadVAO();
                loadedVertexCount = _drawing.GetVAO().VertexCount;
                //Logger.Log("Loaded: " + Bounds);
                IsDirty = false;
            }
            else if (IsDirtyPost)
            {
                _drawing.RefreshBuffers_Step2_AddVertexes();
                _drawing.RefreshBuffers_Step3_LoadVAO();
                loadedVertexCount = _drawing.GetVAO().VertexCount;
                IsDirtyPost = false;
            }
        }

        public void DebugLog()
        {
            int countA = 0;
            foreach (var p in _primitiveCache.Values)
                countA += p.ShadowVertexRepr.Count + p.CurrentVertexRepr.Count;

            Logger.Log($"REGION {Index}: Chached={countA}; LoadedVertexCount={loadedVertexCount}; IsDifferent: {countA != loadedVertexCount}");
        }

        public void ExecuteActions()
        {
            while (_actionQueue.Count > 0)
            {
                var action = _actionQueue.Dequeue();
                action(this);
            }
            if (IsDirty)
                _drawing.RefreshBuffers_Step2_AddVertexes();
        }

        public void ExecutePostActions()
        {
            foreach (var action in _postActions)
                action();
            _postActions.Clear();
        }

        public void AddUpdateCachedRegionActionIfNeeded(object id)
        {
            var previousRegionById = Controller.GetCachedRegion(id);
            if (previousRegionById == null)
                _postActions.Add(() => Controller.SetCachedRegion(id, this));
            else if (previousRegionById != this)
                _postActions.Add(() =>
                {
                    Controller.SetCachedRegion(id, this);
                    previousRegionById.TryRemoveDrawingPrimitive(id);
                });
        }

        public SizeF SetText(object id, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
        {
            var size = SetTextMulti(id, fontData, scale, text, pos, aligment, color, dropShadows);
            RefreshBuffers();
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

            AddUpdateCachedRegionActionIfNeeded(id);

            IsDirty = true;

            return size;
        }

        public bool RemoveText(object id)
        {
            if (!RemoveTextMulti(id))
                return false;

            RefreshBuffers();
            return true;
        }


        public bool RemoveTextMulti(object id)
        {
            if (!_primitiveCache.TryGetValue(id, out var dp))
            {
                AddUpdateCachedRegionActionIfNeeded(id);
                return false;
            }

            _drawing.DrawingPimitiveses.Remove(dp);
            _primitiveCache.Remove(id);

            AddUpdateCachedRegionActionIfNeeded(id);

            IsDirty = true;

            return true;
        }

        public bool RemoveTextsMulti(ICollection<object> ids)
        {
            bool result = false;
            foreach (var id in ids)
                result |= RemoveTextMulti(id);
            return result;
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
