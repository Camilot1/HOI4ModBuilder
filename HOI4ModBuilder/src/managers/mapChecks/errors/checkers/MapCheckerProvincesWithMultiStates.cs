using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.errors;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerProvincesWithMultiStates : MapChecker
    {
        public MapCheckerProvincesWithMultiStates()
            : base("MapCheckerProvincesWithMultiStates", (int)EnumMapErrorCode.PROVINCE_MULTI_STATES, (list) =>
            {
                foreach (var s in StateManager.GetValues())
                {
                    foreach (var p in s.Provinces)
                    {
                        if (p.State.Id.GetValue() != s.Id.GetValue())
                            list.Add(new MapCheckData(p.center, (int)EnumMapErrorCode.PROVINCE_MULTI_STATES));
                    }
                }
            })
        { }
    }
}
