using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.history.states
{
    public class StateHistory : AbstractParseObject, IComparable<StateHistory>, IComparer<StateHistory>
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public override IParseObject GetEmptyCopy() => new StateHistory();

        public DateTime dateTime;

        public readonly GameParameter<Country> Owner = new GameParameter<Country>()
            .INIT_SetValueParseAdapter((o, token) => CountryManager.GetCountry((string)token))
            .INIT_SetValueSaveAdapter((c) => c?.Tag);
        public readonly GameParameter<Country> Controller = new GameParameter<Country>()
            .INIT_SetValueParseAdapter((o, token) => CountryManager.GetCountry((string)token))
            .INIT_SetValueSaveAdapter((c) => c?.Tag);
        public readonly GameList<VictoryPoint> VictoryPoints = new GameList<VictoryPoint>()
            .INIT_SetForceSeparateLineSave(true)
            .INIT_SetAllowsInlineAdd(true)
            .INIT_SetValueParseAdapter((o, token) => new VictoryPoint(ParserUtils.Parse<ushort>(token)))
            .INIT_SetSortAtSaving(true);
        public readonly GameParameter<StateBuildings> StateBuildings = new GameParameter<StateBuildings>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>()
        {
            { "owner", o => ((StateHistory)o).Owner },
            { "controller", o => ((StateHistory)o).Controller },
            { "victory_points", o => ((StateHistory)o).VictoryPoints },
            { "buildings", o => ((StateHistory)o).StateBuildings },
        };
        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;


        public readonly GameList<ScriptBlockParseObject> DynamicScriptBlocks = new GameList<ScriptBlockParseObject>();
        public readonly GameDictionary<DateTime, StateHistory> InnerHistories = new GameDictionary<DateTime, StateHistory>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>()
        {
            { "dynamicScriptBlocks", new DynamicGameParameter {
                provider = o => ((StateHistory)o).DynamicScriptBlocks,
                factory = (o, key) => ParserUtils.GetAnyScriptBlockParseObject((IParentable)o, key)
            } },
            { "dynamicInnerHistories", new DynamicGameParameter {
                parseInnerBlock = true,
                provider = o => ((StateHistory)o).InnerHistories,
                factory = (o, key) => new StateHistory((IParentable)o, ParserUtils.Parse<DateTime>(key))
            } },
        };
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;

        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter(new[] { "history", "states" }, "StateHistory")
            .Add(STATIC_ADAPTER.Keys)
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;

        public bool WillHavePortInHistory(Province province)
        {
            var stateBuildings = StateBuildings.GetValue();
            if (stateBuildings != null && stateBuildings.WillHavePortInHistory(province))
                return true;

            foreach (var innerHistory in InnerHistories.Values)
            {
                if (innerHistory.WillHavePortInHistory(province))
                    return true;
            }
            return false;
        }

        public bool ForEachVictoryPoints(Func<DateTime, StateHistory, VictoryPoint, bool> action)
        {
            foreach (var vp in VictoryPoints)
            {
                var result = action(dateTime, this, vp);
                if (result)
                    return true;
            }

            foreach (var innerStateHistory in InnerHistories.Values)
                if (innerStateHistory.ForEachVictoryPoints(action))
                    return true;

            return false;
        }


        public bool TryGetProvinceBuildings(Province province, out ProvinceBuildings buildings)
        {
            var value = StateBuildings.GetValue();
            if (value != null)
                return value.TryGetProvinceBuildings(province, out buildings);

            buildings = null;
            return false;
        }
        public bool SetProvinceBuildings(Province province, ProvinceBuildings buildings)
            => StateBuildings.GetValue() != null &&
            StateBuildings.GetValue().SetProvinceBuildings(province, buildings);

        public bool SetProvinceBuildingLevel(Province province, Building building, uint newCount)
            => StateBuildings.GetValue() != null &&
            StateBuildings.GetValue().SetProvinceBuildingLevel(province, building, newCount);

        public bool SetStateBuildingLevel(Building building, uint newCount)
            => StateBuildings.GetValue() != null &&
            StateBuildings.GetValue().SetStateBuildingLevel(building, newCount);

        public bool RemoveProvinceBuildings(Province province)
            => StateBuildings.GetValue() != null &&
            StateBuildings.GetValue().RemoveProvinceBuildings(province);

        public StateHistory() { }

        public StateHistory(IParentable parent, DateTime dateTime)
        {
            SetParent(parent);
            this.dateTime = dateTime;
        }

        public bool HasAnyProvinceBuildings()
            => StateBuildings.GetValue() != null && StateBuildings.GetValue().HasAnyProvinceBuildings();


        public uint GetStateBuildingLevel(Building building)
        {
            if (StateBuildings.GetValue() == null)
                return 0;

            StateBuildings.GetValue().Buildings.TryGetValue(building, out uint value);
            return value;
        }

        public void Activate(DateTime dateTime, State state)
        {
            if (Owner.GetValue() != null)
                state.owner = Owner.GetValue();
            if (Controller.GetValue() != null)
                state.controller = Controller.GetValue();

            if (VictoryPoints.Count > 0)
            {
                foreach (var vp in VictoryPoints)
                    state.victoryPoints[vp.province] = vp.value;
            }

            StateBuildings.GetValue()?.Activate(state);

            foreach (var scriptBlock in DynamicScriptBlocks)
            {
                var info = scriptBlock.ScriptBlockInfo;
                if (info == null)
                    continue;

                var blockName = info.GetBlockName();
                if (blockName == "add_manpower")
                    state.CurrentManpower += (int)scriptBlock.GetValue();
                else if (blockName == "set_demilitarized_zone")
                    state.CurrentIsDemilitarized = (bool)scriptBlock.GetValue();
                else if (blockName == "set_state_category")
                    state.CurrentStateCategory = StateCategoryManager.GetStateCategory((string)scriptBlock.GetValue());
                else if (blockName == "set_state_name")
                    state.CurrentName = (string)scriptBlock.GetValue();
                else if (blockName == "reset_state_name" && (bool)scriptBlock.GetValue())
                    state.CurrentName = state.Name.GetValue().stringValue;
                //TODO add resources and victory points effects support
            }

            foreach (var innerHistory in InnerHistories.Values)
            {
                if (innerHistory.dateTime > dateTime)
                    break;

                innerHistory.Activate(dateTime, state);
            }
        }

        public override void Validate(LinkedLayer layer)
        {
            InnerHistories.SortIfNeeded();
            base.Validate(layer);
        }

        public bool SortInnerHistoriesIfNeeded() => InnerHistories.SortIfNeeded();

        public int CompareTo(StateHistory other)
            => dateTime.CompareTo(other.dateTime);

        public int Compare(StateHistory x, StateHistory y)
        {
            if (x is null || y is null)
                throw new ArgumentException("Некорректное значение параметра для сортировки");
            return x.dateTime.CompareTo(y.dateTime);
        }
    }
}
