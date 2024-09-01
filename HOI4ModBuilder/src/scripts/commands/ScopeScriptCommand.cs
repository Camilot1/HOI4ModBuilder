
using HOI4ModBuilder.src.scripts.commands.keywords;
using HOI4ModBuilder.src.scripts.objects;

namespace HOI4ModBuilder.src.scripts.commands
{
    public abstract class ScopeScriptCommand : ScriptCommand
    {
        public abstract EnumVarsScopeType GetEnumVarsScopeType();
        public bool CheckExitScope()
        {
            var enumVarsScopeType = GetEnumVarsScopeType();

            if (_varsScope.TryGetValue(ContinueKeyword.GetKeyword(), out IScriptObject continueVar) &&
                continueVar is BooleanObject continueObj && continueObj.Value)
            {
                if (enumVarsScopeType == EnumVarsScopeType.FOR)
                {
                    continueObj.Value = false;
                }
                return true;
            }

            if (_varsScope.TryGetValue(BreakKeyword.GetKeyword(), out IScriptObject breakVar) &&
                breakVar is BooleanObject breakObj && breakObj.Value)
            {
                return true;
            }

            return false;
        }
    }
}
