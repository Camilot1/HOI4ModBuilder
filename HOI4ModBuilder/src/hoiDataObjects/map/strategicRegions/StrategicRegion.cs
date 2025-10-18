using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.classes;
using HOI4ModBuilder.src.utils.structs;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.map
{
    public class StrategicRegion : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
            => obj is StrategicRegion region && Id == region.Id;

        public bool needToSave;
        public FileInfo FileInfo { get; set; }

        private bool _silentLoad;

        public ushort Id { get; private set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value) needToSave = true;
                _name = value;
            }
        }

        private ProvincialTerrain _terrain;
        public ProvincialTerrain Terrain
        {
            get => _terrain;
            set
            {
                if (_terrain != value) needToSave = true;
                _terrain = value;
            }
        }

        private GameList<Province> Provinces = new GameList<Province>();
        public bool HasProvince(Province province) => Provinces.Contains(province);

        private List<ProvinceBorder> _borders = new List<ProvinceBorder>(0);
        public List<ProvinceBorder> Borders { get => _borders; }

        public void ForEachAdjacentProvince(Action<Province, Province> action)
        {
            foreach (var p in Provinces)
                p.ForEachAdjacentProvince((thisProvince, otherProvince) =>
                {
                    if (thisProvince.Region == this)
                        action(thisProvince, otherProvince);
                });
        }

        private RegionStaticModifiers _staticModifiers = new RegionStaticModifiers();
        public RegionStaticModifiers StaticModifiers
        {
            get => _staticModifiers;
            set
            {
                if (_staticModifiers != value) needToSave = true;
                _staticModifiers = value;
            }
        }

        private RegionWeather _weather;
        public RegionWeather Weather
        {
            get => _weather;
            set
            {
                if (_weather != value) needToSave = true;
                _weather = value;
                _weather.Region = this;
            }
        }
        public int GetWeatherPeriodsCount() => _weather != null ? _weather.GetPeriodsCount() : 0;

        public bool TryGetWeatherPeriod(int index, out WeatherPeriod period)
        {
            if (_weather == null)
            {
                period = null;
                return false;
            }
            else return _weather.TryGetPeriod(index, out period);
        }

        private List<AiArea> _aiAreas = new List<AiArea>();
        public List<AiArea> AiAreas
        {
            get => _aiAreas;
        }

        public int color;
        public Point2F center;
        public bool dislayCenter;
        public uint pixelsCount;
        public Bounds4S bounds;

        public StrategicRegion(FileInfo fileInfo)
        {
            this.FileInfo = fileInfo;
            _weather = new RegionWeather(this);
        }

        public StrategicRegion(FileInfo fileInfo, bool silentLoad) : this(fileInfo)
        {
            _silentLoad = silentLoad;
        }

        public void AddProvince(Province province)
        {
            Provinces.Add(province);
            Provinces.Sort((x, y) => x.Id.CompareTo(y.Id));
            province.Region = this;
            CalculateCenter();
            MapManager.FontRenderController?.AddEventData(EnumMapRenderEvents.REGIONS_IDS, this);
            needToSave = true;
        }

        public bool RemoveProvince(Province province)
        {
            if (Provinces.Remove(province))
            {
                province.Region = null;
                needToSave = true;
                CalculateCenter();
                MapManager.FontRenderController?.AddEventData(EnumMapRenderEvents.REGIONS_IDS, this);
                return true;
            }
            return false;
        }

        public bool RemoveProvinceData(Province province)
        {
            if (!RemoveProvince(province))
                return false;

            return true;
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

        public void SetSilent(bool value)
        {
            if (value)
            {
                foreach (var p in Provinces)
                    p.Region = null;
            }
            else
            {
                foreach (var p in Provinces)
                    p.Region = this;
                InitBorders();
                CalculateCenter();
                CalculateColor();
            }
        }

        public void UpdateTerrain(ProvincialTerrain terrain)
        {
            if (Terrain == terrain || !terrain.isNavalTerrain)
                return;
            Terrain = terrain;
            needToSave = true;
        }

        public void TransferProvincesFrom(StrategicRegion otherRegion)
        {

            foreach (var p in Provinces)
                p.Region = null;

            Provinces = otherRegion.Provinces;
            otherRegion.Provinces = new GameList<Province>();

            foreach (var p in Provinces)
                p.Region = this;
            InitBorders();
            CalculateCenter();

            needToSave = true;
        }

        public void Save(StringBuilder sb)
        {
            string tab = "\t";
            sb.Append("strategic_region = {").Append(Constants.NEW_LINE);
            sb.Append(tab).Append("id = ").Append(Id).Append(Constants.NEW_LINE);
            sb.Append(tab).Append("name = \"").Append(_name).Append("\"").Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);
            if (_terrain != null) sb.Append(tab).Append("naval_terrain = ").Append(_terrain.name).Append(Constants.NEW_LINE);

            sb.Append(tab).Append("provinces = {").Append(Constants.NEW_LINE);
            sb.Append(tab).Append(tab);
            foreach (var province in Provinces) sb.Append(province.Id).Append(' ');
            sb.Append(Constants.NEW_LINE).Append(tab).Append('}').Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);

            _staticModifiers.Save(sb, tab);
            _weather.Save(sb, tab);

            sb.Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {

            switch (token)
            {
                case "id":
                    Id = parser.ReadUInt16();
                    CalculateColor();
                    break;
                case "name": _name = parser.ReadString(); break;
                case "naval_terrain":
                    string terrainValue = parser.ReadString();

                    if (!TerrainManager.TryGetProvincialTerrain(terrainValue, out _terrain))
                        Logger.LogError(
                            EnumLocKey.ERROR_REGION_INCORRECT_NAVAL_TERRAIN_VALUE,
                            new Dictionary<string, string>
                            {
                                { "{regionId}", $"{Id}" },
                                { "{value}", $"{terrainValue}" }
                            }
                        );
                    break;
                case "provinces":
                    foreach (string provinceIdString in parser.ReadStringList())
                    {
                        if (!ushort.TryParse(provinceIdString, out ushort provinceId))
                            Logger.LogError(
                                EnumLocKey.ERROR_REGION_INCORRECT_PROVINCE_ID,
                                new Dictionary<string, string>
                                {
                                    { "{regionId}", $"{Id}" },
                                    { "{provinceId}", $"{provinceId}" }
                                }
                            );

                        if (!ProvinceManager.TryGetProvince(provinceId, out Province province))
                            Logger.LogError(
                                EnumLocKey.ERROR_REGION_PROVINCE_NOT_FOUND,
                                new Dictionary<string, string>
                                {
                                    { "{regionId}", $"{Id}" },
                                    { "{provinceId}", $"{provinceId}" }
                                }
                            );
                        Provinces.Add(province);
                        if (!_silentLoad) province.Region = this;
                    }
                    Provinces.Sort((x, y) => x.Id.CompareTo(y.Id));
                    break;
                case "static_modifiers":
                    parser.Parse(_staticModifiers);
                    break;
                case "weather":
                    parser.Parse(_weather);
                    break;
                default:
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_REGION_UNKNOWN_TOKEN,
                        new Dictionary<string, string>
                        {
                            { "{regionId}", $"{Id}" },
                            { "{token}", token }
                        }
                    ));
            }
        }

        public void InitBorders()
        {
            _borders.Clear();

            foreach (var p in Provinces)
            {
                foreach (var b in p.borders)
                {
                    if (b.provinceA == null || b.provinceB == null || b.provinceA.Region == null || b.provinceB.Region == null || b.provinceA.Region.Id != b.provinceB.Region.Id)
                    {
                        _borders.Add(b);
                        StrategicRegionManager.AddRegionsBorder(b);
                    }
                }
            }
        }

        public void CalculateColor()
        {
            Random random = new Random(Id);
            color = Utils.ArgbToInt(
                        255,
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256)
                    );
        }

        public void UpdateId(ushort id)
        {
            if (Id == id) return;
            if (StrategicRegionManager.ContainsRegionIdKey(id))
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_REGION_ID_UPDATE_VALUE_IS_USED,
                    new Dictionary<string, string> { { "{id}", $"{id}" } }
                ));
            else StrategicRegionManager.RemoveRegion(Id);

            Id = id;
            CalculateColor();

            StrategicRegionManager.AddRegion(Id, this);
        }

        public void ForEachProvince(Action<StrategicRegion, Province> action)
        {
            if (action == null) return;
            foreach (var p in Provinces)
                action(this, p);
        }
        public void ForEachProvince(Action<Province> action)
        {
            if (action == null) return;
            foreach (var p in Provinces)
                action(p);
        }

        public void Validate(out bool hasChanged)
        {
            hasChanged = false;
            if (!Utils.IsProvincesListSorted(Provinces))
            {
                Provinces.Sort();
                needToSave = true;
                hasChanged = true;
            }

            if (Utils.RemoveDuplicateProvinces(Provinces))
            {
                needToSave = true;
                hasChanged = true;
            }

            // Удаляем провинции, не принадлежащие данному региону
            for (int i = 0; i < Provinces.Count; i++)
            {
                var p = Provinces[i];
                if (p.Region != this)
                {
                    Provinces.RemoveAt(i);
                    i--;
                    hasChanged = true;
                }
            }
        }
    }

    public class RegionStaticModifiers : IParadoxRead
    {
        //TODO Найти список всех существующих статических модификаторов и отревакторить
        public Dictionary<string, string> modifiers = new Dictionary<string, string>();

        public void Save(StringBuilder sb, string tab)
        {
            if (modifiers.Count == 0) return;

            string tab2 = tab + tab;
            sb.Append(tab).Append("static_modifiers = {").Append(Constants.NEW_LINE);
            foreach (var pair in modifiers)
                sb.Append(tab2).Append(pair.Key).Append(" = ").Append(pair.Value).Append(Constants.NEW_LINE);
            sb.Append(tab).Append("}").Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            modifiers[token] = parser.ReadString();
        }
    }

    public class RegionWeather : IParadoxRead
    {

        public StrategicRegion Region { set; get; }

        private bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;
                foreach (var _period in _periods)
                {
                    if (_period.NeedToSave) return true;
                }

                return false;
            }
            set => _needToSave = value;
        }

        private List<WeatherPeriod> _periods;
        public int GetPeriodsCount() => _periods.Count;
        public void ForEachPeriod(Action<WeatherPeriod> action) => _periods.ForEach(action);
        public void AddPeriod(WeatherPeriod period)
        {
            period.Weather = this;
            _periods.Add(period);
            _needToSave = true;
        }
        public void RemovePeriod(int index)
        {
            _periods[index].Weather = null;
            _periods.RemoveAt(index);
            _needToSave = true;
        }
        public WeatherPeriod GetPeriod(int index) => _periods[index];
        public bool TryGetPeriod(int index, out WeatherPeriod period)
        {
            if (index < 0 || index >= _periods.Count)
            {
                period = null;
                return false;
            }
            else
            {
                period = _periods[index];
                return true;
            }
        }
        public void ClearPeriods()
        {
            _periods.Clear();
            _needToSave = true;
        }

        public RegionWeather(StrategicRegion region)
        {
            Region = region;
            _periods = new List<WeatherPeriod>();
        }

        public void Save(StringBuilder sb, string tab)
        {
            string tab2 = tab + tab;
            sb.Append(tab).Append("weather = {").Append(Constants.NEW_LINE);
            foreach (var period in _periods) period.Save(sb, tab2, tab);
            sb.Append(tab).Append('}').Append(Constants.NEW_LINE);
        }


        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "period")
            {
                var period = new WeatherPeriod(this);
                parser.Parse(period);
                _periods.Add(period);
            }
            else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_REGION_WEATHER_UNKNOWN_TOKEN,
                        new Dictionary<string, string>
                        {
                            { "{regionId}", $"{Region.Id}" },
                            { "{token}", token }
                        }
                    ));
        }
    }

    public class WeatherPeriod : IParadoxRead
    {
        private RegionWeather _weather;
        public RegionWeather Weather
        {
            get => _weather;
            set
            {
                if (_weather == value) return;

                _needToSave = true;
                _weather.NeedToSave = true;
                _weather = value;
            }
        }

        private bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                (_between != null && _between.NeedToSave);
        }

        private DatePeriod _between = new DatePeriod();
        public DatePeriod Between { get => _between; set => Utils.Setter(ref _between, ref value, ref _needToSave); }

        private float[] _temperature = new float[2];
        public float[] Temperature { get => _temperature; set => Utils.Setter(ref _temperature, ref value, ref _needToSave); }

        private float _noPhenomenon;
        public float NoPhenomenon { get => _noPhenomenon; set => Utils.Setter(ref _noPhenomenon, ref value, ref _needToSave); }

        private float _rainLight;
        public float RainLight { get => _rainLight; set => Utils.Setter(ref _rainLight, ref value, ref _needToSave); }

        private float _rainHeavy;
        public float RainHeavy { get => _rainHeavy; set => Utils.Setter(ref _rainHeavy, ref value, ref _needToSave); }

        private float _snow;
        public float Snow { get => _snow; set => Utils.Setter(ref _snow, ref value, ref _needToSave); }

        private float _blizzard;
        public float Blizzard { get => _blizzard; set => Utils.Setter(ref _blizzard, ref value, ref _needToSave); }

        private float _arcticWater;
        public float ArcticWater { get => _arcticWater; set => Utils.Setter(ref _arcticWater, ref value, ref _needToSave); }

        private float _mud;
        public float Mud { get => _mud; set => Utils.Setter(ref _mud, ref value, ref _needToSave); }

        private float _sandstorm;
        public float Sandstorm { get => _sandstorm; set => Utils.Setter(ref _sandstorm, ref value, ref _needToSave); }

        private float _minSnowLevel;
        public float MinSnowLevel { get => _minSnowLevel; set => Utils.Setter(ref _minSnowLevel, ref value, ref _needToSave); }

        public WeatherPeriod(RegionWeather weather)
        {
            _weather = weather;
        }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            sb.Append(outTab).Append("period = {").Append(Constants.NEW_LINE);

            sb.Append(outTab).Append(tab).Append("between = { ")
                .Append(_between.StartDay).Append('.').Append(_between.StartMonth).Append(' ')
                .Append(_between.EndDay).Append('.').Append(_between.EndMonth).Append(" }").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("temperature = { ")
                .Append(Utils.FloatToString(_temperature[0])).Append(' ')
                .Append(Utils.FloatToString(_temperature[1])).Append(" }").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("no_phenomenon = ").Append(Utils.FloatToString(_noPhenomenon)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("rain_light = ").Append(Utils.FloatToString(_rainLight)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("rain_heavy = ").Append(Utils.FloatToString(_rainHeavy)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("snow = ").Append(Utils.FloatToString(_snow)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("blizzard = ").Append(Utils.FloatToString(_blizzard)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("arctic_water = ").Append(Utils.FloatToString(_arcticWater)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("mud = ").Append(Utils.FloatToString(_mud)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("sandstorm = ").Append(Utils.FloatToString(_sandstorm)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("min_snow_level = ").Append(Utils.FloatToString(_minSnowLevel)).Append(Constants.NEW_LINE);

            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "between":
                    parser.Parse(_between);
                    break;
                case "temperature":
                    IList<double> list = parser.ReadDoubleList();
                    if (list.Count == 2)
                    {
                        _temperature[0] = (float)list[0];
                        _temperature[1] = (float)list[1];
                    }
                    break;
                case "no_phenomenon":
                    _noPhenomenon = parser.ReadFloat();
                    break;
                case "rain_light":
                    _rainLight = parser.ReadFloat();
                    break;
                case "rain_heavy":
                    _rainHeavy = parser.ReadFloat();
                    break;
                case "snow":
                    _snow = parser.ReadFloat();
                    break;
                case "blizzard":
                    _blizzard = parser.ReadFloat();
                    break;
                case "arctic_water":
                    _arcticWater = parser.ReadFloat();
                    break;
                case "mud":
                    _mud = parser.ReadFloat();
                    break;
                case "sandstorm":
                    _sandstorm = parser.ReadFloat();
                    break;
                case "min_snow_level":
                    _minSnowLevel = parser.ReadFloat();
                    break;
                default:
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_REGION_WEATHER_PERIOD_UNKNOWN_TOKEN,
                        new Dictionary<string, string>
                        {
                            { "{regionId}", $"{_weather.Region.Id}" },
                            { "{period}", _between.ToString() },
                            { "{token}", token }
                        }
                    ));
            }
        }
    }

    public class DatePeriod : IParadoxRead
    {
        private bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        private byte _startDay, _startMonth;
        private byte _endDay, _endMonth;
        public byte StartDay { get => _startDay; set => Utils.Setter(ref _startDay, ref value, ref _needToSave); }
        public byte StartMonth { get => _startMonth; set => Utils.Setter(ref _startMonth, ref value, ref _needToSave); }
        public byte EndDay { get => _endDay; set => Utils.Setter(ref _endDay, ref value, ref _needToSave); }
        public byte EndMonth { get => _endMonth; set => Utils.Setter(ref _endMonth, ref value, ref _needToSave); }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            try
            {
                string[] data = token.Split('.');
                _startDay = byte.Parse(data[0]);
                _startMonth = byte.Parse(data[1]);

                token = parser.ReadString();
                data = token.Split('.');
                _endDay = byte.Parse(data[0]);
                _endMonth = byte.Parse(data[1]);
            }
            catch (Exception ex)
            {
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_INCORRECT_DATE_PERIOD,
                    new Dictionary<string, string> { { "{token}", token } }
                ), ex);
            }
        }

        public override string ToString() => "" + _startDay + '.' + _startMonth + ' ' + _endDay + '.' + _endMonth;
    }

    class StrategicRegionFile : IParadoxRead
    {
        private readonly bool _isSilentLoad;
        private readonly FileInfo _currentFile;
        private readonly Dictionary<ushort, StrategicRegion> _regions;
        private StrategicRegion _region;

        public StrategicRegionFile(bool isSilentLoad, FileInfo currentFile, Dictionary<ushort, StrategicRegion> regions)
        {
            _isSilentLoad = isSilentLoad;
            _currentFile = currentFile;
            _regions = regions;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "strategic_region")
            {
                try
                {
                    if (_region != null)
                        throw new Exception(GuiLocManager.GetLoc(EnumLocKey.ERROR_MULTI_REGIONS_IN_FILE));

                    _region = new StrategicRegion(_currentFile, _isSilentLoad);

                    parser.Parse(_region);
                    _region.needToSave = _currentFile.needToSave;

                    if (_regions.ContainsKey(_region.Id))
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.ERROR_REGION_DUPLICATE_ID,
                            new Dictionary<string, string>
                            {
                                { "{reginId}", $"{_region.Id}" },
                                { "{firstFilePath}", _regions[_region.Id].FileInfo.filePath }
                            }
                        ));

                    _regions[_region.Id] = _region;
                }
                catch (Exception ex)
                {
                    string idString = _region.Id == 0 ? GuiLocManager.GetLoc(EnumLocKey.ERROR_REGION_UNSUCCESSFUL_REGION_ID_PARSE_RESULT) : $"{_region.Id}";
                    Logger.LogExceptionAsError(
                        EnumLocKey.ERROR_WHILE_REGION_LOADING,
                        new Dictionary<string, string>
                        {
                            { "{regionId}", idString },
                            { "{filePath}", _currentFile.filePath }
                        },
                        ex
                    );
                }
            }
        }
    }
}
