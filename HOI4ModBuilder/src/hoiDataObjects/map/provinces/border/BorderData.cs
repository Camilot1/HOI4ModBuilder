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
            AddOrMerge(point);
            return this;
        }

        public BorderData Merge(BorderData other)
        {
            if (other.points == null || other.points.Count == 0)
                return this;

            if (points == null)
                points = new HashSet<ValueDirectionalPos>(other.points.Count);

            foreach (var point in other.points)
                AddOrMerge(point);

            return this;
        }

        public List<List<Value2S>> AssembleBorders(short width)
        {
            if (points == null || points.Count == 0)
                return new List<List<Value2S>>(0);

            var nodes = new Dictionary<Value2S, byte>(points.Count);
            foreach (var point in points)
                nodes[point.pos] = point.flags;

            var adjacency = new Dictionary<Value2S, List<Value2S>>(points.Count);

            foreach (var entry in nodes)
            {
                var pos = entry.Key;
                var flags = entry.Value;

                if ((flags & 0b1000) != 0)
                {
                    var left = new Value2S((short)(pos.x - 1), pos.y);
                    if (nodes.ContainsKey(left))
                        AddEdge(adjacency, pos, left);
                }
                if ((flags & 0b0100) != 0)
                {
                    var up = new Value2S(pos.x, (short)(pos.y - 1));
                    if (nodes.ContainsKey(up))
                        AddEdge(adjacency, pos, up);
                }
                if ((flags & 0b0010) != 0)
                {
                    var right = new Value2S((short)(pos.x + 1), pos.y);
                    if (right.x == width)
                        right.x = 0;
                    if (nodes.ContainsKey(right))
                        AddEdge(adjacency, pos, right);
                }
                if ((flags & 0b0001) != 0)
                {
                    var down = new Value2S(pos.x, (short)(pos.y + 1));
                    if (nodes.ContainsKey(down))
                        AddEdge(adjacency, pos, down);
                }
            }

            var result = new List<List<Value2S>>();
            var visitedEdges = new HashSet<EdgeKey>(adjacency.Count * 2);

            foreach (var entry in nodes)
            {
                if (!adjacency.ContainsKey(entry.Key))
                    result.Add(new List<Value2S> { entry.Key });
            }

            foreach (var entry in adjacency)
            {
                var start = entry.Key;
                var neighbors = entry.Value;
                if (neighbors.Count == 0)
                    continue;
                if (neighbors.Count == 2)
                    continue;

                foreach (var neighbor in neighbors)
                {
                    var edge = new EdgeKey(start, neighbor);
                    if (!visitedEdges.Contains(edge))
                        result.Add(TracePath(start, neighbor, adjacency, visitedEdges, width));
                }
            }

            foreach (var entry in adjacency)
            {
                var start = entry.Key;
                foreach (var neighbor in entry.Value)
                {
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
        {
            if (points == null)
                points = new HashSet<ValueDirectionalPos>(16);

            if (points.TryGetValue(point, out var existing))
            {
                byte mergedFlags = (byte)(existing.flags | point.flags);
                if (mergedFlags != existing.flags)
                {
                    existing.flags = mergedFlags;
                    points.Remove(existing);
                    points.Add(existing);
                }
                return;
            }

            points.Add(point);
        }

        private static void AddEdge(Dictionary<Value2S, List<Value2S>> adjacency, Value2S a, Value2S b)
        {
            if (!adjacency.TryGetValue(a, out var listA))
            {
                listA = new List<Value2S>(2);
                adjacency[a] = listA;
            }
            if (!listA.Contains(b))
                listA.Add(b);

            if (!adjacency.TryGetValue(b, out var listB))
            {
                listB = new List<Value2S>(2);
                adjacency[b] = listB;
            }
            if (!listB.Contains(a))
                listB.Add(a);
        }

        private static List<Value2S> TracePath(
            Value2S start,
            Value2S next,
            Dictionary<Value2S, List<Value2S>> adjacency,
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

                var nextCandidate = neighbors[0].Equals(current) ? neighbors[1] : neighbors[0];
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
    }
}
