using HOI4ModBuilder.src.forms.recoveryForms;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools.dataRecovery
{
    class StrategicRegionsDataRecoveryTool
    {
        public static bool TransferRegionInfo(TransferInfo transferInfo, StrategicRegion oldRegion)
        {
            if (!StrategicRegionManager.TryGetRegion(oldRegion.Id, out StrategicRegion currentRegion))
            {
                if (!transferInfo.TransferFilesIfRegionIdNotFound) return false;

                oldRegion.SetSilent(false);
                StrategicRegionManager.TryAddRegion(oldRegion);
                return true;
            }
            else
            {
                if (transferInfo.Name) currentRegion.Name = oldRegion.Name;
                if (transferInfo.NavalTerrain) currentRegion.Terrain = oldRegion.Terrain;
                if (transferInfo.Provinces) currentRegion.TransferProvincesFrom(oldRegion);
                if (transferInfo.StaticModifiers) currentRegion.StaticModifiers = oldRegion.StaticModifiers;
                if (transferInfo.Weather) currentRegion.Weather = oldRegion.Weather;

                return transferInfo.HasAnyTransferFlag;
            }
        }
    }
}
