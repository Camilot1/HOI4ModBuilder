using HOI4ModBuilder.src.scripts.commands.declarators;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HOI4ModBuilder.src.scripts.commands.declarators.vars;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class ByteObject : IScriptObject, INumberObject, IPrimitiveObject
    {
        public byte Value { get; set; }
        public object GetValue() => Value;

        public ByteObject() { }
        public ByteObject(byte value)
        {
            Value = value;
        }

        public bool IsSameType(IScriptObject scriptObject) => scriptObject is ByteObject;
        public IScriptObject GetEmptyCopy() => new ByteObject();
        public IScriptObject GetCopy() => new ByteObject(Value);
        public string GetKeyword() => ByteDeclarator.GetKeyword();
        public override string ToString() => GetKeyword() + "(" + Value + ")";
        public override bool Equals(object obj)
            => obj is ByteObject @object &&
                   Value == @object.Value;
        public override int GetHashCode() => Value.GetHashCode();

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value = (byte)(booleanObject.Value ? 1 : 0);
            else if (value is ByteObject byteObject)
                Value = (byte)byteObject.Value;
            else if (value is CharObject charObject)
                Value = (byte)charObject.Value;
            else if (value is IntObject intObject)
                Value = (byte)intObject.Value;
            else if (value is FloatObject floatObject)
                Value = (byte)floatObject.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void Add(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value = (byte)(Value + (byte)(booleanObject.Value ? 1 : 0));
            else if (value is ByteObject byteObject)
                Value = (byte)(Value + byteObject.Value);
            else if (value is CharObject charObject)
                Value = (byte)(Value + charObject.Value);
            else if (value is IntObject intObject)
                Value = (byte)(Value + intObject.Value);
            else if (value is FloatObject floatObject)
                Value = (byte)(Value + floatObject.Value);
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }
        public void Subtract(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value = (byte)(Value - (byte)(booleanObject.Value ? 1 : 0));
            else if (value is ByteObject byteObject)
                Value = (byte)(Value - byteObject.Value);
            else if (value is CharObject charObject)
                Value = (byte)(Value - charObject.Value);
            else if (value is IntObject intObject)
                Value = (byte)(Value - intObject.Value);
            else if (value is FloatObject floatObject)
                Value = (byte)(Value - floatObject.Value);
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void Multiply(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value = (byte)(Value * (byte)(booleanObject.Value ? 1 : 0));
            else if (value is ByteObject byteObject)
                Value = (byte)(Value * byteObject.Value);
            else if (value is CharObject charObject)
                Value = (byte)(Value * charObject.Value);
            else if (value is IntObject intObject)
                Value = (byte)(Value * intObject.Value);
            else if (value is FloatObject floatObject)
                Value = (byte)(Value * floatObject.Value);
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void Divide(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value = (byte)(Value / (byte)(booleanObject.Value ? 1 : 0));
            else if (value is ByteObject byteObject)
                Value = (byte)(Value / byteObject.Value);
            else if (value is CharObject charObject)
                Value = (byte)(Value / charObject.Value);
            else if (value is IntObject intObject)
                Value = (byte)(Value / intObject.Value);
            else if (value is FloatObject floatObject)
                Value = (byte)(Value / floatObject.Value);
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void Modulo(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is BooleanObject booleanObject)
                Value = (byte)(Value % (byte)(booleanObject.Value ? 1 : 0));
            else if (value is ByteObject byteObject)
                Value = (byte)(Value % byteObject.Value);
            else if (value is CharObject charObject)
                Value = (byte)(Value % charObject.Value);
            else if (value is IntObject intObject)
                Value = (byte)(Value % intObject.Value);
            else if (value is FloatObject floatObject)
                Value = (byte)(Value % floatObject.Value);
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public bool IsGreaterThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value > 1) : (Value > 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = Value > byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = Value > charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value > intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value > floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsGreaterThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value >= 1) : (Value >= 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = Value >= byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = Value >= charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value >= intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value >= floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsLowerThan(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value < 1) : (Value < 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = Value < byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = Value < charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value < intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value < floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsLowerThanOrEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value <= 1) : (Value <= 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = Value <= byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = Value <= charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value <= intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value <= floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value == 1) : (Value == 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = Value == byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = Value == charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value == intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value == floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public bool IsNotEquals(int lineIndex, string[] args, IRelativeObject relativeObject, BooleanObject result)
        {
            if (relativeObject is BooleanObject booleanObject)
                result.Value = booleanObject.Value ? (Value != 1) : (Value != 0);
            else if (relativeObject is ByteObject byteObject)
                result.Value = Value != byteObject.Value;
            else if (relativeObject is CharObject charObject)
                result.Value = Value != charObject.Value;
            else if (relativeObject is IntObject intObject)
                result.Value = Value != intObject.Value;
            else if (relativeObject is FloatObject floatObject)
                result.Value = Value != floatObject.Value;
            else throw new InvalidValueTypeScriptException(lineIndex, args, relativeObject);

            return result.Value;
        }

        public int CompareTo(object obj)
        {
            if (obj is IScriptObject scriptObject)
                return Value.CompareTo(scriptObject.GetValue());
            else return 0;
        }
    }
}
