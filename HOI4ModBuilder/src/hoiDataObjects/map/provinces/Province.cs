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
                else ProvinceManager.RemoveProvinceById(Id); //TODO Добавить обработчик внутри менеджена на обновление id провинции и словарей с ВП и постройками

                _id = value;
                HasChangedId = true;

                ProvinceManager.AddProvince(Id, this);
                State?.Validate();
                Region?.Validate();

                //TODO Переделать, вынеся проверки в сами Adjacency
                if (adjacencies.Count > 0) AdjacenciesManager.NeedToSaveAdjacencies = true;
                foreach (var adj in adjacencies)
                {
                    if (adj.HasRuleRequiredProvince(this)) AdjacenciesManager.NeedToSaveAdjacencyRules = true;
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

        public SupplyNode supplyNode;
        public List<Railway> railways = new List<Railway>(0);
        public List<Adjacency> adjacencies = new List<Adjacency>(0);
        public List<ProvinceBorder> borders = new List<ProvinceBorder>(3);

        public Dictionary<Building, uint> buildings = new Dictionary<Building, uint>(0);

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
            sb.Append(Id).Append(';').
                Append((byte)(Color >> 16)).Append(';').
                Append((byte)(Color >> 8)).Append(';').
                Append((byte)Color).Append(';').
                Append(GetTypeString()).Append(';');
            if (IsCoastal) sb.Append("true;");
            else sb.Append("false;");
            if (Terrain == null) sb.Append("unknown;");
            else sb.Append(Terrain.name).Append(';');
            sb.Append(ContinentId).Append(Constants.NEW_LINE);
        }

        public string GetTypeString()
        {
            switch (TypeId)
            {
                case 0: return "land";
                case 1: return "sea";
                case 2: return "lake";
                default:
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_PROVINCE_INCORRECT_TYPE_ID,
                        new Dictionary<string, string>
                        {
                            { "{provinceId}", $"{Id}" },
                            { "{typeId}", $"{TypeId}" }
                        }
                    ));
            }
        }

        public bool CheckCoastalType()
        {
            //sea (id = 1)
            if (TypeId == 1) return HasBorderWithOtherThanThisTypeId(1);
            //land or lakes (id = 0 or id = 2)
            else return HasBorderWithTypeId(1);
        }

        public bool HasBorderWithOtherThanThisTypeId(byte otherTypeId)
        {
            foreach (var b in borders)
            {
                if (Id != b.provinceA.Id && b.provinceA.TypeId != otherTypeId ||
                    Id != b.provinceB.Id && b.provinceB.TypeId != otherTypeId)
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
                if (Id != b.provinceA.Id && b.provinceA.TypeId == otherTypeId ||
                    Id != b.provinceB.Id && b.provinceB.TypeId == otherTypeId)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasBorderWith(Province province)
        {
            if (Id == province.Id) return false;
            foreach (var border in borders)
            {
                if (border.provinceA.Id == province.Id || border.provinceB.Id == province.Id) return true;
            }
            return false;
        }

        public ProvinceBorder GetBorderWith(Province province)
        {
            if (Id == province.Id) return null;
            foreach (var border in borders)
            {
                if (border.provinceA.Id == province.Id || border.provinceB.Id == province.Id) return border;
            }
            return null;
        }

        public List<Province> GetBorderProvinces()
        {
            var borderProvinces = new List<Province>();
            foreach (var b in borders)
            {
                if (b.provinceA.Id == Id) borderProvinces.Add(b.provinceB);
                else if (b.provinceB.Id == Id) borderProvinces.Add(b.provinceA);
            }
            return borderProvinces;
        }

        public bool HasSeaConnectionWith(Province province)
        {
            if (Id == province.Id) return false;
            foreach (var adj in adjacencies)
            {
                if (adj.HasConnectionWithProvince(province)) return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Province province && Id == province.Id;
        }

        public int CompareTo(Province other)
        {
            if (other == null) return 1;
            return Id - other.Id;
        }

    }


}
