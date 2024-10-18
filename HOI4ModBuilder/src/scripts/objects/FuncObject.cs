using HOI4ModBuilder.src.scripts.commands.declarators.funcs;
using HOI4ModBuilder.src.scripts.objects.interfaces;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class FuncObject : IFuncObject
    {
        public void Call(IScriptObject[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public IScriptObject GetCopy() => new FuncObject();
        public IScriptObject GetEmptyCopy() => new FuncObject();
        public string GetKeyword() => FuncDeclarator.GetKeyword();

        public bool[] GetOutParams()
        {
            throw new System.NotImplementedException();
        }

        public object GetValue()
        {
            throw new System.NotImplementedException();
        }

        public bool IsSameType(IScriptObject scriptObject) => scriptObject is FuncObject;
        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct FuncParameter
    {
        public bool IsMarkedAsOut { get; private set; }
        public IScriptObject Value { get; private set; }
        public string Name { get; private set; }

        public FuncParameter(bool isMarkedAsOut, IScriptObject value, string name)
        {
            IsMarkedAsOut = isMarkedAsOut;
            Value = value;
            Name = name;
        }
    }
}
