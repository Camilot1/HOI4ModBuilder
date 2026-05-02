using HOI4ModBuilder.src.hoiDataObjects.map.renderer.enums;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.openTK.text
{
    public enum EnumTextLayerDependencySource
    {
        Province,
        State,
        Region
    }

    public readonly struct TextLayerDependency
    {
        public EnumMapRenderEvents EventFlag { get; }
        public EnumTextLayerDependencySource Source { get; }

        public TextLayerDependency(EnumMapRenderEvents eventFlag, EnumTextLayerDependencySource source)
        {
            EventFlag = eventFlag;
            Source = source;
        }
    }

    public readonly struct TextLayerDependencies
    {
        public static readonly TextLayerDependencies None
            = new TextLayerDependencies(
                (int)EnumMapRenderEvents.NONE,
                Array.Empty<TextLayerDependency>()
            );

        public int Flags { get; }
        public IReadOnlyList<TextLayerDependency> Items { get; }

        private TextLayerDependencies(int flags, IReadOnlyList<TextLayerDependency> items)
        {
            Flags = flags;
            Items = items;
        }

        public bool Matches(EnumMapRenderEvents eventFlag, EnumTextLayerDependencySource source)
        {
            if (Items == null)
                return false;

            foreach (var item in Items)
                if (item.EventFlag == eventFlag && item.Source == source)
                    return true;

            return false;
        }

        public static TextLayerDependencies FromProvince(params EnumMapRenderEvents[] eventFlags)
            => From(EnumTextLayerDependencySource.Province, eventFlags);

        public static TextLayerDependencies FromState(params EnumMapRenderEvents[] eventFlags)
            => From(EnumTextLayerDependencySource.State, eventFlags);

        public static TextLayerDependencies FromRegion(params EnumMapRenderEvents[] eventFlags)
            => From(EnumTextLayerDependencySource.Region, eventFlags);

        public static TextLayerDependencies Combine(params TextLayerDependencies[] dependenciesList)
        {
            if (dependenciesList == null || dependenciesList.Length == 0)
                return None;

            var items = new List<TextLayerDependency>();
            int flags = 0;

            foreach (var dependencies in dependenciesList)
            {
                flags |= dependencies.Flags;
                if (dependencies.Items == null)
                    continue;

                foreach (var item in dependencies.Items)
                    if (!Contains(items, item))
                        items.Add(item);
            }

            return new TextLayerDependencies(flags, items);
        }

        private static TextLayerDependencies From(
            EnumTextLayerDependencySource source,
            params EnumMapRenderEvents[] eventFlags)
        {
            if (eventFlags == null || eventFlags.Length == 0)
                return None;

            var items = new List<TextLayerDependency>(eventFlags.Length);
            int flags = 0;
            foreach (var eventFlag in eventFlags)
            {
                flags |= (int)eventFlag;

                var dependency = new TextLayerDependency(eventFlag, source);
                if (!Contains(items, dependency))
                    items.Add(dependency);
            }

            return new TextLayerDependencies(flags, items);
        }

        private static bool Contains(List<TextLayerDependency> items, TextLayerDependency target)
        {
            foreach (var item in items)
                if (item.EventFlag == target.EventFlag && item.Source == target.Source)
                    return true;

            return false;
        }
    }
}
