using HOI4ModBuilder.src.dataObjects.replaceTags;
using HOI4ModBuilder.src.utils.json;
using Newtonsoft.Json;
using System.Collections.Generic;
using static HOI4ModBuilder.src.dataObjects.argBlocks.InfoArgsBlock;

namespace HOI4ModBuilder.src.dataObjects.argBlocks
{
    class InfoArgsBlock
    {
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; private set; }
        [JsonProperty("specificScopes")]
        private EnumScope[] _specificScopes;
        [JsonProperty("functions")]
        [JsonConverter(typeof(EnumArrayToStringConverter<EnumArgsBlockFunctions>))]
        private EnumArgsBlockFunctions[] _functions;

        [JsonProperty("mandatoryInnerBlocks")]
        public Dictionary<string, InfoArgsBlock> MandatoryInnerArgsBlocks { get; private set; }
        [JsonProperty("allowedInnerBlocks")]
        public Dictionary<string, InfoArgsBlock> AllowedInnerArgsBlocks { get; private set; }

        [JsonProperty("allowedUniversalParamsInfo")]
        public UniversalParamsInfo AllowedUniversalParamsInfo { get; private set; }

        [JsonProperty("allowedValueTypes")]
        [JsonConverter(typeof(EnumArrayToStringConverter<EnumNewArgsBlockValueType>))]
        public EnumNewArgsBlockValueType[] AllowedValueTypes { get; private set; }
        [JsonProperty("defaultValueType")]
        public EnumNewArgsBlockValueType DefaultValueType { get; private set; }
        [JsonProperty("defaultValue")]
        public object DefaultValue { get; private set; }
        [JsonProperty("canHaveAnyInnerBlocks")]
        public bool CanHaveAnyInnerBlocks { get; private set; }
        [JsonProperty("allowedSpecialDemiliters")]
        [JsonConverter(typeof(EnumArrayToStringConverter<EnumDemiliter>))]
        public EnumDemiliter[] AllowedSpecialDemiliters { get; private set; }


        public bool CanHaveMandatoryBlocks => MandatoryInnerArgsBlocks != null && MandatoryInnerArgsBlocks.Count > 0;
        public bool HasAllowedBlocks => AllowedInnerArgsBlocks != null;
        public bool CanHaveUniversalParams => AllowedUniversalParamsInfo != null && AllowedUniversalParamsInfo.CanHaveUniversalParams;

        public bool TryGetMandatoryBlock(string name, ref InfoArgsBlock mandatoryBlock)
            => MandatoryInnerArgsBlocks != null && MandatoryInnerArgsBlocks.TryGetValue(name, out mandatoryBlock);
        public bool TryGetAllowedBlock(string name, ref InfoArgsBlock allowedBlock)
            => AllowedInnerArgsBlocks != null && AllowedInnerArgsBlocks.TryGetValue(name, out allowedBlock);


        public InfoArgsBlock() { }

        public InfoArgsBlock(string name)
        {
            Name = name;
        }

        public InfoArgsBlock(string name, EnumScope[] specificScopes)
        {
            Name = name;
            _specificScopes = specificScopes;
        }

        public InfoArgsBlock(string name, Dictionary<string, InfoArgsBlock> mandatoryInnerArgsBlocks)
        {
            Name = name;
            MandatoryInnerArgsBlocks = mandatoryInnerArgsBlocks;
        }

        public InfoArgsBlock(InfoArgsBlock other)
        {
            Name = other.Name;
            _specificScopes = other._specificScopes;
            _functions = other._functions;
            AllowedInnerArgsBlocks = other.AllowedInnerArgsBlocks;
            AllowedValueTypes = other.AllowedValueTypes;
            DefaultValueType = other.DefaultValueType;
            DefaultValue = other.DefaultValue;
        }
        public InfoArgsBlock(string name, EnumScope[] specificScopes, EnumNewArgsBlockValueType[] allowedValueTypes)
        {
            Name = name;
            _specificScopes = specificScopes;
            AllowedValueTypes = allowedValueTypes;
        }

        public InfoArgsBlock(string name, EnumScope[] specificScopes, EnumNewArgsBlockValueType[] allowedValueTypes, EnumNewArgsBlockValueType defaultValueType, object defaultValue)
        {
            Name = name;
            _specificScopes = specificScopes;
            AllowedValueTypes = allowedValueTypes;
            DefaultValueType = defaultValueType;
            DefaultValue = defaultValue;
        }

