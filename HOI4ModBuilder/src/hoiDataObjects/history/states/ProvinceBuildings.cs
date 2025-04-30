using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.history.states
{

    public class ProvinceBuildings : AbstractParseObject
    {
        public readonly GameDictionary<Building, uint> Buildings = new GameDictionary<Building, uint>()
            .INIT_SetKeyParseAdapter(token => BuildingManager.GetBuilding(token))
            .INIT_SetKeySaveAdapter(building => building?.Name);

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>()
        {
            { "dynamicBuildings", new DynamicGameParameter {
                provider = o => ((ProvinceBuildings)o).Buildings,
                factory = (o, key) => new uint()
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
                return Buildings.Remove(building);

            if (Buildings.TryGetValue(building, out var result))
            {
                if (result == newCount)
                    return false;

                if (newCount == 0)
                    Buildings.Remove(building);
                else
                    Buildings[building] = newCount;

                return true;
            }
            else
            {
                if (newCount == 0)
                    return false;

                Buildings[building] = result;
                return true;
            }
        }

        public void Activate(State state, Province province)
        {
            if (!state.provincesBuildings.TryGetValue(province, out var buildings))
            {
                buildings = new Dictionary<Building, uint>(0);
                state.provincesBuildings[province] = buildings;
            }

            foreach (var entry in Buildings)
                buildings[entry.Key] = entry.Value;
        }

        public bool WillHavePortInHistory()
        {
            foreach (var entry in Buildings)
                if (entry.Key.IsPort.GetValue())
                    return true;
            return false;
        }

    }
}
