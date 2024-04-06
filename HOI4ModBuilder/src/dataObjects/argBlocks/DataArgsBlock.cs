using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.dataObjects.argBlocks
{
    class DataArgsBlock : IParadoxRead
    {
        private bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave;
        }

        //Список с блокам на том же уровне, что и этот блок (его соседи, а не данные внутри)
        public List<DataArgsBlock> CurrentLevelDataBlocks { get; set; }

        public string SpecialName { private get; set; }
        public InfoArgsBlock InfoArgsBlock { get; set; }

        //Список с блоками внутри данного блока
        public List<DataArgsBlock> innerArgsBlocks = new List<DataArgsBlock>(0);

        private string _demiliter;
        public string Demiliter { get => _demiliter; set => Utils.Setter(ref _demiliter, ref value, ref _needToSave); }
        public EnumNewArgsBlockValueType ValueType { get; set; }

        private object _value;
        public object Value { get => _value; set => Utils.Setter(ref _value, ref value, ref _needToSave); }
        public void SetSilentValue(object value) => _value = value;

        public byte CurrentMandatoryInnerArgsBlocksCount { get; set; }
        public bool IsUniversalParameter { get; set; }


        public bool HasInfoBlock => InfoArgsBlock != null;
        public bool CanHaveMandatoryBlocks => HasInfoBlock && InfoArgsBlock.CanHaveMandatoryBlocks;
        public bool HasAllowedBlocks => HasInfoBlock && InfoArgsBlock.HasAllowedBlocks;
        public bool CanHaveUniversalParams => HasInfoBlock && InfoArgsBlock.CanHaveUniversalParams;
        public bool TryGetMandatoryBlock(string name, ref InfoArgsBlock mandatoryBlock)
            => HasInfoBlock && InfoArgsBlock.TryGetMandatoryBlock(name, ref mandatoryBlock);
        public bool TryGetAllowedBlock(string name, ref InfoArgsBlock mandatoryBlock)
            => HasInfoBlock && InfoArgsBlock.TryGetMandatoryBlock(name, ref mandatoryBlock);



        public virtual void Save(StringBuilder sb, string outTab, string tab)
        {
            if (innerArgsBlocks.Count == 0 && Value != null && (InfoArgsBlock == null || Value != InfoArgsBlock.DefaultValue || InfoArgsBlock.DefaultValueType == EnumNewArgsBlockValueType.NONE))
            {
                if (CurrentLevelDataBlocks.Count == 1) sb.Append(GetName()).Append(' ').Append(Demiliter).Append(' ').Append(GetFormattedValue());
                else sb.Append(outTab).Append(GetName()).Append(' ').Append(Demiliter).Append(' ').Append(GetFormattedValue()).Append(Constants.NEW_LINE);
            }
            else if (innerArgsBlocks.Count == 1)
            {
                if (innerArgsBlocks[0].innerArgsBlocks.Count > 0)
                {
                    sb.Append(outTab).Append(GetName()).Append(" = {").Append(Constants.NEW_LINE);
                    innerArgsBlocks[0].Save(sb, outTab + tab, tab);
                    sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
                }
                else
                {
                    sb.Append(outTab).Append(GetName()).Append(" = { ");
                    innerArgsBlocks[0].Save(sb, outTab + tab, tab);
                    sb.Append(" }").Append(Constants.NEW_LINE);
                }
            }
            else if (innerArgsBlocks.Count > 1)
            {
                sb.Append(outTab).Append(GetName()).Append(" = {").Append(Constants.NEW_LINE);
                foreach (var block in innerArgsBlocks) block.Save(sb, outTab + tab, tab);
                sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
            }
        }

        private string GetFormattedValue()
        {
            switch (ValueType)
            {
                case EnumNewArgsBlockValueType.BOOLEAN:
                    return (bool)Value ? "yes" : "no";
                case EnumNewArgsBlockValueType.FLOAT:
                case EnumNewArgsBlockValueType.DECIMAL:
                    return ("" + Value).Replace(',', '.');
                case EnumNewArgsBlockValueType.NAME:
                    return "\"" + Value + "\"";
                default:
                    return "" + Value;
            }
        }

        public void CheckAfterParsing()
        {
            if (InfoArgsBlock == null) return;

            if (InfoArgsBlock.MandatoryInnerArgsBlocks != null && InfoArgsBlock.MandatoryInnerArgsBlocks.Count != CurrentMandatoryInnerArgsBlocksCount && Value == null && (InfoArgsBlock.AllowedUniversalParamsInfo == null || !InfoArgsBlock.AllowedUniversalParamsInfo.IgnoreMandatoryInnerBlocksCheck))
                //TODO Выводить, каких блоков не хватает
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_DONT_HAVE_ALL_MANDATORY_BLOCKS,
                    new Dictionary<string, string> { { "{blockName}", GetName() } }
                ));

            if (InfoArgsBlock.AllowedUniversalParamsInfo != null)
            {
                var universalParamsInfo = InfoArgsBlock.AllowedUniversalParamsInfo;
                short universalParametersCount = 0;
                short commonParametersCount = 0;

                foreach (var innerBlock in innerArgsBlocks)
                {
                    if (innerBlock.IsUniversalParameter) universalParametersCount++;
                    else commonParametersCount++;
                }

                if (universalParamsInfo.MaxUniversalParamsCount < universalParametersCount)
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_HAS_MORE_UNIVERSAL_PARAMS_THAN_ALLOWED,
                        new Dictionary<string, string>
                        {
                            { "{blockName}", GetName() },
                            { "{currentCount}", $"{universalParametersCount}" },
                            { "{maxAllowedCount}", $"{universalParamsInfo.MaxUniversalParamsCount}" }
                        }
                    ));

                if (universalParametersCount > 0 && commonParametersCount > 0 && !universalParamsInfo.CanBeMixedWithOutherTypeBlocks)
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_HAS_COMMON_AND_UNIVERSAL_PARAMS_BUT_THIS_NOT_ALLOWED,
                        new Dictionary<string, string>
                        {
                            { "{blockName}", GetName() },
                            { "{cause}", "canBeMixedWithOutherTypeBlocks = false" }
                        }
                    ));
            }
        }

        public string GetName() => SpecialName ?? InfoArgsBlock.Name;

        public virtual void TokenCallback(ParadoxParser parser, string token)
        {
            DataArgsBlocksManager.ParseDataArgsBlock(parser, this, token, CurrentLevelDataBlocks, innerArgsBlocks);
        }
    }

}
