using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;

namespace HOI4ModBuilder.src.tools.autotools
{
    public class AutoToolRemoveSeaAndLakesContinents : AbstractAutoTool
    {
        public static void Execute(bool displayResultMessage)
        {
            int counter = 0;
            ProvinceManager.ForEachProvince((p) =>
            {
                if (p.Type != EnumProvinceType.LAND && p.ContinentId != 0)
                {
                    counter++;
                    p.ContinentId = 0;
                }
            });

            if (displayResultMessage)
                PostAction(false, counter);
        }
    }
}
