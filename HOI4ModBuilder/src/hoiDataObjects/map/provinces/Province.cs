using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.supply;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects.map
{
    class Province : IComparable<Province>
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
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
                State?.Validate();
                Region?.Validate();

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

        private byte _typeId;
        public byte TypeId
        {
            get => _typeId;
            set
            {
                if (_typeId == value) return;

                _typeId = value;
                ProvinceManager.NeedToSave = true;
            }
        }
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

        private byte _continentId;
        public byte ContinentId
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
        public void ClearBuildings() => _buildings = null;


        public uint pixelsCount;
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

        public Province(ushort id, int color, byte typeId, bool isCoastal, ProvincialTerrain terrain, byte continentId)
        {
            _id = id;
            _color = color;
            _isCoastal = isCoastal;
            _typeId = typeId;
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

        public string GetTypeString()
        {
            switch (_typeId)
            {
                case 0: return "land";
                case 1: return "sea";
                case 2: return "lake";
                default:
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_INCORRECT_TYPE_ID,
                        new Dictionary<string, string>
                        {
                            { "{provinceId}", $"{_id}" },
                            { "{typeId}", $"{_typeId}" }
                        }
                    ));
            }
        }

        public bool CheckCoastalType()
        {
            //sea (id = 1)
            if (_typeId == 1) return HasBorderWithOtherThanThisTypeId(1);
            //land or lakes (id = 0 or id = 2)
            else return HasBorderWithTypeId(1);
        }

        public bool HasBorderWithOtherThanThisTypeId(byte otherTypeId)
        {
            foreach (var b in borders)
            {
                if (_id != b.provinceA._id && b.provinceA.TypeId != otherTypeId ||
                    _id != b.provinceB._id && b.provinceB.TypeId != otherTypeId)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasBorderWithTypeId(byte otherTypeId)
        {
            foreach (var b in borders)
            {
                if (_id != b.provinceA._id && b.provinceA.TypeId == otherTypeId ||
                    _id != b.provinceB._id && b.provinceB.TypeId == otherTypeId)
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
