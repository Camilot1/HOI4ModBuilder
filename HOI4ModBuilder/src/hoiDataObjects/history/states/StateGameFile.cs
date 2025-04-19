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

        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter(new[] { "history", "states" }, "StateFile")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;

        public override void Validate(LinkedLayer layer)
        {
            var state = State.GetValue();

            if (state != null)
            {
                foreach (var p in state.Provinces)
                    p.State = state;
            }

            base.Validate(layer);
        }

        public StateGameFile() { }
        public StateGameFile(FileInfo fileInfo) : base(fileInfo) { }
        public StateGameFile(FileInfo fileInfo, bool allowsConstants) : base(fileInfo, allowsConstants) { }
    }
}
