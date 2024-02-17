using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static HOI4ModBuilder.src.dataObjects.argBlocks.InfoArgsBlock;

namespace HOI4ModBuilder.src.hoiDataObjects.history.states
{
    class StateHistory : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool isStartHistory;
        public State state;
        public Country owner;
        public Country controller;
        public List<Country> addCoresOf = new List<Country>(0);
        public List<Country> removeCoresOf = new List<Country>(0);
        public List<Country> addClaimsBy = new List<Country>(0);
        public List<Country> removeClaimsBy = new List<Country>(0);

        public int addManpower;
        public bool[] isDemilitarized;
        public Dictionary<Province, uint> victoryPoints = new Dictionary<Province, uint>(0);

        public List<DataArgsBlock> dataArgsBlocks = new List<DataArgsBlock>(0);

        public Dictionary<Building, uint> stateBuildings = new Dictionary<Building, uint>(0);
        public Dictionary<Province, Dictionary<Building, uint>> provincesBuildings = new Dictionary<Province, Dictionary<Building, uint>>(0);

        public StateHistory(State state)
        {
            this.state = state;
        }

        public bool HasAnyData()
        {
            return state != null || owner != null || controller != null ||
                addCoresOf.Count > 0 || removeCoresOf.Count > 0 ||
                addClaimsBy.Count > 0 || removeClaimsBy.Count > 0 ||
                addManpower != 0 || isDemilitarized != null ||
                victoryPoints.Count > 0 || stateBuildings.Count > 0 ||
                HasAnyProvinceBuildings() || dataArgsBlocks.Count > 0;
        }

        public bool HasAnyProvinceBuildings() => provincesBuildings.Count() > 0;

        public bool SetProvinceBuilding(Province province, Building building, uint newCount)
        {
            if (!provincesBuildings.TryGetValue(province, out Dictionary<Building, uint> buildings))
            { //Если для указанной провинции ещё нет построек
                if (newCount != 0)
                {
                    //Создаём словарь построек
                    buildings = new Dictionary<Building, uint>(0);
                    provincesBuildings[province] = buildings;

                    //Обновляем количество постройки в словаре
                    buildings[building] = newCount;
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
                if (newCount == 0 && isStartHistory)
                {
                    buildings.Remove(building);
                    if (buildings.Count == 0) provincesBuildings.Remove(province);
                }
                else buildings[building] = newCount;

                //Если в провинции 0 построек этого типа, то удаляем постройку из списка построек провинций
                if (newCount == 0) province.RemoveBuilding(building);
                //Иначе меняем число постройки в провинции на 0
                else province.SetBuilding(building, newCount);
                return true;
            }
            else return false;
        }

        public bool SetStateBuilding(Building building, uint newCount)
        {
            if (!stateBuildings.TryGetValue(building, out uint count))
            { //Если в словаре ещё нет постройки
                if (newCount != 0)
                { //И число новой постройки не равно 0
                    //Добавляем постройку в словари
                    stateBuildings[building] = newCount;
                    return true;
                }
                else return false;
            }
            else if (count != newCount)
            { //Если в словаре уже есть эта постройка
                //Меняем число постройки в области
                if (newCount == 0 && isStartHistory) stateBuildings.Remove(building);
                else stateBuildings[building] = newCount;
                return true;
            }
            else return false;
        }

        public void Save(StringBuilder sb, string prefix, string outTab, string tab, Dictionary<DateTime, StateHistory> innerHistories)
        {
            if (!HasAnyData()) return;

            string tab2 = tab + tab;
            string newTab = outTab + tab;

            sb.Append(outTab).Append(prefix).Append(" = {").Append(Constants.NEW_LINE);

            if (owner != null)
                sb.Append(outTab).Append(tab).Append("owner = ").Append(owner.Tag).Append(Constants.NEW_LINE);
            if (controller != null)
                sb.Append(outTab).Append(tab).Append("controller = ").Append(controller.Tag).Append(Constants.NEW_LINE);

            if ((owner != null || controller != null) &&
                (victoryPoints.Count > 0 || stateBuildings.Count > 0 || provincesBuildings.Count > 0 ||
                innerHistories != null && innerHistories.Count > 0 || dataArgsBlocks.Count > 0))
                sb.Append(outTab).Append(tab).Append(Constants.NEW_LINE);

            if (victoryPoints.Count > 0)
            {

                var provincesKeys = new List<Province>(victoryPoints.Keys);
                provincesKeys.Sort((x, y) => x.Id.CompareTo(y.Id));

                foreach (var province in provincesKeys)
                    sb.Append(outTab).Append(tab).Append("victory_points = { ")
                        .Append(province.Id).Append(' ').Append(victoryPoints[province])
                      .Append(" }").Append(Constants.NEW_LINE);

                sb.Append(outTab).Append(tab).Append(Constants.NEW_LINE);
            }

            if (stateBuildings.Count > 0 || provincesBuildings.Count > 0)
            {
                sb.Append(outTab).Append(tab).Append("buildings = {").Append(Constants.NEW_LINE);

                foreach (var building in stateBuildings.Keys)
                    sb.Append(outTab).Append(tab2).Append(building.name).Append(" = ").Append(stateBuildings[building]).Append(Constants.NEW_LINE);

                if (provincesBuildings.Count > 0)
                {
                    if (stateBuildings.Count > 0) sb.Append(Constants.NEW_LINE);

                    var provincesKeys = new List<Province>(provincesBuildings.Keys);
                    provincesKeys.Sort((x, y) => x.Id.CompareTo(y.Id));

                    foreach (var province in provincesKeys)
                    {
                        var provinceBuildinsById = provincesBuildings[province];
                        bool manyBuildings = provinceBuildinsById.Count != 1;

                        sb.Append(outTab).Append(tab2).Append(province.Id).Append(" = { ");
                        if (manyBuildings) sb.Append(Constants.NEW_LINE);

                        foreach (var building in provinceBuildinsById.Keys)
                        {
                            if (manyBuildings)
                            {
                                sb.Append(outTab).Append(tab2).Append(tab).Append(building.name)
                                  .Append(" = ").Append(provinceBuildinsById[building]).Append(Constants.NEW_LINE);
                            }
                            else sb.Append(building.name).Append(" = ").Append(provinceBuildinsById[building]).Append(' ');
                        }

                        if (manyBuildings) sb.Append(outTab).Append(tab2);
                        sb.Append("}").Append(Constants.NEW_LINE);
                    }
                }
                sb.Append(outTab).Append(tab).Append("}").Append(Constants.NEW_LINE)
                    .Append(outTab).Append(tab).Append(Constants.NEW_LINE);
            }

            if (innerHistories != null && innerHistories.Count > 0)
            {
                foreach (DateTime dateTime in innerHistories.Keys)
                    innerHistories[dateTime].Save(sb, Utils.DateStampToString(dateTime), newTab, tab, null);
            }

            if (dataArgsBlocks.Count == 1 && dataArgsBlocks[0].innerArgsBlocks.Count == 0) sb.Append(newTab);
            foreach (var dataBlock in dataArgsBlocks) dataBlock.Save(sb, newTab, tab);
            if (dataArgsBlocks.Count == 1) sb.Append(Constants.NEW_LINE);

            sb.Append(outTab).Append("}").Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);
        }

