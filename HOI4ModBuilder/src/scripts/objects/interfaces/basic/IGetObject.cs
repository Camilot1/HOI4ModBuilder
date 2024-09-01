
namespace HOI4ModBuilder.src.scripts.objects.interfaces.basic
{
    public interface IGetObject
    {
        IScriptObject GetValueType();
        void Get(int lineIndex, string[] args, IScriptObject key, IScriptObject value);
        void HasValue(int lineIndex, string[] args, IScriptObject value, BooleanObject result);
        void GetValues(int lineIndex, string[] args, IScriptObject values);
    }
}
