
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;

namespace HOI4ModBuilder.src.tools.autotools
{
    public class AutoToolValidateAllRegions : AbstractAutoTool
    {
        public static void Execute(bool displayResultMessage)
        {
            int counter = 0;
            StrategicRegionManager.ForEachRegion((r) =>
            {
                r.Validate(out bool hasChanged);
                if (hasChanged)
                    counter++;
            });

            if (displayResultMessage)
                PostAction(true, counter);
        }
    }
}
