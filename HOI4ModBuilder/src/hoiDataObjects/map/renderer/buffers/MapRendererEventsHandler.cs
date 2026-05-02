using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers.structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers
{
    public class MapRendererEventsHandler
    {
        public static void OnProvinceCreate(Province p)
        {
            if (!MainForm.UpdateGLControl)
                return;

            MapRendererBuffersManager.InvalidateBuffer(MapRendererBuffersManager.PixelsToProvinceIdsKey);
            MapRendererBuffersManager.InvalidateBuffer(MapRendererBuffersManager.ProvinceDataByIdKey);
        }

        public static void OnProvinceRemove(Province p)
        {
            if (!MainForm.UpdateGLControl)
                return;

            MapRendererBuffersManager.InvalidateBuffer(MapRendererBuffersManager.PixelsToProvinceIdsKey);
            MapRendererBuffersManager.InvalidateBuffer(MapRendererBuffersManager.ProvinceDataByIdKey);
        }

        public static void OnProvinceColorChange(Province p, int fromColor, int toColor)
        {
            if (!MainForm.UpdateGLControl)
                return;

            MapRendererBuffersManager.InvalidateBuffer(MapRendererBuffersManager.PixelsToProvinceIdsKey);
            var provinceDataRecord = MapRendererBuffersBuilder.BuildProvinceDataRecord(p);
            provinceDataRecord.Color = toColor;
            UpdateProvinceDataRecord(p, provinceDataRecord);
        }

        public static void OnProvinceIDChange(Province p, ushort fromID, ushort toID)
        {
            if (!MainForm.UpdateGLControl)
                return;

            MapRendererBuffersManager.InvalidateBuffer(MapRendererBuffersManager.PixelsToProvinceIdsKey);
            MapRendererBuffersManager.InvalidateBuffer(MapRendererBuffersManager.ProvinceDataByIdKey);
        }

        public static void OnProvinceTerrainChanged(Province p) => UpdateProvinceDataRecord(p);

        public static void OnProvinceContinentChanged(Province p) => UpdateProvinceDataRecord(p);

        public static void OnProvinceStateChanged(Province p) => UpdateProvinceDataRecord(p);

        public static void OnProvinceStateIdChanged(Province p) => UpdateProvinceDataRecord(p);

        public static void OnProvinceRegionChanged(Province p) => UpdateProvinceDataRecord(p);
        public static void OnProvinceRegionIdChanged(Province p) => UpdateProvinceDataRecord(p);

        public static void OnProvinceVictoryPointsChanged(Province p) => UpdateProvinceDataRecord(p);

        public static void OnProvincePixelsCountChanged(Province p) => UpdateProvinceDataRecord(p);

        public static void OnProvinceTypeFlagsChanged(Province p) => UpdateProvinceDataRecord(p);

        private static void UpdateProvinceDataRecord(Province p)
        {
            if (!MainForm.UpdateGLControl)
                return;

            UpdateProvinceDataRecord(p, MapRendererBuffersBuilder.BuildProvinceDataRecord(p));
        }

        private static void UpdateProvinceDataRecord(Province p, ProvinceDataRecord provinceDataRecord)
        {
            if (!MainForm.UpdateGLControl)
                return;

            MapRendererBuffersManager.UpdateIntBufferRange(
                MapRendererBuffersManager.ProvinceDataByIdKey,
                MapRendererBuffersBuilder.GetProvinceDataStartIndex(p.Id),
                provinceDataRecord.ToIntArray()
            );
        }

        public static void OnStateColorChanged(State s) => UpdateStateDataRecord(s);

        public static void OnStateCategoryChanged(State s) => UpdateStateDataRecord(s);

        private static void UpdateStateDataRecord(State s)
        {
            if (!MainForm.UpdateGLControl)
                return;

            UpdateStateDataRecord(s, MapRendererBuffersBuilder.BuildStateDataRecord(s));
        }

        private static void UpdateStateDataRecord(State s, StateDataRecord stateDataRecord)
        {
            if (!MainForm.UpdateGLControl)
                return;

            MapRendererBuffersManager.UpdateIntBufferRange(
                MapRendererBuffersManager.StateDataByIdKey,
                MapRendererBuffersBuilder.GetStateDataStartIndex(s.Id.GetValue()),
                stateDataRecord.ToIntArray()
            );
        }


        public static void OnStrategicRegionColorChanged(StrategicRegion r) => UpdateStrategicRegionDataRecord(r);

        private static void UpdateStrategicRegionDataRecord(StrategicRegion r)
        {
            if (!MainForm.UpdateGLControl)
                return;

            UpdateStrategicRegionDataRecord(r, MapRendererBuffersBuilder.BuildStrategicRegionDataRecord(r));
        }

        private static void UpdateStrategicRegionDataRecord(StrategicRegion r, StrategicRegionDataRecord strategicRegionDataRecord)
        {
            if (!MainForm.UpdateGLControl)
                return;

            MapRendererBuffersManager.UpdateIntBufferRange(
                MapRendererBuffersManager.StrategicRegionDataByIdKey,
                MapRendererBuffersBuilder.GetStrategicRegionDataStartIndex(r.Id),
                strategicRegionDataRecord.ToIntArray()
            );
        }
    }
}
