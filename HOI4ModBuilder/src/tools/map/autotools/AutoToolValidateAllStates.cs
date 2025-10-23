
using HOI4ModBuilder.src.hoiDataObjects.history.states;

namespace HOI4ModBuilder.src.tools.autotools
{
    public class AutoToolValidateAllStates : AbstractAutoTool
    {
        public static void Execute(bool displayResultMessage)
        {
            int counter = 0;
            StateManager.ForEachState((s) =>
            {
                s.Validate(out bool hasChanged);
                if (hasChanged)
                    counter++;
            });

            if (displayResultMessage)
                PostAction(true, counter);
        }
    }
}
