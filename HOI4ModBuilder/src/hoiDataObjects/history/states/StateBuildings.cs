using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HOI4ModBuilder.src.hoiDataObjects.history.states
{

    public class StateBuildings : AbstractParseObject
    {
        public readonly GameDictionary<Building, uint> Buildings = new GameDictionary<Building, uint>()
            .INIT_SetKeyParseAdapter(token => BuildingManager.GetBuilding(token))
            .INIT_SetKeySaveAdapter(building => building?.Name);
        public readonly GameDictionary<Province, ProvinceBuildings> Provinces = new GameDictionary<Province, ProvinceBuildings>()
            .INIT_SetKeyParseAdapter(token => ProvinceManager.GetProvince(ParserUtils.Parse<ushort>(token)))
            .INIT_SetKeySaveAdapter(province => province?.Id)
            .INIT_SetSortAtSaving(true);

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>()
        {
            { "dynamicBuildings", new DynamicGameParameter {
                provider = o => ((StateBuildings)o).Buildings,
                factory = (o, key) => BuildingManager.HasBuilding(key) ? new uint?(0) : null
            } },
            { "dynamicProvinces", new DynamicGameParameter {
                parseInnerBlock = true,
                provider = o => ((StateBuildings)o).Provinces,
                factory = (o, key) => ushort.TryParse(key, out var _) ? new ProvinceBuildings((StateBuildings) o) : null
            } },
        };
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "history", "states" }, "StateBuildings")
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public StateBuildings() { }
        public StateBuildings(StateHistory parent)
        {
            SetParent(parent);
        }

        public override IParseObject GetEmptyCopy() => new StateBuildings();

        public bool TryGetProvinceBuildings(Province province, out ProvinceBuildings buildings)
            => Provinces.TryGetValue(province, out buildings);
        public bool SetProvinceBuildings(Province province, ProvinceBuildings buildings)
        {
            buildings.SetParent(this);
            Provinces[province] = buildings;
            return true;
        }
        public bool RemoveProvinceBuildings(Province province)
        {
            bool result = Provinces.Remove(province);
            if (result)
                SetNeedToSave(result);
            return result;
        }
        public bool HasAnyProvinceBuildings()
            => Provinces.Count > 0;

        public void Activate(State state)
        {
            foreach (var stateBuilding in Buildings)
                state.stateBuildings[stateBuilding.Key] = stateBuilding.Value;

            foreach (var provinceBuilding in Provinces)
                provinceBuilding.Value.Activate(state, provinceBuilding.Key);
        }

        public bool WillHavePortInHistory(Province province)
        {
            if (Provinces.TryGetValue(province, out var provinceBuildings))
                return provinceBuildings.WillHavePortInHistory();
            return false;
        }

        public bool SetStateBuildingLevel(Building building, uint newCount)
        {
            if (!Buildings.TryGetValue(building, out uint count))
            { //Если в словаре ещё нет постройки
                if (newCount != 0)
                { //И число новой постройки не равно 0
                    //Добавляем постройку в словари
                    Buildings[building] = newCount;
                    return true;
                }
                else return false;
            }
            else if (count != newCount)
            { //Если в словаре уже есть эта постройка
                //Меняем число постройки в области
                if (newCount == 0)
                    Buildings.Remove(building);
                else
                    Buildings[building] = newCount;
                return true;
            }
            else return false;
        }

        public bool SetProvinceBuildingLevel(Province province, Building building, uint newCount)
        {
            if (Provinces.TryGetValue(province, out var provinceBuildings))
                return provinceBuildings.SetProvinceBuildingLevel(building, newCount);
            if (newCount == 0)
                return false;

            provinceBuildings = new ProvinceBuildings(this);
            Provinces[province] = provinceBuildings;
            return provinceBuildings.SetProvinceBuildingLevel(building, newCount);
        }
    }
}
