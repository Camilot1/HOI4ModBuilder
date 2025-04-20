using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.warnings;

namespace HOI4ModBuilder.src.managers.mapChecks.warnings.checkers
{
    public class MapCheckerStatesVictoryPointsForForeignProvinces : MapChecker
    {
        public MapCheckerStatesVictoryPointsForForeignProvinces()
            : base((int)EnumMapWarningCode.STATE_VICTORY_POINT_FOR_FOREIGN_PROVINCE, (list) =>
            {
                foreach (var s in StateManager.GetStates())
                {
                    s.ForEachVictoryPoints((dateTime, stateHistory, victoryPoint) =>
                    {
                        if (victoryPoint.province != null && victoryPoint.province.State != s)
                        {
                            list.Add(new MapCheckData(s.center, (int)EnumMapWarningCode.STATE_VICTORY_POINT_FOR_FOREIGN_PROVINCE));
                            return true;
                        }
                        return false;
                    });
                }
            })
        { }
    }
}
