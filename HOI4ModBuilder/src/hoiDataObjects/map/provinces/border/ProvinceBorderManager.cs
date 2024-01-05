using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.hoiDataObjects.map
{
    public class ProvinceBorderManager
    {
        private static Value2US size;
        private static int[,] pixels;
        private static Dictionary<Value2US, BorderInfo>[] borderInfoMap;
        private static HashSet<Value2US> previouslyDeleted;
        private static int provinceBorderCount = 0;

        public static void Init(int[] values, ushort width, ushort height)
        {
            Stopwatch stopwatch = new Stopwatch();
            provinceBorderCount = 0;

            Tuple<EnumLocKey, Action>[] actions =
            {
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => {
                    stopwatch.Start();
                    InitPixels(values, width, height);
                    borderInfoMap = new Dictionary<Value2US, BorderInfo>[] {
                        new Dictionary<Value2US, BorderInfo>(), //Все границы
                        new Dictionary<Value2US, BorderInfo>(), //Границы с 1 направлением
                        new Dictionary<Value2US, BorderInfo>(), //Границы с 2 направлениями
                        new Dictionary<Value2US, BorderInfo>(), //Границы с 3 направлениями
                        new Dictionary<Value2US, BorderInfo>() //Границы с 4 направлениями
                    };
                    previouslyDeleted = new HashSet<Value2US>();
                }),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => InitBorderInfoMap()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => {
                    InitBorders();

                    stopwatch.Stop();
                    pixels = null;
                    borderInfoMap = null;
                    previouslyDeleted = null;
                    Logger.Log($"BordersInitAndAssemble = {stopwatch.ElapsedMilliseconds} ms");
                    Logger.Log($"ProvinceBorderCount = {provinceBorderCount}");
                }),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_STATES_BORDERS_ASSEMBLE, () => StateManager.InitStatesBorders()),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_REGIONS_BORDERS_ASSEMBLE, () => StrategicRegionManager.InitRegionsBorders()),
            };

            MainForm.ExecuteActions(actions);
        }

        private static void InitPixels(int[] values, ushort width, ushort height)
        {
            size.x = width;
            size.y = height;
            pixels = new int[width, height];

            int pixelCount = width * height;
            int x, y;

            for (int i = 0; i < pixelCount; i++)
            {
                //Вычисляем координаты пикселя
                x = i % width;
                y = i / width;

                pixels[x, y] = values[i];
            }
        }

        private static void InitBorderInfoMap()
        {
            int l, lu, u, cur;
            ushort rightBorderX = (ushort)(size.x - 1);

            //Левый верхний угол
            l = pixels[rightBorderX, 0];
            cur = pixels[0, 0];
            if (cur != l)
            {
                Value2US pos;
                pos.x = 0;
                pos.y = 0;
                BorderInfo borderInfo = new BorderInfo();
                borderInfo.pos = pos;
                borderInfo.directionCount++;
                borderInfo.d = new Tuple<int, int>(cur, l);

                borderInfoMap[0].Add(pos, borderInfo);
                borderInfoMap[borderInfo.directionCount].Add(pos, borderInfo);
            }

            //Первый столбец
            for (ushort y = 1; y < size.y; y++)
            {
                l = pixels[rightBorderX, y];
                lu = pixels[rightBorderX, y - 1];
                u = pixels[0, y - 1];
                cur = pixels[0, y];

                if (lu != u || u != cur || cur != l || l != lu)
                {
                    Value2US pos;
                    pos.x = 0;
                    pos.y = y;
                    var borderInfo = new BorderInfo();
                    borderInfo.pos = pos;

                    if (lu != u)
                    {
                        borderInfo.directionCount++;
                        borderInfo.u = new Tuple<int, int>(lu, u);
                    }
                    if (u != cur)
                    {
                        borderInfo.r = new Tuple<int, int>(u, cur);
                        borderInfo.directionCount++;
                    }
                    if (cur != l)
                    {
                        borderInfo.directionCount++;
                        borderInfo.d = new Tuple<int, int>(cur, l);
                    }
                    if (l != lu) //TODO возможно, стоит убрать. На первом столбце это не нужно, по идее
                    {
                        //borderInfo.directionCount++;
                        //borderInfo.l = new Tuple<Color3B, Color3B>(l, lu);
                    }

                    borderInfoMap[0].Add(pos, borderInfo);
                    borderInfoMap[borderInfo.directionCount].Add(pos, borderInfo);

                }
            }

            //Первая строка
            for (ushort x = 1; x < size.x; x++)
            {
                l = pixels[x - 1, 0];
                cur = pixels[x, 0];

                if (cur != l)
                {
                    Value2US pos;
                    pos.x = x;
                    pos.y = 0;
                    var borderInfo = new BorderInfo
                    {
                        pos = pos,
                        directionCount = 1,
                        d = new Tuple<int, int>(cur, l)
                    };

                    borderInfoMap[0].Add(pos, borderInfo);
                    borderInfoMap[borderInfo.directionCount].Add(pos, borderInfo);
                }
            }

            //Левый нижний угол
            l = pixels[rightBorderX, size.y - 1];
            cur = pixels[0, size.y - 1];
            if (cur != l)
            {
                Value2US pos;
                pos.x = 0;
                pos.y = size.y;
                var borderInfo = new BorderInfo
                {
                    pos = pos,
                    directionCount = 1,
                    u = new Tuple<int, int>(l, cur)
                };

                borderInfoMap[0].Add(pos, borderInfo);
                borderInfoMap[borderInfo.directionCount].Add(pos, borderInfo);
            }


            //Все пиксели кроме граничных
            for (ushort y = 1; y < size.y; y++)
            {
                for (ushort x = 1; x < size.x; x++)
                {
                    l = pixels[x - 1, y];
                    lu = pixels[x - 1, y - 1];
                    u = pixels[x, y - 1];
                    cur = pixels[x, y];

                    if (lu != u || u != cur || cur != l || l != lu)
                    {
                        Value2US pos;
                        pos.x = x;
                        pos.y = y;
                        var borderInfo = new BorderInfo
                        {
                            pos = pos
                        };

                        if (lu != u)
                        {
                            borderInfo.directionCount++;
                            borderInfo.u = new Tuple<int, int>(lu, u);
                        }
                        if (u != cur)
                        {
                            borderInfo.r = new Tuple<int, int>(u, cur);
                            borderInfo.directionCount++;
                        }
                        if (cur != l)
                        {
                            borderInfo.directionCount++;
                            borderInfo.d = new Tuple<int, int>(cur, l);
                        }
                        if (l != lu)
                        {
                            borderInfo.directionCount++;
                            borderInfo.l = new Tuple<int, int>(l, lu);
                        }

                        borderInfoMap[0].Add(pos, borderInfo);
                        borderInfoMap[borderInfo.directionCount].Add(pos, borderInfo);

                    }
                }
            }

            //Последняя строка
            for (ushort x = 1; x < size.x; x++)
            {
                l = pixels[x - 1, size.y - 1];
                cur = pixels[x, size.y - 1];

                if (l != cur)
                {
                    Value2US pos;
                    pos.x = x;
                    pos.y = size.y;
                    var borderInfo = new BorderInfo
                    {
                        pos = pos,
                        directionCount = 1,
                        u = new Tuple<int, int>(l, cur)
                    };

                    borderInfoMap[0].Add(pos, borderInfo);
                    borderInfoMap[borderInfo.directionCount].Add(pos, borderInfo);
                }
            }

            //Последний столбец
            for (ushort y = 1; y < size.y; y++)
            {
                u = pixels[rightBorderX, y - 1];
                cur = pixels[rightBorderX, y];

                if (u != cur)
                {
                    Value2US pos;
                    pos.x = size.x;
                    pos.y = y;
                    var borderInfo = new BorderInfo
                    {
                        pos = pos,
                        directionCount = 1,
                        l = new Tuple<int, int>(cur, u)
                    };

                    borderInfoMap[0].Add(pos, borderInfo);
                    borderInfoMap[borderInfo.directionCount].Add(pos, borderInfo);

                }
            }
        }

        private static void InitBorders()
        {
            Province lp, rp;

            var pixels = new List<Value2US>();
            //Проходим по граничным пикселям с 4 направлениями
            Logger.Log($"X-Cross count: {borderInfoMap[4].Values.Count}. Coordinates: ");
            foreach (var borderInfo in borderInfoMap[4].Values)
            {
                Logger.Log($"x = {borderInfo.pos.x}; y = {borderInfo.pos.y}");
                ProcessBorderNodePixel(pixels, borderInfo.pos, borderInfo, EnumDirection.NONE);
            }
            //Проходим по граничным пикселям с 3 направлениями
            foreach (var borderInfo in borderInfoMap[3].Values)
            {
                previouslyDeleted.Add(borderInfo.pos);
                ProcessBorderNodePixel(pixels, borderInfo.pos, borderInfo, EnumDirection.NONE);
            }

            int leftColor, rightColor;
            EnumDirection direction;

            //Проходим по граничным пикселям с 1 направлением
            foreach (var borderInfo in borderInfoMap[1].Values)
            {
                borderInfoMap[0].Remove(borderInfo.pos);


                if (borderInfo.r != null)
                {
                    direction = EnumDirection.RIGHT;
                    leftColor = borderInfo.r.Item1;
                    rightColor = borderInfo.r.Item2;
                }
                else if (borderInfo.d != null)
                {
                    direction = EnumDirection.DOWN;
                    leftColor = borderInfo.d.Item1;
                    rightColor = borderInfo.d.Item2;
                }
                else if (borderInfo.l != null)
                {
                    direction = EnumDirection.LEFT;
                    leftColor = borderInfo.l.Item1;
                    rightColor = borderInfo.l.Item2;
                }
                else if (borderInfo.r != null)
                {
                    direction = EnumDirection.UP;
                    leftColor = borderInfo.u.Item1;
                    rightColor = borderInfo.u.Item2;
                }
                else continue;

                pixels.Add(borderInfo.pos);
                ProcessBorderPixel(pixels, borderInfo, leftColor, rightColor, direction);

                if (pixels.Count > 1)
                {
                    ProvinceManager.TryGetProvince(leftColor, out lp);
                    ProvinceManager.TryGetProvince(rightColor, out rp);
                    new ProvinceBorder(provinceBorderCount, pixels, lp, rp);
                    provinceBorderCount++;
                }
                pixels.Clear();
            }

            //Проходим по граничным пикселям с 2 направлениями
            foreach (var borderInfo in borderInfoMap[2].Values)
            {
                borderInfoMap[0].Remove(borderInfo.pos);
                previouslyDeleted.Add(borderInfo.pos);

                if (borderInfo.r != null)
                {
                    direction = EnumDirection.RIGHT;
                    leftColor = borderInfo.r.Item1;
                    rightColor = borderInfo.r.Item2;
                }
                else if (borderInfo.d != null)
                {
                    direction = EnumDirection.DOWN;
                    leftColor = borderInfo.d.Item1;
                    rightColor = borderInfo.d.Item2;
                }
                else if (borderInfo.l != null)
                {
                    direction = EnumDirection.LEFT;
                    leftColor = borderInfo.l.Item1;
                    rightColor = borderInfo.l.Item2;
                }
                else if (borderInfo.r != null)
                {
                    direction = EnumDirection.UP;
                    leftColor = borderInfo.u.Item1;
                    rightColor = borderInfo.u.Item2;
                }
                else continue;

                pixels.Add(borderInfo.pos);
                ProcessBorderPixel(pixels, borderInfo, leftColor, rightColor, direction);
                if (pixels.Count > 1)
                {
                    ProvinceManager.TryGetProvince(leftColor, out lp);
                    ProvinceManager.TryGetProvince(rightColor, out rp);
                    new ProvinceBorder(provinceBorderCount, pixels, lp, rp);
                    provinceBorderCount++;
                }
                pixels.Clear();
            }
        }

        private static void ProcessBorderPixel(in List<Value2US> pixels, in BorderInfo borderInfo, in int leftColor, in int rightColor, EnumDirection direction)
        {
            //Выбираем следующую координату по направлению движения
            Value2US newPos;
            if (borderInfo.r != null && direction != EnumDirection.LEFT && borderInfo.r.Item1 == leftColor && borderInfo.r.Item2 == rightColor)
            {
                borderInfo.r = null;
                direction = EnumDirection.RIGHT;
                newPos.x = (ushort)(borderInfo.pos.x + 1);
                newPos.y = borderInfo.pos.y;
            }
            else if (borderInfo.d != null && direction != EnumDirection.UP && borderInfo.d.Item1 == leftColor && borderInfo.d.Item2 == rightColor)
            {
                borderInfo.d = null;
                direction = EnumDirection.DOWN;
                newPos.x = borderInfo.pos.x;
                newPos.y = (ushort)(borderInfo.pos.y + 1);
            }
            else if (borderInfo.l != null && direction != EnumDirection.RIGHT && borderInfo.l.Item1 == leftColor && borderInfo.l.Item2 == rightColor)
            {
                borderInfo.l = null;
                direction = EnumDirection.LEFT;
                newPos.x = (ushort)(borderInfo.pos.x - 1);
                newPos.y = borderInfo.pos.y;
            }
            else if (borderInfo.u != null && direction != EnumDirection.DOWN && borderInfo.u.Item1 == leftColor && borderInfo.u.Item2 == rightColor)
            {
                borderInfo.u = null;
                direction = EnumDirection.UP;
                newPos.x = borderInfo.pos.x;
                newPos.y = (ushort)(borderInfo.pos.y - 1);
            }
            else return;

            //Получаем следующий пиксель по направлению движения
            borderInfoMap[0].TryGetValue(newPos, out BorderInfo nextBorderInfo);

            //Если пиксель существует
            if (nextBorderInfo != null)
            {
                //Если это обычный граничный пиксель с 2 направлениями
                if (nextBorderInfo.directionCount == 2)
                {
                    pixels.Add(newPos);
                    borderInfoMap[0].Remove(newPos); //Удаляем пиксель из словаря общих граничных пикселей
                                                     //borderInfoMap[2].Remove(newPos); //Удаляем пиксель из словаря граничных пикселей с 2 направлениями

                    if (direction == EnumDirection.RIGHT) nextBorderInfo.l = null;
                    else if (direction == EnumDirection.DOWN) nextBorderInfo.u = null;
                    else if (direction == EnumDirection.LEFT) nextBorderInfo.r = null;
                    else if (direction == EnumDirection.UP) nextBorderInfo.d = null;

                    ProcessBorderPixel(pixels, nextBorderInfo, leftColor, rightColor, direction);
                }
                //Если это граничный пиксель с пересечением нескольких границ
                else if (nextBorderInfo.directionCount >= 3)
                {
                    pixels.Add(newPos);
                    borderInfoMap[0].Remove(newPos); //Удаляем пиксель из словаря общих граничных пикселей

                    if (direction == EnumDirection.RIGHT) nextBorderInfo.l = null;
                    else if (direction == EnumDirection.DOWN) nextBorderInfo.u = null;
                    else if (direction == EnumDirection.LEFT) nextBorderInfo.r = null;
                    else if (direction == EnumDirection.UP) nextBorderInfo.d = null;
                    return;
                    //ProcessBorderNodePixel(pixels, nextBorderInfo, direction);
                }
                //Если это конечный граничный пиксель
                else if (nextBorderInfo.directionCount == 1)
                {
                    pixels.Add(newPos); //Добавляем пиксель к границе
                    borderInfoMap[0].Remove(newPos); //Удаляем пиксель из словаря общих граничных пикселей
                    //borderInfoMap[1].Remove(newPos); //Удаляем пиксель из словаря граничных пикселей с 1 направлением

                    if (direction == EnumDirection.RIGHT) nextBorderInfo.l = null;
                    else if (direction == EnumDirection.DOWN) nextBorderInfo.u = null;
                    else if (direction == EnumDirection.LEFT) nextBorderInfo.r = null;
                    else if (direction == EnumDirection.UP) nextBorderInfo.d = null;
                }
            }
            else if (previouslyDeleted.Contains(newPos))
            {
                pixels.Add(newPos);
            }


        }

        private static void ProcessBorderNodePixel(in List<Value2US> pixels, in Value2US startBorderPos, in BorderInfo borderInfo, in EnumDirection direction)
        {
            Province lp, rp;
            //Убираем направление, откуда пришли в этот пиксель
            if (direction == EnumDirection.RIGHT) borderInfo.l = null;
            else if (direction == EnumDirection.DOWN) borderInfo.u = null;
            else if (direction == EnumDirection.LEFT) borderInfo.r = null;
            else if (direction == EnumDirection.UP) borderInfo.d = null;

            int leftColor, rightColor;

            //Рекурсивно проходим по всем соединённым граничным пикселям
            if (borderInfo.r != null)
            {
                //Добавляем текущий граничный пиксель в список
                pixels.Add(borderInfo.pos);
                leftColor = borderInfo.r.Item1;
                rightColor = borderInfo.r.Item2;
                ProcessBorderPixel(pixels, borderInfo, leftColor, rightColor, EnumDirection.RIGHT);

                if (pixels.Count > 1)
                {
                    ProvinceManager.TryGetProvince(leftColor, out lp);
                    ProvinceManager.TryGetProvince(rightColor, out rp);
                    new ProvinceBorder(provinceBorderCount, pixels, lp, rp);
                    provinceBorderCount++;
                }
                pixels.Clear();
                borderInfo.r = null;
            }
            if (borderInfo.d != null)
            {
                //Добавляем текущий граничный пиксель в список
                pixels.Add(borderInfo.pos);
                leftColor = borderInfo.d.Item1;
                rightColor = borderInfo.d.Item2;
                ProcessBorderPixel(pixels, borderInfo, leftColor, rightColor, EnumDirection.DOWN);

                if (pixels.Count > 1)
                {
                    ProvinceManager.TryGetProvince(leftColor, out lp);
                    ProvinceManager.TryGetProvince(rightColor, out rp);
                    new ProvinceBorder(provinceBorderCount, pixels, lp, rp);
                    provinceBorderCount++;
                }
                pixels.Clear();
                borderInfo.d = null;
            }
            if (borderInfo.l != null)
            {
                //Добавляем текущий граничный пиксель в список
                pixels.Add(borderInfo.pos);
                leftColor = borderInfo.l.Item1;
                rightColor = borderInfo.l.Item2;
                ProcessBorderPixel(pixels, borderInfo, leftColor, rightColor, EnumDirection.LEFT);

                if (pixels.Count > 1)
                {
                    ProvinceManager.TryGetProvince(leftColor, out lp);
                    ProvinceManager.TryGetProvince(rightColor, out rp);
                    new ProvinceBorder(provinceBorderCount, pixels, lp, rp);
                    provinceBorderCount++;
                }
                pixels.Clear();
                borderInfo.l = null;
            }
            if (borderInfo.u != null)
            {
                //Добавляем текущий граничный пиксель в список
                pixels.Add(borderInfo.pos);
                leftColor = borderInfo.u.Item1;
                rightColor = borderInfo.u.Item2;
                ProcessBorderPixel(pixels, borderInfo, leftColor, rightColor, EnumDirection.UP);

                if (pixels.Count > 1)
                {
                    ProvinceManager.TryGetProvince(leftColor, out lp);
                    ProvinceManager.TryGetProvince(rightColor, out rp);
                    new ProvinceBorder(provinceBorderCount, pixels, lp, rp);
                    provinceBorderCount++;
                }
                pixels.Clear();
                borderInfo.u = null;
            }
        }

        private class BorderInfo
        {
            public byte directionCount = 0;
            public Value2US pos;
            public Tuple<int, int> u, r, d, l;

            public override bool Equals(object obj)
            {
                return obj is BorderInfo info &&
                       EqualityComparer<Tuple<int, int>>.Default.Equals(u, info.u) &&
                       EqualityComparer<Tuple<int, int>>.Default.Equals(r, info.r) &&
                       EqualityComparer<Tuple<int, int>>.Default.Equals(d, info.d) &&
                       EqualityComparer<Tuple<int, int>>.Default.Equals(l, info.l);
            }

            public override int GetHashCode()
            {
                int hashCode = -1287137045;
                hashCode = hashCode * -1521134295 + EqualityComparer<Tuple<int, int>>.Default.GetHashCode(u);
                hashCode = hashCode * -1521134295 + EqualityComparer<Tuple<int, int>>.Default.GetHashCode(r);
                hashCode = hashCode * -1521134295 + EqualityComparer<Tuple<int, int>>.Default.GetHashCode(d);
                hashCode = hashCode * -1521134295 + EqualityComparer<Tuple<int, int>>.Default.GetHashCode(l);
                return hashCode;
            }
        }

    }

}
