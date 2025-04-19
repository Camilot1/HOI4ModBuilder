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

        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter(new[] { "history", "states" }, "ProvinceBuildings")
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;

        public override IParseObject GetEmptyCopy() => new ProvinceBuildings();

        public ProvinceBuildings() { }
        public ProvinceBuildings(StateBuildings parent)
        {
            SetParent(parent);
        }

        public bool SetProvinceBuildingLevel(Building building, uint newCount)
        {
            /*
            if (!Provinces.TryGetValue(province, out ProvinceBuildings buildings))
            { //Если для указанной провинции ещё нет построек
                if (newCount != 0)
                {
                    //Создаём словарь построек
                    buildings = new ProvinceBuildings(this, province);
                    Provinces[province] = buildings;

                    //Обновляем количество постройки в словаре
                    buildings.Buildings[building] = newCount;
                    province.SetBuilding(building, newCount);
                    return true;
                }
                else return false;
            }
            else if (!buildings.TryGetValue(building, out uint count))
            { //Если в словаре ещё нет постройки
                if (newCount != 0)
                { //И число новой постройки не равно 0
                    //Добавляем постройку в словари
                    buildings[building] = newCount;
                    province.SetBuilding(building, newCount);
                    return true;
                }
                else return false;
            }
            else if (count != newCount)
            { //Если в словаре уже есть эта постройка
                //Меняем число постройки в области
                if (newCount == 0 && dateTime == default)
                {
                    buildings.Remove(building);
                    if (buildings.Count == 0)
                        Provinces.Remove(province);
                }
                else buildings[building] = newCount;

                //Если в провинции 0 построек этого типа, то удаляем постройку из списка построек провинций
                if (newCount == 0) province.RemoveBuilding(building);
                //Иначе меняем число постройки в провинции на 0
                else province.SetBuilding(building, newCount);
                return true;
            }
            */
            return false;
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
