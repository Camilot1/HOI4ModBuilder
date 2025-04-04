using HOI4ModBuilder.src.dataObjects.argBlocks;

namespace HOI4ModBuilder.src.newParser.interfaces
{
    public interface IScriptBlockInfo
    {
        string GetBlockName();
        EnumScope GetInnerScope();
        EnumKeyValueDemiliter[] GetAllowedSpecialDemiliters();
        EnumValueType[] GetAllowedValueTypes();
        bool IsAllowsInlineValue();
        bool IsAllowsBlockValue();

    }
}
