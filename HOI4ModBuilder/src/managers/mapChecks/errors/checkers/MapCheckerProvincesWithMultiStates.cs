using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.errors;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerProvincesWithMultiStates : MapChecker
    {
        public MapCheckerProvincesWithMultiStates()
            : base((int)EnumMapErrorCode.PROVINCE_MULTI_STATES, (list) =>
            {
                foreach (var s in StateManager.GetStates())
                {
                    foreach (var p in s.provinces)
                    {
                        if (p.State.Id != s.Id)
                            list.Add(new MapCheckData(p.center, (int)EnumMapErrorCode.PROVINCE_MULTI_STATES));
                    }
                }
            })
        { }
    }
}
