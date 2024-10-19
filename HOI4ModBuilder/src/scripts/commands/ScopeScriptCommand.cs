
using HOI4ModBuilder.src.scripts.commands.keywords;
using HOI4ModBuilder.src.scripts.objects;

namespace HOI4ModBuilder.src.scripts.commands
{
    public abstract class ScopeScriptCommand : ScriptCommand
    {
        public abstract EnumVarsScopeType GetEnumVarsScopeType();
        public bool CheckExitScope()
        {
            if (_innerVarsScope == null)
                return false;

            var enumVarsScopeType = GetEnumVarsScopeType();

            if (_innerVarsScope.TryGetValue(ContinueKeyword.GetKeyword(), out var continueVar) &&
                continueVar is BooleanObject continueObj && continueObj.Value)
            {
                if (enumVarsScopeType == EnumVarsScopeType.FOR)
                {
                    continueObj.Value = false;
                }
                return true;
            }

            if (_innerVarsScope.TryGetValue(BreakKeyword.GetKeyword(), out var breakVar) &&
                breakVar is BooleanObject breakObj && breakObj.Value)
            {
                return true;
            }

            return false;
        }
    }
}
