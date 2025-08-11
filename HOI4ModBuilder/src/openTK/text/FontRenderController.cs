using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder.src.openTK.text
{
    public class FontRenderController : IDisposable
    {
        private Dictionary<Value2S, FontRenderRegion> _regions;

        private int _regionSize;
        public bool IsPerforming { get; private set; }
        public int EventsFlags { get; private set; }
        private Action<int, ICollection<object>> _eventHandler;
        private List<object> _eventHandlerPayload = new List<object>();

        public FontRenderController(int regionSize, int capacity)
        {
            _regionSize = regionSize;

            _regions = new Dictionary<Value2S, FontRenderRegion>(capacity);
        }

        public FontRenderController TryStart(out bool result) => TryStart(0, out result);

        public FontRenderController TryStart(int eventsFlags, out bool result)
        {
            if (IsPerforming)
            {
                result = false;
                return null;
            }

            result = true;

            if (!MapManager.displayLayers[(int)EnumAdditionalLayers.TEXT])
                return null;

            IsPerforming = true;
            EventsFlags = eventsFlags;
            return this;
        }

        public FontRenderController SetScale(float scale)
        {
            MapManager.TextScale = scale;
            return this;
        }

        public void PushAction(Vector3 pos, Action<FontRenderRegion> action)
        {
            var key = new Value2S
            {
                x = (short)(pos.X / _regionSize),
                y = (short)(pos.Y / _regionSize)
            };


            if (!_regions.TryGetValue(key, out var region))
            {
                region = new FontRenderRegion(key, _regionSize);
                _regions[key] = region;
            }
            region.PushAction(action);
        }

        public FontRenderRegion GetRegion(Vector3 pos)
        {
            var key = new Value2S
            {
                x = (short)(pos.X / _regionSize),
                y = (short)(pos.Y / _regionSize)
            };
            if (!_regions.TryGetValue(key, out var region))
            {
                region = new FontRenderRegion(key, _regionSize);
                _regions[key] = region;
            }
            return region;
        }

        public FontRenderController ClearAllMulti()
        {
            foreach (var region in _regions.Values)
                region.ClearAllMulti();

            return this;
        }

        public FontRenderController ClearAll()
        {
            foreach (var region in _regions.Values)
                if (region.ClearAllMulti())
                    region.RefreshBuffers();

            return this;
        }

        public FontRenderController SetEventsHandler(int eventsFlags, Action<int, ICollection<object>> eventHandler)
        {
            EventsFlags = eventsFlags;
            _eventHandler = eventHandler;
            return this;
        }

        public bool AddEventData(EnumMapRenderEvents eventFlag, object value)
            => AddEventData((int)eventFlag, value);

        public bool AddEventData(int eventFlags, object value)
        {
            if ((EventsFlags & eventFlags) == 0)
                return false;

            if (!_eventHandlerPayload.Contains(value))
            {
                _eventHandlerPayload.Add(value);
                return true;
            }
            return false;
        }
        public bool TryPostLatestEvent()
        {
            if (_eventHandler == null || _eventHandlerPayload.Count == 0)
                return false;

            _eventHandler(EventsFlags, _eventHandlerPayload);
            ClearEventPayload();
            return true;
        }
        public List<object> GetEventPayload() => _eventHandlerPayload;
        public void ClearEventPayload() => _eventHandlerPayload.Clear();

        public void EndAssembleParallel()
        {
            var tasks = new Task[_regions.Count];

            int index = 0;
            foreach (var region in _regions.Values)
            {
                tasks[index] = Task.Run(() => region.ExecuteActions());
                index++;
            };

            Task.WhenAll(tasks)
                .ContinueWith(_ => MainForm.Instance.InvokeAction(() =>
                {
                    LoadRegionsVAOs();
                    IsPerforming = false;
                }));
        }

        public void EndAssembleParallelWithWait()
        {
            var tasks = new Task[_regions.Count];

            int index = 0;
            foreach (var region in _regions.Values)
            {
                tasks[index] = Task.Run(() => region.ExecuteActions());
                index++;
            };

            Task.WaitAll(tasks);

            LoadRegionsVAOs();
            IsPerforming = false;
        }

        public void End()
        {
            IsPerforming = false;
        }

        private void LoadRegionsVAOs()
        {
            foreach (var region in _regions.Values)
                region.LoadVAO();
        }

        public void ForEachRegion(Action<FontRenderRegion> action)
        {
            foreach (var region in _regions.Values)
                action(region);
        }

        public void Render(Matrix4 proj, Bounds4F viewportBounds)
        {
            if (_regions == null || _regions.Count == 0 || IsPerforming)
                return;

            foreach (var region in _regions.Values)
            {
                if (region.ChacheCount > 0 && region.IsIntersectsWith(viewportBounds))
                {
                    region.Render(proj);
                }
            }
        }

        public void RenderDebug()
        {
            foreach (var region in _regions.Values)
            {
                var seed = region.Index.GetHashCode();
                var random = new Random(seed);
                GL.Color4(
                    0.25f + random.NextDouble() * 0.75f,
                    0.25f + random.NextDouble() * 0.75f,
                    0.25f + random.NextDouble() * 0.75f,
                    0.25f
                    );

                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(region.Bounds.left, region.Bounds.top);
                GL.Vertex2(region.Bounds.left, region.Bounds.bottom);
                GL.Vertex2(region.Bounds.right, region.Bounds.bottom);
                GL.Vertex2(region.Bounds.right, region.Bounds.top);
                GL.End();
            }
        }

        public void Dispose()
        {
            if (_regions == null)
                return;

            foreach (var region in _regions.Values)
                region.Dispose();

            _regions.Clear();
        }

        public FontRenderController ForEachProvince(
            Func<Province, bool> checker,
            Action<FontRenderRegion, Province, Vector3> action)
        {
            ProvinceManager.ForEachProvince(p =>
            {
                if (!checker(p))
                    return;

                var pos = p.center.ToVec3(MapManager.MapSize.y);
                MapManager.FontRenderController?.PushAction(pos, fontRegion => action(fontRegion, p, pos));
            });

            return this;
        }

        public FontRenderController ForEachProvince(
            ICollection<object> provinces,
            Func<Province, bool> checker,
            Action<FontRenderRegion, Province, Vector3> action)
        {
            foreach (var obj in provinces)
            {
                if (!(obj is Province p) || !checker(p))
                    continue;

                var pos = p.center.ToVec3(MapManager.MapSize.y);
                MapManager.FontRenderController?.PushAction(pos, fontRegion => action(fontRegion, p, pos));
            }

            return this;
        }

        public FontRenderController ForEachState(
            Func<State, bool> checker,
            Action<FontRenderRegion, State, Vector3> action)
        {
            StateManager.ForEachState(s =>
            {
                if (!checker(s))
                    return;

                var pos = s.center.ToVec3(MapManager.MapSize.y);
                MapManager.FontRenderController?.PushAction(pos, fontRegion => action(fontRegion, s, pos));
            });

            return this;
        }

        public FontRenderController ForEachState(
            ICollection<object> states,
            Func<State, bool> checker,
            Action<FontRenderRegion, State, Vector3> action)
        {
            foreach (var obj in states)
            {
                if (!(obj is State s) || !checker(s))
                    continue;

                var pos = s.center.ToVec3(MapManager.MapSize.y);
                MapManager.FontRenderController?.PushAction(pos, fontRegion => action(fontRegion, s, pos));
            }

            return this;
        }

        public FontRenderController ForEachRegion(
            Func<StrategicRegion, bool> checker,
            Action<FontRenderRegion, StrategicRegion, Vector3> action)
        {
            StrategicRegionManager.ForEachRegion(r =>
            {
                if (!checker(r))
                    return;

                var pos = r.center.ToVec3(MapManager.MapSize.y);
                MapManager.FontRenderController?.PushAction(pos, fontRegion => action(fontRegion, r, pos));
            });

            return this;
        }
        public FontRenderController ForEachRegion(
            ICollection<object> regions,
            Func<StrategicRegion, bool> checker,
            Action<FontRenderRegion, StrategicRegion, Vector3> action)
        {
            foreach (var obj in regions)
            {
                if (!(obj is StrategicRegion r) || !checker(r))
                    continue;

                var pos = r.center.ToVec3(MapManager.MapSize.y);
                MapManager.FontRenderController?.PushAction(pos, fontRegion => action(fontRegion, r, pos));
            }

            return this;
        }

        public FontRenderController SetEventsHandlerProvincesIdsReinit(float scale, Color color, QFontAlignment alignment)
        {
            return SetEventsHandler((int)EnumMapRenderEvents.PROVINCES_IDS, (flags, objs) =>
            {
                TryStart(EventsFlags, out var eventResult)?
                .ForEachProvince(objs, p => true, (fontRegion, p, pos) =>
                {
                    PushAction(pos, r => r.RemoveTextMulti(p.Id));
                    if (ProvinceManager.TryGetProvince(p.Id, out var province) && province == p)
                        PushAction(pos, r => r.SetTextMulti(
                                p.Id, TextRenderManager.Instance.FontData64, scale,
                                p.Id + "", pos, alignment, color, true
                            ));
                })
                .EndAssembleParallelWithWait();
            });
        }

        public FontRenderController SetEventsHandlerStatesIdsReinit(float scale, Color color, QFontAlignment alignment)
        {
            return SetEventsHandler((int)EnumMapRenderEvents.STATES_IDS, (flags, objs) =>
            {
                TryStart(EventsFlags, out var eventResult)?
                .ForEachState(objs, p => true, (fontRegion, s, pos) =>
                {
                    PushAction(pos, r => r.RemoveTextMulti(s.Id.GetValue()));
                    if (StateManager.TryGetState(s.Id.GetValue(), out var state) && state == s)
                        PushAction(pos, r => r.SetTextMulti(
                            s.Id.GetValue(), TextRenderManager.Instance.FontData64, scale,
                            s.Id.GetValue() + "", pos, alignment, color, true
                        ));
                })
                .EndAssembleParallelWithWait();
            });
        }

        public FontRenderController SetEventsHandlerRegionsIdsReinit(float scale, Color color, QFontAlignment alignment)
        {
            return SetEventsHandler((int)EnumMapRenderEvents.REGIONS_IDS, (flags, objs) =>
            {
                TryStart(EventsFlags, out var eventResult)?
                .ForEachRegion(objs, r => true, (fontRegion, r, pos) =>
                {
                    PushAction(pos, fr => fontRegion.RemoveTextMulti(r.Id));
                    if (StrategicRegionManager.TryGetRegion(r.Id, out var region) && region == r)
                        PushAction(pos, fr => fontRegion.SetTextMulti(
                            r.Id, TextRenderManager.Instance.FontData64, scale,
                            r.Id + "", pos, alignment, color, true
                        ));
                })
                .EndAssembleParallelWithWait();
            });
        }
    }
}
