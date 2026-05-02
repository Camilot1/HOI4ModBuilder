using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.openTK.text
{
    public class TextRenderInvalidationBatch
    {
        private readonly Dictionary<EnumMapRenderEvents, HashSet<Province>> _provincesByEvent;
        private readonly Dictionary<EnumMapRenderEvents, HashSet<State>> _statesByEvent;
        private readonly Dictionary<EnumMapRenderEvents, HashSet<StrategicRegion>> _regionsByEvent;

        public int EventsFlags { get; private set; }
        public bool IsEmpty => EventsFlags == 0;

        public TextRenderInvalidationBatch()
        {
            _provincesByEvent = new Dictionary<EnumMapRenderEvents, HashSet<Province>>();
            _statesByEvent = new Dictionary<EnumMapRenderEvents, HashSet<State>>();
            _regionsByEvent = new Dictionary<EnumMapRenderEvents, HashSet<StrategicRegion>>();
        }

        public TextRenderInvalidationBatch(TextRenderInvalidationBatch other)
            : this()
        {
            if (other == null)
                return;

            EventsFlags = other.EventsFlags;
            CopySets(other._provincesByEvent, _provincesByEvent);
            CopySets(other._statesByEvent, _statesByEvent);
            CopySets(other._regionsByEvent, _regionsByEvent);
        }

        public bool HasMatchingDependencies(TextLayerDependencies dependencies)
        {
            if (dependencies.Items == null)
                return false;

            foreach (var dependency in dependencies.Items)
                if (HasEntries(dependency))
                    return true;

            return false;
        }

        public HashSet<Province> CollectAffectedProvinces(TextLayerDependencies dependencies)
        {
            var provinces = new HashSet<Province>();
            if (dependencies.Items == null)
                return provinces;

            foreach (var dependency in dependencies.Items)
            {
                switch (dependency.Source)
                {
                    case EnumTextLayerDependencySource.Province:
                        AddRange(provinces, GetProvinces(dependency.EventFlag));
                        break;

                    case EnumTextLayerDependencySource.State:
                        foreach (var state in GetStates(dependency.EventFlag))
                        {
                            if (state == null)
                                continue;

                            foreach (var province in state.Provinces)
                                if (province != null)
                                    provinces.Add(province);
                        }
                        break;

                    case EnumTextLayerDependencySource.Region:
                        foreach (var region in GetRegions(dependency.EventFlag))
                        {
                            if (region == null)
                                continue;

                            foreach (var province in region.Provinces)
                                if (province != null)
                                    provinces.Add(province);
                        }
                        break;
                }
            }

            return provinces;
        }

        public HashSet<State> CollectAffectedStates(TextLayerDependencies dependencies)
        {
            var states = new HashSet<State>();
            if (dependencies.Items == null)
                return states;

            foreach (var dependency in dependencies.Items)
            {
                switch (dependency.Source)
                {
                    case EnumTextLayerDependencySource.Province:
                        foreach (var province in GetProvinces(dependency.EventFlag))
                            if (province?.State != null)
                                states.Add(province.State);
                        break;

                    case EnumTextLayerDependencySource.State:
                        AddRange(states, GetStates(dependency.EventFlag));
                        break;

                    case EnumTextLayerDependencySource.Region:
                        foreach (var region in GetRegions(dependency.EventFlag))
                        {
                            if (region == null)
                                continue;

                            foreach (var province in region.Provinces)
                                if (province?.State != null)
                                    states.Add(province.State);
                        }
                        break;
                }
            }

            return states;
        }

        public HashSet<StrategicRegion> CollectAffectedRegions(TextLayerDependencies dependencies)
        {
            var regions = new HashSet<StrategicRegion>();
            if (dependencies.Items == null)
                return regions;

            foreach (var dependency in dependencies.Items)
            {
                switch (dependency.Source)
                {
                    case EnumTextLayerDependencySource.Province:
                        foreach (var province in GetProvinces(dependency.EventFlag))
                            if (province?.Region != null)
                                regions.Add(province.Region);
                        break;

                    case EnumTextLayerDependencySource.State:
                        foreach (var state in GetStates(dependency.EventFlag))
                        {
                            if (state == null)
                                continue;

                            foreach (var province in state.Provinces)
                                if (province?.Region != null)
                                    regions.Add(province.Region);
                        }
                        break;

                    case EnumTextLayerDependencySource.Region:
                        AddRange(regions, GetRegions(dependency.EventFlag));
                        break;
                }
            }

            return regions;
        }

        public bool Add(EnumMapRenderEvents eventFlag, Province province)
        {
            if (province == null)
                return false;

            EventsFlags |= (int)eventFlag;
            return GetOrCreate(_provincesByEvent, eventFlag).Add(province);
        }

        public bool Add(EnumMapRenderEvents eventFlag, State state)
        {
            if (state == null)
                return false;

            EventsFlags |= (int)eventFlag;
            return GetOrCreate(_statesByEvent, eventFlag).Add(state);
        }

        public bool Add(EnumMapRenderEvents eventFlag, StrategicRegion region)
        {
            if (region == null)
                return false;

            EventsFlags |= (int)eventFlag;
            return GetOrCreate(_regionsByEvent, eventFlag).Add(region);
        }

        public void Clear()
        {
            EventsFlags = 0;
            _provincesByEvent.Clear();
            _statesByEvent.Clear();
            _regionsByEvent.Clear();
        }

        private bool HasEntries(TextLayerDependency dependency)
        {
            switch (dependency.Source)
            {
                case EnumTextLayerDependencySource.Province:
                    return HasValues(_provincesByEvent, dependency.EventFlag);

                case EnumTextLayerDependencySource.State:
                    return HasValues(_statesByEvent, dependency.EventFlag);

                case EnumTextLayerDependencySource.Region:
                    return HasValues(_regionsByEvent, dependency.EventFlag);

                default:
                    return false;
            }
        }

        private ICollection<Province> GetProvinces(EnumMapRenderEvents eventFlag)
        {
            if (_provincesByEvent.TryGetValue(eventFlag, out var provinces))
                return provinces;

            return Empty<Province>.Items;
        }

        private ICollection<State> GetStates(EnumMapRenderEvents eventFlag)
        {
            if (_statesByEvent.TryGetValue(eventFlag, out var states))
                return states;

            return Empty<State>.Items;
        }

        private ICollection<StrategicRegion> GetRegions(EnumMapRenderEvents eventFlag)
        {
            if (_regionsByEvent.TryGetValue(eventFlag, out var regions))
                return regions;

            return Empty<StrategicRegion>.Items;
        }

        private static bool HasValues<T>(
            Dictionary<EnumMapRenderEvents, HashSet<T>> dictionary,
            EnumMapRenderEvents eventFlag)
        {
            return dictionary.TryGetValue(eventFlag, out var values) && values.Count > 0;
        }

        private static HashSet<T> GetOrCreate<T>(
            Dictionary<EnumMapRenderEvents, HashSet<T>> dictionary,
            EnumMapRenderEvents eventFlag)
        {
            if (!dictionary.TryGetValue(eventFlag, out var values))
            {
                values = new HashSet<T>();
                dictionary[eventFlag] = values;
            }

            return values;
        }

        private static void AddRange<T>(HashSet<T> target, ICollection<T> source)
        {
            foreach (var value in source)
                if (value != null)
                    target.Add(value);
        }

        private static void CopySets<T>(
            Dictionary<EnumMapRenderEvents, HashSet<T>> source,
            Dictionary<EnumMapRenderEvents, HashSet<T>> target)
        {
            foreach (var pair in source)
                target[pair.Key] = new HashSet<T>(pair.Value);
        }

        private static class Empty<T>
        {
            public static readonly T[] Items = new T[0];
        }
    }
}
