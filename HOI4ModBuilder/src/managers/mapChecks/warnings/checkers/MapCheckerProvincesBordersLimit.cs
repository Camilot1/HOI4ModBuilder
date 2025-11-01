using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerProvincesBordersLimit : MapChecker
    {
        public MapCheckerProvincesBordersLimit()
            : base("MapCheckerProvincesBordersLimit", (int)EnumMapWarningCode.PROVINCE_HAS_MORE_THAN_8_BORDERS, (list) =>
            {

                foreach (var p in ProvinceManager.GetProvinces())
                {
                    if (p.borders.Count > 8)
                    {
                        list.Add(new MapCheckData(p.center, (int)EnumMapWarningCode.PROVINCE_HAS_MORE_THAN_8_BORDERS));
                        Logger.Log($"province {p.Id} has {p.borders.Count} borders");
                    }
                }
            })
        { }
    }
}
