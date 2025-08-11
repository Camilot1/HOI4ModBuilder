using HOI4ModBuilder.src.scripts.commands.commands;
using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.commands.functions.console;
using HOI4ModBuilder.src.scripts.commands.functions.files;
using HOI4ModBuilder.src.scripts.commands.functions.map.provinces;
using HOI4ModBuilder.src.scripts.commands.functions.map.regions.weather;
using HOI4ModBuilder.src.scripts.commands.functions.map.regions;
using HOI4ModBuilder.src.scripts.commands.functions.map.states;
using HOI4ModBuilder.src.scripts.commands.functions.map;
using HOI4ModBuilder.src.scripts.commands.functions.time;
using HOI4ModBuilder.src.scripts.commands.functions.utils;
using HOI4ModBuilder.src.scripts.commands.functions;
using HOI4ModBuilder.src.scripts.commands.keywords;
using HOI4ModBuilder.src.scripts.commands.methods;
using HOI4ModBuilder.src.scripts.commands.operators.arithmetical;
using HOI4ModBuilder.src.scripts.commands;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using System.Collections.Generic;
using System;
using HOI4ModBuilder.src.utils;
using System.Text;
using System.IO;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.primitives;

namespace HOI4ModBuilder.src.scripts
{
    public class ScriptFabricsRegister
    {
        private static readonly string _scriptsFolderPath = FileManager.AssembleFolderPath(new string[] { "data", "scripts" });

        private static Dictionary<string, Func<ScriptCommand>> _commandsFabrics;
        public static bool TryGetCommandFabric(string commandName, out Func<ScriptCommand> commandFabric)
            => _commandsFabrics.TryGetValue(commandName, out commandFabric);

        private static Dictionary<string, Func<IScriptObject>> _scriptObjectsFabrics;
        public static bool HasScriptObjectFabric(string keyword)
            => _scriptObjectsFabrics.ContainsKey(keyword);

        public static IScriptObject ProduceNewScriptObject(int lineIndex, string[] args, string name, int argIndex)
        {
            if (_scriptObjectsFabrics.TryGetValue(name, out Func<IScriptObject> fabric))
                return fabric();
            else if (name == AnyObject.KEY)
                return new AnyObject();
            else
                throw new InvalidTypeScriptException(lineIndex, args, name, argIndex);
        }

        public static void Init()
        {
            RegisterCommandFabrics();
        }

