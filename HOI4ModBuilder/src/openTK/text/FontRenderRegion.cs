using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
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
        private static readonly Vector2 TextDropShadowOffset = new Vector2(0.1f, 0.1f);
        private static readonly Color TextDropShadowColor = Color.FromArgb(208, Color.Black);
        private readonly QFontDrawing _drawing;
        private bool _isDisposed;

        private Dictionary<TextRenderKey, QFontDrawingPimitive> _primitiveCache = new Dictionary<TextRenderKey, QFontDrawingPimitive>(128);
        public int ChacheCount => _primitiveCache.Count;
        public bool TryRemoveDrawingPrimitive(TextRenderKey key)
        {
            if (_primitiveCache.TryGetValue(key, out var dp))
            {
                _drawing.DrawingPimitiveses.Remove(dp);
                _primitiveCache.Remove(key);
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

            Logger.Log($"REGION {Index}: Cached={countA}; LoadedVertexCount={loadedVertexCount}; IsDifferent: {countA != loadedVertexCount}");
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

        public void AddUpdateCachedRegionActionIfNeeded(TextRenderKey key)
        {
            var previousRegionById = Controller.GetCachedRegion(key);
            if (ReferenceEquals(previousRegionById, this))
                return;

            if (previousRegionById == null)
                _postActions.Add(() => Controller.SetCachedRegion(key, this));
            else
                _postActions.Add(() =>
                {
                    Controller.SetCachedRegion(key, this);
                    previousRegionById.TryRemoveDrawingPrimitive(key);
                });
        }

        private SizeF SetText(TextRenderKey key, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
        {
            var size = SetTextMulti(key, fontData, scale, text, pos, aligment, color, dropShadows);
            RefreshBuffers();
            return size;
        }

        private SizeF SetTextMulti(TextRenderKey key, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
        {
            bool isNewPrimitive = !_primitiveCache.TryGetValue(key, out var dp) || !ReferenceEquals(dp.Font, fontData.Font);
            if (isNewPrimitive)
            {
                if (dp != null)
                    _drawing.DrawingPimitiveses.Remove(dp);

                dp = new QFontDrawingPimitive(fontData.Font, new QFontRenderOptions());
                _primitiveCache[key] = dp;
                _drawing.DrawingPimitiveses.Add(dp);
            }

            Vector2 targetCenter = new Vector2(pos.X / scale, pos.Y / scale);

            pos.X = targetCenter.X;
            pos.Y = targetCenter.Y;

            AddUpdateCachedRegionActionIfNeeded(key);

            if (!isNewPrimitive && dp.MatchesCachedLayout(text, aligment, color, dropShadows, targetCenter))
                return dp.LastSize;

            dp.ResetGeometry();
            dp.UpdateRenderOptions(color, dropShadows, TextDropShadowOffset, TextDropShadowColor);
            var size = dp.Print(text, pos, aligment);
            dp.RecenterGeometry(targetCenter);
            dp.UpdateCachedLayout(text, aligment, color, dropShadows, targetCenter);

            IsDirty = true;

            return size;
        }

        private bool RemoveText(TextRenderKey key)
        {
            if (!RemoveTextMulti(key))
                return false;

            RefreshBuffers();
            return true;
        }

        private bool RemoveTextMulti(TextRenderKey key)
        {
            if (!_primitiveCache.TryGetValue(key, out var dp))
            {
                AddUpdateCachedRegionActionIfNeeded(key);
                return false;
            }

            _drawing.DrawingPimitiveses.Remove(dp);
            _primitiveCache.Remove(key);

            AddUpdateCachedRegionActionIfNeeded(key);

            IsDirty = true;

            return true;
        }

        public bool RemoveTextsMulti(ICollection<TextRenderKey> keys)
        {
            bool result = false;
            foreach (var key in keys)
                result |= RemoveTextMulti(key);
            return result;
        }

        private SizeF SetEntityText<TEntity>(
            TEntity entity, Func<TEntity, TextRenderKey> keyFactory,
            FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment,
            Color color, bool dropShadows, bool multi)
            => multi
                ? SetTextMulti(keyFactory(entity), fontData, scale, text, pos, aligment, color, dropShadows)
                : SetText(keyFactory(entity), fontData, scale, text, pos, aligment, color, dropShadows);

        private bool RemoveEntityText<TEntity>(
            TEntity entity, Func<TEntity, TextRenderKey> keyFactory, bool multi)
            => multi
                ? RemoveTextMulti(keyFactory(entity))
                : RemoveText(keyFactory(entity));

        public SizeF SetProvinceText(Province province, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
            => SetEntityText(province, TextRenderKey.ForProvince, fontData, scale, text, pos, aligment, color, dropShadows, false);

        public SizeF SetProvinceTextMulti(Province province, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
            => SetEntityText(province, TextRenderKey.ForProvince, fontData, scale, text, pos, aligment, color, dropShadows, true);

        public bool RemoveProvinceText(Province province)
            => RemoveEntityText(province, TextRenderKey.ForProvince, false);

        public bool RemoveProvinceTextMulti(Province province)
            => RemoveEntityText(province, TextRenderKey.ForProvince, true);

        public SizeF SetStateText(State state, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
            => SetEntityText(state, TextRenderKey.ForState, fontData, scale, text, pos, aligment, color, dropShadows, false);

        public SizeF SetStateTextMulti(State state, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
            => SetEntityText(state, TextRenderKey.ForState, fontData, scale, text, pos, aligment, color, dropShadows, true);

        public bool RemoveStateText(State state)
            => RemoveEntityText(state, TextRenderKey.ForState, false);

        public bool RemoveStateTextMulti(State state)
            => RemoveEntityText(state, TextRenderKey.ForState, true);

        public SizeF SetRegionText(StrategicRegion region, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
            => SetEntityText(region, TextRenderKey.ForRegion, fontData, scale, text, pos, aligment, color, dropShadows, false);

        public SizeF SetRegionTextMulti(StrategicRegion region, FontData fontData, float scale, string text, Vector3 pos, QFontAlignment aligment, Color color, bool dropShadows)
            => SetEntityText(region, TextRenderKey.ForRegion, fontData, scale, text, pos, aligment, color, dropShadows, true);

        public bool RemoveRegionText(StrategicRegion region)
            => RemoveEntityText(region, TextRenderKey.ForRegion, false);

        public bool RemoveRegionTextMulti(StrategicRegion region)
            => RemoveEntityText(region, TextRenderKey.ForRegion, true);

        public void Render(Matrix4 proj, float geometryScale)
        {
            if (_drawing.DrawingPimitiveses.Count == 0 || _drawing._vertexArrayObject == null)
                return;

            _drawing.ProjectionMatrix = proj;
            _drawing.GeometryScale = geometryScale;
            _drawing.Draw();
            _drawing.DisableShader();
        }
    }
}
