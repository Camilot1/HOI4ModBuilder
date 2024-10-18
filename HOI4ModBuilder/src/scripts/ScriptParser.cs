using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.scripts.commands;
using HOI4ModBuilder.src.scripts.commands.declarators.funcs;
using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace HOI4ModBuilder.src.scripts
{
    public class ScriptParser
    {
        private static bool _isInited = Init();
        public static void Wake() { }
        public static readonly string TRUE_KEY = "TRUE";
        public static readonly string FALSE_KEY = "FALSE";

        public static bool IsDebug { get; set; }
        public static bool NextStep { get; set; }
        public static bool IsTerminated { get; set; }
        public static Action<ScriptCommand, int, VarsScope> DebugConsumer { set; get; }

        private static bool Init()
        {
            if (_isInited) return true;

            ScriptFabricsRegister.Init();
            ScriptFabricsRegister.SaveDocumentation();

            //ScriptUtils.TestInterpolation();

            return true;
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

                if (!ScriptFabricsRegister.TryGetCommandFabric(args[0], out var commandFabric))
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
            else if (Utils.TryParseFloat(value, out var floatValue))
                return new FloatObject(floatValue);
            else if (value.Equals("TRUE"))
                return new BooleanObject(true);
            else if (value.Equals("FALSE"))
                return new BooleanObject(false);
            else
                return null;
        }

        public static string FormatToString(IScriptObject obj)
        {
            if (obj is IPrimitiveObject)
            {
                var value = obj.GetValue();
                if (value is string)
                    return "\"" + value + "\"";
                else return value.ToString();
            }
            else return obj.ToString();
        }

        public static void ParseFuncDeclaratorArgs(
            string[] lines, ref int index, int indent, string[] args,
            out string funcName, out FuncParameter[] funcParams
        )
        {
            var funcParamsList = new List<FuncParameter>();

            int lineIndex = index;

            if (IsLastArg(args, "("))
            {
                int newIndent = indent + 1;
                if (args.Length != 3)
                    throw new InvalidArgsCountScriptException(lineIndex, args);

                funcName = args[1];

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
                        break;

                    if (innerIndent != newIndent)
                        throw new InvalidMultilineArgsScriptException(lineIndex, callInnerArgs);

                    TryGetArg(callInnerArgs, 0, out string callInnerArg);
                    bool isMarkedAsOut = callInnerArg == "OUT";

                    int typeNameIndex = isMarkedAsOut ? 1 : 0;
                    var typeName = callInnerArgs[typeNameIndex];
                    int varNameIndex = isMarkedAsOut ? 1 : 0;
                    var varName = callInnerArgs[varNameIndex];

                    var scriptObject = ScriptFabricsRegister.ProduceNewScriptObject(index, callInnerArgs, typeName, typeNameIndex);

                    funcParamsList.Add(new FuncParameter(isMarkedAsOut, scriptObject, varName));

                    index++;
                }

                funcParams = funcParamsList.ToArray();
            }
            else throw new InvalidMultilineArgsScriptException(lineIndex, args);
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
                var multilineArgs = new List<string> { args[0] };
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
                        break;

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


                    if (ScriptFabricsRegister.TryGetCommandFabric(callInnerArgs[commandIndex], out var commandFabric))
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
            else if (ScriptFabricsRegister.HasScriptObjectFabric(varName))
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
