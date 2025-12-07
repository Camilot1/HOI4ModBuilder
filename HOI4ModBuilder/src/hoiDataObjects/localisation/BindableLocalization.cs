using HOI4ModBuilder.src.dataObjects.argBlocks;
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

namespace HOI4ModBuilder.src.hoiDataObjects.localisation
{
    public class BindableLocalization : AbstractParseObject
    {
        public readonly GameParameter<GameString> LocalizationKey = new GameParameter<GameString>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "localization_key", o => ((BindableLocalization)o).LocalizationKey },
        };
        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        public readonly GameDictionary<GameString, GameString> Parameters = new GameDictionary<GameString, GameString>()
            .INIT_SetKeyParseAdapter((o, token) => new GameString { stringValue = token });

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "dynamicParameters", new DynamicGameParameter {
                provider = o => ((BindableLocalization)o).Parameters,
                factory = (o, key) => new GameString()
            } },
        };

        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;

        public override IParseObject GetEmptyCopy() => new BindableLocalization();

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "localization" }, "BindableLocalization")
            .Add(STATIC_ADAPTER.Keys)
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;
    }
}
