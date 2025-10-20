using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.classes;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HOI4ModBuilder.hoiDataObjects.map
{
    public class State : AbstractParseObject, IScriptBlockInfo
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
            => obj is State state && Id.GetValue() == state.Id.GetValue();

        public int Color { get; private set; }

        public readonly GameParameter<ushort> Id = new GameParameter<ushort>()
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
            })
            .INIT_SetValueSetAdapter((o, value) =>
            {
                var _id = (ushort)value;
                if (StateManager.ContainsStateIdKey(_id))
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_STATE_ID_UPDATE_VALUE_IS_USED,
                        new Dictionary<string, string> { { "{id}", $"{value}" } }
                    ));
                else StateManager.RemoveState(_id);
                var parameter = (GameParameter<ushort>)o;
                parameter.SetNeedToSave(true);
                var state = (State)parameter.GetParent();
                StateManager.AddState(_id, state);

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
        public List<Country> CurrentCoresOf { get; set; } = new List<Country>(0);
        public List<Country> CurrentClaimsBy { get; set; } = new List<Country>(0);

        public readonly GameParameter<StateHistory> History = new GameParameter<StateHistory>()
            .INIT_SetValueParseAdapter((o, token) => new StateHistory((IParentable)o, default));

        public StateHistory CurrentHistory { get; set; }

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "id", (o) => ((State)o).Id },
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

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "history", "states" }, "State")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new State();

        public bool HasChangedId { get; private set; }

        public string GetBlockName() => "" + Id.GetValue();
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

        public Country owner;
        public Country controller;
        public Dictionary<Province, uint> victoryPoints = new Dictionary<Province, uint>(0);
        public void ForEachVictoryPoints(Func<DateTime, StateHistory, VictoryPoint, bool> action)
            => History.GetValue()?.ForEachVictoryPoints(action);

        public Dictionary<Building, uint> stateBuildings = new Dictionary<Building, uint>(0);
        public Dictionary<Province, Dictionary<Building, uint>> provincesBuildings = new Dictionary<Province, Dictionary<Building, uint>>(0);

        public uint GetStateBuildingCount(Building building)
        {
            stateBuildings.TryGetValue(building, out uint count);
            return count;
        }

        public void TransferDataFrom(State state)
        {
            center = state.center;
            dislayCenter = state.dislayCenter;
            pixelsCount = state.pixelsCount;
            borders = state.borders;
        }

        public Point2F center;
        public bool dislayCenter;
        public uint pixelsCount;
        public Bounds4S bounds;

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

        public void AddProvince(Province province)
        {
            if (province == null)
                return;

            Provinces.Add(province);
            Provinces.Sort((x, y) => x.Id.CompareTo(y.Id));
            province.State = this;
            CalculateCenter();
            MapManager.FontRenderController?.AddEventData(EnumMapRenderEvents.STATES_IDS, this);
            SetNeedToSave(true);
        }

        public bool RemoveProvince(Province province)
        {
            if (Provinces.Remove(province))
            {
                province.State = null;
                SetNeedToSave(true);
                CalculateCenter();
                MapManager.FontRenderController?.AddEventData(EnumMapRenderEvents.STATES_IDS, this);
                return true;
            }
            return false;
        }

        public bool RemoveProvinceData(Province province)
        {
            if (!RemoveProvince(province))
                return false;

            victoryPoints.Remove(province);
            provincesBuildings.Remove(province);

            History.GetValue()?.RemoveProvinceData(province);

            return true;
        }

        public uint GetResourceCount(string tag) => GetResourceCount(ResourceManager.GetResource(tag));
        public uint GetResourceCount(Resource resource)
        {
            if (resource == null)
                return 0;
            Resources.TryGetValue(resource, out var count);
            return count;
        }
        public bool SetResourceCount(string tag, uint count) => SetResourceCount(ResourceManager.GetResource(tag), count);
        public bool SetResourceCount(Resource resource, uint count)
        {
            if (resource == null)
                return false;

            if (Resources.TryGetValue(resource, out var prevCount))
            {
                if (prevCount == count)
                    return false;
                else if (count == 0)
                    Resources.Remove(resource);
                else
                    Resources[resource] = count;

                Resources.SetNeedToSave(true);
                return true;
            }
            else
            {
                if (count == 0)
                    return false;

                Resources[resource] = count;
                Resources.SetNeedToSave(true);
                return true;
            }
        }

        public void CalculateCenter()
        {
            bounds.SetZero();

            var commonCenter = new CommonCenter();

            foreach (var province in Provinces)
            {
                if (pixelsCount == 0)
                    bounds.Set(province.bounds);
                else
                    bounds.ExpandIfNeeded(province.bounds);

                commonCenter.Push((uint)province.pixelsCount, province.center);
            }

            commonCenter.Get(out pixelsCount, out center);
        }

        public bool SetVictoryPoints(Province province, uint newValue)
        {
            if (History.GetValue() == null)
                return false;

            if (History.GetValue().SetVictoryPoints(province, newValue))
            {
                SetNeedToSave(true);
                UpdateByDateTimeStamp(DataManager.currentDateStamp[0]);
                return true;
            }
            return false;
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

        public IListObject GetHistoryScriptBlocks(DateTime dateTime)
        {
            var list = new ListObject();

            if (History.GetValue() == null)
                return list;

            var history = History.GetValue();

            if (dateTime == default)
            {
                foreach (var block in history.DynamicScriptBlocks)
                    block.SaveToListObject(list);
                return list;
            }

            foreach (var innerHistoryEntry in history.InnerHistories)
            {
                if (innerHistoryEntry.Key == dateTime)
                {
                    foreach (var block in innerHistoryEntry.Value.DynamicScriptBlocks)
                        block.SaveToListObject(list);
                    break;
                }
            }

            return list;
        }

        public void SetHistoryScriptBlocks(DateTime dateTime, IListObject list)
        {
            if (History.GetValue() == null)
                return;

            var history = History.GetValue();


            if (dateTime == default)
            {
                ScriptBlockParseObject.LoadFromListObject(history, list, history.DynamicScriptBlocks);
                return;
            }

            foreach (var innerHistoryEntry in history.InnerHistories)
            {
                if (innerHistoryEntry.Key == dateTime)
                {
                    ScriptBlockParseObject.LoadFromListObject(history, list, innerHistoryEntry.Value.DynamicScriptBlocks);
                    return;
                }
            }
        }

        public void UpdateByDateTimeStamp(DateTime dateTime)
        {
            ClearData();
            History.GetValue()?.Activate(dateTime, this);
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

            foreach (var country in CurrentCoresOf)
                country.hasCoresAtStates.Remove(this);
            CurrentCoresOf = new List<Country>(0);

            foreach (var country in CurrentClaimsBy)
                country.hasClaimsAtState.Remove(this);
            CurrentClaimsBy = new List<Country>(0);

            CurrentHistory = null;

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

        public bool isCoastalState()
        {
            foreach (var p in Provinces)
            {
                if (p.Type == EnumProvinceType.SEA)
                    return true;
            }
            return false;
        }

        public void Validate(out bool hasChanged)
        {
            hasChanged = false;

            // Проверяем и сортируем провинции в стейте
            if (!Utils.IsProvincesListSorted(Provinces))
            {
                Provinces.Sort();
                SetNeedToSave(true);
                hasChanged = true;
            }

            // Удаляем дубликаты провинций в стейте
            if (Utils.RemoveDuplicateProvinces(Provinces))
            {
                SetNeedToSave(true);
                hasChanged = true;
            }

            // Удаляем провинции, не принадлежащие данному стейту
            for (int i = 0; i < Provinces.Count; i++)
            {
                var p = Provinces[i];
                if (p.State != this)
                {
                    Provinces.RemoveAt(i);
                    i--;
                    hasChanged = true;
                }
            }

            //
            var vpInfoList = new List<VPInfo>();
            ForEachVictoryPoints((dateTime, stateHistory, victoryPoint) =>
            {
                if (victoryPoint.province != null && victoryPoint.province.State != this)
                    vpInfoList.Add(new VPInfo(dateTime, stateHistory, victoryPoint));
                return false;
            });

            foreach (var vpInfo in vpInfoList)
            {
                if (vpInfo.dateTime == default)
                {
                    var newStateHistory = vpInfo.victoryPoint.province.State.History.GetValue();
                    if (newStateHistory != null)
                    {
                        History.GetValue().VictoryPoints.Remove(vpInfo.victoryPoint);
                        VictoryPoint tempVP = null;
                        foreach (var vp in newStateHistory.VictoryPoints)
                        {
                            if (vp.province == vpInfo.victoryPoint.province)
                            {
                                tempVP = vp;
                                break;
                            }
                        }

                        if (tempVP != null)
                            tempVP.value = vpInfo.victoryPoint.value;
                        else
                            newStateHistory.VictoryPoints.Add(vpInfo.victoryPoint);

                        hasChanged = true;
                    }
                }
                else if (
                    History.GetValue().InnerHistories.TryGetValue(vpInfo.dateTime, out StateHistory stateHistory) &&
                    vpInfo.victoryPoint.province.State.History.GetValue().InnerHistories.TryGetValue(vpInfo.dateTime, out StateHistory newStateHistory)
                )
                {
                    stateHistory.VictoryPoints.Remove(vpInfo.victoryPoint);
                    VictoryPoint tempVP = null;
                    foreach (var vp in newStateHistory.VictoryPoints)
                    {
                        if (vp.province == vpInfo.victoryPoint.province)
                        {
                            tempVP = vp;
                            break;
                        }
                    }

                    if (tempVP != null)
                        tempVP.value = vpInfo.victoryPoint.value;
                    else
                        newStateHistory.VictoryPoints.Add(vpInfo.victoryPoint);

                    hasChanged = true;
                }
            }
        }

        private struct VPInfo
        {
            public DateTime dateTime;
            public StateHistory stateHistory;
            public VictoryPoint victoryPoint;

            public VPInfo(DateTime dateTime, StateHistory stateHistory, VictoryPoint victoryPoint)
            {
                this.dateTime = dateTime;
                this.stateHistory = stateHistory;
                this.victoryPoint = victoryPoint;
            }
        }
    }
}
