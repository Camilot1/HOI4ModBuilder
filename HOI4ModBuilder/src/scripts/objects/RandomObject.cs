using System;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class RandomObject : IScriptObject
    {
        public Random Value { get; set; }
        public object GetValue() => Value;

        public RandomObject() { Value = new Random(); }
        public RandomObject(int seed) { Value = new Random(seed); }

        public bool IsSameType(IScriptObject scriptObject) => scriptObject is RandomObject;
        public IScriptObject GetEmptyCopy() => new RandomObject();
        public IScriptObject GetCopy() => new RandomObject();
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
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }
    }
}