        public InfoArgsBlock(string name, EnumScope[] specificScopes, EnumArgsBlockFunctions[] functions, Dictionary<string, InfoArgsBlock> allowedInnerArgsBlocks, EnumNewArgsBlockValueType[] allowedValueTypes, EnumNewArgsBlockValueType defaultValueType, object defaultValue)
        {
            Name = name;
            _specificScopes = specificScopes;
            _functions = functions;
            AllowedInnerArgsBlocks = allowedInnerArgsBlocks;
            AllowedValueTypes = allowedValueTypes;
            DefaultValueType = defaultValueType;
            DefaultValue = defaultValue;
        }

        public DataArgsBlock GetNewDataArgsBlockInstance()
        {
            return new DataArgsBlock { InfoArgsBlock = this };
        }

        public List<InfoArgsBlock> GetReplaceTagCopies()
        {
            if (!Name.Contains("<")) return new List<InfoArgsBlock>(0);
            var newNames = ReplaceTagsManager.AssembleNameWithReplaceTags(Name);

            List<InfoArgsBlock> newBlocks = new List<InfoArgsBlock>(newNames.Count);
            foreach (var newName in newNames)
            {
                var newBlock = new InfoArgsBlock(this);
                newBlock.Name = newName;
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

        public enum EnumScope
        {
            NONE,
            VAR,
            CONDITIONAL,
            ALL,
            COUNTRY,
            STATE,
            COMBATANT,
            UNIT_LEADER,
            OPERATIVE,
            DIVISION,
            ARMY,
            NAVAL,
            AIR,
            PEACE,
            POLITICS,
            AI,
            DEFENSIVE,
            AGGRESSIVE,
            WAR_PRODUCTION,
            MILITARY_ADVANCEMENTS,
            MILITARY_EQUIPMENT,
            AUTONOMY,
            GOVERNMENT_IN_EXILE,
            INTELLIGENCE_AGENCY,
            MIO,
            PURCHASE_CONTRACT,
            BUILDING,
            STRATEGIC_REGION,
        }

        public enum EnumArgsBlockFunctions
        {
            NONE,

            MODIFIER_BUILDING_SET_IS_PORT,

            EFFECT_STATE_ADD_CORE_OF,
            EFFECT_STATE_REMOVE_CORE_OF,
            EFFECT_STATE_ADD_CLAIM_BY,
            EFFECT_STATE_REMOVE_CLAIM_BY,
            EFFECT_STATE_SET_PROVINCE_CONTROLLER,
            EFFECT_STATE_ADD_MANPOWER,
            EFFECT_STATE_SET_DEMILITARIZED_ZONE,

            EFFECT_COUNTRY_ADD_MANPOWER
        }

        public enum EnumParameterValueType
        {
            NONE,
            SCOPE,
            STRING,
            BOOLEAN,
            INT,
            FLOAT,
            DECIMAL
        }

        public enum EnumDemiliter
        {
            EQUALS,
            LESS_THAN,
            GREATER_THAN
        }

        public enum EnumComparisonOperator
        {
            EQUALS,
            NOT_EQUALS,
            LESS_THAN,
            LESS_THAN_OR_EQUALS,
            GREATER_THAN,
            GREATER_THAN_OR_EQUALS
        }

    }

    class UniversalParamsInfo
    {
        [JsonProperty("maxUniversalParamsCount")]
        public short MaxUniversalParamsCount { get; private set; }
        [JsonProperty("ignoreMandatoryInnerBlocksCheck")]
        public bool IgnoreMandatoryInnerBlocksCheck { get; private set; }
        [JsonProperty("ignoreAllowedInnerBlocksCheck")]
        public bool IgnoreAllowedInnerBlocksCheck { get; private set; }
        [JsonProperty("canBeMixedWithOutherTypeBlocks")]
        public bool CanBeMixedWithOutherTypeBlocks { get; private set; }
        [JsonProperty("allowedValueTypes")]
        [JsonConverter(typeof(EnumArrayToStringConverter<EnumNewArgsBlockValueType>))]
        public EnumNewArgsBlockValueType[] AllowedValueTypes { get; private set; }
        [JsonProperty("allowedDemiliters")]
        [JsonConverter(typeof(EnumArrayToStringConverter<EnumDemiliter>))]
        public EnumDemiliter[] AllowedDemiliters { get; private set; }

        public bool CanHaveUniversalParams => MaxUniversalParamsCount > 0 && AllowedValueTypes != null && AllowedValueTypes.Length > 0;
    }
}