        private static void RegisterCommandFabrics()
        {
            _commandsFabrics = new Dictionary<string, Func<ScriptCommand>>();
            _scriptObjectsFabrics = new Dictionary<string, Func<IScriptObject>>();

            RegisterFabric(BranchDeclarator.GetKeyword(), () => new BranchDeclarator());

            RegisterFabric(ForDeclarator.GetKeyword(), () => new ForDeclarator());

            RegisterFabric(ListDeclarator.GetKeyword(), () => new ListDeclarator(), () => new ListObject());
            RegisterFabric(MapDeclarator.GetKeyword(), () => new MapDeclarator(), () => new MapObject());

            RegisterFabric(BooleanDeclarator.GetKeyword(), () => new BooleanDeclarator(), () => new BooleanObject());
            RegisterFabric(ByteDeclarator.GetKeyword(), () => new ByteDeclarator(), () => new ByteObject());
            RegisterFabric(CharDeclarator.GetKeyword(), () => new CharDeclarator(), () => new CharObject());
            RegisterFabric(ColorDeclarator.GetKeyword(), () => new ColorDeclarator(), () => new ColorObject());
            RegisterFabric(FloatDeclarator.GetKeyword(), () => new FloatDeclarator(), () => new FloatObject());
            RegisterFabric(IntDeclarator.GetKeyword(), () => new IntDeclarator(), () => new IntObject());
            RegisterFabric(StringDeclarator.GetKeyword(), () => new StringDeclarator(), () => new StringObject());

            //RegisterFabric(BitmapDeclarator.GetKeyword(), () => new BitmapDeclarator(), () => new BitmapObject());
            RegisterFabric(FileDeclarator.GetKeyword(), () => new FileDeclarator(), () => new FileObject());
            RegisterFabric(PairCommentedDeclarator.GetKeyword(), () => new PairCommentedDeclarator(), () => new PairCommentedObject());
            RegisterFabric(PairDeclarator.GetKeyword(), () => new PairDeclarator(), () => new PairObject());
            RegisterFabric(RandomDeclarator.GetKeyword(), () => new RandomDeclarator(), () => new RandomObject());

            RegisterFabric(ConsoleClearFunc.GetKeyword(), () => new ConsoleClearFunc());
            RegisterFabric(ConsoleWriteFunc.GetKeyword(), () => new ConsoleWriteFunc());
            RegisterFabric(ConsoleWriteLnFunc.GetKeyword(), () => new ConsoleWriteLnFunc());

            RegisterFabric(GetModDirectoryPathFunc.GetKeyword(), () => new GetModDirectoryPathFunc());

            RegisterFabric(GetAllProvincesIdsFunc.GetKeyword(), () => new GetAllProvincesIdsFunc());
            RegisterFabric(GetProvinceAdjacentProvincesIdsFunc.GetKeyword(), () => new GetProvinceAdjacentProvincesIdsFunc());
            RegisterFabric(GetProvinceCenterFunc.GetKeyword(), () => new GetProvinceCenterFunc());
            RegisterFabric(GetProvincePixelsCountFunc.GetKeyword(), () => new GetProvincePixelsCountFunc());
            RegisterFabric(GetProvinceRegionIdFunc.GetKeyword(), () => new GetProvinceRegionIdFunc());
            RegisterFabric(GetProvinceStateIdFunc.GetKeyword(), () => new GetProvinceStateIdFunc());
            RegisterFabric(GetProvinceTerrainFunc.GetKeyword(), () => new GetProvinceTerrainFunc());
            RegisterFabric(SetProvinceTerrainFunc.GetKeyword(), () => new SetProvinceTerrainFunc());

            RegisterFabric(GetRegionWeatherPeriodArcticWaterFunc.GetKeyword(), () => new GetRegionWeatherPeriodArcticWaterFunc());
            RegisterFabric(GetRegionWeatherPeriodBetweenFunc.GetKeyword(), () => new GetRegionWeatherPeriodBetweenFunc());
            RegisterFabric(GetRegionWeatherPeriodBlizzardFunc.GetKeyword(), () => new GetRegionWeatherPeriodBlizzardFunc());
            RegisterFabric(GetRegionWeatherPeriodMinSnowLevelFunc.GetKeyword(), () => new GetRegionWeatherPeriodMinSnowLevelFunc());
            RegisterFabric(GetRegionWeatherPeriodMudFunc.GetKeyword(), () => new GetRegionWeatherPeriodMudFunc());
            RegisterFabric(GetRegionWeatherPeriodNoPhenomenonFunc.GetKeyword(), () => new GetRegionWeatherPeriodNoPhenomenonFunc());
            RegisterFabric(GetRegionWeatherPeriodRainHeavyFunc.GetKeyword(), () => new GetRegionWeatherPeriodRainHeavyFunc());
            RegisterFabric(GetRegionWeatherPeriodRainLightFunc.GetKeyword(), () => new GetRegionWeatherPeriodRainLightFunc());
            RegisterFabric(GetRegionWeatherPeriodSandstormFunc.GetKeyword(), () => new GetRegionWeatherPeriodSandstormFunc());
            RegisterFabric(GetRegionWeatherPeriodsCountFunc.GetKeyword(), () => new GetRegionWeatherPeriodsCountFunc());
            RegisterFabric(GetRegionWeatherPeriodSnowFunc.GetKeyword(), () => new GetRegionWeatherPeriodSnowFunc());
            RegisterFabric(GetRegionWeatherPeriodTemperatureFunc.GetKeyword(), () => new GetRegionWeatherPeriodTemperatureFunc());
            RegisterFabric(IsRegionHasWeatherFunc.GetKeyword(), () => new IsRegionHasWeatherFunc());

            RegisterFabric(GetAllRegionsIdsFunc.GetKeyword(), () => new GetAllRegionsIdsFunc());
            RegisterFabric(GetRegionAdjacentProvincesIdsFunc.GetKeyword(), () => new GetRegionAdjacentProvincesIdsFunc());
            RegisterFabric(GetRegionCenterFunc.GetKeyword(), () => new GetRegionCenterFunc());
            RegisterFabric(GetRegionPixelsCountFunc.GetKeyword(), () => new GetRegionPixelsCountFunc());

            RegisterFabric(GetAllStatesIdsFunc.GetKeyword(), () => new GetAllStatesIdsFunc());
            RegisterFabric(GetStateAdjacentProvincesIdsFunc.GetKeyword(), () => new GetStateAdjacentProvincesIdsFunc());
            RegisterFabric(GetStateAdjacentStatesIdsFunc.GetKeyword(), () => new GetStateAdjacentStatesIdsFunc());
            RegisterFabric(GetStateBuildingLevelFunc.GetKeyword(), () => new GetStateBuildingLevelFunc());
            RegisterFabric(GetStateCategoryFunc.GetKeyword(), () => new GetStateCategoryFunc());
            RegisterFabric(GetStateCenterFunc.GetKeyword(), () => new GetStateCenterFunc());
            RegisterFabric(GetStateControllerFunc.GetKeyword(), () => new GetStateControllerFunc());
            RegisterFabric(GetStateOwnerFunc.GetKeyword(), () => new GetStateOwnerFunc());
            RegisterFabric(GetStatePixelsCountFunc.GetKeyword(), () => new GetStatePixelsCountFunc());
            RegisterFabric(GetStateProvincesIdsFunc.GetKeyword(), () => new GetStateProvincesIdsFunc());
            RegisterFabric(GetStatePopulationFunc.GetKeyword(), () => new GetStatePopulationFunc());
            RegisterFabric(GetStateRegionIdFunc.GetKeyword(), () => new GetStateRegionIdFunc());
            RegisterFabric(GetStateScriptBlocksFunc.GetKeyword(), () => new GetStateScriptBlocksFunc());
            RegisterFabric(SetStateBuildingLevelFunc.GetKeyword(), () => new SetStateBuildingLevelFunc());
            RegisterFabric(SetStateScriptBlocksFunc.GetKeyword(), () => new SetStateScriptBlocksFunc());

            RegisterFabric(GetMapSizeFunc.GetKeyword(), () => new GetMapSizeFunc());

            RegisterFabric(GetDaysSinceYearStartFunc.GetKeyword(), () => new GetDaysSinceYearStartFunc());
            RegisterFabric(GetWeeksSinceYearStartFunc.GetKeyword(), () => new GetWeeksSinceYearStartFunc());

            RegisterFabric(InterpolateFunc.GetKeyword(), () => new InterpolateFunc());

            RegisterFabric(BreakKeyword.GetKeyword(), () => new BreakKeyword());
            RegisterFabric(ContinueKeyword.GetKeyword(), () => new ContinueKeyword());

            RegisterFabric(GetInlineCommentMethod.GetKeyword(), () => new GetInlineCommentMethod());
            RegisterFabric(GetPrevCommentsMethod.GetKeyword(), () => new GetPrevCommentsMethod());
            RegisterFabric(SetInlineCommentMethod.GetKeyword(), () => new SetInlineCommentMethod());
            RegisterFabric(SetPrevCommentsMethod.GetKeyword(), () => new SetPrevCommentsMethod());

            RegisterFabric(CeilMethod.GetKeyword(), () => new CeilMethod());
            RegisterFabric(ClampMethod.GetKeyword(), () => new ClampMethod());
            RegisterFabric(FloorMethod.GetKeyword(), () => new FloorMethod());
            RegisterFabric(MaxMethod.GetKeyword(), () => new MaxMethod());
            RegisterFabric(MinMethod.GetKeyword(), () => new MinMethod());
            RegisterFabric(RoundMethod.GetKeyword(), () => new RoundMethod());

            RegisterFabric(AppendMethod.GetKeyword(), () => new AppendMethod());
            RegisterFabric(ClearMethod.GetKeyword(), () => new ClearMethod());
            RegisterFabric(DeleteMethod.GetKeyword(), () => new DeleteMethod());
            RegisterFabric(FormatMethod.GetKeyword(), () => new FormatMethod());
            RegisterFabric(GetKeyMethod.GetKeyword(), () => new GetKeyMethod());
            RegisterFabric(GetKeysMethod.GetKeyword(), () => new GetKeysMethod());
            RegisterFabric(GetMethod.GetKeyword(), () => new GetMethod());
            RegisterFabric(GetSizeMethod.GetKeyword(), () => new GetSizeMethod());
            RegisterFabric(GetValueMethod.GetKeyword(), () => new GetValueMethod());
            RegisterFabric(HasKeyMethod.GetKeyword(), () => new HasKeyMethod());
            RegisterFabric(IsExistsMethod.GetKeyword(), () => new IsExistsMethod());
            RegisterFabric(NextFloatMethod.GetKeyword(), () => new NextFloatMethod());
            RegisterFabric(NextIntMethod.GetKeyword(), () => new NextIntMethod());
            RegisterFabric(PutMethod.GetKeyword(), () => new PutMethod());
            RegisterFabric(ReadMethod.GetKeyword(), () => new ReadMethod());
            RegisterFabric(ReverseMethod.GetKeyword(), () => new ReverseMethod());
            RegisterFabric(SetKeyMethod.GetKeyword(), () => new SetKeyMethod());
            RegisterFabric(SetSeedMethod.GetKeyword(), () => new SetSeedMethod());
            RegisterFabric(SetValueMethod.GetKeyword(), () => new SetValueMethod());
            RegisterFabric(SetSizeMethod.GetKeyword(), () => new SetSizeMethod());
            RegisterFabric(ShuffleMethod.GetKeyword(), () => new ShuffleMethod());
            RegisterFabric(SortMethod.GetKeyword(), () => new SortMethod());
            RegisterFabric(SplitMethod.GetKeyword(), () => new SplitMethod());
            RegisterFabric(SwapMethod.GetKeyword(), () => new SwapMethod());
            RegisterFabric(TrimMethod.GetKeyword(), () => new TrimMethod());
            RegisterFabric(WriteMethod.GetKeyword(), () => new WriteMethod());

            RegisterFabric(AddOperator.GetKeyword(), () => new AddOperator());
            RegisterFabric(DivideOperator.GetKeyword(), () => new DivideOperator());
            RegisterFabric(ModuloOperator.GetKeyword(), () => new ModuloOperator());
            RegisterFabric(MultiplyOperator.GetKeyword(), () => new MultiplyOperator());
            RegisterFabric(SetOperator.GetKeyword(), () => new SetOperator());
            RegisterFabric(SubtractOperator.GetKeyword(), () => new SubtractOperator());
        }

