using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class BooleanObject : IScriptObject, IPrimitiveObject, IRelativeObject
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
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public bool IsGreaterThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = (Value ? 1 : 0) > (booleanObject.Value ? 1 : 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = (byte)(Value ? 1 : 0) > byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = (char)(Value ? 1 : 0) > charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = (Value ? 1 : 0) > intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = (float)(Value ? 1 : 0) > floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsGreaterThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = (Value ? 1 : 0) >= (booleanObject.Value ? 1 : 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = (byte)(Value ? 1 : 0) >= byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = (char)(Value ? 1 : 0) >= charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = (Value ? 1 : 0) >= intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = (Value ? 1 : 0) >= floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsLowerThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = (Value ? 1 : 0) < (booleanObject.Value ? 1 : 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = (byte)(Value ? 1 : 0) < byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = (char)(Value ? 1 : 0) < charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = (Value ? 1 : 0) < intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = (Value ? 1 : 0) < floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsLowerThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = (Value ? 1 : 0) <= (booleanObject.Value ? 1 : 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = (byte)(Value ? 1 : 0) <= byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = (char)(Value ? 1 : 0) <= charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = (Value ? 1 : 0) <= intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = (Value ? 1 : 0) <= floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = Value == booleanObject.Value;
            else if (relativeObject is ByteObject byteObject)
                result.Value = (byte)(Value ? 1 : 0) == byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = (char)(Value ? 1 : 0) == charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = (Value ? 1 : 0) == intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = (Value ? 1 : 0) == floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsNotEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = Value != booleanObject.Value;
            else if (relativeObject is ByteObject byteObject)
                result.Value = (byte)(Value ? 1 : 0) != byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = (char)(Value ? 1 : 0) != charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = (Value ? 1 : 0) != intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = (Value ? 1 : 0) != floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }
    }
}
