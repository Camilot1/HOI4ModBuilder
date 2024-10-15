using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using System;

namespace HOI4ModBuilder.src.scripts.objects
{
    public class RandomObject : IRandomObject
    {
        public Random Value { get; set; }
        public object GetValue() => Value;

        public RandomObject() { Value = new Random(); }
        public RandomObject(int seed) { Value = new Random(seed); }

        public bool IsSameType(IScriptObject scriptObject) => scriptObject is RandomObject;
        public IScriptObject GetEmptyCopy() => new RandomObject();
        public IScriptObject GetCopy() => new RandomObject();
        public string GetKeyword() => RandomDeclarator.GetKeyword();
        public override string ToString() => GetKeyword() + "(" + Value + ")";
        public override bool Equals(object obj)
            => obj is RandomObject @object &&
                   Value == @object.Value;
        public override int GetHashCode() => Value.GetHashCode();

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is RandomObject obj)
                Value = obj.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }

        public void SetSeed(int lineIndex, string[] args, INumberObject value)
        {
            int seed = Convert.ToInt32(value.GetValue());
            Value = new Random(seed);
        }

        public void NextInt(int lineIndex, string[] args, INumberObject maxValue, INumberObject result)
        {
            int max = Convert.ToInt32(maxValue.GetValue());
            result.Set(lineIndex, args, new IntObject(Value.Next(max)));
        }

        public void NextInt(int lineIndex, string[] args, INumberObject minValue, INumberObject maxValue, INumberObject result)
        {
            int min = Convert.ToInt32(minValue.GetValue());
            int max = Convert.ToInt32(maxValue.GetValue());
            result.Set(lineIndex, args, new IntObject(Value.Next(min, max)));
        }

        public void NextFloat(int lineIndex, string[] args, INumberObject result)
        {
            float value = (float)Value.NextDouble();
            result.Set(lineIndex, args, new FloatObject(value));
        }

        public void NextFloat(int lineIndex, string[] args, INumberObject maxValue, INumberObject result)
        {
            var max = Convert.ToDouble(maxValue.GetValue());
            var value = (float)(Value.NextDouble() * max);
            result.Set(lineIndex, args, new FloatObject(value));
        }

        public void NextFloat(int lineIndex, string[] args, INumberObject minValue, INumberObject maxValue, INumberObject result)
        {
            var min = (float)Convert.ToDouble(minValue.GetValue());
            var max = (float)Convert.ToDouble(maxValue.GetValue());
            var value = (float)(Value.NextDouble() * (max - min) + min);
            result.Set(lineIndex, args, new FloatObject(value));
        }
    }
}
