using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.buildings
{
    public enum EnumBuildingSlotCategory
    {
        SHARED,
        NON_SHARED,
        PROVINCIAL
    }

    public class BuildingLevelCap : AbstractParseObject
    {
        public EnumBuildingSlotCategory GetSlotCategory()
        {
            if (ProvinceMaxCount.GetValue() > 0)
                return EnumBuildingSlotCategory.PROVINCIAL;
            else if (SharedSlots.GetValue())
                return EnumBuildingSlotCategory.SHARED;
            else
                return EnumBuildingSlotCategory.NON_SHARED;
        }

        public uint GetProvinceMaxCount() => ProvinceMaxCount.GetValue();
        public uint GetStateMaxCount() => StateMaxCount.GetValue() > 0 ? StateMaxCount.GetValue() : 15;

        public readonly GameParameter<bool> SharedSlots = new GameParameter<bool>();
        public readonly GameParameter<uint> StateMaxCount = new GameParameter<uint>();
        public readonly GameParameter<uint> ProvinceMaxCount = new GameParameter<uint>();
        public readonly GameParameter<GameString> GroupBy = new GameParameter<GameString>();
        public readonly GameParameter<GameString> ExclusiveWith = new GameParameter<GameString>();

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "shares_slots", o => ((BuildingLevelCap)o).SharedSlots },
            { "state_max", o => ((BuildingLevelCap)o).StateMaxCount },
            { "province_max", o => ((BuildingLevelCap)o).ProvinceMaxCount },
            { "group_by", o => ((BuildingLevelCap)o).GroupBy },
            { "exclusive_with", o => ((BuildingLevelCap)o).ExclusiveWith },
        };

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;
        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common", "buildings" }, "BuildingLevelCap")
            .Add(STATIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new BuildingLevelCap();
    }
}
