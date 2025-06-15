using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.history.states
{

    public class ProvinceBuildings : AbstractParseObject
    {
        public readonly GameList<ScriptBlockParseObject> Buildings = new GameList<ScriptBlockParseObject>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>()
        {
            { "dynamicBuildings", new DynamicGameParameter {
                provider = o => ((ProvinceBuildings)o).Buildings,
                factory = (o, key) => ParserUtils.GetBuildingCustomBlockParseObject((IParentable)o, key)
            } },
        };
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "history", "states" }, "ProvinceBuildings")
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new ProvinceBuildings();

        public ProvinceBuildings() { }
        public ProvinceBuildings(StateBuildings parent)
        {
            SetParent(parent);
        }

        public bool SetProvinceBuildingLevel(Building building, uint newCount)
        {
            if (newCount == 0)
                return Buildings.RemoveFirstFromEndIf((o) => o.ScriptBlockInfo.GetBlockName() == building.Name);

            if (Buildings.TryGetFirstFromEndIf((o) => o.ScriptBlockInfo.GetBlockName() == building.Name, out var result))
            {

                uint resultCount = GetBuildingLevel(result, building);

                if (resultCount == newCount)
                    return false;

                if (newCount == 0)
                    Buildings.RemoveFirstFromEndIf((o) => o.ScriptBlockInfo.GetBlockName() == building.Name);
                else
                    SetBuildingLevel(result, building, (int)newCount);

                return true;
            }
            else
            {
                if (newCount == 0)
                    return false;

                var newObj = ParserUtils.GetBuildingCustomBlockParseObject(this, building.Name);
                newObj.SetValue(newCount);
                Buildings.Add(newObj);
                return true;
            }
        }

        private uint GetBuildingLevel(ScriptBlockParseObject block, Building building)
        {
            var value = block.GetValueRaw();

            if (value is int intValue)
                return (uint)intValue;

            if (!(value is GameList<ScriptBlockParseObject> innerList))
                throw new Exception("Building \"" + building.Name + "\" invalid definition of value");

            if (!innerList.TryGetFirstFromEndIf((o) => o.ScriptBlockInfo.GetBlockName() == "level", out var levelBlock))
                throw new Exception("Building \"" + building.Name + "\" definition does not contain level value");

            value = levelBlock.GetValueRaw();
            if (!(value is int intValueInner))
                throw new Exception("Building \"" + building.Name + "\" invalid definition of level value");

            return (uint)intValueInner;
        }

        private void SetBuildingLevel(ScriptBlockParseObject block, Building building, int value)
        {
            var rawValue = block.GetValueRaw();

            if (rawValue is int intValue)
            {
                block.SetValue(value);
                return;
            }

            if (!(rawValue is GameList<ScriptBlockParseObject> innerList))
                throw new Exception("Building \"" + building.Name + "\" invalid definition of value");

            if (!innerList.TryGetFirstFromEndIf((o) => o.ScriptBlockInfo.GetBlockName() == "level", out var levelBlock))
                throw new Exception("Building \"" + building.Name + "\" definition does not contain level value");

            rawValue = levelBlock.GetValueRaw();
            if (!(rawValue is int))
                throw new Exception("Building \"" + building.Name + "\" invalid definition of level value");

            levelBlock.SetValue(value);
            return;
        }

        public void Activate(State state, Province province)
        {
            if (!state.provincesBuildings.TryGetValue(province, out var buildings))
            {
                buildings = new Dictionary<Building, uint>(0);
                state.provincesBuildings[province] = buildings;
            }

            foreach (var entry in Buildings)
            {
                if (BuildingManager.TryGetBuilding(entry.ScriptBlockInfo.GetBlockName(), out var building))
                    buildings[building] = GetBuildingLevel(entry, building);
            }
        }

        public bool WillHavePortInHistory()
        {
            foreach (var entry in Buildings)
            {
                if (!BuildingManager.TryGetBuilding(entry.ScriptBlockInfo.GetBlockName(), out var building))
                    continue;

                if (building.IsPort.GetValue())
                    return true;
            }
            return false;
        }

        public override void Validate(LinkedLayer layer)
        {
            HashSet<string> definedBuildings = new HashSet<string>();

            Buildings.RemoveFirstFromEndIf(o =>
            {
                var buildingName = o.ScriptBlockInfo.GetBlockName();
                if (definedBuildings.Contains(buildingName))
                    return true;

                definedBuildings.Add(buildingName);
                return false;
            });

            base.Validate(layer);
        }

    }
}
