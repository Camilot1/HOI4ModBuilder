using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.buildings
{
    class MapPositionsManager
    {
        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "map" });
        private static readonly string UNITSTACKS_FILE_NAME = "unitstacks.txt";

        private static readonly Random _random = new Random();
        private static Dictionary<int, Info> _pixelsByProvincesIds = new Dictionary<int, Info>(0);
        private static Dictionary<int, Value2US> _portsPoses = new Dictionary<int, Value2US>(0);
        private static Dictionary<ushort, ushort> _rocketSites = new Dictionary<ushort, ushort>(0);
        private static Dictionary<ushort, ushort> _airports = new Dictionary<ushort, ushort>(0);

        //TODO Доделать
        public static void CalculateAndSave(Settings settings)
        {
            InitPixels();
            CalculateAndSaveBuildings(settings);
            CalculateAndSaveUnitStacks(settings);
            Clear();
        }

        private static void InitPixels()
        {
            _pixelsByProvincesIds = new Dictionary<int, Info>(ProvinceManager.ProvincesCount);

            foreach (var p in ProvinceManager.GetProvinces())
            {
                _pixelsByProvincesIds[p.Color.GetHashCode()] = new Info(0, 0, 0, 0, 0, new Value2US[p.pixelsCount]);
            }

            int[] pixelColors = MapManager.ProvincesPixels;
            int index;
            ushort x, y;
            int width = MapManager.MapSize.x;
            int height = MapManager.MapSize.y;

            int pixelCount = pixelColors.Count();
            int color, prevColor = -1;
            Info info = null;

            for (int i = 0; i < pixelCount; i++)
            {
                color = pixelColors[i];
                if (prevColor != color)
                {
                    info = _pixelsByProvincesIds[color];
                    prevColor = color;
                }

                if (info != null)
                {
                    x = (ushort)(i % width);
                    y = (ushort)(i / width);

                    if (info.count == 0)
                    {
                        info.min.x = x;
                        info.min.y = y;
                        info.max.x = x;
                        info.max.y = y;
                    }
                    else
                    {
                        if (x > info.max.x) info.max.x = x;
                        else if (x < info.min.x) info.min.x = x;

                        if (y > info.max.y) info.max.y = y;
                        else if (y < info.min.y) info.min.y = y;
                    }

                    info.values[info.count] = new Value2US((ushort)(i % height), (ushort)(i / width));
                    info.count++;
                }
            }
        }

        public static void Clear()
        {
            _pixelsByProvincesIds = new Dictionary<int, Info>(0);
            _portsPoses = new Dictionary<int, Value2US>(0);
            _rocketSites = new Dictionary<ushort, ushort>(0);
            _airports = new Dictionary<ushort, ushort>(0);
        }

        private static void CalculateAndSaveBuildings(Settings settings)
        {

        }

        private static void CalculateAndSaveUnitStacks(Settings settings)
        {
            var lines = new List<UnitStackLine>(0);

            foreach (var p in ProvinceManager.GetProvinces())
            {
                CalculateUnitStacks(lines, p);
            }

            lines.OrderBy(x => x.type).ThenBy(x => x.id);
            string[] exportLines = new string[lines.Count()];
            var sb = new StringBuilder();

            int count = lines.Count;
            UnitStackLine line;

            for (int i = 0; i < count; i++)
            {
                line = lines[i];
                sb.Append(line.id).Append(';')
                    .Append(line.type).Append(';')
                    .Append(line.x.ToString("#.##")).Append(';')
                    .Append(line.y.ToString("#.##")).Append(';')
                    .Append(line.z.ToString("#.##")).Append(';')
                    .Append(line.radians.ToString("#.##")).Append(';')
                    .Append(line.offset.ToString("#.##")).Append(Constants.NEW_LINE);
                exportLines[i] = sb.ToString();
                sb.Length = 0;
            }

            File.WriteAllLines(settings.modDirectory + FOLDER_PATH + UNITSTACKS_FILE_NAME, exportLines);
            lines.Clear();
        }

        private static void CalculateUnitStacks(List<UnitStackLine> list, Province p)
        {
            ushort id = p.Id;
            EnumProvinceType type = p.Type;
            float x = p.center.x;
            float y = p.center.y;
            float radians = 0;
            float offset = GetOffset();

            //standstill #0
            list.Add(new UnitStackLine(id, 0, x, GetY(x, y), y, radians, offset));
            //standstill RG #21
            x -= 3;
            list.Add(new UnitStackLine(id, 21, x, GetY(x, y), y, radians, offset));

            var borders = new List<ProvinceBorder>(p.borders);
            borders.OrderBy(o => o.center.y).ThenBy(o => o.center.x);

            if (type != EnumProvinceType.LAKE)
            {
                for (int i = 0; i < 8 && i < borders.Count; i++)
                {
                    var b = borders[i];
                    Calculate(p.center, b.center, EnumCalculateType.PERCENT, 0.5f, out x, out y, out radians);
                    //moving #1-8
                    list.Add(new UnitStackLine(id, (byte)(1 + i), x, GetY(x, y), y, radians, offset));
                    //moving RG #22-29
                    list.Add(new UnitStackLine(id, (byte)(22 + i), x, GetY(x, y), y, radians, offset));
                }
            }

            var info = _pixelsByProvincesIds[p.Color.GetHashCode()];
            float rValue = info.max.x - p.center.x;
            float lValue = p.center.x - info.min.x;
            float value = lValue < rValue ? lValue : rValue;

            //attacking #9
            x = p.center.x - 0.4f * value;
            y = p.center.y;
            list.Add(new UnitStackLine(id, 9, x, GetY(x, y), y, 1.57f, offset));

            //defending #10
            x = p.center.x + 0.4f * value;
            list.Add(new UnitStackLine(id, 10, x, GetY(x, y), y, -1.57f, offset));

            if (type == EnumProvinceType.SEA) //Если sea
            {
                //disembark
                //ship in port
                //ship in port moving
            }

            //victory point #38
            x = p.center.x + 0.4f * value;
            list.Add(new UnitStackLine(id, 10, x, GetY(x, y), y, -1.57f, offset));

        }

        private static float GetY(float x, float y)
        {
            return 0f;
        }

        private static void Calculate(Point2F start, Point2F finish, EnumCalculateType type, float value, out float x, out float y, out float radians)
        {
            float x1 = finish.x - start.x;
            float y1 = finish.y - start.y;
            radians = (float)(Math.Acos(y1 / (Math.Sqrt(x1 * x1 + y1 * y1))));

            if (type == EnumCalculateType.PERCENT)
            {
                x = start.x + value * x1;
                y = start.y + value * y1;
            }
            else if (type == EnumCalculateType.PX_FROM_START)
            {
                x = (float)(value * Math.Sin(radians));
                y = (float)(value * Math.Cos(radians));
            }
            else
            {
                x = (float)(finish.x - value * Math.Sin(radians));
                y = (float)(finish.y - value * Math.Cos(radians));
            }
        }

        private static float GetOffset()
        {
            return 0.02f * _random.Next(127);
        }

        enum EnumCalculateType
        {
            PERCENT,
            PX_FROM_START,
            PX_FROM_END
        }
    }

    public class Info
    {
        public int count;
        public Value2US min, max;
        public Value2US[] values;

        public Info(int count, ushort minX, ushort minY, ushort maxX, ushort maxY, Value2US[] values)
        {
            this.count = count;
            min.x = minX;
            min.y = minY;
            max.x = maxX;
            max.y = maxY;
            this.values = values;
        }
    }


    class UnitStackLine
    {
        public ushort id;
        public byte type;
        public float x, y, z;
        public float radians;
        public float offset;

        public UnitStackLine(ushort id, byte type, float x, float y, float z, float radians, float offset)
        {
            this.id = id;
            this.type = type;
            this.x = x;
            this.y = y;
            this.z = z;
            this.radians = radians;
            this.offset = offset;
        }
    }
}
