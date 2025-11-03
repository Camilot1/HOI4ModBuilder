using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers.errors;
using HOI4ModBuilder.src.managers.warnings;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.managers.mapChecks.errors.checkers
{
    public class MapCheckerDividedStates : MapChecker
    {
        public MapCheckerDividedStates()
            : base("MapCheckerDividedStates", (int)EnumMapWarningCode.DIVIDED_STATE, (list) =>
            {
                foreach (var s in StateManager.GetStates())
                {
                    if (s.Provinces.Count == 0)
                        continue;

                    var usedProvinces = new HashSet<Province>();
                    var nextProvinces = new Queue<Province>();

                    var startProvince = s.Provinces[0];
                    nextProvinces.Enqueue(startProvince);
                    usedProvinces.Add(startProvince);

                    while (nextProvinces.Count > 0)
                    {
                        var p = nextProvinces.Dequeue();

                        p.ForEachAdjacentProvince((thisP, otherP) =>
                        {
                            if (otherP.State != s || usedProvinces.Contains(otherP))
                                return;

                            nextProvinces.Enqueue(otherP);
                            usedProvinces.Add(otherP);
                        });
                    }

                    if (usedProvinces.Count != s.Provinces.Count)
                        list.Add(new MapCheckData(startProvince.center, (int)EnumMapWarningCode.DIVIDED_STATE));
                }
            })
        { }
    }
}