        private static void RegisterFabric(string keyword, Func<ScriptCommand> fabric)
            => RegisterFabric(keyword, fabric, null);

        private static void RegisterFabric(string keyword, Func<ScriptCommand> fabric, Func<IScriptObject> scriptObjectFabric)
        {
            if (_commandsFabrics.ContainsKey(keyword))
                throw new Exception();
            _commandsFabrics[keyword] = fabric;

            if (scriptObjectFabric != null)
            {
                if (_scriptObjectsFabrics.ContainsKey(keyword))
                    throw new Exception();
                _scriptObjectsFabrics[keyword] = scriptObjectFabric;
            }
        }

        public static void SaveDocumentation()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var entry in _commandsFabrics)
            {
                Logger.TryOrCatch(
                    () =>
                    {
                        var scriptObject = entry.Value();

                        sb.Append("Keyword: ").Append(Constants.NEW_LINE)
                            .Append('\t').Append(entry.Key).Append(Constants.NEW_LINE);

                        sb.Append("Command Path:").Append(Constants.NEW_LINE)
                            .Append('\t').Append(scriptObject.GetPath()).Append(Constants.NEW_LINE);

                        if (_scriptObjectsFabrics.TryGetValue(entry.Key, out var scriptObjectFabric))
                        {
                            sb.Append("Implements:").Append(Constants.NEW_LINE)
                                .Append('\t');
                            GetImplements(sb, scriptObjectFabric());
                            sb.Append(Constants.NEW_LINE);
                        }

                        sb.Append("Documentation:").Append(Constants.NEW_LINE);
                        foreach (var line in scriptObject.GetDocumentation())
                            sb.Append('\t').Append(line).Append(Constants.NEW_LINE);

                        sb.Append(Constants.NEW_LINE).Append(Constants.NEW_LINE).Append(Constants.NEW_LINE);
                    },
                    (ex) => Logger.LogExceptionAsError(
                        EnumLocKey.EXCEPTION_DURING_SCRIPT_PARSER_DOCUMENTATION_SAVING,
                        new Dictionary<string, string> { { "{fabric}", entry.Key } },
                        ex
                    )
                );
            }

