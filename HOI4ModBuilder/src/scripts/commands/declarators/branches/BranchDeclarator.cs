using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.scripts.commands.declarators
{
    public class BranchDeclarator : ScopeScriptCommand
    {
        private static readonly string _if_keyword = "IF";
        private static readonly string _else_if_keyword = "ELSE_IF";
        private static readonly string _else_keyword = "ELSE";
        public static new string GetKeyword() => _if_keyword;
        public override string GetPath() => "commands.declarators.branches." + _if_keyword;

        private static readonly Dictionary<string, Func<int, string[], IRelativeObject, IRelativeObject, BooleanObject, bool>> _relationFuncs = new Dictionary<string, Func<int, string[], IRelativeObject, IRelativeObject, BooleanObject, bool>>
        {
            { ">", (lineIndex, args, a, b, r) => a.IsGreaterThan(lineIndex, args, b, r) },
            { ">=", (lineIndex, args, a, b, r) => a.IsGreaterThanOrEquals(lineIndex, args, b, r) },
            { "<", (lineIndex, args, a, b, r) => a.IsLowerThan(lineIndex, args, b, r) },
            { "<=", (lineIndex, args, a, b, r) => a.IsLowerThanOrEquals(lineIndex, args, b, r) },
            { "==", (lineIndex, args, a, b, r) => a.IsEquals(lineIndex, args, b, r) },
            { "!=", (lineIndex, args, a, b, r) => a.IsNotEquals(lineIndex, args, b, r) }
        };

        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            $"{_if_keyword} <{BooleanDeclarator.GetKeyword()}:value>",
            "\t<INNER_CODE>",
            $"{_else_if_keyword} <{BooleanDeclarator.GetKeyword()}:value>",
            "\t<INNER_CODE>",
            _else_keyword,
            "\t<INNER_CODE>",
            "======== OR ========",
            $"{_if_keyword} <IRELATIVE:value_a> <RELATIVE_OPERATOR> <IRELATIVE:value_b>",
            "\t<INNER_CODE>",
            $"{_else_if_keyword} <IRELATIVE:value_a> <RELATIVE_OPERATOR> <IRELATIVE:value_a>",
            "\t<INNER_CODE>",
            _else_keyword,
            "\t<INNER_CODE>",
            "======== WHERE ========",
            $"RELATIVE_OPERATORS: {string.Join(", ", _relationFuncs.Keys)}"
        };


        public override ScriptCommand CreateEmptyCopy() => new ForDeclarator();

        private BranchDeclarator _prev;
        private BranchDeclarator _next;

        public override EnumVarsScopeType GetEnumVarsScopeType() => EnumVarsScopeType.BRANCH;

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            Func<bool> checkFunc = ParseCheckFunc(index, varsScope, args);

            index++;
            int innerIndent = indent + 1;
            var innerVarsScope = new VarsScope(varsScope, EnumVarsScopeType.BRANCH);
            var commands = ScriptParser.Parse(lines, ref index, indent + 1, innerVarsScope);

            if (index < lines.Length)
            {
                var line = lines[index];
                var chainArgs = ScriptParser.GetStringArgs(lineIndex, line);
                if (chainArgs.Length > 0)
                {
                    if (chainArgs[0] == _else_if_keyword || chainArgs[1] == _else_keyword)
                    {
                        _next = new BranchDeclarator() { _prev = this };
                        _next.Parse(lines, ref index, innerIndent, varsScope, chainArgs);
                    }
                }
            }

            index--;

            _innerVarsScope = innerVarsScope;
            _action = delegate ()
            {
                if (checkFunc())
                {
                    innerVarsScope.ClearLocalVars();
                    foreach (var command in commands)
                    {
                        command.Execute();
                        if (CheckExitScope())
                            return;
                    }
                }
                else _next?.Execute();
            };
        }

        private Func<bool> ParseCheckFunc(int index, VarsScope varsScope, string[] args)
        {
            Func<bool> checkFunc = null;

            if (args.Length == 1 && args[0] == _else_keyword)
            {
                checkFunc = () => true;
            }
            else if (args.Length == 2)
            {
                if (args[0] == _else_if_keyword)
                {
                    if (_prev == null)
                        throw new InvalidOperationScriptException(index, args, args[0]);
                }
                else if (args[0] != _if_keyword)
                    throw new InvalidOperationScriptException(index, args, args[0]);

                checkFunc = () =>
                {
                    int argIndex = 1;
                    var variable = ScriptParser.ParseValue(varsScope, args[argIndex]);
                    if (variable is BooleanObject booleanObject)
                        return booleanObject.Value;
                    else throw new InvalidValueTypeScriptException(index, args, variable, argIndex);
                };
            }
            else if (args.Length == 4)
            {
                if (args[0] == _else_if_keyword)
                {
                    if (_prev == null)
                        throw new InvalidOperationScriptException(index, args, args[0]);
                }
                else if (args[0] != _if_keyword)
                    throw new InvalidOperationScriptException(index, args, args[0]);

                checkFunc = () =>
                {
                    int argIndexVarA = 1;
                    var varA = ScriptParser.ParseValue(varsScope, args[argIndexVarA]);
                    if (!(varA is IRelativeObject objA))
                        throw new InvalidValueTypeScriptException(index, args, varA, argIndexVarA);

                    var relationFunc = _relationFuncs[args[2]];
                    if (relationFunc == null)
                        throw new InvalidOperationScriptException(index, args, args[2]);

                    int argIndexVarB = 3;
                    var varB = ScriptParser.ParseValue(varsScope, args[argIndexVarB]);
                    if (!(varB is IRelativeObject objB))
                        throw new InvalidValueTypeScriptException(index, args, varB, argIndexVarB);

                    return relationFunc(index, args, objA, objB, new BooleanObject());
                };
            }
            else throw new InvalidArgsCountScriptException(index, args);

            return checkFunc;
        }
    }
}
