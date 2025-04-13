using HOI4ModBuilder.src.hoiDataObjects.map.provinces.border;
using HOI4ModBuilder.src.utils.structs;
using System.Collections.Generic;
using System;

namespace HOI4ModBuilder.src.utils.borders
{
    public class BordersAssembler
    {
        private Dictionary<int, List<BorderData>> _bordersData;
        public Dictionary<int, List<BorderData>> BordersData => _bordersData;
        private Action<short, short, int, int, int, int, byte>[] _actionsTable;

        public void Reset()
        {
            _bordersData = new Dictionary<int, List<BorderData>>();
        }

        public BordersAssembler()
        {
            _bordersData = new Dictionary<int, List<BorderData>>();
            _actionsTable = new Action<short, short, int, int, int, int, byte>[]
            {
                (x, y, lu, ru, rd, ld, f) => { //0b0000
                    //PushData(x, y, ld, lu, f);
                    //PushData(x, y, lu, ru, f);
                    //PushData(x, y, ru, rd, f);
                    //PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b0001
                    //PushData(x, y, ld, lu, f);
                    //PushData(x, y, lu, ru, f);
                    //PushData(x, y, ru, rd, f);
                    PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b0010
                    //PushData(x, y, ld, lu, f);
                    //PushData(x, y, lu, ru, f);
                    PushData(x, y, ru, rd, f);
                    //PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b0011
                    //PushData(x, y, ld, lu, f);
                    //PushData(x, y, lu, ru, f);
                    PushData(x, y, ru, rd, f);
                    PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b0100
                    //PushData(x, y, ld, lu, f);
                    PushData(x, y, lu, ru, f);
                    //PushData(x, y, ru, rd, f);
                    //PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b0101
                    //PushData(x, y, ld, lu, f);
                    PushData(x, y, lu, ru, f);
                    //PushData(x, y, ru, rd, f);
                    PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b0110
                    //PushData(x, y, ld, lu, f);
                    PushData(x, y, lu, ru, f);
                    PushData(x, y, ru, rd, f);
                    //PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b0111
                    //PushData(x, y, ld, lu, f);
                    PushData(x, y, lu, ru, f);
                    PushData(x, y, ru, rd, f);
                    PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b1000
                    PushData(x, y, ld, lu, f);
                    //PushData(x, y, lu, ru, f);
                    //PushData(x, y, ru, rd, f);
                    //PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b1001
                    PushData(x, y, ld, lu, f);
                    //PushData(x, y, lu, ru, f);
                    //PushData(x, y, ru, rd, f);
                    PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b1010
                    PushData(x, y, ld, lu, f);
                    //PushData(x, y, lu, ru, f);
                    PushData(x, y, ru, rd, f);
                    //PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b1011
                    PushData(x, y, ld, lu, f);
                    //PushData(x, y, lu, ru, f);
                    PushData(x, y, ru, rd, f);
                    PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b1100
                    PushData(x, y, ld, lu, f);
                    PushData(x, y, lu, ru, f);
                    //PushData(x, y, ru, rd, f);
                    //PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b1101
                    PushData(x, y, ld, lu, f);
                    PushData(x, y, lu, ru, f);
                    //PushData(x, y, ru, rd, f);
                    PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b1110
                    PushData(x, y, ld, lu, f);
                    PushData(x, y, lu, ru, f);
                    PushData(x, y, ru, rd, f);
                    //PushData(x, y, rd, ld, f);
                },
                (x, y, lu, ru, rd, ld, f) => { //0b1111
                    PushData(x, y, ld, lu, f);
                    PushData(x, y, lu, ru, f);
                    PushData(x, y, ru, rd, f);
                    PushData(x, y, rd, ld, f);
                },
            };
        }

        public void AcceptBorderPixel(int x, int y, int lu, int ru, int rd, int ld)
        {
            //Если не граница провинций, то выходим
            if (lu == ru && ru == rd && rd == ld)
                return;

            List<BorderData> dataList = null;

            byte flags = 0;
            int notEqualsCounter = 0;

            if (lu != ld)
            {
                flags |= 0b1000;
                notEqualsCounter++;
            }
            if (lu != ru)
            {
                flags |= 0b0100;
                notEqualsCounter++;
            }
            if (ru != rd)
            {
                flags |= 0b0010;
                notEqualsCounter++;
            }
            if (rd != ld)
            {
                flags |= 0b0001;
                notEqualsCounter++;
            }

            _actionsTable[flags]((short)x, (short)y, lu, ru, rd, ld, flags);
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

            var pos = new ValueDirectionalPos()
            {
                pos = new Value2S { x = x, y = y },
                flags = flags
            };

            bool hasFound = false;
            foreach (var data in dataList)
            {
                if (data.provinceMinColor == tempMinColor && data.provinceMaxColor == tempMaxColor)
                {
                    data.points.Add(pos);
                    hasFound = true;
                    break;
                }
            }

            if (!hasFound)
                dataList.Add(new BorderData(tempMinColor, tempMaxColor).Add(pos));
        }
    }
}
