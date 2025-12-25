using HOI4ModBuilder.src.hoiDataObjects.map.provinces.border;
using HOI4ModBuilder.src.utils.structs;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.utils.borders
{
    public class BordersAssembler
    {
        private Dictionary<int, List<BorderData>> _bordersData;
        private Dictionary<int, Dictionary<int, int>> _bordersIndex;
        private int _borderDataCount;
        public Dictionary<int, List<BorderData>> BordersData => _bordersData;
        public int BorderDataCount => _borderDataCount;

        public void Reset()
        {
            _bordersData = new Dictionary<int, List<BorderData>>();
            _bordersIndex = new Dictionary<int, Dictionary<int, int>>();
            _borderDataCount = 0;
        }

        public BordersAssembler()
        {
            _bordersData = new Dictionary<int, List<BorderData>>();
            _bordersIndex = new Dictionary<int, Dictionary<int, int>>();
            _borderDataCount = 0;
        }

        public void AcceptBorderPixel(int x, int y, int lu, int ru, int rd, int ld)
        {
            //Если не граница провинций, то выходим
            if (lu == ru && ru == rd && rd == ld)
                return;

            short sx = (short)x;
            short sy = (short)y;

            if (lu != ld)
                PushData(sx, sy, lu, ld, 0b1000);
            if (lu != ru)
                PushData(sx, sy, lu, ru, 0b0100);
            if (ru != rd)
                PushData(sx, sy, ru, rd, 0b0010);
            if (rd != ld)
                PushData(sx, sy, rd, ld, 0b0001);
        }

        public void PushData(short x, short y, int colorA, int colorB, byte flags)
        {
            int tempMinColor, tempMaxColor;

            if (colorA < colorB)
            {
                tempMinColor = colorA;
                tempMaxColor = colorB;
            }
            else
            {
                tempMinColor = colorB;
                tempMaxColor = colorA;
            }

            if (!_bordersData.TryGetValue(tempMinColor, out var dataList))
            {
                dataList = new List<BorderData>(4);
                _bordersData[tempMinColor] = dataList;
            }
            if (!_bordersIndex.TryGetValue(tempMinColor, out var dataIndex))
            {
                dataIndex = new Dictionary<int, int>(4);
                _bordersIndex[tempMinColor] = dataIndex;
            }

            var pos = new ValueDirectionalPos()
            {
                pos = new Value2S { x = x, y = y },
                flags = flags
            };

            if (dataIndex.TryGetValue(tempMaxColor, out var index))
            {
                dataList[index] = dataList[index].Add(pos);
                return;
            }

            dataList.Add(new BorderData(tempMinColor, tempMaxColor).Add(pos));
            dataIndex[tempMaxColor] = dataList.Count - 1;
            _borderDataCount++;
        }

        public void MergeFrom(BordersAssembler other)
        {
            if (other == null || other._bordersData == null || other._bordersData.Count == 0)
                return;

            foreach (var entry in other._bordersData)
            {
                if (!_bordersData.TryGetValue(entry.Key, out var dataList))
                {
                    dataList = new List<BorderData>(entry.Value.Count);
                    _bordersData[entry.Key] = dataList;
                }
                if (!_bordersIndex.TryGetValue(entry.Key, out var dataIndex))
                {
                    dataIndex = new Dictionary<int, int>(entry.Value.Count);
                    _bordersIndex[entry.Key] = dataIndex;
                }

                foreach (var data in entry.Value)
                    MergeBorderData(dataList, dataIndex, data);
            }
        }

        private void MergeBorderData(List<BorderData> dataList, Dictionary<int, int> dataIndex, BorderData data)
        {
            if (dataIndex.TryGetValue(data.provinceMaxColor, out var index))
            {
                dataList[index] = dataList[index].Merge(data);
                return;
            }

            dataList.Add(data);
            dataIndex[data.provinceMaxColor] = dataList.Count - 1;
            _borderDataCount++;
        }
    }
}
