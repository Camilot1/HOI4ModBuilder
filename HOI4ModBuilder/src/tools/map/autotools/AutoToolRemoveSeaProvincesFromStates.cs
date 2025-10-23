
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;

namespace HOI4ModBuilder.src.tools.autotools
{
    public class AutoToolRemoveSeaProvincesFromStates : AbstractAutoTool
    {
        public static void Execute(bool displayResultMessage)
        {
            int counter = 0;
            ProvinceManager.ForEachProvince((p) =>
            {
                if (p.Type == EnumProvinceType.SEA && p.State != null)
                {
                    counter++;
                    p.State.RemoveProvince(p);
                }
            });

            if (displayResultMessage)
                PostAction(false, counter);
        }
    }
}
