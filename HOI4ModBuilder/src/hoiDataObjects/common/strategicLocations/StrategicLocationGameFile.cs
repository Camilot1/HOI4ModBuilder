using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.strategicLocations
{
    public class StrategicLocationGameFile : GameFile
    {
        public readonly GameDictionary<string, StrategicLocation> StrategicLocations = new GameDictionary<string, StrategicLocation>();


        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "strategic_locations", new DynamicGameParameter {
                parseInnerBlock = true,
                provider = o => ((StrategicLocationGameFile)o).StrategicLocations,
                factory = (o, key) => new StrategicLocation(key)
            } },
        };

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common", "strategicLocations" }, "StrategicLocationGamwFile")
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();

        public StrategicLocationGameFile() : base()
        { }
        public StrategicLocationGameFile(FileInfo fileInfo) : base(fileInfo)
        { }

        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new StrategicLocationGameFile();


        public override void Validate(LinkedLayer layer)
        {
            base.Validate(layer);

            foreach (var strategicLocation in StrategicLocations.Values)
            {
                if (StrategicLocationManager.Has(strategicLocation.Name))
                    Logger.LogLayeredWarning(layer, EnumLocKey.WARNING_OBJECT_PARAMETER_OVERRIDE, new Dictionary<string, string>
                    {
                        { "{object}", "strategic_locations" },
                        { "{parameter}", "name" },
                        { "{value}", $"{strategicLocation.Name}" },
                    });
                else
                    StrategicLocationManager.AddSilent(strategicLocation);
            }
        }
    }
}
