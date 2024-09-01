using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class BooleanObject : IScriptObject, IPrimitiveObject
    {
        public bool Value { get; set; }
        public object GetValue() => Value;

        public BooleanObject() { }
        public BooleanObject(bool value) { Value = value; }

        public bool IsSameType(IScriptObject scriptObject) => scriptObject is BooleanObject;
        public IScriptObject GetEmptyCopy() => new BooleanObject();
        public IScriptObject GetCopy() => new BooleanObject(Value);
        public string GetKeyword() => BooleanDeclarator.GetKeyword();
        public override string ToString() => GetKeyword() + "(" + Value + ")";
        public override bool Equals(object obj)
            => obj is BooleanObject @object &&
                   Value == @object.Value;
        public override int GetHashCode() => Value.GetHashCode();

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value = booleanObject.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args);
        }
    }
}
