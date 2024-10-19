using HOI4ModBuilder.src.scripts.objects;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.scripts
{
    public class VarsScope
    {
        public EnumVarsScopeType EnumVarScopeType { get; private set; }
        public VarsScope Prev { get; private set; }
        private Dictionary<string, IScriptObject> _vars = new Dictionary<string, IScriptObject>(0);
        public Dictionary<string, IScriptObject> GetVars() => _vars;
        private Dictionary<string, IScriptObject> _funcs = new Dictionary<string, IScriptObject>(0);
        public Dictionary<string, IScriptObject> GetFuncs() => _funcs;

        public VarsScope(EnumVarsScopeType enumVarScopeType)
        {
            EnumVarScopeType = enumVarScopeType;
        }

        public VarsScope(VarsScope prev, EnumVarsScopeType enumVarScopeType) : this(enumVarScopeType)
        {
            Prev = prev;
        }

        public VarsScope GetSpecificParentScope(Func<EnumVarsScopeType, bool> checkFunc)
        {
            if (checkFunc(EnumVarScopeType))
                return this;
            else if (Prev != null)
                return Prev.GetSpecificParentScope(checkFunc);
            else return null;
        }

        public bool GetSpecificParentScopes(List<VarsScope> stackTrace, Func<EnumVarsScopeType, bool> checkFunc)
        {
            stackTrace.Add(this);

            if (checkFunc(EnumVarScopeType))
                return true;
            else if (Prev != null)
                return Prev.GetSpecificParentScopes(stackTrace, checkFunc);
            else
                return false;
        }

        public bool PutLocalVarInAllSpecificParentScopes(string name, IScriptObject sharedVar, Func<EnumVarsScopeType, bool> checkFunc)
        {
            var stackTrace = new List<VarsScope>();

            bool result = GetSpecificParentScopes(stackTrace, checkFunc);

            if (result)
            {
                foreach (var scope in stackTrace)
                    scope.PutLocalVariable(name, sharedVar);
            }

            return result;
        }

        public bool HasLocalVar(string name) => _vars.ContainsKey(name);
        public bool ClearLocalVar(string name) => _vars.Remove(name);
        public void ClearLocalVars() => _vars.Clear();

        public bool TryGetLocalValue(string name, out IScriptObject value)
        {
            if (_vars.TryGetValue(name, out value))
                return true;
            else
                return false;
        }

        public bool TryGetValue(string name, out IScriptObject value)
        {
            if (_vars.TryGetValue(name, out value))
                return true;
            else if (Prev != null)
                return Prev.TryGetValue(name, out value);
            else
                return false;
        }

        public void UpdateValue(int lineIndex, string[] args, string name, IScriptObject value)
        {
            if (_vars.TryGetValue(name, out var tableValue))
                tableValue.Set(lineIndex, args, value);
            else
                Prev?.UpdateValue(lineIndex, args, name, value);
        }

        public void PutLocalVariable(string name, IScriptObject value)
        {
            _vars[name] = value;
        }

        public IScriptObject GetValue(string name)
        {
            if (_vars.TryGetValue(name, out IScriptObject value))
                return value;
            else if (Prev != null)
                return Prev.GetValue(name);
            else
                return null;
        }

        public bool TryDeclareVar(string name, IScriptObject value)
        {
            /*
            if (HasLocalVar(name))
                return false;
            */

            PutLocalVariable(name, value);
            return true;
        }
    }

    public enum EnumVarsScopeType
    {
        MAIN,
        FUNC,
        FOR,
        BRANCH
    }
}
