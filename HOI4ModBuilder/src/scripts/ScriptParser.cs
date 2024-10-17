using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.scripts.commands;
using HOI4ModBuilder.src.scripts.commands.commands;
using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.commands.functions;
using HOI4ModBuilder.src.scripts.commands.functions.console;
using HOI4ModBuilder.src.scripts.commands.functions.files;
using HOI4ModBuilder.src.scripts.commands.functions.map;
using HOI4ModBuilder.src.scripts.commands.functions.map.provinces;
using HOI4ModBuilder.src.scripts.commands.functions.map.regions;
using HOI4ModBuilder.src.scripts.commands.functions.map.regions.weather;
using HOI4ModBuilder.src.scripts.commands.functions.map.states;
using HOI4ModBuilder.src.scripts.commands.functions.time;
using HOI4ModBuilder.src.scripts.commands.functions.utils;
using HOI4ModBuilder.src.scripts.commands.keywords;
using HOI4ModBuilder.src.scripts.commands.methods;
using HOI4ModBuilder.src.scripts.commands.operators.arithmetical;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.utils;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace HOI4ModBuilder.src.scripts
{
    public class ScriptParser
    {
        private static readonly string _scriptsFolderPath = FileManager.AssembleFolderPath(new string[] { "data", "scripts" });

        private static bool _isInited = Init();
        public static void Wake() { }
        public static readonly string TRUE_KEY = "TRUE";
        public static readonly string FALSE_KEY = "FALSE";

        private static Dictionary<string, Func<ScriptCommand>> _commandsFabrics;
        private static Dictionary<string, Func<IScriptObject>> _scriptObjectsFabrics;

        public static bool IsDebug { get; set; }
        public static bool NextStep { get; set; }
        public static bool IsTerminated { get; set; }
        public static Action<ScriptCommand, int, VarsScope> DebugConsumer { set; get; }

        public static IScriptObject GetScriptObjectEmptyCopy(string name)
        {
            if (_scriptObjectsFabrics.TryGetValue(name, out Func<IScriptObject> fabric))
            {
                return fabric();
            }
            else return null;
        }

        private static bool Init()
        {
            if (_isInited) return true;

            RegisterCommandFabrics();
            SaveDocumentation();

            //ScriptUtils.TestInterpolation();

            return true;
        }


        private static void RegisterCommandFabrics()
        {
            _commandsFabrics = new Dictionary<string, Func<ScriptCommand>>();
            _scriptObjectsFabrics = new Dictionary<string, Func<IScriptObject>>();

            RegisterFabric(BranchDeclarator.GetKeyword(), () => new BranchDeclarator());

            RegisterFabric(ForDeclarator.GetKeyword(), () => new ForDeclarator());

            RegisterFabric(BooleanDeclarator.GetKeyword(), () => new BooleanDeclarator(), () => new BooleanObject());
            RegisterFabric(CharDeclarator.GetKeyword(), () => new CharDeclarator(), () => new CharObject());
            RegisterFabric(FileDeclarator.GetKeyword(), () => new FileDeclarator(), () => new FileObject());
            RegisterFabric(FloatDeclarator.GetKeyword(), () => new FloatDeclarator(), () => new FloatObject());
            RegisterFabric(IntDeclarator.GetKeyword(), () => new IntDeclarator(), () => new IntObject());
            RegisterFabric(ListDeclarator.GetKeyword(), () => new ListDeclarator(), () => new ListObject());
            RegisterFabric(MapDeclarator.GetKeyword(), () => new MapDeclarator(), () => new MapObject());
            RegisterFabric(RandomDeclarator.GetKeyword(), () => new RandomDeclarator(), () => new RandomObject());
            RegisterFabric(StringDeclarator.GetKeyword(), () => new StringDeclarator(), () => new StringObject());

            RegisterFabric(ConsoleClearFunc.GetKeyword(), () => new ConsoleClearFunc());
            RegisterFabric(ConsoleWriteFunc.GetKeyword(), () => new ConsoleWriteFunc());
            RegisterFabric(ConsoleWriteLnFunc.GetKeyword(), () => new ConsoleWriteLnFunc());

            RegisterFabric(GetModDirectoryPathFunc.GetKeyword(), () => new GetModDirectoryPathFunc());

            RegisterFabric(GetAllProvincesIdsFunc.GetKeyword(), () => new GetAllProvincesIdsFunc());
            RegisterFabric(GetProvinceAdjacentProvincesIdsFunc.GetKeyword(), () => new GetProvinceAdjacentProvincesIdsFunc());
            RegisterFabric(GetProvinceCenterFunc.GetKeyword(), () => new GetProvinceCenterFunc());
            RegisterFabric(GetProvincePixelsCountFunc.GetKeyword(), () => new GetProvincePixelsCountFunc());
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
            RegisterFabric(GetStateBuildingLevelFunc.GetKeyword(), () => new GetStateBuildingLevelFunc());
            RegisterFabric(GetStateCenterFunc.GetKeyword(), () => new GetStateCenterFunc());
            RegisterFabric(GetStateControllerFunc.GetKeyword(), () => new GetStateControllerFunc());
            RegisterFabric(GetStateOwnerFunc.GetKeyword(), () => new GetStateOwnerFunc());
            RegisterFabric(GetStatePixelsCountFunc.GetKeyword(), () => new GetStatePixelsCountFunc());
            RegisterFabric(GetStateProvincesIdsFunc.GetKeyword(), () => new GetStateProvincesIdsFunc());
            RegisterFabric(GetStatePopulationFunc.GetKeyword(), () => new GetStatePopulationFunc());
            RegisterFabric(GetStateRegionIdFunc.GetKeyword(), () => new GetStateRegionIdFunc());
            RegisterFabric(SetStateBuildingLevelFunc.GetKeyword(), () => new SetStateBuildingLevelFunc());

            RegisterFabric(GetMapSizeFunc.GetKeyword(), () => new GetMapSizeFunc());

            RegisterFabric(GetDaysSinceYearStartFunc.GetKeyword(), () => new GetDaysSinceYearStartFunc());
            RegisterFabric(GetWeeksSinceYearStartFunc.GetKeyword(), () => new GetWeeksSinceYearStartFunc());

            RegisterFabric(InterpolateFunc.GetKeyword(), () => new InterpolateFunc());

            RegisterFabric(BreakKeyword.GetKeyword(), () => new BreakKeyword());
            RegisterFabric(ContinueKeyword.GetKeyword(), () => new ContinueKeyword());

            RegisterFabric(AppendMethod.GetKeyword(), () => new AppendMethod());
            RegisterFabric(CeilMethod.GetKeyword(), () => new CeilMethod());
            RegisterFabric(ClampMethod.GetKeyword(), () => new ClampMethod());
            RegisterFabric(ClearMethod.GetKeyword(), () => new ClearMethod());
            RegisterFabric(DeleteMethod.GetKeyword(), () => new DeleteMethod());
            RegisterFabric(FloorMethod.GetKeyword(), () => new FloorMethod());
            RegisterFabric(FormatMethod.GetKeyword(), () => new FormatMethod());
            RegisterFabric(GetKeysMethod.GetKeyword(), () => new GetKeysMethod());
            RegisterFabric(GetMethod.GetKeyword(), () => new GetMethod());
            RegisterFabric(GetSizeMethod.GetKeyword(), () => new GetSizeMethod());
            RegisterFabric(HasKeyMethod.GetKeyword(), () => new HasKeyMethod());
            RegisterFabric(IsExistsMethod.GetKeyword(), () => new IsExistsMethod());
            RegisterFabric(MaxMethod.GetKeyword(), () => new MaxMethod());
            RegisterFabric(MinMethod.GetKeyword(), () => new MinMethod());
            RegisterFabric(NextFloatMethod.GetKeyword(), () => new NextFloatMethod());
            RegisterFabric(NextIntMethod.GetKeyword(), () => new NextIntMethod());
            RegisterFabric(PutMethod.GetKeyword(), () => new PutMethod());
            RegisterFabric(ReadMethod.GetKeyword(), () => new ReadMethod());
            RegisterFabric(RoundMethod.GetKeyword(), () => new RoundMethod());
            RegisterFabric(SetSeedMethod.GetKeyword(), () => new SetSeedMethod());
            RegisterFabric(SetSizeMethod.GetKeyword(), () => new SetSizeMethod());
            RegisterFabric(SplitMethod.GetKeyword(), () => new SplitMethod());
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

        private static void SaveDocumentation()
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
                            ScriptUtils.GetImplements(sb, scriptObjectFabric());
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

            File.WriteAllText(_scriptsFolderPath + "_documentation.info", sb.ToString());
        }

        public static IScriptObject ParseValue(VarsScope varsScope, string value, int lineIndex, string[] args, Func<IScriptObject, bool> typeCheck)
        {
            var obj = ParseValue(varsScope, value);
            if (obj == null)
                throw new VariableIsNotDeclaredScriptException(lineIndex, args, value);
            if (!typeCheck(obj))
                throw new InvalidValueTypeScriptException(lineIndex, args, obj);
            return obj;
        }
        public static IScriptObject GetValue(VarsScope varsScope, string name, int lineIndex, string[] args, Func<IScriptObject, bool> typeCheck)
        {
            var obj = varsScope.GetValue(name);
            if (obj == null)
                throw new VariableIsNotDeclaredScriptException(lineIndex, args, name);
            if (!typeCheck(obj))
                throw new InvalidValueTypeScriptException(lineIndex, args, obj);
            return obj;
        }

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

        public static Action Parse(string filePath)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                return Parse(lines);
            }
            else return null;
        }

        public static Action Parse(string[] lines)
        {
            if (lines == null) return null;

            int index = 0;
            int indent = 0;
            var commands = Parse(lines, ref index, indent, new VarsScope(EnumVarsScopeType.MAIN));

            return new Action(() => commands.ForEach(command => command.Execute()));
        }
        public static List<ScriptCommand> Parse(string[] lines, ref int index, int indent, VarsScope innerVarsScope)
        {
            var commands = new List<ScriptCommand>();

            while (index < lines.Length && (indent == GetIndent(lines[index], out bool isEmpty) || isEmpty))
            {
                if (isEmpty)
                {
                    index++;
                    continue;
                }

                string[] args = GetStringArgs(index, lines[index]);

                if (!_commandsFabrics.TryGetValue(args[0], out var commandFabric))
                    throw new UnknownCommandScriptException(index, args, args[0]);

                var newCommand = commandFabric();
                newCommand.Parse(lines, ref index, indent, innerVarsScope, args);
                commands.Add(newCommand);

                index++;
            }

            return commands;
        }

        public static int GetIndent(string line, out bool isEmpty)
        {
            isEmpty = true;
            int counter = 0;
            foreach (char ch in line.ToCharArray())
            {
                if (char.IsWhiteSpace(ch))
                    counter++;
                else if (ch != '#')
                {
                    isEmpty = false;
                    break;
                }
                else break;

            }
            return counter;
        }

        public static string[] GetStringArgs(int lineIndex, string line)
        {
            List<string> args = new List<string>();
            var sb = new StringBuilder();

            bool isParsingString = false;
            bool isParsingChar = false;

            foreach (char ch in line.ToCharArray())
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (isParsingString)
                    {
                        sb.Append(ch);
                    }
                    else if (isParsingChar)
                    {
                        throw new InvalidValueTypeScriptException(lineIndex, new string[] { line }, sb.ToString());
                    }
                    else if (sb.Length != 0)
                    {
                        args.Add(sb.ToString());
                        sb.Length = 0;
                    }
                }
                else if (ch == '"')
                {
                    sb.Append(ch);
                    isParsingString = !isParsingString;
                }
                else if (ch == '\'')
                {
                    sb.Append(ch);
                    isParsingChar = !isParsingChar;
                }
                else if (ch == '#')
                {
                    break;
                }
                else
                {
                    sb.Append(ch);
                }
            }

            if (sb.Length > 0)
            {
                args.Add(sb.ToString());
                sb.Length = 0;
            }

            return args.ToArray();
        }

        public static IScriptObject ParseValue(VarsScope varsScope, string value)
        {
            if (value == null || value.Length == 0)
                return null;

            if (varsScope.TryGetValue(value, out IScriptObject scriptObject))
                return scriptObject;
            else if (value.StartsWith("\"") && value.EndsWith("\""))
                return new StringObject(value.Substring(1, value.Length - 2));
            else if (value.StartsWith("'") && value.EndsWith("'") && value.Length == 3)
                return new CharObject(value[1]);
            else if (int.TryParse(value, out var intValue))
                return new IntObject(intValue);
            else if (float.TryParse(value, out var floatValue))
                return new FloatObject(floatValue);
            else if (value.Equals("TRUE"))
                return new BooleanObject(true);
            else if (value.Equals("FALSE"))
                return new BooleanObject(false);
            else
                return null;
        }

        public static string[] ParseCommandCallArgs(
            Func<string[], bool> checkArgsFunc, bool[] outParams, out Action executeBeforeCall,
            string[] lines, ref int index, int indent, VarsScope varsScope, string[] args
        )
        {
            int lineIndex = index;

            if (IsLastArg(args, "("))
            {
                int newIndent = indent + 1;
                List<string> multilineArgs = new List<string>
                {
                    args[0]
                };
                var executeBeforeCallList = new List<Action>();

                index++;
                while (index < lines.Length)
                {
                    var line = lines[index];
                    int innerIndent = GetIndent(line, out bool isEmpty);
                    var callInnerArgs = GetStringArgs(lineIndex, line);

                    if (isEmpty)
                    {
                        index++;
                        continue;
                    }

                    if (IsLastArg(callInnerArgs, "("))
                        throw new InvalidMultilineArgsScriptException(index, callInnerArgs);

                    if (IsLastArg(callInnerArgs, ")"))
                    {
                        break;
                    }

                    if (innerIndent != newIndent)
                        throw new InvalidMultilineArgsScriptException(lineIndex, callInnerArgs);

                    bool isOutParam = CheckIsOutParam(outParams, multilineArgs.Count - 1);
                    bool hasCallInnerArgs = TryGetArg(callInnerArgs, 0, out string callInnerArg);
                    bool isMarkedAsOut = callInnerArg == "OUT";

                    int commandIndex = isMarkedAsOut ? 1 : 0;
                    int varNameIndex = isMarkedAsOut ? 1 : 0;

                    if (isOutParam && (!hasCallInnerArgs || !isMarkedAsOut))
                        throw new ArgumentMustBeMarkedAsOutScriptException(index, callInnerArgs, commandIndex);
                    else if (!isOutParam && hasCallInnerArgs && isMarkedAsOut)
                        throw new ArgumentMustNotBeMarkedAsOutScriptException(index, callInnerArgs, commandIndex);


                    if (_commandsFabrics.TryGetValue(callInnerArgs[commandIndex], out var commandFabric))
                    {
                        var tempCommand = commandFabric();

                        if (tempCommand is VarDeclarator)
                            varNameIndex += 1;
                        else throw new ArgumentMustBeVarDeclarator(index, callInnerArgs, commandIndex);


                        var newCommandArgs = GetNewArgs(callInnerArgs, commandIndex);

                        int constantIndex = index;
                        executeBeforeCallList.Add(() =>
                        {
                            var newCommand = commandFabric();
                            newCommand.Parse(lines, ref constantIndex, newIndent, varsScope, newCommandArgs);
                            newCommand.Execute();
                        });
                    }
                    multilineArgs.Add(callInnerArgs[varNameIndex]);
                    index++;
                }

                var newArgs = multilineArgs.ToArray();
                if (checkArgsFunc(newArgs))
                {
                    executeBeforeCall = () => executeBeforeCallList.ForEach(a => a());
                    return newArgs;
                }
                else throw new InvalidArgsCountScriptException(lineIndex, args);
            }
            else if (checkArgsFunc(args))
            {
                executeBeforeCall = null;
                return args;
            }
            else throw new InvalidArgsCountScriptException(lineIndex, args);
        }

        public static bool IsLastArg(string[] args, string arg)
            => args != null && args.Length > 0 && args[args.Length - 1] == arg;
        public static bool TryGetArg(string[] args, int index, out string arg)
        {
            if (args != null && args.Length > index)
            {
                arg = args[index];
                return true;
            }
            else
            {
                arg = null;
                return false;
            }
        }
        public static bool CheckIsOutParam(bool[] outParams, int index)
            => outParams != null && outParams.Length > index && outParams[index];
        public static string[] GetNewArgs(string[] args, int startIndex)
        {
            if (args.Length <= startIndex) return new string[0];

            var newArgs = new string[args.Length - startIndex];
            for (int i = 0; i < newArgs.Length; i++)
            {
                newArgs[i] = args[startIndex + i];
            }

            return newArgs;
        }

        private static readonly HashSet<char> _validVarNameChars = new HashSet<char>
        {
            '_', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E',
            'f', 'F', 'g', 'G', 'h', 'H', 'i', 'I', 'j', 'J',
            'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N', 'o', 'O',
            'p', 'P', 'q', 'Q', 'r', 'R', 's', 'S', 't', 'T',
            'u', 'U', 'v', 'V', 'w', 'W', 'x', 'X', 'y', 'Z',
            'z', 'Z'
        };
        public static bool IsValidVarName(string varName)
        {
            if (varName == null || varName.Length == 0)
                return false;
            else if (char.IsDigit(varName[0]))
                return false;
            else if (_scriptObjectsFabrics.ContainsKey(varName))
                return false;
            else
            {
                foreach (char ch in varName.ToCharArray())
                {
                    if (!_validVarNameChars.Contains(ch))
                        return false;
                }
            }

            return true;
        }

        public static void AfterCommand(ScriptCommand command, int lineIndex, VarsScope varsScope)
        {
            if (!IsDebug) return;
            if (IsTerminated)
            {
                IsTerminated = false;
                throw new ProcessTerminatedScriptException(lineIndex);
            }

            while (!NextStep)
            {
                Thread.Sleep(50);
            }
            NextStep = false;

            DebugConsumer?.Invoke(command, lineIndex, varsScope);
        }

        public static string ReplaceSpecialChars(string s)
        {
            return s.Replace("\\t", "\t").Replace("\\n", "\n");
        }
    }
}
