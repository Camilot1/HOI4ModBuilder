using HOI4ModBuilder.hoiDataObjects;
using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.adjacencies;
using HOI4ModBuilder.src.hoiDataObjects.map.railways;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.managers.statistics
{
    public class StatisticsManager
    {
        private enum StatisticsSort
        {
            PROVINCES_BORDERS,
            PROVINCES,
            STATES,
            STATE_CATEGORIES,
            REGIONS,

            CONTINENTS,
            TERRAINS,
            PROVINCE_TERRAINS,
            NAVAL_TERRAINS,

            ADJACENCIES,
            SUPPLY,

            BUILDINGS,
            RESOURCES,
            MANPOWER,
        }
        public static StatistictsData Collect(StatisticsFilters filters)
        {
            /*Func<StatisticsDataType>[] collectors = null;
            collectors = new Func<StatisticsDataType>[] {
                    () => new StatisticsDataType(0, EnumLocKey.PROVINCES_BORDERS, ProvinceBorderManager.ProvinceBorderCount, ushort.MaxValue),
                    () => new StatisticsDataType(1, EnumLocKey.PROVINCES, ProvinceManager.ProvincesCount, null),
                    () => new StatisticsDataType(2, EnumLocKey.STATES, StateManager.GetStates().Count, null),
                    () => new StatisticsDataType(3, EnumLocKey.STRATEGIC_REGIONS, StrategicRegionManager.GetRegions().Count, null),
                    () => new StatisticsDataType(4, EnumLocKey.TERRAINS, TerrainManager.GetAllTerrainKeys().Count, null),
                    () => new StatisticsDataType(5, EnumLocKey.CONTINENTS, ContinentManager.GetContinents().Count, null),
                    () => new StatisticsDataType(6, EnumLocKey.BUILDINGS, BuildingManager.GetBuildings().Count, null),
                    () => new StatisticsDataType(7, EnumLocKey.RESOURCES, ResourceManager.GetResourcesTags().Count, null),
                    () => new StatisticsDataType(8, EnumLocKey.RAILWAYS, SupplyManager.Railways.Count, null),
                    () => new StatisticsDataType(9, EnumLocKey.SUPPLY_HUBS, SupplyManager.SupplyNodes.Count, null),
                    () => new StatisticsDataType(10, EnumLocKey.ADJACENCIES, AdjacenciesManager.GetAdjacencies().Count, null),
                };*/


            var collectors = new Func<StatistictsData>[] {
                () => CollectProvinces(0, filters),
                () => CollectStates(1, filters),
                () => CollectRegions(2, filters),
            };

            return AssembleParallel(collectors);
        }

        private static StatistictsData AssembleParallel(Func<StatistictsData>[] collectors)
        {
            if (collectors == null)
                throw new ArgumentNullException(nameof(collectors));

            var tasks = collectors.Select(c =>
                Task.Factory.StartNew(() => c())
            ).ToArray();

            Task.WaitAll(tasks);

            var results = tasks
                .Where(t => t.Status == TaskStatus.RanToCompletion)
                .Select(t => t.Result);

            var statistics = new StatistictsData(0, true);
            foreach (var result in results)
                foreach (var entry in result.types)
                    statistics.types.Add(entry.Key, entry.Value);

            return statistics;
        }

        private static Func<Province, bool> GetProvinceChecker(StatisticsFilters filters)
        {
            if (filters == null || !filters.HasAnyFilters())
                return p => true;

            return p =>
            {
                var state = p.State;
                if (state != null && filters.CheckStateID(state.Id.GetValue()))
                    return true;

                var region = p.Region;
                if (region != null && filters.CheckRegionID(region.Id))
                    return true;

                if (state != null)
                {
                    if (state.controller != null)
                        return filters.CheckCountryTag(state.controller.Tag);
                    else if (state.owner != null)
                        return filters.CheckCountryTag(state.owner.Tag);
                }

                return false;
            };
        }

        private static Func<State, bool> GetStateChecker(StatisticsFilters filters)
        {
            if (filters == null || !filters.HasAnyFilters())
                return s => true;

            return state =>
            {
                if (filters.CheckStateID(state.Id.GetValue()))
                    return true;

                if (state.controller != null && filters.CheckCountryTag(state.controller.Tag))
                    return true;
                else if (state.owner != null && filters.CheckCountryTag(state.owner.Tag))
                    return true;

                if (filters.HasRegionsIDs())
                {
                    foreach (var province in state.Provinces)
                    {
                        if (province.Region != null && filters.CheckRegionID(province.Region.Id))
                            return true;
                    }
                }

                return false;
            };
        }
        private static Func<StrategicRegion, bool> GetRegionChecker(StatisticsFilters filters)
        {
            if (filters == null || !filters.HasAnyFilters())
                return s => true;

            return region =>
            {
                if (filters.CheckRegionID(region.Id))
                    return true;

                if (filters.HasStatesIDs() || filters.HasCountriesTags())
                {
                    foreach (var province in region.Provinces)
                    {
                        var state = province.State;
                        if (state == null)
                            continue;

                        if (filters.CheckStateID(state.Id.GetValue()))
                            return true;
                        if (state.controller != null && filters.CheckCountryTag(state.controller.Tag))
                            return true;
                        else if (state.owner != null && filters.CheckCountryTag(state.owner.Tag))
                            return true;
                    }
                }
                return false;
            };
        }

        private static StatistictsData CollectProvinces(int sortIndex, StatisticsFilters filters)
        {
            var data = new StatistictsData();

            /** Границы провинций **/
            var dtProvincesBorders = new StatisticsDataType().SetSortIndex((int)StatisticsSort.PROVINCES_BORDERS);
            data.Add(EnumLocKey.PROVINCES_BORDERS, dtProvincesBorders);
            dtProvincesBorders.GetOrCreateInner(EnumLocKey.REGISTERED, ProvinceBorderManager.ProvinceBorderCount, ushort.MaxValue, false);
            dtProvincesBorders.GetOrCreateInner(EnumLocKey.FOUNDED, true);

            /** Смежности **/
            var dtAdjacencies = new StatisticsDataType().SetSortIndex((int)StatisticsSort.ADJACENCIES);
            data.Add(EnumLocKey.ADJACENCIES, dtAdjacencies);
            dtAdjacencies.GetOrCreateInner(EnumLocKey.REGISTERED, AdjacenciesManager.GetAdjacencies().Count, null, false);
            dtAdjacencies.GetOrCreateInner(EnumLocKey.FOUNDED);

            /** Снабжение **/
            var dtSupply = new StatisticsDataType().SetSortIndex((int)StatisticsSort.SUPPLY);
            data.Add(EnumLocKey.SUPPLY, dtSupply);
            // Железные дороги
            dtSupply.GetOrCreateInner(EnumLocKey.RAILWAYS)
                .GetOrCreateInner(EnumLocKey.REGISTERED, SupplyManager.Railways.Count, null, false);
            dtSupply.GetOrCreateInner(EnumLocKey.RAILWAYS)
                .GetOrCreateInner(EnumLocKey.FOUNDED);
            // Узлы снабжения
            dtSupply.GetOrCreateInner(EnumLocKey.SUPPLY_HUBS)
                .GetOrCreateInner(EnumLocKey.REGISTERED, SupplyManager.SupplyNodes.Count, null, false);
            dtSupply.GetOrCreateInner(EnumLocKey.SUPPLY_HUBS)
                .GetOrCreateInner(EnumLocKey.FOUNDED);

            /** Континенты **/
            var dtContinents = new StatisticsDataType().SetSortIndex((int)StatisticsSort.CONTINENTS);
            data.Add(EnumLocKey.CONTINENTS, dtContinents);
            dtContinents.GetOrCreateInner(EnumLocKey.REGISTERED, ContinentManager.GetContinentsCount(), null, false);
            dtContinents.GetOrCreateInner(EnumLocKey.FOUNDED, true);

            /** Местность **/
            var dtTerrains = new StatisticsDataType().SetSortIndex((int)StatisticsSort.TERRAINS);
            data.Add(EnumLocKey.TERRAINS, dtTerrains);
            dtTerrains.GetOrCreateInner(EnumLocKey.REGISTERED, TerrainManager.GetAllTerrainKeys().Count, null, false);

            /** Провинции **/
            var dtProvinces = new StatisticsDataType().SetSortIndex((int)StatisticsSort.PROVINCES);
            data.Add(EnumLocKey.PROVINCES, dtProvinces);
            dtProvinces.GetOrCreateInner(EnumLocKey.REGISTERED, ProvinceManager.GetProvinces().Count, null, false);
            dtProvinces.GetOrCreateInner(EnumLocKey.FOUNDED);

            dtProvinces.GetOrCreateInner(EnumLocKey.IS_COASTAL);
            dtProvinces.GetOrCreateInner(EnumLocKey.PROVINCE_TYPE, true);

            var dtProvinceTerrains = new StatisticsDataType().SetSortIndex((int)StatisticsSort.PROVINCE_TERRAINS);
            data.Add(EnumLocKey.PROVINCES_TERRAINS, dtProvinceTerrains);
            dtProvinceTerrains.GetOrCreateInner(EnumLocKey.REGISTERED, TerrainManager.GetLandTerrainsCount(), null, false);
            dtProvinceTerrains.GetOrCreateInner(EnumLocKey.FOUNDED, true);

            HashSet<ProvinceBorder> usedBorders = new HashSet<ProvinceBorder>(ushort.MaxValue);

            var usedRailways = new HashSet<Railway>(128);
            var usedAdjacencies = new HashSet<Adjacency>(128);

            int sumContinentsCount = 0;
            int sumProvincesTerrainsCount = 0;

            var checkerFunc = GetProvinceChecker(filters);
            ProvinceManager.ForEachProvince(p =>
            {
                if (!checkerFunc(p))
                    return;

                dtProvinces.GetOrCreateInner(EnumLocKey.FOUNDED)
                    .Add(1);

                if (p.SupplyNode != null)
                    dtSupply.GetOrCreateInner(EnumLocKey.SUPPLY_HUBS)
                        .GetOrCreateInner(EnumLocKey.FOUNDED)
                        .Add(1);

                p.ForEachRailway(r => usedRailways.Add(r));

                p.ForEachAdjacency(adj => usedAdjacencies.Add(adj));

                if (usedBorders != null)
                    p.ForEachBorder(b => usedBorders.Add(b));

                dtProvinces.GetOrCreateInner(EnumLocKey.PROVINCE_TYPE)
                    .GetOrCreateInner(p.Type + "")
                    .Add(1);

                var continentText = ContinentManager.GetContinentById(p.ContinentId);
                if (continentText == "")
                    continentText = EnumLocKey.NONE + "";

                dtContinents.GetOrCreateInner(EnumLocKey.FOUNDED)
                    .GetOrCreateInner(continentText)
                    .Add(1);
                sumContinentsCount++;

                if (p.IsCoastal)
                    dtProvinces.GetOrCreateInner(EnumLocKey.IS_COASTAL)
                        .Add(1);

                dtProvinceTerrains.GetOrCreateInner(EnumLocKey.FOUNDED)
                    .GetOrCreateInner(p.Terrain?.name ?? EnumLocKey.NONE + "")
                    .Add(1);
                sumProvincesTerrainsCount++;
            });

            dtSupply.GetOrCreateInner(EnumLocKey.RAILWAYS)
                .GetOrCreateInner(EnumLocKey.FOUNDED)
                .SetCount(usedRailways.Count);

            dtAdjacencies.GetOrCreateInner(EnumLocKey.FOUNDED)
                .SetCount(usedAdjacencies.Count);

            dtProvinceTerrains.GetOrCreateInner(EnumLocKey.FOUNDED, out var PTF)
                .SetCount(PTF.inner.Count)
                .SetSum(sumProvincesTerrainsCount);

            if (usedBorders != null)
                dtProvincesBorders.GetOrCreateInner(EnumLocKey.FOUNDED)
                    .SetCount(usedBorders.Count);

            dtContinents.GetOrCreateInner(EnumLocKey.FOUNDED, out var CF)
                .SetCount(CF.inner.Count)
                .SetSum(sumContinentsCount);

            return data;
        }

        private static StatistictsData CollectStates(int sortIndex, StatisticsFilters filters)
        {
            var data = new StatistictsData();

            /** Постройки **/
            var dtBuildings = new StatisticsDataType().SetSortIndex((int)StatisticsSort.BUILDINGS);
            data.Add(EnumLocKey.BUILDINGS, dtBuildings);

            dtBuildings.GetOrCreateInner(EnumLocKey.REGISTERED, BuildingManager.GetBuildings().Count, null, false);
            dtBuildings.GetOrCreateInner(EnumLocKey.FOUNDED, true);

            /** Ресурсы **/
            var dtResources = new StatisticsDataType().SetSortIndex((int)StatisticsSort.RESOURCES);
            data.Add(EnumLocKey.RESOURCES, dtResources);

            dtResources.GetOrCreateInner(EnumLocKey.REGISTERED, ResourceManager.GetResourcesTags().Count, null, false);
            dtResources.GetOrCreateInner(EnumLocKey.FOUNDED, true);

            /** Категории областей **/
            var dtStateCategories = new StatisticsDataType().SetSortIndex((int)StatisticsSort.STATE_CATEGORIES);
            data.Add(EnumLocKey.STATE_CATEGORIES, dtStateCategories);
            dtStateCategories.GetOrCreateInner(EnumLocKey.REGISTERED, StateCategoryManager.GetStateCategoriesNames().Count, null, false);
            dtStateCategories.GetOrCreateInner(EnumLocKey.FOUNDED, true);

            /** Население **/
            var dtManpower = new StatisticsDataType().SetSortIndex((int)StatisticsSort.MANPOWER);
            data.Add(EnumLocKey.MANPOWER, dtManpower);
            dtManpower.GetOrCreateInner(EnumLocKey.REGISTERED, StateManager.GetSumManpower(), null, false);
            dtManpower.GetOrCreateInner(EnumLocKey.FOUNDED);

            /** Области **/
            var dtStates = new StatisticsDataType().SetSortIndex((int)StatisticsSort.STATES);
            data.Add(EnumLocKey.STATES, dtStates);
            dtStates.GetOrCreateInner(EnumLocKey.REGISTERED, StateManager.GetStates().Count, null, false);
            dtStates.GetOrCreateInner(EnumLocKey.FOUNDED);


            uint sumBuildingsCount = 0;
            uint sumResourcesCount = 0;
            int sumStateCategoriesCount = 0;
            long sumManpowerCount = 0;
            var checkerFunc = GetStateChecker(filters);
            StateManager.ForEachState(s =>
            {
                if (!checkerFunc(s))
                    return;

                dtStates.GetOrCreateInner(EnumLocKey.FOUNDED)
                    .Add(1);

                foreach (var building in s.stateBuildings)
                {
                    dtBuildings.GetOrCreateInner(EnumLocKey.FOUNDED)
                        .GetOrCreateInner(building.Key.Name)
                        .Add((int)building.Value);
                    sumBuildingsCount += building.Value;
                }
                foreach (var provinceBuildings in s.provincesBuildings.Values)
                    foreach (var building in provinceBuildings)
                    {
                        dtBuildings.GetOrCreateInner(EnumLocKey.FOUNDED)
                            .GetOrCreateInner(building.Key.Name)
                            .Add((int)building.Value);
                        sumBuildingsCount += building.Value;
                    }

                foreach (var resource in s.Resources)
                {
                    dtResources.GetOrCreateInner(EnumLocKey.FOUNDED)
                        .GetOrCreateInner(resource.Key.tag)
                        .Add((int)resource.Value);
                    sumResourcesCount += resource.Value;
                }

                sumManpowerCount += s.CurrentManpower;

                dtStateCategories.GetOrCreateInner(EnumLocKey.FOUNDED)
                    .GetOrCreateInner(s.CurrentStateCategory?.name ?? EnumLocKey.NONE + "")
                    .Add(1);
                sumStateCategoriesCount++;
            });

            dtStateCategories.GetOrCreateInner(EnumLocKey.FOUNDED, out var SCF)
                .SetCount(SCF.inner.Count)
                .SetSum(sumStateCategoriesCount);

            dtBuildings.GetOrCreateInner(EnumLocKey.FOUNDED, out var BF)
                .SetCount(BF.inner.Count)
                .SetSum((int)sumBuildingsCount);

            dtResources.GetOrCreateInner(EnumLocKey.FOUNDED, out var RF)
                .SetCount(RF.inner.Count)
                .SetSum((int)sumResourcesCount);

            dtManpower.GetOrCreateInner(EnumLocKey.FOUNDED)
                .SetCount(sumManpowerCount);


            return data;
        }

        private static StatistictsData CollectRegions(int sortIndex, StatisticsFilters filters)
        {
            var data = new StatistictsData();

            /** Регионы **/
            var dtRegions = new StatisticsDataType().SetSortIndex((int)StatisticsSort.REGIONS);
            data.Add(EnumLocKey.STRATEGIC_REGIONS, dtRegions);
            dtRegions.GetOrCreateInner(EnumLocKey.REGISTERED, StrategicRegionManager.GetRegions().Count, null, false);
            dtRegions.GetOrCreateInner(EnumLocKey.FOUNDED);

            /** Местности **/
            var dtNavalTerrains = new StatisticsDataType().SetSortIndex((int)StatisticsSort.NAVAL_TERRAINS);
            data.Add(EnumLocKey.NAVAL_TERRAINS, dtNavalTerrains);
            dtNavalTerrains.GetOrCreateInner(EnumLocKey.REGISTERED, TerrainManager.GetNavalTerrainsCount(), null, false);
            dtNavalTerrains.GetOrCreateInner(EnumLocKey.FOUNDED, true);

            int sumTerrainsCount = 0;

            var checkerFunc = GetRegionChecker(filters);
            StrategicRegionManager.ForEachRegion(r =>
            {
                if (!checkerFunc(r))
                    return;

                dtRegions
                    .GetOrCreateInner(EnumLocKey.FOUNDED)
                    .Add(1);

                dtNavalTerrains.GetOrCreateInner(EnumLocKey.FOUNDED)
                    .GetOrCreateInner(r.Terrain?.name ?? EnumLocKey.NONE + "")
                    .Add(1);
                sumTerrainsCount++;
            });

            dtNavalTerrains.GetOrCreateInner(EnumLocKey.FOUNDED, out var TF)
                .SetCount(TF.inner.Count)
                .SetSum(sumTerrainsCount);

            return data;
        }
    }
}
