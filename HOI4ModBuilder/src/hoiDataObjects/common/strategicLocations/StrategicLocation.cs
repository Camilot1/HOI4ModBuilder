using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.strategicLocations
{
    public class StrategicLocation : AbstractParseObject
    {
        public string Name { get; private set; }

        public readonly GameDictionary<Building, int> Buildings = new GameDictionary<Building, int>()
            .INIT_SetKeyParseAdapter(token => BuildingManager.GetBuilding(token));

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "buildings", new DynamicGameParameter {
                doNotParseNewObj = true,
                provider = o => ((StrategicLocation)o).Buildings,
                factory = (o, key) => BuildingManager.GetBuilding(key),
                valueType = typeof(int)
            } },
        };

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common", "strategicLocations" }, "StrategicLocation")
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new StrategicLocation();

        public StrategicLocation() { }
        public StrategicLocation(string name)
        {
            Name = name;
        }
    }
}
