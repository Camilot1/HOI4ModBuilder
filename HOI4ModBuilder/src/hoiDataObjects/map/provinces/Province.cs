using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.supply;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.Text;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects.map
{
    public enum EnumProvinceType
    {
        LAND,
        SEA,
        LAKE
    }

    public class Province : IComparable<Province>
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool HasChangedId { get; private set; }

        private ushort _id;
        public ushort Id
        {
            get => _id;
            set
            {
                if (_id == value) return;

                if (ProvinceManager.ContainsProvinceIdKey(value))
                    throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_PROVINCE_ID_UPDATE_VALUE_IS_USED,
                            new Dictionary<string, string> { { "{id}", $"{value}" } }
                        ));
                else ProvinceManager.RemoveProvinceById(_id); //TODO Добавить обработчик внутри менеджена на обновление id провинции и словарей с ВП и постройками

                _id = value;
                HasChangedId = true;

                ProvinceManager.AddProvince(_id, this);
                State?.Validate(out bool _);
                Region?.Validate(out bool _);

                //TODO Переделать, вынеся проверки в сами Adjacency
                if (_adjacencies != null)
                {
                    if (_adjacencies.Count > 0) AdjacenciesManager.NeedToSaveAdjacencies = true;
                    foreach (var adj in _adjacencies)
                    {
                        if (adj.HasRuleRequiredProvince(this)) AdjacenciesManager.NeedToSaveAdjacencyRules = true;
                    }
                }
            }
        }

        private int _color;
        public int Color
        {
            get => _color;
            set
            {
                if (_color == value) return;

                if (ProvinceManager.TryGetProvince(value, out Province p))
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_COLOR_UPDATE_VALUE_IS_USED,
                        new Dictionary<string, string>
                        {
                        { "{color}", new Color3B(value).ToString() },
                        { "{otherProvinceId}", $"{p.Id}" }
                        }
                    ));
                else ProvinceManager.RemoveProvinceByColor(Color);

                _color = value;

                ProvinceManager.AddProvince(Color, this);
            }
        }

        private EnumProvinceType _type;
        public EnumProvinceType Type
        {
            get => _type;
            set
            {
                if (_type == value) return;

                _type = value;
                ProvinceManager.NeedToSave = true;
            }
        }
        public string GetTypeString() => _type.ToString().ToLower();

        private bool _isCoastal;
        public bool IsCoastal
        {
            get => _isCoastal;
            set
            {
                if (_isCoastal == value) return;

                _isCoastal = value;
                ProvinceManager.NeedToSave = true;
            }
        }

        private ProvincialTerrain _terrain;
        public ProvincialTerrain Terrain
        {
            get => _terrain;
            set
            {
                if (_terrain == value) return;

                _terrain = value;
                ProvinceManager.NeedToSave = true;
            }
        }

        private int _continentId;
        public int ContinentId
        {
            get => _continentId;
            set
            {
                if (_continentId == value) return;

                _continentId = value;
                ProvinceManager.NeedToSave = true;
            }
        }

        public uint victoryPoints;

        public State State { get; set; }
        public StrategicRegion Region { get; set; }

        public SupplyNode SupplyNode { get; set; }

        private List<Railway> _railways;

        public int GetRailwaysCount()
        {
            if (_railways == null) return 0;
            return _railways.Count;
        }

        public bool AddRailway(Railway railway)
        {
            if (_railways == null)
            {
                _railways = new List<Railway>() { railway };
                return true;
            }
            else if (!_railways.Contains(railway))
            {
                _railways.Add(railway);
                return true;
            }
            else return false;
        }

        public bool RemoveRailway(Railway railway)
        {
            if (_railways == null) return false;
            return _railways.Remove(railway);
        }

        public bool ContainsRailway(Railway railway)
        {
            if (_railways == null) return false;
            return _railways.Contains(railway);
        }

        public void ForEachRailway(Action<Railway> action)
        {
            if (_railways == null) return;
            foreach (var railway in _railways) action(railway);
        }

        public T ForEachRailway<T>(Func<Railway, T> func) where T : class
        {
            if (_railways == null) return null;

            foreach (var railway in _railways)
            {
                T product = func(railway);
                if (product != null) return product;
            }

            return null;
        }

        public bool HasDirectRailwayConnectionWith(Province p)
        {
            if (p == null || Id == p.Id) return false;
            if (GetRailwaysCount() == 0 || p.GetRailwaysCount() == 0) return false;

            foreach (var railway in _railways)
            {
                if (!p.ContainsRailway(railway)) continue;

                if (!railway.TryGetProvinceIndex(this, out int thisProvinceIndex)) return false;

                return
                    thisProvinceIndex > 0 && railway.GetProvince(thisProvinceIndex - 1).Id == p.Id ||
                    thisProvinceIndex < railway.ProvincesCount - 1 && railway.GetProvince(thisProvinceIndex + 1).Id == p.Id;
            }

            return false;
        }

        public bool HasRailwayConnectionWith(Province p)
        {
            if (p == null || Id == p.Id) return false;
            if (GetRailwaysCount() == 0 || p.GetRailwaysCount() == 0) return false;

            foreach (var railway in _railways)
                if (p.ContainsRailway(railway)) return true;

            return false;
        }

        private List<Adjacency> _adjacencies;
        public int GetAdjacenciesCount()
        {
            if (_adjacencies == null) return 0;
            return _adjacencies.Count;
        }

        public bool AddAdjacency(Adjacency adjacency)
        {
            if (_adjacencies == null)
            {
                _adjacencies = new List<Adjacency>() { adjacency };
                return true;
            }
            else if (!_adjacencies.Contains(adjacency))
            {
                _adjacencies.Add(adjacency);
                return true;
            }
            else return false;
        }

        public bool RemoveAdjacency(Adjacency adjacency)
        {
            if (_adjacencies == null) return false;
            return _adjacencies.Remove(adjacency);
        }

        public bool ContainsAdjacency(Adjacency adjacency)
        {
            if (_adjacencies == null) return false;
            return _adjacencies.Contains(adjacency);
        }

        public void ForEachAdjacency(Action<Adjacency> action)
        {
            if (_adjacencies == null) return;
            foreach (var adjacency in _adjacencies) action(adjacency);
        }

        public T ForEachAdjacency<T>(Func<Adjacency, T> func) where T : class
        {
            if (_adjacencies == null) return null;

            foreach (var adjacency in _adjacencies)
            {
                T product = func(adjacency);
                if (product != null) return product;
            }

            return null;
        }

        public List<ProvinceBorder> borders = new List<ProvinceBorder>(4);
        public void ForEachAdjacentProvince(Action<Province, Province> action)
        {
            foreach (var b in borders)
            {
                if (b.provinceA == this) action(this, b.provinceB);
                else if (b.provinceB == this) action(this, b.provinceA);
            }

            if (_adjacencies != null)
            {
                foreach (var adj in _adjacencies)
                {
                    if (adj.GetEnumType() == EnumAdjaciencyType.IMPASSABLE)
                        continue;

                    if (adj.StartProvince == this) action(this, adj.EndProvince);
                    if (adj.EndProvince == this) action(this, adj.StartProvince);
                }
            }
        }

        private Dictionary<Building, uint> _buildings;

        public int GetBuildingsCount()
        {
            if (_buildings == null) return 0;
            return _buildings.Count;
        }
        public void SetBuilding(Building building, uint count)
        {
            if (_buildings == null) _buildings = new Dictionary<Building, uint>() { { building, count } };
            else _buildings[building] = count;
        }

        public bool RemoveBuilding(Building building)
        {
            if (_buildings == null) return false;
            return _buildings.Remove(building);
        }

        public bool TryGetBuildingCount(Building building, out uint count)
        {
            count = 0;
            if (_buildings == null) return false;
            return _buildings.TryGetValue(building, out count);
        }

        public void SetBuildings(Dictionary<Building, uint> buildings) => _buildings = buildings;
        public bool HasPort()
        {
            if (_buildings == null || _buildings.Count == 0) return false;

            foreach (var building in _buildings.Keys)
                if (building.IsPort.GetValue()) return true;

            return false;
        }
        public bool WillHavePortInHistory()
        {
            if (State == null || State.startHistory == null) return false;

            if (State.startHistory.TryGetProvinceBuildings(this, out Dictionary<Building, uint> buildings))
                foreach (var building in buildings.Keys)
                    if (building.IsPort.GetValue()) return true;

            foreach (var stateHistory in State.stateHistories.Values)
                if (stateHistory.TryGetProvinceBuildings(this, out buildings))
                    foreach (var building in buildings.Keys)
                        if (building.IsPort.GetValue()) return true;

            return false;
        }
        public void ClearBuildings() => _buildings = null;

        public bool IsSuitableForShips() => _type == EnumProvinceType.SEA || CanBeNavalBaseForShips();
        public bool CanBeNavalBaseForShips() => _type == EnumProvinceType.LAND && (HasPort() || WillHavePortInHistory());

        public int pixelsCount;
        public bool dislayCenter;
        public Point2F center;

        public Province()
        {

        }

        public Province(ushort id, int color)
        {
            _id = id;
            _color = color;
        }

        public Province(ushort id, int color, EnumProvinceType type, bool isCoastal, ProvincialTerrain terrain, byte continentId)
        {
            _id = id;
            _color = color;
            _isCoastal = isCoastal;
            _type = type;
            _terrain = terrain;
            _continentId = continentId;
        }

        public void AddPixel(int x, int y)
        {
            pixelsCount++;
            center.x += (x - center.x) / pixelsCount;
            center.y += (y - center.y) / pixelsCount;
        }

        public void AddBorder(ProvinceBorder border) => borders.Add(border);
        public void ClearBorders() => borders.Clear();

        public void Save(StringBuilder sb)
        {
            sb.Append(_id).Append(';').
                Append((byte)(_color >> 16)).Append(';').
                Append((byte)(_color >> 8)).Append(';').
                Append((byte)_color).Append(';').
                Append(GetTypeString()).Append(';');
            if (_isCoastal) sb.Append("true;");
            else sb.Append("false;");
            if (_terrain == null) sb.Append("unknown;");
            else sb.Append(_terrain.name).Append(';');
            sb.Append(_continentId).Append(Constants.NEW_LINE);
        }

        public bool CheckCoastalType()
        {
            if (_type == EnumProvinceType.SEA) return HasBorderWithOtherThanThisTypeId(EnumProvinceType.SEA);
            else return HasBorderWithTypeId(EnumProvinceType.SEA);
        }

        public bool HasBorderWithOtherThanThisTypeId(EnumProvinceType otherType)
        {
            foreach (var b in borders)
            {
                if (_id != b.provinceA._id && b.provinceA.Type != otherType ||
                    _id != b.provinceB._id && b.provinceB.Type != otherType)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasBorderWithTypeId(EnumProvinceType otherType)
        {
            foreach (var b in borders)
            {
                if (_id != b.provinceA._id && b.provinceA.Type == otherType ||
                    _id != b.provinceB._id && b.provinceB.Type == otherType)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasBorderWith(Province province)
        {
            if (_id == province._id) return false;
            foreach (var border in borders)
            {
                if (border.provinceA._id == province._id || border.provinceB._id == province._id) return true;
            }
            return false;
        }

        public ProvinceBorder GetBorderWith(Province province)
        {
            if (_id == province._id) return null;
            foreach (var border in borders)
            {
                if (border.provinceA._id == province._id || border.provinceB._id == province._id) return border;
            }
            return null;
        }

        public List<Province> GetBorderProvinces()
        {
            var borderProvinces = new List<Province>();
            foreach (var b in borders)
            {
                if (b.provinceA._id == _id) borderProvinces.Add(b.provinceB);
                else if (b.provinceB._id == _id) borderProvinces.Add(b.provinceA);
            }
            return borderProvinces;
        }

        public bool HasSeaConnectionWith(Province province)
        {
            if (_id == province._id) return false;
            if (_adjacencies == null) return false;
            foreach (var adj in _adjacencies)
            {
                if (adj.HasConnectionWithProvince(province)) return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Province province && _id == province._id;
        }

        public int CompareTo(Province other)
        {
            if (other == null) return 1;
            return _id - other._id;
        }

    }


}
