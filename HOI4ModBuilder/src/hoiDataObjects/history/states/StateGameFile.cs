using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.utils;
using System.Collections.Generic;
using System;
using HOI4ModBuilder.hoiDataObjects.map;

namespace HOI4ModBuilder.src.hoiDataObjects.history.states
{
    public class StateGameFile : GameFile
    {
        public override IParseObject GetEmptyCopy() => new StateGameFile();

        public readonly GameParameter<State> State = new GameParameter<State>()
            .INIT_ProhibitOverwriting(true);

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "state", (o) => ((StateGameFile)o).State },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "history", "states" }, "StateFile")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override void Validate(LinkedLayer layer)
        {
            var state = State.GetValue();

            if (state != null)
            {
                foreach (var p in state.Provinces)
                    p.State = state;

                state.Validate(out bool _);
            }

            base.Validate(layer);
        }

        public StateGameFile() { }
        public StateGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public StateGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants) { }
    }
}
