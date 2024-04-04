using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects.map
{
    class State : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public FileInfo fileInfo;

        public bool HasChangedId { get; private set; }

        private ushort _id;
        public ushort Id
        {
            get => _id;
            set
            {
                if (_id == value) return;

                if (StateManager.ContainsStateIdKey(value))
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_STATE_ID_UPDATE_VALUE_IS_USED,
                        new Dictionary<string, string> { { "{id}", $"{value}" } }
                    ));
                else StateManager.RemoveState(_id);
                _id = value;
                HasChangedId = true;
                StateManager.AddState(_id, this);
            }
        }

        public string startName, name;
        public int startManpower, manpower;
        public StateCategory startStateCategory, stateCategory;
        public List<Province> provinces = new List<Province>(0);
        public Dictionary<Resource, uint> resources = new Dictionary<Resource, uint>(0);
        public List<ProvinceBorder> borders = new List<ProvinceBorder>(0);

        public Country owner;
        public Country controller;
        public List<Country> coresOf = new List<Country>(0);
        public List<Country> claimsBy = new List<Country>(0);
        public bool isImpassible;
        public bool isDemilitarized;
        public Dictionary<Province, uint> victoryPoints = new Dictionary<Province, uint>(0);
        public Dictionary<Building, uint> stateBuildings = new Dictionary<Building, uint>(0);
        public Dictionary<Province, Dictionary<Building, uint>> provincesBuildings = new Dictionary<Province, Dictionary<Building, uint>>(0);

        public float localSupplies;
        public float buildingsMaxLevelFactor = 1f;

        //history
        public StateHistory startHistory;
        public Dictionary<DateTime, StateHistory> stateHistories = new Dictionary<DateTime, StateHistory>(0);
        public StateHistory currentHistory;

        public int color;
        public Point2F center;
        public bool dislayCenter;
        public uint pixelsCount;

        public void AddProvince(Province province)
        {
            provinces.Add(province);
            provinces.Sort((x, y) => x.Id.CompareTo(y.Id));
            province.State = this;
            fileInfo.needToSave = true;
        }

        public bool RemoveProvince(Province province)
        {
            if (provinces.Remove(province))
            {
                province.State = null;
                fileInfo.needToSave = true;
                return true;
            }
            return false;
        }

        public void CalculateCenter()
        {
            double sumX = 0, sumY = 0;
            double pixelsCount = 0;
            foreach (var province in provinces)
            {
                sumX += province.center.x * province.pixelsCount;
                sumY += province.center.y * province.pixelsCount;
                pixelsCount += province.pixelsCount;
            }
            if (pixelsCount != 0)
            {
                center.x = (float)(sumX / pixelsCount);
                center.y = (float)(sumY / pixelsCount);
            }
            this.pixelsCount = (uint)pixelsCount;
        }

        public void SetProvinceBuilding(Province province, Building building, uint newCount)
        {
            if (currentHistory.SetProvinceBuilding(province, building, newCount))
            {
                fileInfo.needToSave = true;
                UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
            }
        }

        public void SetStateBuilding(Building building, uint newCount)
        {
            if (currentHistory.SetStateBuilding(building, newCount))
            {
                fileInfo.needToSave = true;
                UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
            }
        }

        public void Save(StringBuilder sb)
        {
            string tab = "\t";
            string tab2 = tab + tab;

            sb.Append("state = {").Append(Constants.NEW_LINE);
            sb.Append(tab).Append("id = ").Append(_id).Append(Constants.NEW_LINE);
            sb.Append(tab).Append("name = \"").Append(startName).Append("\"").Append(Constants.NEW_LINE);
            sb.Append(tab).Append("manpower = ").Append(startManpower).Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);

            if (startStateCategory != null) sb.Append(tab).Append("state_category = ").Append(startStateCategory.name).Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);

            sb.Append(tab).Append("provinces = {").Append(Constants.NEW_LINE);
            sb.Append(tab2);
            foreach (var province in provinces) sb.Append(province.Id).Append(' ');
            sb.Append(Constants.NEW_LINE).Append(tab).Append('}').Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);

            if (resources.Count > 0)
            {
                sb.Append(tab).Append("resources = {").Append(Constants.NEW_LINE);
                foreach (Resource resource in resources.Keys)
                {
                    sb.Append(tab2).Append(resource.tag).Append(" = ").Append(resources[resource]).Append(Constants.NEW_LINE);
                }
                sb.Append(tab).Append('}').Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);
            }

            startHistory?.Save(sb, "history", tab, tab, stateHistories);

            if (buildingsMaxLevelFactor != 1f) sb.Append(tab).Append("buildings_max_level_factor = ").Append(Utils.FloatToString(buildingsMaxLevelFactor)).Append(Constants.NEW_LINE);
            if (localSupplies != 0) sb.Append(tab).Append("local_supplies = ").Append(Utils.FloatToString(localSupplies)).Append(Constants.NEW_LINE);
            sb.Append('}').Append(Constants.NEW_LINE);
        }

        public void UpdateByDateTimeStamp(DateTime dateTime)
        {
            ClearData();

            var dateTimes = new List<DateTime>(stateHistories.Keys);
            dateTimes.Sort();

            if (startHistory != null)
            {
                startHistory.Activate();
                currentHistory = startHistory;
            }
            foreach (DateTime dt in dateTimes)
            {
                if (dt < dateTime)
                {
                    currentHistory = stateHistories[dt];
                    currentHistory.Activate();
                }
            }

            AddData();
        }

        public void ClearData()
        {
            name = startName;
            manpower = startManpower;
            stateCategory = startStateCategory;

            owner?.ownStates.Remove(this);
            owner = null;
            controller?.controlsStates.Remove(this);
            controller = null;

            foreach (var c in coresOf) c.hasCoresAtStates.Remove(this);
            coresOf = new List<Country>(0);

            foreach (var c in claimsBy) c.hasClaimsAtState.Remove(this);
            coresOf = new List<Country>(0);

            isImpassible = false;
            isDemilitarized = false;

            foreach (var province in victoryPoints.Keys) province.victoryPoints = 0;
            victoryPoints = new Dictionary<Province, uint>(0);
            stateBuildings = new Dictionary<Building, uint>(0);
            foreach (var province in provincesBuildings.Keys) province.ClearBuildings();
            provincesBuildings = new Dictionary<Province, Dictionary<Building, uint>>(0);
        }

        public void AddData()
        {
            foreach (var province in victoryPoints.Keys) province.victoryPoints = victoryPoints[province];
            foreach (var province in provincesBuildings.Keys) province.SetBuildings(provincesBuildings[province]);
            owner?.ownStates.Add(this);
            controller?.controlsStates.Add(this);
            foreach (var country in coresOf) country.hasCoresAtStates.Add(this);
            foreach (var country in claimsBy) country.hasClaimsAtState.Add(this);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            string temp;

            switch (token)
            {
                case "id":
                    temp = parser.ReadString();
                    if (!ushort.TryParse(temp, out _id))
                        Logger.LogError(
                            EnumLocKey.ERROR_STATE_INCORRECT_ID_VALUE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfo?.filePath },
                                { "{stateId}", temp }
                            }
                        );

                    Random random = new Random(_id);
                    color = Utils.ArgbToInt(
                        255,
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256)
                    );
                    break;
                case "name":
                    startName = parser.ReadString();
                    name = startName;
                    break;
                case "manpower":
                    temp = parser.ReadString();
                    if (!int.TryParse(temp, out startManpower))
                        Logger.LogError(
                            EnumLocKey.ERROR_STATE_INCORRECT_MANPOWER,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{_id}" },
                                { "{manpower}", temp }
                            }
                        );

                    manpower = startManpower;
                    break;
                case "resources":
                    parser.Parse(new ResourceDictionary(this, resources));
                    break;
                case "provinces":
                    foreach (int provinceId in parser.ReadIntList())
                    {
                        if (provinceId < 0 || provinceId > ushort.MaxValue)
                            Logger.LogError(
                                EnumLocKey.ERROR_STATE_INCORRECT_PROVINCE_ID,
                                new Dictionary<string, string>
                                {
                                    { "{stateId}", $"{_id}" },
                                    { "{provinceId}", $"{provinceId}" }
                                }
                            );
                        else if (ProvinceManager.TryGetProvince((ushort)provinceId, out Province province))
                            provinces.Add(province);
                        else Logger.LogError(
                                EnumLocKey.ERROR_STATE_PROVINCE_NOT_FOUND,
                                new Dictionary<string, string>
                                {
                                    { "{stateId}", $"{_id}" },
                                    { "{provinceId}", $"{provinceId}" }
                                }
                            );
                    }
                    provinces.Sort((x, y) => x.Id.CompareTo(y.Id));
                    break;
                case "buildings_max_level_factor":
                    temp = parser.ReadString();

                    if (float.TryParse(temp.Replace('.', ','), out float value))
                    {
                        if (value >= 0) buildingsMaxLevelFactor = value;
                        else Logger.LogError(
                                EnumLocKey.ERROR_STATE_INCORRECT_BUILDINGS_MAX_LEVEL_FACTOR_VALUE,
                                new Dictionary<string, string>
                                {
                                    { "{stateId}", $"{_id}" },
                                    { "{value}", $"{temp}" }
                                }
                            );
                    }
                    else Logger.LogError(
                                EnumLocKey.ERROR_STATE_INCORRECT_BUILDINGS_MAX_LEVEL_FACTOR_VALUE,
                                new Dictionary<string, string>
                                {
                                    { "{stateId}", $"{_id}" },
                                    { "{value}", $"{temp}" }
                                }
                            );
                    break;
                case "state_category":
                    temp = parser.ReadString();
                    if (!StateCategoryManager.TryGetStateCategory(temp, out startStateCategory))
                        Logger.LogError(
                            EnumLocKey.ERROR_STATE_STATE_CATEGORY_NOT_FOUND,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{_id}" },
                                { "{stateCategory}", $"{temp}" }
                            }
                        );
                    stateCategory = startStateCategory;
                    break;
                case "history":
                    if (startHistory == null) startHistory = new StateHistory(this);
                    startHistory.isStartHistory = true;
                    parser.Parse(startHistory);
                    startHistory.ExecuteAfterParse();
                    break;
                case "local_supplies":
                    temp = parser.ReadString();

                    if (float.TryParse(temp.Replace('.', ','), out value))
                    {
                        if (value >= 0) localSupplies = value;
                        else Logger.LogError(
                                EnumLocKey.ERROR_STATE_INCORRECT_LOCAL_SUPPLIES_VALUE,
                                new Dictionary<string, string>
                                {
                                    { "{stateId}", $"{_id}" },
                                    { "{value}", $"{temp}" }
                                }
                            );
                    }
                    else Logger.LogError(
                                EnumLocKey.ERROR_STATE_INCORRECT_LOCAL_SUPPLIES_VALUE,
                                new Dictionary<string, string>
                                {
                                    { "{stateId}", $"{_id}" },
                                    { "{value}", $"{temp}" }
                                }
                            );
                    break;
            }
        }

        public void InitBorders()
        {
            borders.Clear();

            foreach (var p in provinces)
            {
                foreach (var b in p.borders)
                {
                    if (b.provinceA == null || b.provinceB == null || b.provinceA.State == null || b.provinceB.State == null || !b.provinceA.State.Equals(b.provinceB.State))
                    {
                        borders.Add(b);
                        StateManager.AddStatesBorder(b);
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is State state && _id == state._id;
        }

        public void Validate()
        {
            if (!Utils.IsProvincesListSorted(provinces))
            {
                provinces.Sort();
                fileInfo.needToSave = true;
            }

            if (Utils.RemoveDuplicateProvinces(provinces))
            {
                fileInfo.needToSave = true;
            }
        }
    }

    class ResourceDictionary : IParadoxRead
    {
        public State state;
        public Dictionary<Resource, uint> stateResources;

        public ResourceDictionary(State state, Dictionary<Resource, uint> stateResources)
        {
            this.state = state;
            this.stateResources = stateResources;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (ResourceManager.TryGetResource(token, out Resource resource))
            {
                if (stateResources.ContainsKey(resource))
                    Logger.LogError(
                        EnumLocKey.ERROR_STATE_DUPLICATE_RESOURCE_STATEMENT,
                        new Dictionary<string, string>
                        {
                            { "{stateId}", $"{state.Id}" },
                            { "{resourceName}", token }
                        }
                    );

                var temp = parser.ReadString();
                if (uint.TryParse(temp, out uint countUint)) stateResources[resource] = countUint;
                else if (float.TryParse(temp.Replace('.', ','), out float countFloat))
                {
                    if (countFloat < 0)
                        Logger.LogError(
                            EnumLocKey.ERROR_STATE_INCORRECT_RESOURCE_COUNT,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{state.Id}" },
                                { "{resourceName}", token },
                                { "{count}", temp }
                            }
                        );

                    stateResources[resource] = (uint)Math.Round(countFloat);
                }
                else Logger.LogError(
                            EnumLocKey.ERROR_STATE_INCORRECT_RESOURCE_COUNT,
                            new Dictionary<string, string>
                            {
                                { "{stateId}", $"{state.Id}" },
                                { "{resourceName}", token },
                                { "{count}", temp }
                            }
                        );

            }
            else Logger.LogError(
                    EnumLocKey.ERROR_STATE_INCORRECT_RESOURCE_NAME,
                    new Dictionary<string, string>
                    {
                        { "{stateId}", $"{state.Id}" },
                        { "{resourceName}", token }
                    }
                );
        }
    }
}
