using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.errors;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    internal class MapCheckerStateWithNoOwner : MapChecker
    {
        public MapCheckerStateWithNoOwner()
            : base("MapCheckerStateWithNoOwner", (int)EnumMapErrorCode.STATE_WITH_NO_OWNER, (list) =>
            {
                foreach (var s in StateManager.GetValues())
                {
                    if (s.owner == null)
                        list.Add(new MapCheckData(s.center, (int)EnumMapErrorCode.STATE_WITH_NO_OWNER));
                }
            })
        { }
    }
}
