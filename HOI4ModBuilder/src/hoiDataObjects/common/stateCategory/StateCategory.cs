using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Structs;

namespace HOI4ModBuilder.src.hoiDataObjects.common.stateCategory
{
    public class StateCategory : AbstractParseObject
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public string name;

        public int ColorInt { get; private set; }
        public readonly GameList<byte> Color = new GameList<byte>();
        public readonly GameParameter<uint> LocalBuildingsSlots = new GameParameter<uint>();
        public readonly GameDictionary<Building, uint> BuildingsMaxLevel = new GameDictionary<Building, uint>()
            .INIT_SetKeyParseAdapter(token => BuildingManager.GetBuilding(token));

        private static readonly Dictionary<string, Func<object, object>> STATIC_ADAPTER = new Dictionary<string, Func<object, object>>
        {
            { "color", o => ((StateCategory)o).Color },
            { "local_building_slots", o => ((StateCategory)o).LocalBuildingsSlots },
            { "buildings_max_level", o => ((StateCategory)o).BuildingsMaxLevel },
        };
        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => STATIC_ADAPTER;


        public readonly GameList<ScriptBlockParseObject> Modifiers = new GameList<ScriptBlockParseObject>();

        private static readonly Dictionary<string, DynamicGameParameter> DYNAMIC_ADAPTER = new Dictionary<string, DynamicGameParameter>
        {
            { "modifiers", new DynamicGameParameter {
                provider = o => ((StateCategory)o).Modifiers,
                factory = (o, key) => ParserUtils.ScriptBlockFabricProvide((IParentable)o, InfoArgsBlocksManager.GetModifier(key))
            } },
        };
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => DYNAMIC_ADAPTER;

        private static readonly SavePattern SAVE_PATTERN = new SavePattern(new[] { "common" }, "StateCategory")
            .Add(STATIC_ADAPTER.Keys)
            .Add(DYNAMIC_ADAPTER.Keys)
            .Load();
        public override SavePattern GetSavePattern() => SAVE_PATTERN;

        public override IParseObject GetEmptyCopy() => new StateCategory();

        public StateCategory() { }
        public StateCategory(string name) : this()
        {
            this.name = name;
        }

        public override void Validate(LinkedLayer layer)
        {
            base.Validate(layer);

            if (Color.Count != 3)
            {
                Logger.LogWarning(
                    EnumLocKey.WARNING_STATE_CATEGORY_INVALID_COLOR,
                    new Dictionary<string, string>
                    {
                        { "{name}", name },
                        { "{filePath}", GetGameFile()?.FilePath },
                        { "{color}", "{" + string.Join(" ", Color) + "}" }
                    }
                );
            }

            ColorInt = Utils.RgbToInt(Color[0], Color[1], Color[2]);
        }
    }
}
