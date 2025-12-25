using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.map.provinces.border
{
    public struct BorderData
    {
        public int provinceMinColor, provinceMaxColor;
        public Dictionary<Value2S, byte> points;

        public BorderData(int provinceMinColor, int provinceMaxColor)
        {
            this.provinceMinColor = provinceMinColor;
            this.provinceMaxColor = provinceMaxColor;
            points = new Dictionary<Value2S, byte>(16);
        }

        public BorderData Add(ValueDirectionalPos point)
        {
            AddOrMerge(point);
            return this;
        }

        public BorderData Merge(BorderData other)
        {
            if (other.points == null || other.points.Count == 0)
                return this;

            if (points == null)
                points = new Dictionary<Value2S, byte>(other.points.Count);

            foreach (var point in other.points)
                AddOrMerge(point.Key, point.Value);

            return this;
        }

        public List<List<Value2S>> AssembleBorders(short width)
        {
            if (points == null || points.Count == 0)
                return new List<List<Value2S>>(0);

            var adjacency = new Dictionary<Value2S, NeighborList>(points.Count);

            foreach (var point in points)
            {
                var pos = point.Key;
                var flags = point.Value;

                if ((flags & 0b1000) != 0)
                {
                    var left = new Value2S((short)(pos.x - 1), pos.y);
                    if (points.ContainsKey(left))
                        AddEdge(adjacency, pos, left);
                }
                if ((flags & 0b0100) != 0)
                {
                    var up = new Value2S(pos.x, (short)(pos.y - 1));
                    if (points.ContainsKey(up))
                        AddEdge(adjacency, pos, up);
                }
                if ((flags & 0b0010) != 0)
                {
                    var right = new Value2S((short)(pos.x + 1), pos.y);
                    if (right.x == width)
                        right.x = 0;
                    if (points.ContainsKey(right))
                        AddEdge(adjacency, pos, right);
                }
                if ((flags & 0b0001) != 0)
                {
                    var down = new Value2S(pos.x, (short)(pos.y + 1));
                    if (points.ContainsKey(down))
                        AddEdge(adjacency, pos, down);
                }
            }

            var result = new List<List<Value2S>>();
            var visitedEdges = new HashSet<EdgeKey>(adjacency.Count * 2);

            foreach (var point in points)
            {
                if (!adjacency.ContainsKey(point.Key))
                    result.Add(new List<Value2S> { point.Key });
            }

            foreach (var entry in adjacency)
            {
                var start = entry.Key;
                var neighbors = entry.Value;
                int neighborsCount = neighbors.Count;
                if (neighborsCount == 0)
                    continue;
                if (neighborsCount == 2)
                    continue;

                for (int i = 0; i < neighborsCount; i++)
                {
                    var neighbor = neighbors.Get(i);
                    var edge = new EdgeKey(start, neighbor);
                    if (!visitedEdges.Contains(edge))
                        result.Add(TracePath(start, neighbor, adjacency, visitedEdges, width));
                }
            }

            foreach (var entry in adjacency)
            {
                var start = entry.Key;
                var neighbors = entry.Value;
                int neighborsCount = neighbors.Count;
                for (int i = 0; i < neighborsCount; i++)
                {
                    var neighbor = neighbors.Get(i);
                    var edge = new EdgeKey(start, neighbor);
                    if (!visitedEdges.Contains(edge))
                        result.Add(TracePath(start, neighbor, adjacency, visitedEdges, width));
                }
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

        private void AddOrMerge(ValueDirectionalPos point)
            => AddOrMerge(point.pos, point.flags);

        private void AddOrMerge(Value2S pos, byte flags)
        {
            if (points == null)
                points = new Dictionary<Value2S, byte>(16);

            if (points.TryGetValue(pos, out var existingFlags))
            {
                byte mergedFlags = (byte)(existingFlags | flags);
                if (mergedFlags != existingFlags)
                    points[pos] = mergedFlags;
                return;
            }

            points[pos] = flags;
        }

        private static void AddEdge(Dictionary<Value2S, NeighborList> adjacency, Value2S a, Value2S b)
        {
            AddNeighbor(adjacency, a, b);
            AddNeighbor(adjacency, b, a);
        }

        private static void AddNeighbor(Dictionary<Value2S, NeighborList> adjacency, Value2S key, Value2S neighbor)
        {
            if (adjacency.TryGetValue(key, out var list))
            {
                if (list.Add(neighbor))
                    adjacency[key] = list;
                return;
            }

            list = new NeighborList();
            list.Add(neighbor);
            adjacency[key] = list;
        }

        private static List<Value2S> TracePath(
            Value2S start,
            Value2S next,
            Dictionary<Value2S, NeighborList> adjacency,
            HashSet<EdgeKey> visitedEdges,
            short width)
        {
            var pixels = new List<Value2S> { start };
            var current = start;
            var candidate = next;

            while (true)
            {
                var edge = new EdgeKey(current, candidate);
                if (!visitedEdges.Add(edge))
                    break;

                AppendPoint(pixels, candidate, width);

                if (!adjacency.TryGetValue(candidate, out var neighbors) || neighbors.Count != 2)
                    break;

                var nextCandidate = neighbors.Get(0).Equals(current) ? neighbors.Get(1) : neighbors.Get(0);
                current = candidate;
                candidate = nextCandidate;
            }

            return pixels;
        }

        private static void AppendPoint(List<Value2S> pixels, Value2S point, short width)
        {
            if (pixels.Count > 0)
            {
                var last = pixels[pixels.Count - 1];
                if (last.GetSquareDistanceTo(point) > 1 && pixels.Count == 1 && pixels[0].x == 0)
                {
                    var first = pixels[0];
                    first.x = width;
                    pixels[0] = first;
                }
            }

            pixels.Add(point);
        }

        private struct EdgeKey : IEquatable<EdgeKey>
        {
            private readonly Value2S _a;
            private readonly Value2S _b;

            public EdgeKey(Value2S a, Value2S b)
            {
                if (Compare(a, b) <= 0)
                {
                    _a = a;
                    _b = b;
                }
                else
                {
                    _a = b;
                    _b = a;
                }
            }

            public bool Equals(EdgeKey other)
                => _a.Equals(other._a) && _b.Equals(other._b);

            public override bool Equals(object obj)
                => obj is EdgeKey other && Equals(other);

            public override int GetHashCode()
                => (_a.GetHashCode() * 397) ^ _b.GetHashCode();

            private static int Compare(Value2S a, Value2S b)
            {
                if (a.x != b.x)
                    return a.x.CompareTo(b.x);
                return a.y.CompareTo(b.y);
            }
        }

        private struct NeighborList
        {
            private Value2S _a;
            private Value2S _b;
            private Value2S _c;
            private Value2S _d;
            private byte _count;

            public int Count => _count;

            public bool Add(Value2S value)
            {
                if (_count == 0)
                {
                    _a = value;
                    _count = 1;
                    return true;
                }
                if (_a.Equals(value))
                    return false;

                if (_count == 1)
                {
                    _b = value;
                    _count = 2;
                    return true;
                }
                if (_b.Equals(value))
                    return false;

                if (_count == 2)
                {
                    _c = value;
                    _count = 3;
                    return true;
                }
                if (_c.Equals(value))
                    return false;

                if (_count == 3)
                {
                    _d = value;
                    _count = 4;
                    return true;
                }
                if (_d.Equals(value))
                    return false;

                return false;
            }

            public Value2S Get(int index)
            {
                switch (index)
                {
                    case 0:
                        return _a;
                    case 1:
                        return _b;
                    case 2:
                        return _c;
                    default:
                        return _d;
                }
            }
        }
    }
}
