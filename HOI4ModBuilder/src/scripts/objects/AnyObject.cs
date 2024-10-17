namespace HOI4ModBuilder.src.scripts.objects
{
    public class AnyObject : IScriptObject
    {
        public object Value { get; private set; }
        public static readonly string KEY = "ANY";
        public IScriptObject GetCopy() => new AnyObject();
        public IScriptObject GetEmptyCopy() => new AnyObject();

        public string GetKeyword() => KEY;

        public object GetValue() => Value;

        public bool IsSameType(IScriptObject scriptObject) => true;

        public void Set(int lineIndex, string[] args, IScriptObject value)
        {
            Value = value.GetValue();
        }
    }
}