            Logger.DisplayErrors();

            if (!Directory.Exists(_scriptsFolderPath))
                Directory.CreateDirectory(_scriptsFolderPath);

            File.WriteAllText(_scriptsFolderPath + "_documentation.info", sb.ToString());
        }

        private static void GetImplements(StringBuilder sb, object obj)
        {
            if (obj is IScriptObject) sb.Append("ISCRIPTOBJECT ");

            if (obj is IBitmapObject) sb.Append("IBITMAP ");
            if (obj is ICollectionObject) sb.Append("ICOLLECTION ");
            if (obj is IFileObject) sb.Append("IFILE ");
            if (obj is IFuncObject) sb.Append("IFUNC ");
            if (obj is IListObject) sb.Append("ILIST ");
            if (obj is ILogicalObject) sb.Append("ILOGICAL ");
            if (obj is IMapObject) sb.Append("IMAP ");
            if (obj is INumberObject) sb.Append("INUMBER ");
            if (obj is IPairObject) sb.Append("IPAIR ");
            if (obj is IRandomObject) sb.Append("IRANDOM ");
            if (obj is IRelativeObject) sb.Append("IRELATIVE ");
            if (obj is IStringObject) sb.Append("ISTRING ");

            if (obj is IAddObject) sb.Append("IADD ");
            if (obj is IAddRangeObject) sb.Append("IADDRANGE ");
            if (obj is IAndObject) sb.Append("IAND ");
            if (obj is IAppendObject) sb.Append("IAPPEND ");
            if (obj is IClearObject) sb.Append("ICLEAR ");
            if (obj is ICommentedObject) sb.Append("ICOMMENTED ");
            if (obj is IDivideObject) sb.Append("IDIVIDE ");
            if (obj is IGetKeyObject) sb.Append("IGETKEY ");
            if (obj is IGetObject) sb.Append("IGET ");
            if (obj is IGetSizeObject) sb.Append("IGETSIZE ");
            if (obj is IGetValueObject) sb.Append("IGETVALUE ");
            if (obj is IInsertObject) sb.Append("IINSERT ");
            if (obj is ILoadObject) sb.Append("ILOAD ");
            if (obj is IModuloObject) sb.Append("IMODULO ");
            if (obj is IMultiplyObject) sb.Append("IMULTIPLY ");
            if (obj is INextFloatObject) sb.Append("INEXTFLOAT ");
            if (obj is INextIntObject) sb.Append("INEXTINT ");
            if (obj is INotObject) sb.Append("INOT ");
            if (obj is IOrObject) sb.Append("IOR ");
            if (obj is IPrimitiveObject) sb.Append("IPRIMITIVE ");
            if (obj is IPutObject) sb.Append("IPUT ");
            if (obj is IReadObject) sb.Append("IREAD ");
            if (obj is IRemoveAtObject) sb.Append("IREMOVEAT ");
            if (obj is IRemoveObject) sb.Append("IREMOVE ");
            if (obj is IReverseObject) sb.Append("IREVERSE ");
            if (obj is ISaveObject) sb.Append("ISAVE ");
            if (obj is ISetKeyObject) sb.Append("ISETKEY ");
            if (obj is ISetObject) sb.Append("ISET ");
            if (obj is ISetSeedObject) sb.Append("ISETSEED ");
            if (obj is ISetSizeObject) sb.Append("ISETSIZE ");
            if (obj is ISetValueObject) sb.Append("ISETVALUE ");
            if (obj is IShuffleObject) sb.Append("ISHUFFLE ");
            if (obj is ISortObject) sb.Append("ISORT ");
            if (obj is ISplitObject) sb.Append("ISPLIT ");
            if (obj is ISubtractObject) sb.Append("ISUBTRACT ");
            if (obj is ISwapObject) sb.Append("ISWAP ");
            if (obj is ITrimObject) sb.Append("ITRIM ");
            if (obj is IWriteObject) sb.Append("IWRITE ");
            if (obj is IXorObject) sb.Append("IXOR ");
        }
    }
}
