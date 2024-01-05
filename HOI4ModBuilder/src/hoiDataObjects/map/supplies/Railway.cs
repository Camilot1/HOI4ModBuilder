using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.utils;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.railways
{
    class Railway
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public byte Level { get; private set; }
        private List<Province> _provinces;

        public Railway(byte level, Province p0, Province p1)
        {
            Level = level;
            _provinces = new List<Province>
            {
                p0,
                p1
            };
        }

        public Railway(byte level, List<Province> provinces)
        {
            Level = level;
            _provinces = provinces;
        }

        public void Draw()
        {
            foreach (Province province in _provinces)
            {
                GL.Vertex2(province.center.x, province.center.y);
            }
        }

        public void UpdateLevel(byte level)
        {
            if (Level == level) return;
            Level = level;
            SupplyManager.NeedToSaveRailways = true;
        }

        public int ProvincesCount => _provinces.Count;

        public bool AddToProvinces()
        {
            if (_provinces == null || _provinces.Count == 0) return false;
            foreach (var p in _provinces) p.railways.Add(this);
            return true;
        }

        public bool RemoveFromProvinces()
        {
            if (_provinces == null || _provinces.Count == 0) return false;
            foreach (var p in _provinces) p.railways.Remove(this);
            return true;
        }

        public bool IsOnRailway(Point2D point)
        {
            for (int i = 0; i < _provinces.Count - 1; i++)
            {
                if (point.IsOnLine(_provinces[i].center, _provinces[i + 1].center, 1.01f))
                {
                    SupplyManager.SelectedRailway = this;
                    return true;
                }
            }
            return false;
        }

        public bool HasProvince(Province p) => _provinces.Contains(p);
        public Province GetProvince(int index) => _provinces[index];
        public Province GetFirstProvince() => _provinces.Count > 0 ? _provinces[0] : null;
        public Province GetLastProvince() => _provinces.Count > 1 ? _provinces[_provinces.Count - 1] : null;

        public bool CanAddProvince(Province p)
        {
            if (p == null || p.TypeId != 0 || _provinces.Count < 2) return false;
            else if (_provinces[0].HasBorderWith(p) || _provinces[0].HasSeaConnectionWith(p)) return true;
            else if (_provinces[_provinces.Count - 1].HasBorderWith(p) || _provinces[_provinces.Count - 1].HasSeaConnectionWith(p)) return true;
            else return false;
        }

        public bool CanRemoveProvince(Province p)
        {
            if (p == null || _provinces.Count < 3) return false;
            else if (_provinces[0].Id == p.Id) return true;
            else if (_provinces[_provinces.Count - 1].Id == p.Id) return true;
            else return false;
        }

        public bool TryAddProvince(Province p)
        {
            if (_provinces.Count < 2 || p.TypeId != 0) return false;

            if (_provinces[0].HasBorderWith(p) || _provinces[0].HasSeaConnectionWith(p))
            {
                _provinces.Insert(0, p);
                p.railways.Add(this);
                return true;
            }
            else if (_provinces[_provinces.Count - 1].HasBorderWith(p) || _provinces[_provinces.Count - 1].HasSeaConnectionWith(p))
            {
                _provinces.Add(p);
                p.railways.Add(this);
                return true;
            }
            return false;
        }

        public bool TryRemoveProvince(Province p)
        {
            if (_provinces.Count < 3) return false;
            if (_provinces[0].Id == p.Id)
            {
                _provinces.RemoveAt(0);
                p.railways.Remove(this);
                return true;
            }
            else if (_provinces[_provinces.Count - 1].Id == p.Id)
            {
                _provinces.RemoveAt(_provinces.Count - 1);
                p.railways.Remove(this);
                return true;
            }
            return false;
        }

        public void Save(StringBuilder sb)
        {
            sb.Append(Level).Append(' ').Append(_provinces.Count);
            foreach (var province in _provinces) sb.Append(' ').Append(province.Id);
            sb.Append(' ').Append(Constants.NEW_LINE);
        }

        public override bool Equals(object obj)
        {
            return obj is Railway railway &&
                   Level == railway.Level &&
                   EqualityComparer<List<Province>>.Default.Equals(_provinces, railway._provinces);
        }
    }
}
