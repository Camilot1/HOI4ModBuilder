using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.map.provinces.border
{
    public struct BorderData
    {
        public int provinceMinColor, provinceMaxColor;
        public HashSet<ValueDirectionalPos> points;

        public BorderData(int provinceMinColor, int provinceMaxColor)
        {
            this.provinceMinColor = provinceMinColor;
            this.provinceMaxColor = provinceMaxColor;
            points = new HashSet<ValueDirectionalPos>(16);
        }

        public BorderData Add(ValueDirectionalPos point)
        {
            points.Add(point);
            return this;
        }

        public List<List<Value2S>> AssembleBorders(short width)
        {
            var linkedDataDictionary = new Dictionary<Value2S, LinkedData<ValueDirectionalPos>>(points.Count);

            foreach (var point in points)
            {
                var linkedData = new LinkedData<ValueDirectionalPos>
                {
                    data = point
                };

                Value2S left = new Value2S((short)(point.pos.x - 1), point.pos.y);
                Value2S up = new Value2S(point.pos.x, (short)(point.pos.y - 1));
                Value2S right = new Value2S((short)(point.pos.x + 1), point.pos.y);
                Value2S down = new Value2S(point.pos.x, (short)(point.pos.y + 1));

                if (right.x == width)
                    right.x = 0;

                if (((point.flags & 0b1000) != 0) && linkedDataDictionary.TryGetValue(left, out var data))
                    TryConnect(linkedData, data);
                if (((point.flags & 0b0100) != 0) && linkedDataDictionary.TryGetValue(up, out data))
                    TryConnect(linkedData, data);
                if (((point.flags & 0b0010) != 0) && linkedDataDictionary.TryGetValue(right, out data))
                    TryConnect(linkedData, data);
                if (((point.flags & 0b0001) != 0) && linkedDataDictionary.TryGetValue(down, out data))
                    TryConnect(linkedData, data);

                linkedDataDictionary[point.pos] = linkedData;

                void TryConnect(LinkedData<ValueDirectionalPos> thisData, LinkedData<ValueDirectionalPos> otherData)
                {
                    if (otherData.prev == null)
                    {
                        if (thisData.next == null)
                        {
                            otherData.prev = thisData;
                            thisData.next = otherData;
                        }
                        else if (thisData.prev == null)
                        {
                            otherData.prev = thisData;
                            Utils.ReverseLinkedData(thisData);
                            thisData.next = otherData;
                        }
                    }
                    else if (otherData.next == null)
                    {
                        if (thisData.prev == null)
                        {
                            otherData.next = thisData;
                            thisData.prev = otherData;
                        }
                        else if (thisData.next == null)
                        {
                            otherData.next = thisData;
                            Utils.ReverseLinkedData(thisData);
                            thisData.prev = otherData;
                        }
                    }
                }
            }

            var result = new List<List<Value2S>>();
            Value2S tempPos = default;

            foreach (var linkedData in linkedDataDictionary.Values)
            {
                if (linkedData.data.isUsed)
                    continue;

                if (linkedData.prev != null && linkedData.next != null)
                    continue;

                AcceptLinkedData(linkedData);
            }

            foreach (var linkedData in linkedDataDictionary.Values)
            {
                if (linkedData.data.isUsed)
                    continue;

                linkedData.data.isUsed = true;
                var pixels = new List<Value2S> { linkedData.data.pos };
                tempPos = linkedData.data.pos;

                var temp = linkedData.next;
                while (temp != null && !temp.data.isUsed)
                {
                    if (tempPos.GetSquareDistanceTo(temp.data.pos) > 1)
                    {
                        if (pixels.Count == 1 && pixels[0].x == 0)
                            pixels[0] = new Value2S { x = width, y = pixels[0].y };
                    }
                    pixels.Add(temp.data.pos);

                    tempPos = temp.data.pos;

                    temp.data.isUsed = true;
                    temp = temp.next;
                }

                result.Add(pixels);
            }

            void AcceptLinkedData(LinkedData<ValueDirectionalPos> linkedData)
            {
                linkedData.data.isUsed = true;

                var pixels = new List<Value2S> { linkedData.data.pos };
                tempPos = linkedData.data.pos;

                if (linkedData.prev == null)
                {
                    var temp = linkedData.next;
                    while (temp != null)
                    {
                        if (tempPos.GetSquareDistanceTo(temp.data.pos) > 1)
                        {
                            if (pixels.Count == 1 && pixels[0].x == 0)
                                pixels[0] = new Value2S { x = width, y = pixels[0].y };
                        }
                        pixels.Add(temp.data.pos);

                        tempPos = temp.data.pos;

                        temp.data.isUsed = true;
                        temp = temp.next;
                    }
                }
                else if (linkedData.next == null)
                {
                    var temp = linkedData.prev;
                    while (temp != null)
                    {
                        if (tempPos.GetSquareDistanceTo(temp.data.pos) > 1)
                        {
                            if (pixels.Count == 1 && pixels[0].x == 0)
                                pixels[0] = new Value2S { x = width, y = pixels[0].y };
                        }
                        pixels.Add(temp.data.pos);

                        tempPos = temp.data.pos;
                        temp.data.isUsed = true;
                        temp = temp.prev;
                    }
                }

                if (pixels.Count == 0)
                    throw new Exception("Invalid LinkedData border info: " + linkedData.data.ToString());

                result.Add(pixels);
            }

            points = null;

            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is BorderData data &&
                   provinceMinColor == data.provinceMinColor &&
                   provinceMaxColor == data.provinceMaxColor;
        }

        public override int GetHashCode()
        {
            int hashCode = -1526548159;
            hashCode = hashCode * -1521134295 + provinceMinColor.GetHashCode();
            hashCode = hashCode * -1521134295 + provinceMaxColor.GetHashCode();
            return hashCode;
        }
    }
}
