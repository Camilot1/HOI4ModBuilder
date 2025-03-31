using HOI4ModBuilder.src.dataObjects.replaceTags;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.utils.json;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.dataObjects.argBlocks
{
    public class InfoArgsBlock : IScriptBlockInfo
    {
        [JsonProperty("name")] public string Name { get; private set; }
        public string GetBlockName() => Name;
        [JsonProperty("isDisabled")] public bool IsDisabled { get; private set; }
        [JsonProperty("specificScopes")] private EnumScope[] _specificScopes;
        [JsonProperty("innerScope")] public EnumScope InnerScope { get; private set; }
        public EnumScope GetInnerScope() => InnerScope;
        public bool IsAllowsInlineValue() => AllowedValueTypes != null && AllowedValueTypes.Length != 0;
        public bool IsAllowsBlockValue() =>
            CanHaveAnyInnerBlocks ||
            MandatoryInnerArgsBlocks != null && MandatoryInnerArgsBlocks.Count > 0 ||
            AllowedInnerArgsBlocks != null && AllowedInnerArgsBlocks.Count > 0 ||
            AllowedUniversalParamsInfo != null && AllowedUniversalParamsInfo.MaxUniversalParamsCount > 0;

        [JsonConverter(typeof(EnumArrayToStringConverter<EnumArgsBlockFunctions>))]
        [JsonProperty("functions")] private EnumArgsBlockFunctions[] _functions;

        [JsonProperty("mandatoryInnerBlocks")] public Dictionary<string, InfoArgsBlock> MandatoryInnerArgsBlocks { get; private set; }
        [JsonProperty("allowedInnerBlocks")] public Dictionary<string, InfoArgsBlock> AllowedInnerArgsBlocks { get; private set; }

        [JsonProperty("allowedUniversalParamsInfo")] public UniversalParamsInfo AllowedUniversalParamsInfo { get; private set; }

        [JsonConverter(typeof(EnumArrayToStringConverter<EnumValueType>))]
        [JsonProperty("allowedValueTypes")] public EnumValueType[] AllowedValueTypes { get; private set; }
        public EnumValueType[] GetAllowedValueTypes() => AllowedValueTypes;
        [JsonProperty("defaultValueType")] public EnumValueType DefaultValueType { get; private set; }
        [JsonProperty("defaultValue")] public object DefaultValue { get; private set; }
        [JsonProperty("canHaveAnyInnerBlocks")] public bool CanHaveAnyInnerBlocks { get; private set; }

        [JsonConverter(typeof(EnumArrayToStringConverter<EnumKeyValueDemiliter>))]
        [JsonProperty("allowedSpecialDemiliters")] public EnumKeyValueDemiliter[] AllowedSpecialDemiliters { get; private set; }
        public EnumKeyValueDemiliter[] GetAllowedSpecialDemiliters() => AllowedSpecialDemiliters;

        [JsonIgnore]
        public bool CanHaveMandatoryBlocks
            => MandatoryInnerArgsBlocks != null &&
                MandatoryInnerArgsBlocks.Count > 0;
        [JsonIgnore]
        public bool HasAllowedBlocks
            => AllowedInnerArgsBlocks != null;
        [JsonIgnore]
        public bool CanHaveUniversalParams
            => AllowedUniversalParamsInfo != null &&
                AllowedUniversalParamsInfo.CanHaveUniversalParams;

        public bool TryGetMandatoryBlock(string name, ref InfoArgsBlock mandatoryBlock)
            => MandatoryInnerArgsBlocks != null && MandatoryInnerArgsBlocks.TryGetValue(name, out mandatoryBlock);
        public bool TryGetAllowedBlock(string name, ref InfoArgsBlock allowedBlock)
            => AllowedInnerArgsBlocks != null && AllowedInnerArgsBlocks.TryGetValue(name, out allowedBlock);

        public InfoArgsBlock() { }

        public void Init(EnumScope innerScope)
        {
            InnerScope = innerScope;
        }

        public InfoArgsBlock(InfoArgsBlock other)
            : this(other.Name, other.InnerScope, other._specificScopes, other.AllowedValueTypes)
        {
            DefaultValueType = other.DefaultValueType;
            DefaultValue = other.DefaultValue;

            _functions = other._functions;
            AllowedInnerArgsBlocks = other.AllowedInnerArgsBlocks;
        }

        public InfoArgsBlock(string name, EnumValueType[] allowedValueTypes)
            : this()
        {
            Name = name;
            AllowedValueTypes = allowedValueTypes;
        }

        public InfoArgsBlock(string name, EnumScope innerScope, EnumScope[] specificScopes, EnumValueType[] allowedValueTypes)
            : this()
        {
            Name = name;
            InnerScope = innerScope;
            _specificScopes = specificScopes;
            AllowedValueTypes = allowedValueTypes;
        }

        public InfoArgsBlock(string name, EnumScope innerScope, EnumScope[] specificScopes, EnumValueType[] allowedValueTypes, EnumValueType defaultValueType, object defaultValue)
            : this(name, innerScope, specificScopes, allowedValueTypes)
        {
            DefaultValueType = defaultValueType;
            DefaultValue = defaultValue;
        }

        public DataArgsBlock GetNewDataArgsBlockInstance() => new DataArgsBlock { InfoArgsBlock = this };

        public List<InfoArgsBlock> GetReplaceTagCopies()
        {
            if (!Name.Contains("<")) return new List<InfoArgsBlock>(0);
            var newNames = ReplaceTagsManager.AssembleNameWithReplaceTags(Name);

            List<InfoArgsBlock> newBlocks = new List<InfoArgsBlock>(newNames.Count);
            foreach (var newName in newNames)
            {
                var newBlock = new InfoArgsBlock(this)
                {
                    Name = newName
                };
                newBlocks.Add(newBlock);
            }
            return newBlocks;
        }

        public EnumArgsBlockFunctions GetFunctionByScope(EnumScope enumScope)
        {
            if (_specificScopes == null || _functions == null) return EnumArgsBlockFunctions.NONE;

            for (int i = 0; i < _specificScopes.Length; i++)
            {
                if (_specificScopes[i] == enumScope) return _functions[i];
            }

            return EnumArgsBlockFunctions.NONE;
        }
    }

    public class UniversalParamsInfo
    {
        [JsonProperty("maxUniversalParamsCount")] public short MaxUniversalParamsCount { get; private set; }
        [JsonProperty("ignoreMandatoryInnerBlocksCheck")] public bool IgnoreMandatoryInnerBlocksCheck { get; private set; }
        [JsonProperty("ignoreAllowedInnerBlocksCheck")] public bool IgnoreAllowedInnerBlocksCheck { get; private set; }
        [JsonProperty("canBeMixedWithOutherTypeBlocks")] public bool CanBeMixedWithOutherTypeBlocks { get; private set; }

        [JsonConverter(typeof(EnumArrayToStringConverter<EnumValueType>))]
        [JsonProperty("allowedValueTypes")] public EnumValueType[] AllowedValueTypes { get; private set; }

        [JsonConverter(typeof(EnumArrayToStringConverter<EnumKeyValueDemiliter>))]
        [JsonProperty("allowedDemiliters")] public EnumKeyValueDemiliter[] AllowedDemiliters { get; private set; }

        [JsonIgnore]
        public bool CanHaveUniversalParams
            => MaxUniversalParamsCount > 0 && AllowedValueTypes != null && AllowedValueTypes.Length > 0;

    }
}
