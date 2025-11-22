using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.src.hoiDataObjects.common.units;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.ai_areas
{
    public class AiArea : IParadoxObject
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private bool _needToSave;
        public bool NeedToSave => _needToSave;

        public bool HasAnyInnerInfo => HasContinents || HasRegions;

        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private static readonly string TOKEN_CONTINENTS = "continents";
        private List<int> _continentIds = new List<int>();

        public bool HasContinents => _continentIds.Count > 0;
        public bool HasContinentId(int continentId) => _continentIds.Contains(continentId);
        public bool AddContinentId(int id)
        {
            if (_continentIds.Contains(id)) return false;

            _continentIds.Add(id);
            _needToSave = true;
            return true;
        }
        public bool RemoveContinentId(int id)
        {
            bool result = _continentIds.Remove(id);
            if (result) _needToSave = true;
            return result;
        }

        private static readonly string TOKEN_STRATEGIC_REGIONS = "strategic_regions";
        private List<StrategicRegion> _regions = new List<StrategicRegion>();
        public bool HasRegions => _regions.Count > 0;
        public bool HasRegion(StrategicRegion region) => region != null && _regions.Contains(region);
        public bool AddRegion(StrategicRegion region)
        {
            if (region == null || _regions.Contains(region)) return false;

            _regions.Add(region);
            _needToSave = true;
            return true;
        }
        public bool RemoveRegion(StrategicRegion region)
        {
            bool result = _regions.Remove(region);
            if (result) _needToSave = true;
            return result;
        }

        public AiArea(string name)
        {
            _name = name;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            ParadoxUtils.StartBlock(sb, outTab, _name);

            string newTab = outTab + tab;
            string newTab2 = newTab + tab;

            if (_continentIds.Count > 0)
            {
                ParadoxUtils.StartBlock(sb, newTab, TOKEN_CONTINENTS);

                foreach (var continentId in _continentIds)
                {
                    var continent = ContinentManager.GetContinentById(continentId);
                    if (continent != "") sb.Append(newTab2).Append(continent).Append(Constants.NEW_LINE);
                }

                ParadoxUtils.EndBlock(sb, newTab);
            }

            if (_regions.Count > 0)
            {
                ParadoxUtils.StartBlock(sb, newTab, TOKEN_STRATEGIC_REGIONS);

                foreach (var region in _regions)
                    sb.Append(newTab2).Append(region.Id).Append(Constants.NEW_LINE);

                ParadoxUtils.EndBlock(sb, newTab);
            }

            ParadoxUtils.EndBlock(sb, outTab);
            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(token, () =>
            {
                if (token == TOKEN_CONTINENTS)
                {
                    var continents = parser.ReadStringList();
                    foreach (var continent in continents)
                    {
                        var continentId = ContinentManager.GetContinentId(continent);
                        if (continentId < 0)
                        {
                            Logger.LogLayeredWarning(
                                prevLayer, token, EnumLocKey.CONTINENT_NOT_FOUND_BY_NAME,
                                new Dictionary<string, string> { { "{continentName}", continent } }
                            );
                            continue;
                        }

                        if (!_continentIds.Contains((byte)continentId)) _continentIds.Add((byte)continentId);
                    }
                }
                else if (token == TOKEN_STRATEGIC_REGIONS)
                {
                    var idStringList = parser.ReadStringList();
                    foreach (var idString in idStringList)
                    {
                        if (!ushort.TryParse(idString, out var regionId))
                        {
                            Logger.LogLayeredWarning(
                                prevLayer, token, EnumLocKey.AI_AREA_HAS_INCORRECT_STRATEGIC_REGION_ID_VALUE,
                                new Dictionary<string, string> { { "{value}", idString } }
                            );
                            continue;
                        }

                        if (!StrategicRegionManager.TryGet(regionId, out var region))
                        {
                            Logger.LogLayeredWarning(
                                prevLayer, token, EnumLocKey.AI_AREA_CONTAINTS_ID_OF_NOT_EXISTING_STRATEGIC_REGION,
                                new Dictionary<string, string> { { "{regionId}", $"{regionId}" } }
                            );
                            continue;
                        }

                        if (!region.AiAreas.Contains(this)) _regions.Add(region);
                    }
                }
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;

        public override bool Equals(object obj)
        {
            return obj is AiArea area &&
                   _hashCode == area._hashCode;
        }
    }

    class AiAreasFile : IParadoxObject
    {
        private bool _needToSave;

        public bool HasAnyInnerInfo => _aiAreas.Count > 0;

        private FileInfo _fileInfo;
        public FileInfo FileInfo { get => _fileInfo; set => Utils.Setter(ref _fileInfo, ref value, ref _needToSave); }

        private static readonly string TOKEN_AI_AREAS = "areas";
        private List<AiArea> _aiAreas = new List<AiArea>();
        public List<AiArea> AiAreas { get => _aiAreas; }

        public AiAreasFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            ParadoxUtils.StartBlock(sb, outTab, TOKEN_AI_AREAS);

            string newTab = outTab + tab;
            foreach (var aiArea in _aiAreas)
            {
                if (aiArea.Save(sb, newTab, tab))
                    sb.Append(Constants.NEW_LINE);
            }

            ParadoxUtils.EndBlock(sb, outTab);
            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(prevLayer.Name, () =>
            {
                if (token == TOKEN_AI_AREAS)
                    parser.AdvancedParse(new LinkedLayer(prevLayer, token), TokenCallbackAreas);
                else
                    throw new UnknownTokenException(token);
            });
        }
        public void TokenCallbackAreas(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(prevLayer.Name, () =>
            {
                Logger.ParseLayeredListedValue(prevLayer, token, ref _aiAreas, parser, new AiArea(token));
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool hasProblems = false;

            HashSet<string> aiAreasNames = new HashSet<string>(_aiAreas.Count);

            foreach (var aiArea in _aiAreas)
            {
                hasProblems |= aiArea.Validate(prevLayer);

                if (aiAreasNames.Contains(aiArea.Name))
                {
                    Logger.LogLayeredWarning(
                        prevLayer,
                        EnumLocKey.AI_AREA_FILE_CONTAINS_AI_AREAS_WITH_DUPLICATE_NAMES,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", _fileInfo?.filePath },
                            { "{aiAreaName}", aiArea.Name }
                        }
                    );
                    hasProblems = true;
                }
                else aiAreasNames.Add(aiArea.Name);
            }

            return hasProblems;

        }
    }
}
