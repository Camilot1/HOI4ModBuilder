using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.managers.statistics
{
    public class StatistictsData
    {
        public bool sortOnSave;
        public int sortIndex;
        public Dictionary<string, StatisticsDataType> types = new Dictionary<string, StatisticsDataType>();

        public StatistictsData() { }
        public StatistictsData(int sortIndex, bool sortOnSave) : this()
        {
            this.sortIndex = sortIndex;
            this.sortOnSave = sortOnSave;
        }

        public StatistictsData Add(EnumLocKey key, StatisticsDataType dataType)
            => Add(key + "", dataType);
        public StatistictsData Add(string key, StatisticsDataType dataType)
        {
            types.Add(key, dataType);
            return this;
        }

        public StatistictsData AddFrom(List<StatistictsData> data)
        {
            data.Sort((x, y) => x.sortIndex.CompareTo(y.sortIndex));
            foreach (var obj in data)
                foreach (var entry in obj.types)
                    types.Add(entry.Key, entry.Value);

            return this;
        }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            var list = new List<(string, StatisticsDataType)>(types.Count);
            foreach (var entry in types)
                list.Add((entry.Key, entry.Value));

            list.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            foreach (var entry in list)
                entry.Item2.Save(sb, outTab, tab, entry.Item1);
        }
    }

    public class StatisticsDataType : IComparable<StatisticsDataType>
    {
        public int? sortIndex;
        public double? count;
        public double? max;
        public double? sum;

        public bool sortInner;
        public Dictionary<string, StatisticsDataType> inner = new Dictionary<string, StatisticsDataType>();

        public StatisticsDataType SetSortIndex(int? sortIndex)
        {
            this.sortIndex = sortIndex;
            return this;
        }

        public StatisticsDataType(double? count, double? max, bool sortInner)
        {
            this.count = count;
            this.max = max;
            this.sortInner = sortInner;
        }
        public StatisticsDataType(double? count, bool sortInner)
            : this(count, null, sortInner)
        { }
        public StatisticsDataType(bool sortInner)
            : this(null, null, sortInner)
        { }
        public StatisticsDataType()
            : this(null, null, false)
        { }

        public StatisticsDataType Add(double count)
        {
            if (this.count == null)
                this.count = count;
            else
                this.count += count;
            return this;
        }
        public StatisticsDataType SetCount(double count)
        {
            this.count = count;
            return this;
        }
        public StatisticsDataType SetSum(double sum)
        {
            this.sum = sum;
            return this;
        }


        public StatisticsDataType GetOrCreateInner(EnumLocKey key)
            => GetOrCreateInner(key + "", null, null, false);
        public StatisticsDataType GetOrCreateInner(EnumLocKey key, out StatisticsDataType type)
            => GetOrCreateInner(key + "", null, null, false, out type);
        public StatisticsDataType GetOrCreateInner(EnumLocKey key, bool sortOnSave)
            => GetOrCreateInner(key + "", null, null, sortOnSave);
        public StatisticsDataType GetOrCreateInner(EnumLocKey key, double? count, double? maxCount, bool sortOnSave)
            => GetOrCreateInner(key + "", count, maxCount, sortOnSave);
        public StatisticsDataType GetOrCreateInner(EnumLocKey key, double? count, double? maxCount, bool sortOnSave, out StatisticsDataType type)
            => GetOrCreateInner(key + "", count, maxCount, sortOnSave, out type);
        public StatisticsDataType GetOrCreateInner(string key)
            => GetOrCreateInner(key, null, null, false);
        public StatisticsDataType GetOrCreateInner(string key, bool sortOnSave)
            => GetOrCreateInner(key, null, null, sortOnSave);
        public StatisticsDataType GetOrCreateInner(string key, double? count, double? maxCount, bool sortInner)
        {
            if (!inner.TryGetValue(key, out StatisticsDataType type))
                inner[key] = type = new StatisticsDataType(count, maxCount, sortInner);
            return type;
        }
        public StatisticsDataType GetOrCreateInner(string key, double? count, double? maxCount, bool sortInner, out StatisticsDataType type)
        {
            if (!inner.TryGetValue(key, out type))
                inner[key] = type = new StatisticsDataType(count, maxCount, sortInner);
            return type;
        }

        public int CompareTo(StatisticsDataType other)
        {
            if (sortIndex != null)
                return ((int)sortIndex).CompareTo(other.sortIndex);
            else if (other.sortIndex != null)
                return -((int)other.sortIndex).CompareTo(sortIndex);

            if (count != null)
                return ((double)count).CompareTo(other.count);
            else if (other.count != null)
                return -((double)other.count).CompareTo(count);

            return 0;
        }

        public void Save(StringBuilder sb, string outTab, string tab, string displayName)
        {
            sb.Append(outTab).Append(GuiLocManager.GetLoc(displayName)).Append(": ");
            if (count != null)
            {
                sb.Append(count).Append(' ');
                if (max != null)
                    sb.Append("/ ").Append(max).Append(' ');
            }
            if (sum != null)
                sb.Append('(').Append(sum).Append(')');
            sb.Append('\n');

            if (inner != null && inner.Count > 0)
            {
                var innerTab = outTab + tab;
                if (sortInner)
                {
                    var list = new List<(string, StatisticsDataType)>(inner.Count);
                    foreach (var entry in inner)
                        list.Add((entry.Key, entry.Value));
                    list.Sort((x, y) => -x.Item2.CompareTo(y.Item2));
                    foreach (var entry in list)
                        entry.Item2.Save(sb, innerTab, tab, entry.Item1);
                }
                else
                {
                    foreach (var entry in inner)
                        entry.Value.Save(sb, innerTab, tab, entry.Key);
                }
            }
        }
    }

    public class StatisticsFilters
    {
        public HashSet<ushort> statesIDs = new HashSet<ushort>();
        public HashSet<ushort> regionsIDs = new HashSet<ushort>();
        public HashSet<string> countriesTags = new HashSet<string>();

        public bool CheckStateID(ushort id)
            => statesIDs.Contains(id);
        public bool CheckRegionID(ushort regionID)
            => regionsIDs.Contains(regionID);
        public bool CheckCountryTag(string tag)
            => countriesTags.Contains(tag);

        public bool HasStatesIDs()
            => statesIDs.Count != 0;
        public bool HasRegionsIDs()
            => regionsIDs.Count != 0;
        public bool HasCountriesTags()
            => countriesTags.Count != 0;

        public bool HasAnyFilters()
            => HasStatesIDs() || HasRegionsIDs() || HasCountriesTags();
    }
}
