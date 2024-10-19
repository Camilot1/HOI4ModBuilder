using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.commands.declarators
{
    public class ForDeclarator : ScopeScriptCommand
    {
        private static readonly string _for_keyword = "FOR";
        private static readonly string _in_keyword = "IN";
        private static readonly string _range_keyword = "RANGE";

        private static readonly int MIN_PARAMETERS = 4;
        private static readonly int MAX_RANGE_PARAMETERS = 3;
        public static new string GetKeyword() => _for_keyword;
        public override string GetPath() => "commands.declarators.vars.cycles." + _for_keyword;
        public override string[] GetDocumentation() => _documentation;
        private static readonly string[] _documentation = new string[]
        {
            _for_keyword + " <iterator_name> IN <ILIST:values>",
            "\t<INNER_CODE>",
            "======== OR ========",
            _for_keyword + " <iterator_name> IN RANGE [optional]<INUMBER:start> <INUMBER:end> [optional]<INUMBER:step>",
            "\t<INNER_CODE>",
        };
        public override ScriptCommand CreateEmptyCopy() => new ForDeclarator();

        public override EnumVarsScopeType GetEnumVarsScopeType() => EnumVarsScopeType.FOR;

        public override void Parse(string[] lines, ref int index, int indent, VarsScope varsScope, string[] args)
        {
            lineIndex = index;
            if (args.Length < MIN_PARAMETERS)
                throw new InvalidArgsCountScriptException(lineIndex, args);

            if (args[2] != _in_keyword)
                throw new InvalidCommandArgsScriptException(lineIndex, args, args[2], 2);

            bool hasRangeKeyword = false;
            if (args[3] == _range_keyword)
            {
                hasRangeKeyword = true;
                if (args.Length == MIN_PARAMETERS || args.Length > MIN_PARAMETERS + MAX_RANGE_PARAMETERS)
                    throw new InvalidArgsCountScriptException(lineIndex, args);
            }
            else if (args.Length > MIN_PARAMETERS)
                throw new InvalidArgsCountScriptException(lineIndex, args);

            index++;
            var innerVarsScope = new VarsScope(varsScope, EnumVarsScopeType.FOR);
            var commands = ScriptParser.Parse(lines, ref index, indent + 1, innerVarsScope);

            index--;

            _innerVarsScope = innerVarsScope;
            _action = delegate ()
            {
                int argIndexIterator = 1;
                var iterator = args[argIndexIterator];
                if (varsScope.HasLocalVar(iterator))
                    throw new VariableIsAlreadyDeclaredScriptException(lineIndex, args, iterator, argIndexIterator);

                IListObject iterationList = null;

                if (hasRangeKeyword)
                {
                    var rangeParams = new INumberObject[args.Length - MIN_PARAMETERS];
                    for (int i = MIN_PARAMETERS; i < args.Length; i++)
                    {
                        var rangeParam = ScriptParser.ParseValue(varsScope, args[i]);
                        if (!(rangeParam is INumberObject rangeNumberParam))
                            throw new InvalidValueTypeScriptException(lineIndex, args, i);

                        rangeParams[i - MIN_PARAMETERS] = rangeNumberParam;
                    }

                    INumberObject startObj = null, endObj = null, stepObj = null;
                    if (rangeParams.Length == 1)
                    {
                        startObj = (INumberObject)rangeParams[0].GetEmptyCopy();
                        endObj = (INumberObject)rangeParams[0].GetCopy();
                        stepObj = (INumberObject)rangeParams[0].GetEmptyCopy();

                        if (endObj.IsLowerThan(lineIndex, args, new IntObject(), new BooleanObject()))
                            stepObj.Set(lineIndex, args, new IntObject(-1));
                        else
                            stepObj.Set(lineIndex, args, new IntObject(1));
                    }
                    else if (rangeParams.Length == 2)
                    {
                        startObj = (INumberObject)rangeParams[0].GetCopy();
                        endObj = (INumberObject)rangeParams[1].GetCopy();
                        stepObj = (INumberObject)rangeParams[0].GetEmptyCopy();

                        if (endObj.IsLowerThan(lineIndex, args, startObj, new BooleanObject()))
                            stepObj.Set(lineIndex, args, new IntObject(-1));
                        else
                            stepObj.Set(lineIndex, args, new IntObject(1));
                    }
                    else if (rangeParams.Length == 3)
                    {
                        startObj = (INumberObject)rangeParams[0].GetCopy();
                        endObj = (INumberObject)rangeParams[1].GetCopy();
                        stepObj = (INumberObject)rangeParams[2].GetCopy();
                    }
                    else throw new InvalidArgsCountScriptException(lineIndex, args);

                    iterationList = AssembleRange(lineIndex, args, startObj, endObj, stepObj);
                }
                else
                {
                    int argIndexList = 3;
                    var tempObj = ScriptParser.ParseValue(varsScope, args[argIndexList]);
                    if (!(tempObj is IListObject listObj))
                        throw new InvalidValueTypeScriptException(lineIndex, args, iterationList, argIndexList);
                    iterationList = listObj;
                }


                iterationList.ForEach(iteratorValue =>
                {
                    if (CheckExitScope())
                        return;

                    innerVarsScope.ClearLocalVars();
                    innerVarsScope.PutLocalVariable(iterator, iteratorValue);

                    foreach (var command in commands)
                    {
                        command.Execute();
                        if (CheckExitScope())
                            break;
                    }
                });
            };
        }

        private IListObject AssembleRange(int lineIndex, string[] args, INumberObject start, INumberObject end, INumberObject step)
        {
            var list = new ListObject(start.GetEmptyCopy());

            var booleanObj = new BooleanObject();

            Func<INumberObject, INumberObject, bool> checkFunc = null;

            bool isInfinite = step.IsEquals(lineIndex, args, new IntObject(), booleanObj);

            var startCopy = (INumberObject)start.GetCopy();
            startCopy.Add(lineIndex, args, step);
            if (start.IsEquals(lineIndex, args, startCopy, booleanObj))
                isInfinite = true;

            if (isInfinite)
                throw new InfiniteCycleScriptException(lineIndex, args, start, end, step);

            bool stepIsPositive = step.IsGreaterThan(lineIndex, args, new IntObject(), booleanObj);

            if (stepIsPositive && start.IsGreaterThan(lineIndex, args, end, booleanObj))
                throw new InfiniteCycleScriptException(lineIndex, args, start, end, step);
            else if (!stepIsPositive && start.IsLowerThan(lineIndex, args, end, booleanObj))
                throw new InfiniteCycleScriptException(lineIndex, args, start, end, step);


            if (stepIsPositive)
                checkFunc = (a, b) => a.IsLowerThan(lineIndex, args, b, booleanObj);
            else
                checkFunc = (a, b) => a.IsGreaterThan(lineIndex, args, b, booleanObj);

            while (checkFunc(start, end))
            {
                list.Add(lineIndex, args, start.GetCopy());
                start.Add(lineIndex, args, step);
            }
            return list;
        }
    }
}
