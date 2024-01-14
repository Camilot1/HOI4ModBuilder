using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.tools.advanced;
using HOI4ModBuilder.src.utils;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
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
                GL.Vertex2(province.center.x, province.center.y);
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
                    return true;
            }
            return false;
        }

        public bool HasProvince(Province p) => _provinces.Contains(p);
        public Province GetProvince(int index) => _provinces[index];
        public Province FirstProvince => _provinces.Count > 0 ? _provinces[0] : null;
        public Province LastProvince => _provinces.Count > 1 ? _provinces[_provinces.Count - 1] : null;

        public bool CanAddProvince(Province p)
        {
            if (p == null || p.TypeId != 0 || _provinces.Count < 2) return false;
            else if ((_provinces[0].HasBorderWith(p) || _provinces[0].HasSeaConnectionWith(p)) && !p.railways.Contains(this)) return true;
            else if ((_provinces[_provinces.Count - 1].HasBorderWith(p) || _provinces[_provinces.Count - 1].HasSeaConnectionWith(p)) && !p.railways.Contains(this)) return true;
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

            if ((_provinces[0].HasBorderWith(p) || _provinces[0].HasSeaConnectionWith(p)) && !p.railways.Contains(this))
            {
                _provinces.Insert(0, p);
                p.railways.Add(this);
                return true;
            }
            else if ((_provinces[_provinces.Count - 1].HasBorderWith(p) || _provinces[_provinces.Count - 1].HasSeaConnectionWith(p)) && !p.railways.Contains(this))
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

        public bool CanSplitAtProvince(Province province, out int provinceIndex)
        {
            provinceIndex = 0;
            if (province == null) return false;

            for (int i = 0; i < _provinces.Count; i++)
                if (_provinces[i].Id == province.Id)
                {
                    if (i == 0 || i == _provinces.Count - 1) return false;

                    provinceIndex = i;
                    return true;
                }

            return false;
        }

        public bool TrySplitAtProvince(Province province, Railway[] railwayContainer)
        {
            if (railwayContainer == null || railwayContainer.Length != 1) return false;

            if (CanSplitAtProvince(province, out int index))
            {
                RemoveFromProvinces();
                var newRailway = new Railway(Level, _provinces.GetRange(index, ProvincesCount - index));
                _provinces.RemoveRange(index + 1, ProvincesCount - index - 1);

                AddToProvinces();

                RailwayTool.SilentAddRailway(newRailway);
                railwayContainer[0] = newRailway;
                return true;
            }
            else return false;
        }

        public bool CanJoin(Railway otherRailway, out EnumCanJoinRailwaysResult canJoinResult, out Province joinProvince, out Province otherJoinProvince)
        {
            joinProvince = null;
            otherJoinProvince = null;
            canJoinResult = EnumCanJoinRailwaysResult.CANT_JOIN;

            if (otherRailway == null || this == otherRailway)
            {
                canJoinResult = EnumCanJoinRailwaysResult.CANT_JOIN;
                return false;
            }

            if (ProvincesCount < 2 || otherRailway.ProvincesCount < 2)
            {
                canJoinResult = EnumCanJoinRailwaysResult.CANT_JOIN_RAILWAY_DOESNT_HAVE_PROVINCES;
                return false;
            }

            if (
                Check(FirstProvince, otherRailway.FirstProvince, ref canJoinResult, ref joinProvince, ref otherJoinProvince) ||
                Check(FirstProvince, otherRailway.LastProvince, ref canJoinResult, ref joinProvince, ref otherJoinProvince) ||
                Check(LastProvince, otherRailway.FirstProvince, ref canJoinResult, ref joinProvince, ref otherJoinProvince) ||
                Check(LastProvince, otherRailway.LastProvince, ref canJoinResult, ref joinProvince, ref otherJoinProvince)
            ) return true;
            else
            {
                canJoinResult = EnumCanJoinRailwaysResult.CANT_JOIN_RAILWAYS_ENDS_DO_NOT_HAS_BORDERS;
                return false;
            }

            bool Check(Province firstProvince, Province secondProvince, ref EnumCanJoinRailwaysResult innerCanJoinResult, ref Province innerJoinProvince, ref Province innerOtherJoinProvince)
            {
                if (firstProvince == secondProvince || (firstProvince.HasBorderWith(secondProvince) || firstProvince.HasSeaConnectionWith(secondProvince)) && !_provinces.Contains(secondProvince) && !otherRailway._provinces.Contains(firstProvince))
                {
                    innerJoinProvince = firstProvince;
                    innerOtherJoinProvince = secondProvince;
                    if (Level != otherRailway.Level) innerCanJoinResult = EnumCanJoinRailwaysResult.CAN_JOIN_BUT_RAILWAYS_HAS_DIFFERENT_LEVELS;
                    else innerCanJoinResult = EnumCanJoinRailwaysResult.CAN_JOIN;

                    return true;
                }
                else return false;
            }

        }
        public bool TryJoinWithRailway(Railway[] otherRailwayContainer)
        {
            if (ProvincesCount < 2 || otherRailwayContainer == null || otherRailwayContainer.Length != 1 || otherRailwayContainer[0] == null || otherRailwayContainer[0].ProvincesCount < 2) return false;

            var otherRailway = otherRailwayContainer[0];

            if (CanJoin(otherRailway, out EnumCanJoinRailwaysResult canJoinRailway, out Province joinProvince, out Province otherJoinProvince))
            {
                switch (canJoinRailway)
                {
                    case EnumCanJoinRailwaysResult.CAN_JOIN: break;
                    case EnumCanJoinRailwaysResult.CAN_JOIN_BUT_RAILWAYS_HAS_DIFFERENT_LEVELS:
                        var dialogResult = MessageBox.Show(
                                GuiLocManager.GetLoc(
                                    EnumLocKey.TOOL_CAN_JOIN_RAILWAYS_BUT_THEY_HAS_DIFFERENT_LEVELS_SHOULD_I_COUNTINUE,
                                    new Dictionary<string, string> {
                                        { "{firstRailwayLevel}", $"{Level}" },
                                        { "{secondRailwayLevel}", $"{otherRailway.Level}" },
                                        { "{newRailwayLevel}", $"{Level}" }
                                    }
                                ),
                                GuiLocManager.GetLoc(EnumLocKey.CHOOSE_ACTION),
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification
                            );

                        if (dialogResult != DialogResult.Yes) return false;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                RemoveFromProvinces();
                otherRailway.RemoveFromProvinces();

                if (FirstProvince == joinProvince && otherRailway.FirstProvince == otherJoinProvince)
                {
                    if (joinProvince == otherJoinProvince) otherRailway._provinces.RemoveAt(0);
                    otherRailway._provinces.Reverse();
                    _provinces.InsertRange(0, otherRailway._provinces);
                }
                else if (FirstProvince == joinProvince && otherRailway.LastProvince == otherJoinProvince)
                {
                    if (joinProvince == otherJoinProvince) _provinces.RemoveAt(0);
                    _provinces.InsertRange(0, otherRailway._provinces);
                }
                else if (LastProvince == joinProvince && otherRailway.FirstProvince == otherJoinProvince)
                {
                    if (joinProvince == otherJoinProvince) otherRailway._provinces.RemoveAt(0);
                    _provinces.AddRange(otherRailway._provinces);
                }
                else if (LastProvince == joinProvince && otherRailway.LastProvince == otherJoinProvince)
                {
                    if (joinProvince == otherJoinProvince) otherRailway._provinces.RemoveAt(otherRailway.ProvincesCount - 1);
                    otherRailway._provinces.Reverse();
                    _provinces.AddRange(otherRailway._provinces);
                }
                else return false;

                AddToProvinces();
                RailwayTool.SilentRemoveRailway(otherRailway);
                otherRailwayContainer[0] = null;
                return true;
            }
            else return false;
        }

        public enum EnumCanJoinRailwaysResult
        {
            CANT_JOIN,
            CANT_JOIN_RAILWAY_DOESNT_HAVE_PROVINCES,
            CANT_JOIN_RAILWAYS_ENDS_DO_NOT_HAS_BORDERS,
            CAN_JOIN_BUT_RAILWAYS_HAS_DIFFERENT_LEVELS,
            CAN_JOIN
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
