using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.common.stateCategory
{
    public class StateCategoryGameFile : GameFile
    {
        public readonly GameDictionary<string, StateCategory> StateCategory = new GameDictionary<string, StateCategory>()
            .INIT_SetValueParseAdapter((o, key, value) => new StateCategory((string)key));

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "state_categories", o => ((StateCategoryGameFile)o).StateCategory },
        };
        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common" }, "StateCategoryFile")
            .Add(STATIC_ADAPTER.Keys)
            .Load();

        public StateCategoryGameFile() : base()
        { }
        public StateCategoryGameFile(FileInfo fileInfo) : base(fileInfo)
        { }

        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new StateCategoryGameFile();
    }
}
