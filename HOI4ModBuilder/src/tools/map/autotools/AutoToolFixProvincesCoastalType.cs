using HOI4ModBuilder.managers;
using System.Runtime.CompilerServices;

namespace HOI4ModBuilder.src.tools.autotools
{
    public class AutoToolFixProvincesCoastalType : AbstractAutoTool
    {
        public static void Execute(bool displayResultMessage)
        {
            int counter = 0;
            ProvinceManager.ForEachProvince((p) =>
            {
                var newIsCoastal = p.CheckCoastalType();
                if (p.IsCoastal != newIsCoastal)
                {
                    counter++;
                    p.IsCoastal = newIsCoastal;
                }
            });

            if (displayResultMessage)
                PostAction(false, counter);
        }
    }
}
