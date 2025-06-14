using HOI4ModBuilder.src.scripts.commands.declarators.vars;
using HOI4ModBuilder.src.scripts.exceptions;
using HOI4ModBuilder.src.scripts.objects.interfaces.basic;
using System.Drawing;

namespace HOI4ModBuilder.src.scripts.objects.primitives
{
    public class ColorObject : IScriptObject, IPrimitiveObject
    {
        public Color Value { get; set; }
        public object GetValue() => Value;

        public ColorObject() { }
        public ColorObject(Color value) { Value = value; }

        public ColorObject(int args)
        {
            Value = Color.FromArgb(args);
        }


        public IScriptObject GetCopy() => new ColorObject(Value);
        public IScriptObject GetEmptyCopy() => new ColorObject();
        public string GetKeyword() => ColorDeclarator.GetKeyword();
        public bool IsSameType(IScriptObject scriptObject) => scriptObject is ColorObject;

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            if (value is ByteObject byteObject)
                Value = Color.FromArgb(byteObject.Value);
            else if (value is CharObject charObject)
                Value = Color.FromArgb(charObject.Value);
            else if (value is IntObject intObject)
                Value = Color.FromArgb(intObject.Value);
            else if (value is FloatObject floatObject)
                Value = Color.FromArgb((int)floatObject.Value);
            else if (value is ColorObject colorObject)
                Value = colorObject.Value;
            else
                throw new InvalidValueTypeScriptException(lineIndex, args, value);
        }
    }
}
