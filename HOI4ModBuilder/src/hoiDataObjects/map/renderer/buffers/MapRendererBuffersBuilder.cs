using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers.structs;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer.buffers
{
    public class MapRendererBuffersBuilder
    {
        public static int[] BuildPixelsToProvinceIds()
        {
            int[] source = MapManager.ProvincesPixels;
            int[] pixelProvinceIds = new int[source.Length];

            int? previousColor = null;
            int previousProvinceId = 0;

            for (int i = 0; i < source.Length; i++)
            {
                int currentColor = source[i];
                if (currentColor != previousColor)
                {
                    previousColor = currentColor;
                    previousProvinceId = ProvinceManager.TryGet(currentColor, out Province province) ? province.Id : 0;
                }

                pixelProvinceIds[i] = previousProvinceId;
            }

            return pixelProvinceIds;
        }

        public static int[] BuildTerrainIdsToColors()
        {
            int[] terrainColors = new int[TerrainManager.GetTerrainsCount() + 1];
            TerrainManager.ForEach(terrain => terrainColors[terrain.id] = terrain.color & 0x00FFFFFF);
            return terrainColors;
        }

        public static int[] BuildContinentIdsToColors()
        {
            int continentsCount = ContinentManager.GetContinentsCount();
            int[] continentColors = new int[continentsCount + 1];

            for (int i = 0; i < continentsCount; i++)
                continentColors[i] = ContinentManager.GetColorById(i) & 0x00FFFFFF;

            return continentColors;
        }

        public static int[] BuildProvinceDataById()
        {
            var data = new int[(ProvinceManager.MaxProvinceId + 1) * ProvinceDataRecord.Stride];
            ProvinceManager.ForEachProvince(p => WriteProvinceDataRecord(data, p));
            return data;
        }

        public static ProvinceDataRecord BuildProvinceDataRecord(Province province)
            => ProvinceDataRecord.FromProvince(province);

        public static int GetProvinceDataStartIndex(ushort provinceId)
            => provinceId * ProvinceDataRecord.Stride;

        public static void WriteProvinceDataRecord(int[] target, Province province)
            => WriteProvinceDataRecord(target, GetProvinceDataStartIndex(province.Id), province);

        public static void WriteProvinceDataRecord(int[] target, int startIndex, Province province)
            => BuildProvinceDataRecord(province).WriteTo(target, startIndex);

        public static int[] BuildStateDataById()
        {
            var data = new int[(StateManager.GetMaxID() + 1) * StateDataRecord.Stride];
            StateManager.ForEachState(s => WriteStateDataRecord(data, s));
            return data;
        }

        public static StateDataRecord BuildStateDataRecord(State state)
            => StateDataRecord.FromState(state);

        public static int GetStateDataStartIndex(ushort stateId)
            => stateId * StateDataRecord.Stride;

        public static void WriteStateDataRecord(int[] target, State state)
            => WriteStateDataRecord(target, GetStateDataStartIndex(state.Id.GetValue()), state);

        public static void WriteStateDataRecord(int[] target, int startIndex, State state)
            => BuildStateDataRecord(state).WriteTo(target, startIndex);

        public static int[] BuildStateCategoryDataById()
        {
            var data = new int[(StateCategoryManager.GetCount() + 1) * StateCategoryDataRecord.Stride];
            StateCategoryManager.ForEach(stateCategory => WriteStateCategoryDataRecord(data, stateCategory));
            return data;
        }

        public static StateCategoryDataRecord BuildStateCategoryDataRecord(StateCategory stateCategory)
            => StateCategoryDataRecord.FromStateCategory(stateCategory);

        public static int GetStateCategoryDataStartIndex(int stateCategoryId)
            => stateCategoryId * StateCategoryDataRecord.Stride;

        public static void WriteStateCategoryDataRecord(int[] target, StateCategory stateCategory)
            => WriteStateCategoryDataRecord(target, GetStateCategoryDataStartIndex(stateCategory.id), stateCategory);

        public static void WriteStateCategoryDataRecord(int[] target, int startIndex, StateCategory stateCategory)
            => BuildStateCategoryDataRecord(stateCategory).WriteTo(target, startIndex);

        public static int[] BuildStrategicRegionDataById()
        {
            if (StrategicRegionManager.GetValues().Count == 0)
                return new int[1];

            var data = new int[(StrategicRegionManager.GetMaxID() + 1) * StrategicRegionDataRecord.Stride];
            StrategicRegionManager.ForEach(r => WriteStrategicRegionDataRecord(data, r));
            return data;
        }

        public static StrategicRegionDataRecord BuildStrategicRegionDataRecord(StrategicRegion region)
            => StrategicRegionDataRecord.FromStrategicRegion(region);

        public static int GetStrategicRegionDataStartIndex(ushort regionId)
            => regionId * StrategicRegionDataRecord.Stride;

        public static void WriteStrategicRegionDataRecord(int[] target, StrategicRegion region)
            => WriteStrategicRegionDataRecord(target, GetStrategicRegionDataStartIndex(region.Id), region);

        public static void WriteStrategicRegionDataRecord(int[] target, int startIndex, StrategicRegion region)
            => BuildStrategicRegionDataRecord(region).WriteTo(target, startIndex);
    }
}
