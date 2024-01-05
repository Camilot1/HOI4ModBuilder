using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.adjacencies
{
    class Adjacency
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public ushort id;
        public bool isNormal;
        private Province _startProvince, _endProvince;
        private EnumAdjaciencyType _enumType;
        private Province _throughProvince;
        private Value2I _start, _end;
        private AdjacencyRule _adjacencyRule;
        private string _comment;

        public ProvinceBorder provinceBorder;

        public Adjacency()
        {

        }

        public Adjacency(ushort id, bool isNormal, Province startProvince, Province endProvince, EnumAdjaciencyType enumType, Value2I start, Value2I end)
        {
            this.id = id;
            this.isNormal = isNormal;
            _startProvince = startProvince;
            _endProvince = endProvince;
            _enumType = enumType;
            _start = start;
            _end = end;
        }

        public void Save(StringBuilder sb)
        {
            if (_startProvince != null) sb.Append(_startProvince.Id).Append(';');
            else sb.Append("-1;");

            if (_endProvince != null) sb.Append(_endProvince.Id).Append(';');
            else sb.Append(-1).Append(';');

            if (_enumType == EnumAdjaciencyType.SEA) sb.Append("sea;");
            else if (_enumType == EnumAdjaciencyType.LAKE) sb.Append("lake;");
            else if (_enumType == EnumAdjaciencyType.IMPASSABLE) sb.Append("impassable;");
            else sb.Append(';');

            if (_throughProvince != null) sb.Append(_throughProvince.Id).Append(';');
            else sb.Append("-1;");

            sb.Append(_start.x).Append(';').Append(_start.y).Append(';').
                    Append(_end.x).Append(';').Append(_end.y).Append(';');

            if (_adjacencyRule != null) sb.Append(_adjacencyRule.name);
            sb.Append(';');

            sb.Append(_comment).Append(Constants.NEW_LINE);
        }

        public void Load(int i, string str, Dictionary<string, AdjacencyRule> rules)
        {
            string[] data = str.Split(';');
            if (data.Length < 9 || data.Length > 10)
            {
                Logger.LogError(
                    EnumLocKey.ERROR_ADJACENCY_LOADING_INCORRECT_PARAMS_COUNT,
                    new Dictionary<string, string> {
                        { "{adjacencyId}", $"{id}"},
                        { "{currentCount}", $"{data.Length}"},
                        { "{correctCount}", "9/10"}
                    }
                );
                return;
            }

            id = (ushort)i;

            //Стартовая провинция
            if (!int.TryParse(data[0], out int startProvinceId))
                Logger.LogError(
                    EnumLocKey.ERROR_ADJACENCY_LOADING_INCORRECT_FIRST_PROVINCE_ID,
                    new Dictionary<string, string> {
                        { "{adjacencyId}", $"{id}"},
                        { "{value}", $"{data[0]}"}
                    }
                );
            else if (startProvinceId >= 0)
            {
                if (!ProvinceManager.TryGetProvince((ushort)startProvinceId, out _startProvince))
                    Logger.LogError(
                        EnumLocKey.ERROR_ADJACENCY_LOADING_FIRST_PROVINCE_DOESNT_EXISTS,
                        new Dictionary<string, string> {
                            { "{adjacencyId}", $"{id}"},
                            { "{value}", $"{startProvinceId}"}
                        }
                    );
            }

            //Конечная провинция
            if (!int.TryParse(data[1], out int endProvinceId))
                Logger.LogError(
                    EnumLocKey.ERROR_ADJACENCY_LOADING_INCORRECT_SECOND_PROVINCE_ID,
                    new Dictionary<string, string> {
                        { "{adjacencyId}", $"{id}"},
                        { "{value}", $"{data[1]}"}
                    }
                );
            else if (endProvinceId >= 0)
            {
                if (!ProvinceManager.TryGetProvince((ushort)endProvinceId, out _endProvince))
                    Logger.LogError(
                        EnumLocKey.ERROR_ADJACENCY_LOADING_SECOND_PROVINCE_DOESNT_EXISTS,
                        new Dictionary<string, string> {
                            { "{adjacencyId}", $"{id}"},
                            { "{value}", $"{endProvinceId}"}
                        }
                    );
            }

            //Тип
            if (data[2] == "") _enumType = EnumAdjaciencyType.NONE;
            else if (data[2] == "sea") _enumType = EnumAdjaciencyType.SEA;
            else if (data[2] == "lake") _enumType = EnumAdjaciencyType.LAKE;
            else if (data[2] == "impassable") _enumType = EnumAdjaciencyType.IMPASSABLE;
            else Logger.LogError(
                    EnumLocKey.ERROR_ADJACENCY_LOADING_INCORRECT_TYPE,
                    new Dictionary<string, string> {
                        { "{adjacencyId}", $"{id}"},
                        { "{value}", $"{data[2]}"}
                    }
                );

            //Обработка разных типов
            if (!int.TryParse(data[3], out int throughProvinceId))
            {
                if (data[8].Trim().Length != 0)
                    Logger.LogError(
                        EnumLocKey.ERROR_ADJACENCY_LOADING_INCORRECT_THIRD_PROVINCE_ID,
                        new Dictionary<string, string> {
                            { "{adjacencyId}", $"{id}"},
                            { "{value}", $"{data[3]}"}
                        }
                    );
            }
            else if (throughProvinceId >= 0)
            {
                if (!ProvinceManager.TryGetProvince((ushort)throughProvinceId, out _throughProvince))
                    Logger.LogError(
                        EnumLocKey.ERROR_ADJACENCY_LOADING_THIRD_PROVINCE_DOESNT_EXISTS,
                        new Dictionary<string, string> {
                            { "{adjacencyId}", $"{id}"},
                            { "{value}", $"{throughProvinceId}"}
                        }
                    );
            }

            if ((startProvinceId < 0 || endProvinceId < 0) && (startProvinceId >= 0 || endProvinceId >= 0 || throughProvinceId >= 0))
                Logger.LogError(
                        EnumLocKey.ERROR_ADJACENCY_LOADING_ADJACENCY_CANT_HAVE_NEGATIVE_AND_POSITIVE_PROVINCE_ID,
                        new Dictionary<string, string> { { "{adjacencyId}", $"{id}" } }
                    );

            //Стартовая точка
            if (!int.TryParse(data[4], out _start.x)) _start.x = -1;
            if (!int.TryParse(data[5], out _start.y)) _start.y = -1;

            //Конечная точка
            if (!int.TryParse(data[6], out _end.x)) _end.x = -1;
            if (!int.TryParse(data[7], out _end.y)) _end.y = -1;

            //Правило смежности
            if (data[8].Length > 0)
            {
                var ruleName = data[8].Trim();
                if (ruleName.Length != 0 && ruleName != "-1" && !rules.TryGetValue(ruleName, out _adjacencyRule))
                {
                    Logger.LogError(
                        EnumLocKey.ERROR_ADJACENCY_LOADING_ADJACENCY_RULE_DOESNT_EXISTS,
                        new Dictionary<string, string> {
                            { "{adjacencyId}", $"{id}"},
                            { "{value}", $"{ruleName}"}
                        }
                    );
                }
            }
            //Комментарий смежности
            if (data.Length > 9) _comment = data[9];
        }

        public void GetProvinces(out Province start, out Province end, out Province through)
        {
            start = _startProvince;
            end = _endProvince;
            through = _throughProvince;
        }

        public void GetDisplayData(out EnumAdjaciencyType enumType, out int startId, out int endId, out int throughId, out Value2I startPos, out Value2I endPos, out string adjacencyRuleName, out string comment)
        {
            enumType = _enumType;
            startId = _startProvince == null ? -1 : _startProvince.Id;
            endId = _endProvince == null ? -1 : _endProvince.Id;
            throughId = _throughProvince == null ? -1 : _throughProvince.Id;
            startPos = _start;
            endPos = _end;

            adjacencyRuleName = _adjacencyRule == null ? "" : _adjacencyRule.name.ToString();
            comment = _comment;
        }

        public bool HasConnectionWithProvince(Province province)
        {
            if (_enumType == EnumAdjaciencyType.IMPASSABLE) return false;
            if (_startProvince != null && _startProvince == province) return true;
            if (_endProvince != null && _endProvince == province) return true;
            return false;
        }

        public bool CanBeDrawn()
        {
            if (_startProvince == null || _endProvince == null) return false;
            else return true;
        }

        public void AddToProvinces()
        {
            _startProvince?.adjacencies.Add(this);
            _endProvince?.adjacencies.Add(this);
            if (_enumType == EnumAdjaciencyType.IMPASSABLE && _startProvince != null && _endProvince != null)
            {
                provinceBorder = _startProvince.GetBorderWith(_endProvince);
            }
            else _throughProvince?.adjacencies.Add(this);
        }

        public void RemoveFromProvinces()
        {
            _startProvince?.adjacencies.Remove(this);
            _throughProvince?.adjacencies.Remove(this);
            _endProvince?.adjacencies.Remove(this);
            provinceBorder = null;
        }

        public void ReplaceProvince(Province oldProvince, Province newProvince)
        {
            if (oldProvince == null || newProvince == null) return;

            if (_startProvince != null && _startProvince.Id == oldProvince.Id)
            {
                oldProvince.adjacencies.Remove(this);
                newProvince.adjacencies.Add(this);
                _startProvince = newProvince;
                AdjacenciesManager.NeedToSaveAdjacencies = true;
            }

            if (_endProvince != null && _endProvince.Id == oldProvince.Id)
            {
                oldProvince.adjacencies.Remove(this);
                newProvince.adjacencies.Add(this);
                _endProvince = newProvince;
                AdjacenciesManager.NeedToSaveAdjacencies = true;
            }

            if (_throughProvince != null && _throughProvince.Id == oldProvince.Id)
            {
                oldProvince.adjacencies.Remove(this);
                newProvince.adjacencies.Add(this);
                _throughProvince = newProvince;
                AdjacenciesManager.NeedToSaveAdjacencies = true;
            }
        }

        public bool GetLine(out Point2F s, out Point2F e)
        {
            if (_startProvince == null || _endProvince == null)
            {
                s = new Point2F(0, 0);
                e = new Point2F(0, 0);
                return false;
            }

            if (_start.x < 0f || _start.y < 0f)
            {
                s.x = _startProvince.center.x;
                s.y = _startProvince.center.y;
            }
            else
            {
                s.x = _start.x;
                s.y = MapManager.MapSize.y - _start.y;
            }

            if (_end.x < 0f || _end.y < 0f)
            {
                e.x = _endProvince.center.x;
                e.y = _endProvince.center.y;
            }
            else
            {
                e.x = _end.x;
                e.y = MapManager.MapSize.y - _end.y;
            }
            return true;
        }

        public void SetStartPos(string str)
        {
            if (Utils.ParseIntPositionFromString(str, out int x, out int y))
            {
                SetStartPos(x, y);
            }
        }

        public void SetEndPos(string str)
        {
            if (Utils.ParseIntPositionFromString(str, out int x, out int y))
            {
                SetEndPos(x, y);
            }
        }

        public void SetStartPos(int x, int y)
        {
            if (x < 0 || x > MapManager.MapSize.x) _start.x = -1;
            else _start.x = x;
            if (y < 0 || y > MapManager.MapSize.y) _start.y = -1;
            else _start.y = y;
            AdjacenciesManager.NeedToSaveAdjacencies = true;
        }
        public void SetEndPos(int x, int y)
        {
            if (x < 0 || x > MapManager.MapSize.x) _end.x = -1;
            else _end.x = x;
            if (y < 0 || y > MapManager.MapSize.y) _end.y = -1;
            else _end.y = y;
            AdjacenciesManager.NeedToSaveAdjacencies = true;
        }

        public void SetStartProvince(Province province)
        {
            if (province == null || _endProvince == province || _throughProvince == province) return;

            _startProvince?.adjacencies.Remove(this);
            _startProvince = province;
            _startProvince.adjacencies.Add(this);
            AdjacenciesManager.NeedToSaveAdjacencies = true;
        }

        public void SetEndProvince(Province province)
        {
            if (province == null || _startProvince == province || _throughProvince == province) return;

            _endProvince?.adjacencies.Remove(this);
            _endProvince = province;
            _endProvince.adjacencies.Add(this);
            AdjacenciesManager.NeedToSaveAdjacencies = true;
        }

        public void SetThroughProvince(Province province)
        {
            if (province == null || _startProvince == province || _endProvince == province) return;

            _throughProvince?.adjacencies.Remove(this);
            _throughProvince = province;
            _throughProvince.adjacencies.Add(this);
            AdjacenciesManager.NeedToSaveAdjacencies = true;
        }

        public bool HasRuleRequiredProvince(Province province)
        {
            if (_adjacencyRule == null) return false;
            return _adjacencyRule.requiredProvinces.Contains(province);
        }

        public void AddRuleRequiredProvince(Province province)
        {
            if (_adjacencyRule != null && !HasRuleRequiredProvince(province))
            {
                _adjacencyRule.requiredProvinces.Add(province);
                AdjacenciesManager.NeedToSaveAdjacencyRules = true;
            }
        }

        public void RemoveRuleRequiredProvince(Province province)
        {
            if (_adjacencyRule != null && HasRuleRequiredProvince(province))
            {
                _adjacencyRule.requiredProvinces.Remove(province);
                AdjacenciesManager.NeedToSaveAdjacencyRules = true;
            }
        }

        public List<Province> GetRuleRequiredProvinces()
        {
            if (_adjacencyRule == null) return null;
            else return _adjacencyRule.requiredProvinces;
        }

        public EnumAdjaciencyType GetEnumType()
        {
            return _enumType;
        }

        public void SetComment(string text)
        {
            _comment = text;
            AdjacenciesManager.NeedToSaveAdjacencies = true;
        }

        public override bool Equals(object obj)
        {
            return obj is Adjacency adjacency &&
                   id == adjacency.id &&
                   isNormal == adjacency.isNormal &&
                   EqualityComparer<Province>.Default.Equals(_startProvince, adjacency._startProvince) &&
                   EqualityComparer<Province>.Default.Equals(_endProvince, adjacency._endProvince) &&
                   _enumType == adjacency._enumType &&
                   EqualityComparer<Province>.Default.Equals(_throughProvince, adjacency._throughProvince) &&
                   EqualityComparer<Value2I>.Default.Equals(_start, adjacency._start) &&
                   EqualityComparer<Value2I>.Default.Equals(_end, adjacency._end) &&
                   EqualityComparer<AdjacencyRule>.Default.Equals(_adjacencyRule, adjacency._adjacencyRule) &&
                   _comment == adjacency._comment &&
                   EqualityComparer<ProvinceBorder>.Default.Equals(provinceBorder, adjacency.provinceBorder);
        }

    }

    public enum EnumAdjaciencyType
    {
        NONE,
        SEA,
        LAKE,
        IMPASSABLE
    }
}
