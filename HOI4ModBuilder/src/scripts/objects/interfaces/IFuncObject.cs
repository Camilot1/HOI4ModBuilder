namespace HOI4ModBuilder.src.scripts.objects.interfaces
{
    public interface IFuncObject : IScriptObject
    {
        void Call(IScriptObject[] parameters);
        bool[] GetOutParams();
    }
}
