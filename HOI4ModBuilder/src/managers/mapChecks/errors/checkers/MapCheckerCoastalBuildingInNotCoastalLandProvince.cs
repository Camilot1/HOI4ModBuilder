using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.errors;
using System;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerCoastalBuildingInNotCoastalLandProvince : MapChecker
    {
        public MapCheckerCoastalBuildingInNotCoastalLandProvince()
            : base("MapCheckerCoastalBuildingInNotCoastalLandProvince", (int)EnumMapErrorCode.COASTAL_BUILDING_IN_NOT_COASTAL_LAND_PROVINCE, (list) =>
            {
                foreach (var p in ProvinceManager.GetProvinces())
                {
                    if (p.IsCoastal)
                        continue;

                    p.ForEachBuilding((building, count) =>
                    {
                        if (building.IsOnlyCoastal.GetValue() && count > 0)
                        {
                            list.Add(new MapCheckData(p.center, (int)EnumMapErrorCode.COASTAL_BUILDING_IN_NOT_COASTAL_LAND_PROVINCE));
                            return;
                        }
                    });
                }
            })
        { }
    }
}
