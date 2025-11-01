using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerProvinceWrongColors : MapChecker
    {
        public MapCheckerProvinceWrongColors()
            : base("MapCheckerProvinceWrongColors", (int)EnumMapWarningCode.PROVINCE_WRONG_COLOR, (list) =>
            {
                foreach (var p in ProvinceManager.GetProvinces())
                {
                    Utils.IntToRgb(p.Color, out var r, out var g, out var b);
                    ColorUtils.RgbToHsv(r, g, b, out var h, out var s, out var v);

                    var hsvRange = SettingsManager.GetHSVRanges(p.Type);
                    if (!hsvRange.IsValidHSV(h, s, v))
                        list.Add(new MapCheckData(p.center, (int)EnumMapWarningCode.PROVINCE_WRONG_COLOR));
                }
            })
        { }
    }
}
