﻿using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map
{
    class StrategicRegion : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool needToSave;
        public ushort Id { get; private set; }
        public string name;
        public ProvincialTerrain terrain;

        public List<Province> provinces = new List<Province>();
        public List<ProvinceBorder> borders = new List<ProvinceBorder>(0);

        public RegionStaticModifiers staticModifiers = new RegionStaticModifiers();
        public RegionWeather weather;

        public int color;
        public Point2F center;
        public bool dislayCenter;
        public uint pixelsCount;

        public StrategicRegion()
        {
            weather = new RegionWeather(this);
        }

        public void AddProvince(Province province)
        {
            provinces.Add(province);
            provinces.Sort((x, y) => x.Id.CompareTo(y.Id));
            province.region = this;
            needToSave = true;
        }

        public bool RemoveProvince(Province province)
        {
            if (provinces.Remove(province))
            {
                province.region = null;
                needToSave = true;
                return true;
            }
            return false;
        }

        public void CalculateCenter()
        {
            double sumX = 0, sumY = 0;
            double pixelsCount = 0;
            foreach (var province in provinces)
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

        public void Save(StringBuilder sb)
        {
            string tab = "\t";
            sb.Append("strategic_region = {").Append(Constants.NEW_LINE);
            sb.Append(tab).Append("id = ").Append(Id).Append(Constants.NEW_LINE);
            sb.Append(tab).Append("name = \"").Append(name).Append("\"").Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);
            if (terrain != null) sb.Append(tab).Append("naval_terrain = ").Append(terrain.name).Append(Constants.NEW_LINE);

            sb.Append(tab).Append("provinces = {").Append(Constants.NEW_LINE);
            sb.Append(tab).Append(tab);
            foreach (var province in provinces) sb.Append(province.Id).Append(' ');
            sb.Append(Constants.NEW_LINE).Append(tab).Append('}').Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);

            staticModifiers.Save(sb, tab);
            weather.Save(sb, tab);

            sb.Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {

            switch (token)
            {
                case "id":
                    Id = parser.ReadUInt16();

                    var random = new Random(Id);
                    color = Utils.ArgbToInt(
                        255,
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256)
                    );
                    break;
                case "name": name = parser.ReadString(); break;
                case "naval_terrain":
                    string terrainValue = parser.ReadString();

                    if (!TerrainManager.TryGetProvincialTerrain(terrainValue, out terrain))
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
                        provinces.Add(province);
                        province.region = this;
                    }
                    provinces.Sort((x, y) => x.Id.CompareTo(y.Id));
                    break;
                case "static_modifiers":
                    parser.Parse(staticModifiers);
                    break;
                case "weather":
                    parser.Parse(weather);
                    break;
                default:
                    //TODO Добавить доп параметры
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

        public override bool Equals(object obj)
        {
            return obj is StrategicRegion region && Id == region.Id;
        }

        public void InitBorders()
        {
            borders.Clear();

            foreach (var p in provinces)
            {
                foreach (var b in p.borders)
                {
                    if (b.provinceA == null || b.provinceB == null || b.provinceA.region == null || b.provinceB.region == null || !b.provinceA.region.Equals(b.provinceB.region))
                    {
                        borders.Add(b);
                        StrategicRegionManager.AddRegionsBorder(b);
                    }
                }
            }
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
            StrategicRegionManager.AddRegion(Id, this);
        }
        public void Validate()
        {
            if (!Utils.IsProvincesListSorted(provinces))
            {
                provinces.Sort();
                needToSave = true;
            }

            if (Utils.RemoveDuplicateProvinces(provinces)) needToSave = true;
        }
    }

    class RegionStaticModifiers : IParadoxRead
    {
        //TODO Найти список всех существующих статических модификаторов и отревакторить
        public Dictionary<string, string> modifiers = new Dictionary<string, string>();

        public void Save(StringBuilder sb, string tab)
        {
            if (modifiers.Count == 0) return;

            string tab2 = tab + tab;
            sb.Append(tab).Append("static_modifiers = {").Append(Constants.NEW_LINE);
            foreach (var pair in modifiers) sb.Append(tab2).Append(pair.Key).Append(" = ").Append(pair.Value).Append(Constants.NEW_LINE);
            sb.Append(tab).Append("}").Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            modifiers[token] = parser.ReadString();
        }
    }

    class RegionWeather : IParadoxRead
    {
        public StrategicRegion region;
        public List<WeatherPeriod> periods = new List<WeatherPeriod>(0);

        public RegionWeather(StrategicRegion region)
        {
            this.region = region;
        }

        public void Save(StringBuilder sb, string tab)
        {
            string tab2 = tab + tab;
            sb.Append(tab).Append("weather = {").Append(Constants.NEW_LINE);
            foreach (var period in periods) period.Save(sb, tab2, tab);
            sb.Append(tab).Append('}').Append(Constants.NEW_LINE);
        }


        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "period")
            {
                var period = new WeatherPeriod(this);
                parser.Parse(period);
                periods.Add(period);
            }
            else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_REGION_WEATHER_UNKNOWN_TOKEN,
                        new Dictionary<string, string>
                        {
                            { "{regionId}", $"{region.Id}" },
                            { "{token}", token }
                        }
                    ));
        }
    }

    class WeatherPeriod : IParadoxRead
    {
        public RegionWeather weather;

        public DatePeriod between = new DatePeriod();
        public float[] temperature = new float[2];
        public float noPhenomenon;
        public float rainLight;
        public float rainHeavy;
        public float snow;
        public float blizzard;
        public float arcticWater;
        public float mud;
        public float sandstorm;
        public float minSnowLevel;

        public WeatherPeriod(RegionWeather weather)
        {
            this.weather = weather;
        }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            sb.Append(outTab).Append("period = {").Append(Constants.NEW_LINE);

            sb.Append(outTab).Append(tab).Append("between = { ")
                .Append(between.startDay).Append('.').Append(between.startMonth).Append(' ')
                .Append(between.endDay).Append('.').Append(between.endMonth).Append(" }").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("temperature = { ")
                .Append(Utils.FloatToString(temperature[0])).Append(' ')
                .Append(Utils.FloatToString(temperature[1])).Append(" }").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("no_phenomenon = ").Append(Utils.FloatToString(noPhenomenon)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("rain_light = ").Append(Utils.FloatToString(rainLight)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("rain_heavy = ").Append(Utils.FloatToString(rainHeavy)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("snow = ").Append(Utils.FloatToString(snow)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("blizzard = ").Append(Utils.FloatToString(blizzard)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("arctic_water = ").Append(Utils.FloatToString(arcticWater)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("mud = ").Append(Utils.FloatToString(mud)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("sandstorm = ").Append(Utils.FloatToString(sandstorm)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("min_snow_level = ").Append(Utils.FloatToString(minSnowLevel)).Append(Constants.NEW_LINE);

            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "between":
                    parser.Parse(between);
                    break;
                case "temperature":
                    IList<double> list = parser.ReadDoubleList();
                    if (list.Count == 2)
                    {
                        temperature[0] = (float)list[0];
                        temperature[1] = (float)list[1];
                    }
                    break;
                case "no_phenomenon":
                    noPhenomenon = parser.ReadFloat();
                    break;
                case "rain_light":
                    rainLight = parser.ReadFloat();
                    break;
                case "rain_heavy":
                    rainHeavy = parser.ReadFloat();
                    break;
                case "snow":
                    snow = parser.ReadFloat();
                    break;
                case "blizzard":
                    blizzard = parser.ReadFloat();
                    break;
                case "arcticWater": //TODO Remove after few updates
                case "arctic_water":
                    arcticWater = parser.ReadFloat();
                    break;
                case "mud":
                    mud = parser.ReadFloat();
                    break;
                case "sandstorm":
                    sandstorm = parser.ReadFloat();
                    break;
                case "minSnowLevel": //TODO Remove after few updates
                case "min_snow_level":
                    minSnowLevel = parser.ReadFloat();
                    break;
                default:
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_REGION_WEATHER_PERIOD_UNKNOWN_TOKEN,
                        new Dictionary<string, string>
                        {
                            { "{regionId}", $"{weather.region.Id}" },
                            { "{period}", between.ToString() },
                            { "{token}", token }
                        }
                    ));
            }
        }
    }

    class DatePeriod : IParadoxRead
    {
        public byte startDay, startMonth;
        public byte endDay, endMonth;

        public void TokenCallback(ParadoxParser parser, string token)
        {
            try
            {
                string[] data = token.Split('.');
                startDay = byte.Parse(data[0]);
                startMonth = byte.Parse(data[1]);

                token = parser.ReadString();
                data = token.Split('.');
                endDay = byte.Parse(data[0]);
                endMonth = byte.Parse(data[1]);
            }
            catch (Exception ex)
            {
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_INCORRECT_DATE_PERIOD,
                    new Dictionary<string, string> { { "{token}", token } }
                ), ex);
            }
        }

        public override string ToString()
        {
            return "" + startDay + '.' + startMonth + ' ' + endDay + '.' + endMonth;
        }
    }
}
