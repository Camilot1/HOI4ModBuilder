using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    public class BuildingPrerequisite : AbstractParseObject
    {
        public readonly GameParameter<BuildingPrerequisiteDependency> BuildingDependency = new GameParameter<BuildingPrerequisiteDependency>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "building_dependency", o => ((BuildingPrerequisite)o).BuildingDependency },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common", "buildings" }, "BuildingPrerequisite")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new BuildingPrerequisite();
    }

    public class BuildingPrerequisiteDependency : AbstractParseObject
    {
        public readonly GameParameter<Building> Building = new GameParameter<Building>()
            .INIT_ForceValueInlineParse(true)
            .INIT_SetValueParseAdapter((o, token) => BuildingManager.GetBuilding((string)token));
        public readonly GameParameter<uint> Level = new GameParameter<uint>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "building", o => ((BuildingPrerequisiteDependency)o).Building },
            { "level", o => ((BuildingPrerequisiteDependency)o).Level },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common", "buildings" }, "BuildingPrerequisiteDependency")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new BuildingPrerequisiteDependency();
    }
}
