using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerProvincesBordersMismatches : MapChecker
    {
        public MapCheckerProvincesBordersMismatches()
            : base((int)EnumMapWarningCode.PROVINCE_BORDERS_MISMATCH, (list) =>
            {
                foreach (var p in ProvinceManager.GetProvinces())
                {
                    if (p.Type == EnumProvinceType.LAKE && p.HasBorderWithTypeId(EnumProvinceType.SEA))
                        list.Add(new MapCheckData(p.center, (int)EnumMapWarningCode.PROVINCE_BORDERS_MISMATCH));
                }
            })
        { }
    }
}
