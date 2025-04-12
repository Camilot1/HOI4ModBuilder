using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers.warnings;
using HOI4ModBuilder.src.utils.structs;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerProvinceWrongColors : MapChecker
    {
        public MapCheckerProvinceWrongColors()
            : base((int)EnumMapWarningCode.PROVINCE_WRONG_COLOR, (list) =>
            {
                foreach (var p in ProvinceManager.GetProvinces())
                {
                    var color = new Color3B(p.Color);

                    int max = color.red;
                    if (color.green > max) max = color.green;
                    if (color.blue > max) max = color.blue;

                    int sum = color.red + color.green + color.blue;

                    //TODO Переделать: добавить поддержку кастомных условий
                    //land
                    if (p.Type == EnumProvinceType.LAND && sum < 340)
                        list.Add(new MapCheckData(p.center, (int)EnumMapWarningCode.PROVINCE_WRONG_COLOR));
                    //sea, lake
                    else if (p.Type != EnumProvinceType.LAND && (sum > 339 || max > 127))
                        list.Add(new MapCheckData(p.center, (int)EnumMapWarningCode.PROVINCE_WRONG_COLOR));
                }
            })
        { }
    }
}