        public void Activate()
        {
            if (owner != null) state.owner = owner;
            if (controller != null) state.controller = controller;
            if (addCoresOf.Count > 0) state.coresOf.AddRange(addCoresOf);
            if (removeCoresOf.Count > 0) state.coresOf.RemoveAll(c => removeCoresOf.Contains(c));
            if (addClaimsBy.Count > 0) state.claimsBy.AddRange(addClaimsBy);
            if (removeClaimsBy.Count > 0) state.claimsBy.RemoveAll(c => removeClaimsBy.Contains(c));
            state.manpower += addManpower;
            if (isDemilitarized != null) state.isDemilitarized = isDemilitarized[0];

            if (victoryPoints.Count > 0)
            {
                foreach (var province in victoryPoints.Keys)
                    state.victoryPoints[province] = victoryPoints[province];
            }

            if (stateBuildings.Count > 0)
            {
                foreach (var building in stateBuildings.Keys)
                    state.stateBuildings[building] = stateBuildings[building];
            }

            if (provincesBuildings.Count > 0)
            {
                foreach (var province in provincesBuildings.Keys)
                {
                    if (!state.provincesBuildings.TryGetValue(province, out Dictionary<Building, uint> provinceBuildings))
                    {
                        provinceBuildings = new Dictionary<Building, uint>(0);
                        state.provincesBuildings[province] = provinceBuildings;
                    }

                    var dictionary = provincesBuildings[province];
                    if (dictionary != null && dictionary.Count > 0)
                    {
                        foreach (var building in dictionary.Keys)
                            provinceBuildings[building] = dictionary[building];
                    }
                }
            }

        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "owner":
                    string countryTag = parser.ReadString();
                    if (CountryManager.TryGetCountry(countryTag, out Country newOwner)) owner = newOwner;
                    else throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.ERROR_STATE_HISTORY_OWNER_COUNTRY_NOT_FOUND,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{state.Id}" },
                                { "{countryTag}", countryTag }
                            }
                        ));
                    break;

                case "controller":
                    countryTag = parser.ReadString();
                    if (CountryManager.TryGetCountry(countryTag, out Country newController)) controller = newController;
                    else throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.ERROR_STATE_HISTORY_CONTROLLER_COUNTRY_NOT_FOUND,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{state.Id}" },
                                { "{countryTag}", countryTag }
                            }
                        ));
                    break;
                case "victory_points":
                    IList<string> values = parser.ReadStringList();
                    if (values.Count != 2)
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.ERROR_STATE_HISTORY_VICTORY_POINTS_INCORRECT_PARAMS_COUNT,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{state.Id}" },
                                { "{currentCount}", $"{values.Count}" },
                                { "{currentCount}", "2" },
                            }
                        ));

                    if (!ushort.TryParse(values[0], out ushort provinceId))
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.ERROR_STATE_HISTORY_VICTORY_POINTS_INCORRECT_PROVINCE_ID_VALUE,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{state.Id}" },
                                { "{provinceId}", values[0] }
                            }
                        ));

                    if (!ProvinceManager.TryGetProvince(provinceId, out Province province))
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.ERROR_STATE_HISTORY_VICTORY_POINTS_INCORRECT_PROVINCE_NOT_FOUND,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{state.Id}" },
                                { "{provinceId}", values[0] }
                            }
                        ));

                    if (uint.TryParse(values[1], out uint vpValue)) { }
                    else if (float.TryParse(values[1].Replace('.', ','), out float vpValueFloat) && vpValueFloat > 0)
                        vpValue = (uint)Math.Round(vpValueFloat);
                    else throw new Exception(GuiLocManager.GetLoc(
                             EnumLocKey.ERROR_STATE_HISTORY_VICTORY_POINTS_INCORRECT_POINTS_VALUE,
                             new Dictionary<string, string>
                             {
                                 { "{stateId}", $"{state.Id}" },
                                 { "{value}", values[1] }
                             }
                         ));

                    if (victoryPoints.TryGetValue(province, out uint oldVP))
                    {
                        if (oldVP > 0)
                            throw new Exception(GuiLocManager.GetLoc(
                                        EnumLocKey.ERROR_STATE_HISTORY_VICTORY_POINTS_PROVINCE_ALREADY_HAS_VICTORY_POINTS,
                                        new Dictionary<string, string>
                                        {
                                        { "{stateId}", $"{state.Id}" },
                                        { "{provinceId}", $"{province.Id}" },
                                        { "{newVictoryPoints}", $"{vpValue}" },
                                        { "{oldVictoryPoints}", $"{oldVP}" }
                                        }
                                    ));
                    }
                    else victoryPoints[province] = vpValue;

                    break;

                case "buildings":
                    parser.Parse(new StateBuildingsDictionary(state, stateBuildings, provincesBuildings));
                    break;

                default:
                    //DateStamps
                    if (Utils.TryParseDateTimeStamp(token, out DateTime dateTime))
                    {
                        if (!state.stateHistories.TryGetValue(dateTime, out StateHistory stateHistory))
                        {
                            stateHistory = new StateHistory(state);
                            state.stateHistories[dateTime] = stateHistory;
                        }
                        parser.Parse(stateHistory);
                        stateHistory.ExecuteAfterParse();
                        break;
                    }

                    try
                    {
                        DataArgsBlocksManager.ParseDataArgsBlock(parser, null, token, null, dataArgsBlocks);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_WHILE_STATE_HISTORY_LOADING,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{state.Id}" },
                                { "{token}", token }
                            }
                        ), ex);
                    }
                    break;
            }
        }

        public void ExecuteAfterParse()
        {
            ExecuteBlockFunctions();
        }

        private void ExecuteBlockFunctions()
        {
            foreach (var dataBlock in dataArgsBlocks)
            {
                if (dataBlock.InfoArgsBlock == null) continue;

                var function = dataBlock.InfoArgsBlock.GetFunctionByScope(EnumScope.STATE);
                if (function == EnumArgsBlockFunctions.NONE) continue;

                Country country;
                try
                {
                    switch (function) //TODO Перепроверить, правильно ли работает
                    {
                        case EnumArgsBlockFunctions.EFFECT_STATE_ADD_CORE_OF:
                            if (CountryManager.TryGetCountry((string)dataBlock.Value, out country))
                                addCoresOf.Add(country);
                            break;
                        case EnumArgsBlockFunctions.EFFECT_STATE_REMOVE_CORE_OF:
                            if (CountryManager.TryGetCountry((string)dataBlock.Value, out country))
                                removeCoresOf.Add(country);
                            break;
                        case EnumArgsBlockFunctions.EFFECT_STATE_ADD_CLAIM_BY:
                            if (CountryManager.TryGetCountry((string)dataBlock.Value, out country))
                                addClaimsBy.Add(country);
                            break;
                        case EnumArgsBlockFunctions.EFFECT_STATE_REMOVE_CLAIM_BY:
                            if (CountryManager.TryGetCountry((string)dataBlock.Value, out country))
                                removeClaimsBy.Add(country);
                            break;
                        case EnumArgsBlockFunctions.EFFECT_STATE_ADD_MANPOWER:
                            addManpower = (int)dataBlock.Value;
                            break;
                        case EnumArgsBlockFunctions.EFFECT_STATE_SET_DEMILITARIZED_ZONE:
                            isDemilitarized = new bool[] { (bool)dataBlock.Value };
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_CANT_EXECUTE_BLOCK_FUNCTIONS,
                        new Dictionary<string, string>
                        {
                            { "{functionName}", function.ToString() },
                            { "{blockName}", dataBlock.GetName() },
                            { "{blockValue}", $"{dataBlock.Value}" }
                        }
                    ), ex);
                }
            }
        }
    }

    class StateBuildingsDictionary : IParadoxRead
    {
        public State state;
        public Dictionary<Building, uint> stateBuildings;
        public Dictionary<Province, Dictionary<Building, uint>> provincesBuildings;

        public StateBuildingsDictionary(State state, Dictionary<Building, uint> stateBuildings, Dictionary<Province, Dictionary<Building, uint>> provincesBuildings)
        {
            this.state = state;
            this.stateBuildings = stateBuildings;
            this.provincesBuildings = provincesBuildings;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (ushort.TryParse(token, out ushort id))
            {
                if (ProvinceManager.TryGetProvince(id, out Province province))
                {
                    if (!provincesBuildings.TryGetValue(province, out Dictionary<Building, uint> provinceBuildings))
                    {
                        provinceBuildings = new Dictionary<Building, uint>(0);
                        provincesBuildings[province] = provinceBuildings;
                    }
                    parser.Parse(new ProvinceBuildingsDictionary(state, province, provinceBuildings));
                }
                else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_STATE_HISTORY_BUILDINGS_UNKNOWN_PROVINCE_ID,
                        new Dictionary<string, string>
                        {
                            { "{stateId}", $"{state.Id}" },
                            { "{provinceId}", $"{id}" }
                        }
                    ));
            }
            else if (BuildingManager.TryGetBuilding(token, out Building building))
            {
                if (building.enumBuildingSlotCategory != Building.EnumBuildingSlotCategory.PROVINCIAL)
                    stateBuildings[building] = parser.ReadUInt32();
                else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_STATE_HISTORY_BUILDINGS_PROVINCIAL_BUILDING_IN_STATE,
                        new Dictionary<string, string>
                        {
                            { "{stateId}", $"{state.Id}" },
                            { "{buildingName}", building.name }
                        }
                    ));
            }
            else throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.ERROR_STATE_HISTORY_BUILDINGS_UNKNOWN_TOKEN,
                    new Dictionary<string, string>
                    {
                        { "{stateId}", $"{state.Id}" },
                        { "{token}", token }
                    }
                ));
        }
    }

    class ProvinceBuildingsDictionary : IParadoxRead
    {
        public State state;
        public Province province;
        public Dictionary<Building, uint> provinceBuildings;

        public ProvinceBuildingsDictionary(State state, Province province, Dictionary<Building, uint> provinceBuildings)
        {
            this.state = state;
            this.province = province;
            this.provinceBuildings = provinceBuildings;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (BuildingManager.TryGetBuilding(token, out Building building))
            {
                if (building.enumBuildingSlotCategory == Building.EnumBuildingSlotCategory.PROVINCIAL)
                    provinceBuildings[building] = parser.ReadUInt32();
                else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_STATE_HISTORY_BUILDINGS_NOT_PROVINCIAL_BUILDING_IN_PROVINCE,
                        new Dictionary<string, string>
                        {
                            { "{stateId}", $"{state.Id}" },
                            { "{provinceId}", $"{province.Id}" },
                            { "{buildingName}", building.name }
                        }
                    ));
            }
            else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_STATE_HISTORY_BUILDINGS_UNKNOWN_TOKEN_IN_PROVINCE,
                        new Dictionary<string, string>
                        {
                            { "{stateId}", $"{state.Id}" },
                            { "{provinceId}", $"{province.Id}" },
                            { "{token}", token }
                        }
                    ));
        }
    }
}
