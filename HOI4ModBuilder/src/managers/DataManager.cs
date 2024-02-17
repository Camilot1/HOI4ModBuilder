using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.src;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.dataObjects.replaceTags;
using HOI4ModBuilder.src.hoiDataObjects.common.bookmarks;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.ideologies;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.common.units;
using HOI4ModBuilder.src.hoiDataObjects.common.units.divisionsNames;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using System;
using System.Diagnostics;

namespace HOI4ModBuilder.managers
{
    class DataManager
    {
        public static DateTime[] currentDateStamp;

        public static void Load(Settings settings)
        {
            var stopwatch = Stopwatch.StartNew();
            currentDateStamp = null;

            Tuple<EnumLocKey, Action>[] actions =
            {
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_REPLACE_TAGS, () => ReplaceTagsManager.Load(settings)),

                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_INFO_ARGS_BLOCKS, () => InfoArgsBlocksManager.Load(settings)),

                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_BOOKMARKS, () => BookmarkManager.Load(settings)),

                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_TERRAINS, () => TerrainManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_CONTINENTS, () => ContinentManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_BUILDINGS, () => BuildingManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_COUNTRIES, () => CountryManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_RESOURCES, () => ResourceManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_STATE_CATEGORIES, () => StateCategoryManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_IDEOLOGIES, () => IdeologyManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_PROVINCES_DEFINITION, () => ProvinceManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_STATES, () => StateManager.Load(settings)),
                new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_REGIONS, () => StrategicRegionManager.Load(settings)),
                //new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_UNITS, () => SubUnitManager.Load(settings)),
                //new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_DIVISION_NAMES_GROUPS, () => DivisionNamesGroupManager.Load(settings)),
                //new Tuple<EnumLocKey, Action>(EnumLocKey.MAP_TAB_PROGRESSBAR_LOADING_OOBS, () => OOBManager.Load(settings)),
            };

            MainForm.ExecuteActions(actions);

            stopwatch.Stop();
            Console.WriteLine("Загрузка DataManager = " + stopwatch.ElapsedMilliseconds + " ms.");
        }

    }
}
