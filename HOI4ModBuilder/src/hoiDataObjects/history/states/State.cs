using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace HOI4ModBuilder.hoiDataObjects.map
{
    public class State : AbstractParseObject, IScriptBlockInfo
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public int Color { get; private set; }

        public readonly GameParameter<ushort> IdNew = new GameParameter<ushort>()
            .INIT_SetValueParseAdapter((o, token) =>
            {
                var state = (State)((IParentable)o).GetParent();
                var value = (string)token;

                if (!ushort.TryParse(value, out var _id))
                    Logger.LogError(
                        EnumLocKey.ERROR_STATE_INCORRECT_ID_VALUE,
                        new Dictionary<string, string>
                        {
                                { "{filePath}", state.GetGameFile()?.FileInfo?.filePath },
                                { "{stateId}", value }
                        }
                    );

                Random random = new Random(_id);
                state.Color = Utils.ArgbToInt(
                    255,
                    (byte)random.Next(0, 256),
                    (byte)random.Next(0, 256),
                    (byte)random.Next(0, 256)
                );

                return _id;
            });

        public readonly GameParameter<GameString> Name = new GameParameter<GameString>();
        public string CurrentName { get; set; }

        public readonly GameParameter<StateCategory> StateCategory = new GameParameter<StateCategory>()
            .INIT_SetValueParseAdapter((o, token) => StateCategoryManager.GetStateCategory((string)token))
            .INIT_SetValueSaveAdapter((o) => o.name);
        public StateCategory CurrentStateCategory { get; set; }

        public readonly GameParameter<int> Manpower = new GameParameter<int>();
        public int CurrentManpower { get; set; }

        public readonly GameDictionary<Resource, uint> Resources = new GameDictionary<Resource, uint>()
            .INIT_SetKeyParseAdapter((token) => ResourceManager.GetResource(token))
            .INIT_SetKeySaveAdapter((resource) => resource.tag);

        public readonly GameList<Province> Provinces = new GameList<Province>()
            .INIT_SetValueParseAdapter((o, token) =>
            {
                if (ushort.TryParse(token, out var provinceId))
                    return ProvinceManager.GetProvince(provinceId);
                return null;
            })
            .INIT_SetValueSaveAdapter((province) => province.Id);

        public readonly GameParameter<float> BuildingsMaxLevelFactor = new GameParameter<float>();
        public readonly GameParameter<float> LocalSupplies = new GameParameter<float>();
        public readonly GameParameter<bool> IsImpassable = new GameParameter<bool>();
        public bool CurrentIsDemilitarized { get; set; }

        public readonly GameParameter<StateHistory> History = new GameParameter<StateHistory>()
            .INIT_SetValueParseAdapter((o, token) => new StateHistory((IParentable)o, default));

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "id", (o) => ((State)o).IdNew }, //TODO
            { "name", (o) => ((State)o).Name },
            { "state_category", (o) => ((State)o).StateCategory },
            { "manpower", (o) => ((State)o).Manpower },
            { "resources", (o) => ((State)o).Resources },
            { "provinces", (o) => ((State)o).Provinces },
            { "buildings_max_level_factor", (o) => ((State)o).BuildingsMaxLevelFactor },
            { "local_supplies", (o) => ((State)o).LocalSupplies },
            { "impassable", (o) => ((State)o).IsImpassable },
            { "history", (o) => ((State)o).History },
        };
        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;

        private static readonly SaveAdapter SAVE_ADAPTER = new SaveAdapter(new[] { "history", "states" }, "State")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SaveAdapter GetSaveAdapter() => SAVE_ADAPTER;

        public override IParseObject GetEmptyCopy() => new State();

        public bool HasChangedId { get; private set; }

        private ushort _id;
        /*
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
        */
        public string GetBlockName() => "" + _id;
        public EnumScope GetInnerScope() => EnumScope.STATE;
        public EnumKeyValueDemiliter[] GetAllowedSpecialDemiliters() => null;
        public bool IsAllowsInlineValue() => false;
        public bool IsAllowsBlockValue() => true;
        public EnumValueType[] GetAllowedValueTypes() => null;

        public bool TryGetRegionId(out ushort regionId)
        {
            regionId = 0;
            foreach (var p in Provinces)
            {
                if (p.Region != null)
                {
                    regionId = p.Region.Id;
                    return true;
                }
            }
            return false;
        }

        public List<ProvinceBorder> borders = new List<ProvinceBorder>(0);
        public void ForEachAdjacentProvince(Action<Province, Province> action)
        {
            foreach (var p in Provinces)
                p.ForEachAdjacentProvince((thisProvince, otherProvince) =>
                {
                    if (thisProvince.State == this)
                        action(thisProvince, otherProvince);
                });
        }

        public Country owner;
        public Country controller;
        public Dictionary<Province, uint> victoryPoints = new Dictionary<Province, uint>(0);
        public void ForEachVictoryPoints(Func<DateTime, StateHistory, Province, uint, bool> action)
        {
            History.GetValue()?.ForEachVictoryPoints(action);
        }

        public Dictionary<Building, uint> stateBuildings = new Dictionary<Building, uint>(0);
        public Dictionary<Province, Dictionary<Building, uint>> provincesBuildings = new Dictionary<Province, Dictionary<Building, uint>>(0);

        public Point2F center;
        public bool dislayCenter;
        public uint pixelsCount;

        public void AddProvince(Province province)
        {
            Provinces.Add(province);
            Provinces.Sort((x, y) => x.Id.CompareTo(y.Id));
            province.State = this;
            SetNeedToSave(true);
        }

        public bool RemoveProvince(Province province)
        {
            if (Provinces.Remove(province))
            {
                province.State = null;
                SetNeedToSave(true);
                return true;
            }
            return false;
        }

        public void CalculateCenter()
        {
            double sumX = 0, sumY = 0;
            double pixelsCount = 0;
            foreach (var province in Provinces)
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

        public void SetProvinceBuildingLevel(Province province, Building building, uint newCount)
        {
            if (History.GetValue() == null)
                return;

            if (History.GetValue().SetProvinceBuildingLevel(province, building, newCount))
            {
                SetNeedToSave(true);
                UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
            }
        }

        public void SetStateBuildingLevel(Building building, uint newCount)
        {
            if (History.GetValue() == null)
                return;

            if (History.GetValue().SetStateBuildingLevel(building, newCount))
            {
                SetNeedToSave(true);
                UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
            }
        }

        public uint GetStateBuildingLevel(Building building)
        {
            if (History.GetValue() == null)
                return 0;

            return History.GetValue().GetStateBuildingLevel(building);
        }

        public void UpdateByDateTimeStamp(DateTime dateTime)
        {
            ClearData();
            History?.GetValue().Activate(dateTime, this);
            AddData();
        }

        public void ClearData()
        {
            CurrentName = Name.GetValue().stringValue;
            CurrentManpower = Manpower.GetValue();
            CurrentStateCategory = StateCategory.GetValue();

            owner?.ownStates.Remove(this);
            owner = null;
            controller?.controlsStates.Remove(this);
            controller = null;

            foreach (var province in victoryPoints.Keys)
                province.victoryPoints = 0;
            victoryPoints = new Dictionary<Province, uint>(0);
            stateBuildings = new Dictionary<Building, uint>(0);

            foreach (var province in provincesBuildings.Keys)
                province.ClearBuildings();
            provincesBuildings = new Dictionary<Province, Dictionary<Building, uint>>(0);
        }

        public void AddData()
        {
            foreach (var province in victoryPoints.Keys)
                province.victoryPoints = victoryPoints[province];
            foreach (var province in provincesBuildings.Keys)
                province.SetBuildings(provincesBuildings[province]);
            owner?.ownStates.Add(this);
            controller?.controlsStates.Add(this);
        }


        public void InitBorders()
        {
            borders.Clear();

            foreach (var p in Provinces)
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

        public void Validate(out bool hasChanged)
        {
            hasChanged = false;

            if (!Utils.IsProvincesListSorted(Provinces))
            {
                Provinces.Sort();
                SetNeedToSave(true);
                hasChanged = true;
            }

            if (Utils.RemoveDuplicateProvinces(Provinces))
            {
                SetNeedToSave(true);
                hasChanged = true;
            }

            var vpInfoList = new List<VPInfo>();
            ForEachVictoryPoints((dateTime, stateHistory, province, value) =>
            {
                if (province.State != this && province.State != null)
                    vpInfoList.Add(new VPInfo(dateTime, stateHistory, province, value));
                return false;
            });

            /*
            foreach (var vpInfo in vpInfoList)
            {
                if (vpInfo.dateTime == default)
                {
                    var newStateHistory = vpInfo.province.State.History.GetValue();
                    if (newStateHistory != null)
                    {
                        History.GetValue()._victoryPoints.Remove(vpInfo.province);
                        if (!newStateHistory._victoryPoints.ContainsKey(vpInfo.province))
                            newStateHistory._victoryPoints[vpInfo.province] = vpInfo.value;
                        hasChanged = true;
                    }
                }
                else if (
                    History.GetValue().InnerHistories.TryGetValue(vpInfo.dateTime, out StateHistory stateHistory) &&
                    vpInfo.province.State.History.GetValue().InnerHistories.TryGetValue(vpInfo.dateTime, out StateHistory newStateHistory)
                )
                {
                    stateHistory._victoryPoints.Remove(vpInfo.province);
                    if (!newStateHistory._victoryPoints.ContainsKey(vpInfo.province))
                    {
                        newStateHistory._victoryPoints[vpInfo.province] = vpInfo.value;
                    }
                    hasChanged = true;
                }
            }
            */
        }

        private struct VPInfo
        {
            public DateTime dateTime;
            public StateHistory stateHistory;
            public Province province;
            public uint value;

            public VPInfo(DateTime dateTime, StateHistory stateHistory, Province province, uint value)
            {
                this.dateTime = dateTime;
                this.stateHistory = stateHistory;
                this.province = province;
                this.value = value;
            }
        }
    }
}
