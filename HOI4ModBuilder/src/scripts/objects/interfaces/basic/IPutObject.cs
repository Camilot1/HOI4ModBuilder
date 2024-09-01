namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface IPutObject
    {
        IScriptObject GetKeyType();
        IScriptObject GetValueType();

        void Put(int lineIndex, string[] args, IScriptObject key, IScriptObject value);

        void HasKey(int lineIndex, string[] args, IScriptObject key, BooleanObject result);
        void GetKeys(int lineIndex, string[] args, IScriptObject keys);
        void HasValue(int lineIndex, string[] args, IScriptObject value, BooleanObject result);
        void GetValues(int lineIndex, string[] args, IScriptObject values);
    }
}
