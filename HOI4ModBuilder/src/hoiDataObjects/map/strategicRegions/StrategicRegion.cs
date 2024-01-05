using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map
{
    class StrategicRegion : IParadoxRead
    {
        public bool needToSave;
        public ushort Id { get; private set; }
        public string name;
        public List<Province> provinces = new List<Province>();
        public List<ProvinceBorder> borders = new List<ProvinceBorder>(0);

        public RegionWeather weather = new RegionWeather();

        public int color;
        public Point2F center;
        public bool dislayCenter;
        public uint pixelsCount;

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

            sb.Append(tab).Append("provinces = {").Append(Constants.NEW_LINE);
            sb.Append(tab).Append(tab);
            foreach (var province in provinces) sb.Append(province.Id).Append(' ');
            sb.Append(Constants.NEW_LINE).Append(tab).Append('}').Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);

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
                case "provinces":
                    foreach (ushort provinceId in parser.ReadIntList())
                    {
                        if (ProvinceManager.TryGetProvince(provinceId, out Province province))
                        {
                            provinces.Add(province);
                            province.region = this;
                        }
                    }
                    provinces.Sort((x, y) => x.Id.CompareTo(y.Id));
                    break;
                case "weather":
                    weather = new RegionWeather();
                    parser.Parse(weather);
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is StrategicRegion region && Id == region.Id;
        }

        public override int GetHashCode() => Id;

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
                    new Dictionary<string, string> { { "{id}", $"{id}"} }
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

    class RegionWeather : IParadoxRead
    {
        public List<WeatherPeriod> periods = new List<WeatherPeriod>(0);

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
                var period = new WeatherPeriod();
                parser.Parse(period);
                periods.Add(period);
            }
        }
    }

    class WeatherPeriod : IParadoxRead
    {
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
            sb.Append(outTab).Append(tab).Append("arcticWater = ").Append(Utils.FloatToString(arcticWater)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("mud = ").Append(Utils.FloatToString(mud)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("sandstorm = ").Append(Utils.FloatToString(sandstorm)).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("minSnowLevel = ").Append(Utils.FloatToString(minSnowLevel)).Append(Constants.NEW_LINE);

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
                case "arcticWater":
                    arcticWater = parser.ReadFloat();
                    break;
                case "mud":
                    mud = parser.ReadFloat();
                    break;
                case "sandstorm":
                    sandstorm = parser.ReadFloat();
                    break;
                case "min_snow_level":
                    minSnowLevel = parser.ReadFloat();
                    break;
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
    }
}
