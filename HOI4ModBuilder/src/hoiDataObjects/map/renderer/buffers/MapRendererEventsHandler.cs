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

        public static void UpdateProvinceDataRecord(Province p)
        {
            if (!MainForm.UpdateGLControl)
                return;

            UpdateProvinceDataRecord(p, MapRendererBuffersBuilder.BuildProvinceDataRecord(p));
        }

        public static void UpdateProvinceDataRecord(Province p, ProvinceDataRecord provinceDataRecord)
        {
            if (!MainForm.UpdateGLControl)
                return;

            MapRendererBuffersManager.UpdateIntBufferRange(
                MapRendererBuffersManager.ProvinceDataByIdKey,
                MapRendererBuffersBuilder.GetProvinceDataStartIndex(p.Id),
                provinceDataRecord.ToIntArray()
            );
        }

        public static void UpdateStateDataRecord(State s)
        {
            if (!MainForm.UpdateGLControl)
                return;

            UpdateStateDataRecord(s, MapRendererBuffersBuilder.BuildStateDataRecord(s));
        }

        public static void UpdateStateDataRecord(State s, StateDataRecord stateDataRecord)
        {
            if (!MainForm.UpdateGLControl)
                return;

            MapRendererBuffersManager.UpdateIntBufferRange(
                MapRendererBuffersManager.StateDataByIdKey,
                MapRendererBuffersBuilder.GetStateDataStartIndex(s.Id.GetValue()),
                stateDataRecord.ToIntArray()
            );
        }


        public static void UpdateRegionDataRecord(StrategicRegion r)
        {
            if (!MainForm.UpdateGLControl)
                return;

            UpdateRegionDataRecord(r, MapRendererBuffersBuilder.BuildStrategicRegionDataRecord(r));
        }

        public static void UpdateRegionDataRecord(StrategicRegion r, StrategicRegionDataRecord strategicRegionDataRecord)
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
