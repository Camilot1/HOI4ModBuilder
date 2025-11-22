using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using System;
using System.Collections.Generic;
using System.Linq;
using Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using System.Text;
using HOI4ModBuilder.src.parser;
using HOI4ModBuilder.managers;

namespace HOI4ModBuilder.src.dataObjects
{
    class DataArgsBlocksManager
    {
        private static readonly Dictionary<string, EnumKeyValueDemiliter> demilitersDict = new Dictionary<string, EnumKeyValueDemiliter>
        {
            { "<", EnumKeyValueDemiliter.LESS_THAN },
            { ">", EnumKeyValueDemiliter.GREATER_THAN }
        };

        public static void ParseDataArgsBlocks(ParadoxParser parser, string token, List<DataArgsBlock> innerLevelDataBlocks)
        {
            ParseDataArgsBlock(parser, null, token, innerLevelDataBlocks);
        }
        public static void ParseModifiers(ParadoxParser parser, string token, List<DataArgsBlock> innerLevelDataBlocks)
        {
            ParseModifier(parser, null, token, innerLevelDataBlocks);
        }

        public static void SaveDataArgsBlocks(StringBuilder sb, string outTab, string tab, string name, List<DataArgsBlock> dataArgsBlocks)
        {
            if (dataArgsBlocks.Count == 0)
            {
                return;
            }
            else if (dataArgsBlocks.Count == 1)
            {
                if (dataArgsBlocks[0].innerArgsBlocks.Count > 0)
                {
                    ParadoxUtils.StartBlock(sb, outTab, name);
                    dataArgsBlocks[0].Save(sb, outTab + tab, tab);
                    ParadoxUtils.EndBlock(sb, outTab);
                }
                else
                {
                    ParadoxUtils.StartInlineBlock(sb, outTab, name);
                    sb.Append(' ');
                    dataArgsBlocks[0].Save(sb, outTab + tab, tab);
                    ParadoxUtils.EndBlock(sb, " ");
                }

            }
            else if (dataArgsBlocks.Count > 1)
            {
                ParadoxUtils.StartBlock(sb, outTab, name);
                foreach (var block in dataArgsBlocks) block.Save(sb, outTab + tab, tab);
                ParadoxUtils.EndBlock(sb, outTab);
            }
        }

        public static void ParseModifier(ParadoxParser parser, DataArgsBlock currentDataBlock, string token, List<DataArgsBlock> innerLevelDataBlocks)
        {
            //Если не был получен список для внутренних блоков, то создаём его
            if (innerLevelDataBlocks == null) innerLevelDataBlocks = new List<DataArgsBlock>(0);

            if (TryParseOtherJSONBlocks()) return;
            //Иначе выкидываем ошибку о неизвестном токене
            else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_NOT_ALLOWED_TOKEN,
                        new Dictionary<string, string>
                        {
                            { "{blockName}", currentDataBlock?.GetName() },
                            { "{token}", token }
                        }
                    ));



            bool TryParseOtherJSONBlocks() //Попытаться распарсить блоки, прописанные в .json файлах
            {
                if (InfoArgsBlocksManager.TryGetModifier(token, out var infoArgsBlock))
                {
                    var dataBlock = infoArgsBlock.GetNewDataArgsBlockInstance();
                    dataBlock.CurrentLevelDataBlocks = innerLevelDataBlocks;
                    TryParseBlockValue(parser, dataBlock);
                    dataBlock.CheckAfterParsing();
                    innerLevelDataBlocks.Add(dataBlock);

                    return true;
                }
                else return false;
            }

        }

        public static void ParseDataArgsBlock(ParadoxParser parser, DataArgsBlock currentDataBlock, string token, List<DataArgsBlock> innerLevelDataBlocks)
        {
            //Если не был получен список для внутренних блоков, то создаём его
            if (innerLevelDataBlocks == null) innerLevelDataBlocks = new List<DataArgsBlock>(0);

            if (TryParseInnerBlocks()) return;
            else if (TryParseOtherJSONBlocks()) return;
            else if (TryParseScopeBlocks()) return;
            else if (TryParseUniversalParams()) return;
            //Иначе выкидываем ошибку о неизвестном токене
            else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_NOT_ALLOWED_TOKEN,
                        new Dictionary<string, string>
                        {
                            { "{blockName}", currentDataBlock?.GetName() },
                            { "{token}", token }
                        }
                    ));


            bool TryParseInnerBlocks() //Попытаться распарсить внутренние блоки, прописанные в обязательных и разрешённых блоках
            {
                //Если у блока нет InfoArgsBlock, значит нет смысла пытаться парсить обязательные и разрешённые внутренние блоки, т.к. они у него точно не определены
                if (currentDataBlock == null || !currentDataBlock.HasInfoBlock) return false;

                var infoBlock = currentDataBlock.InfoArgsBlock;
                InfoArgsBlock newInfoBlock = null;

                //Проверяем, есть ли токен в списке обязательных блоков
                if (infoBlock.TryGetMandatoryBlock(token, ref newInfoBlock))
                {
                    var dataBlock = newInfoBlock.GetNewDataArgsBlockInstance();
                    dataBlock.CurrentLevelDataBlocks = innerLevelDataBlocks;
                    if (parser.NextIsBracketed()) parser.Parse(dataBlock);
                    else TryParseBlockValue(parser, dataBlock);
                    dataBlock.CheckAfterParsing();
                    innerLevelDataBlocks.Add(dataBlock);
                    currentDataBlock.CurrentMandatoryInnerArgsBlocksCount++;
                    return true;
                }
                //Проверяем, объявлен ли список разрешённых блоков
                else if (infoBlock.TryGetAllowedBlock(token, ref newInfoBlock))
                {
                    var dataBlock = newInfoBlock.GetNewDataArgsBlockInstance();
                    dataBlock.CurrentLevelDataBlocks = innerLevelDataBlocks;
                    if (parser.NextIsBracketed()) parser.Parse(dataBlock);
                    else TryParseBlockValue(parser, dataBlock);
                    dataBlock.CheckAfterParsing();
                    innerLevelDataBlocks.Add(dataBlock);
                    return true;
                }
                else if (infoBlock.CanHaveAnyInnerBlocks || infoBlock.CanHaveUniversalParams && (!infoBlock.HasAllowedBlocks || infoBlock.AllowedUniversalParamsInfo.IgnoreAllowedInnerBlocksCheck))
                {
                    return false;
                }
                else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_NOT_ALLOWED_TOKEN,
                        new Dictionary<string, string>
                        {
                            { "{blockName}", currentDataBlock?.GetName() },
                            { "{token}", token }
                        }
                    ));
            }

            bool TryParseOtherJSONBlocks() //Попытаться распарсить блоки, прописанные в .json файлах
            {
                if (InfoArgsBlocksManager.TryGetInfoArgsBlock(token, out var infoArgsBlock))
                {
                    var dataBlock = infoArgsBlock.GetNewDataArgsBlockInstance();
                    dataBlock.CurrentLevelDataBlocks = innerLevelDataBlocks;
                    if (parser.NextIsBracketed()) parser.Parse(dataBlock);
                    else TryParseBlockValue(parser, dataBlock);
                    dataBlock.CheckAfterParsing();
                    innerLevelDataBlocks.Add(dataBlock);

                    return true;
                }
                else return false;
            }

            bool TryParseScopeBlocks() //Попытаться распарсить блоки стран, областей
            {
                //Пробуем распарсить тег страны
                if (CountryManager.TryGetCountry(token, out var country))
                {
                    //TODO Разобраться с использованием косметических тегов стран
                    var dataBlock = new DataArgsBlock { SpecialName = country.Tag, CurrentLevelDataBlocks = innerLevelDataBlocks };
                    parser.Parse(dataBlock);
                    dataBlock.CheckAfterParsing();
                    innerLevelDataBlocks.Add(dataBlock);

                    return true;
                }
                //Иначе пробуем распарсить id области
                else if (ushort.TryParse(token, out ushort stateId) && StateManager.ContainsStateIdKey(stateId))
                {
                    var dataBlock = new DataArgsBlock { SpecialName = $"{stateId}", CurrentLevelDataBlocks = innerLevelDataBlocks };
                    parser.Parse(dataBlock);
                    dataBlock.CheckAfterParsing();
                    innerLevelDataBlocks.Add(dataBlock);

                    return true;
                }
                else return false;
            }

            bool TryParseUniversalParams() //Попытаться распарсить внутренние универсальные параметры
            {
                if (currentDataBlock != null && currentDataBlock.InfoArgsBlock != null && currentDataBlock.InfoArgsBlock.CanHaveUniversalParams)
                {
                    var universalParamsInfo = currentDataBlock.InfoArgsBlock.AllowedUniversalParamsInfo;
                    if (parser.NextIsBracketed())
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_UNIVERSAL_PARAM_CANT_BE_A_BLOCK,
                            new Dictionary<string, string>
                            {
                            { "{blockName}", currentDataBlock?.GetName() },
                            { "{token}", token }
                            }
                        ));

                    var dataBlock = new DataArgsBlock { SpecialName = token, CurrentLevelDataBlocks = innerLevelDataBlocks, IsUniversalParameter = true };
                    TryParseUniversalParameterValue(parser, dataBlock, universalParamsInfo);
                    dataBlock.CheckAfterParsing();
                    innerLevelDataBlocks.Add(dataBlock);

                    return true;
                }
                else return false;
            }
        }

        private static void Parse(ParadoxParser parser, DataArgsBlock dataBlock, EnumValueType[] allowedTypes, EnumKeyValueDemiliter[] allowedDemiliters)
        {
            string value = parser.ReadString();

            if (demilitersDict.TryGetValue(value, out var enumDemiliter))
            {
                if (allowedDemiliters == null || !allowedDemiliters.Contains(enumDemiliter))
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_PARAM_HAS_UNSUPPORTED_DEMILITER,
                        new Dictionary<string, string>
                        {
                            { "{name}", dataBlock?.GetName() },
                            { "{demiliter}", value }
                        }
                    ));

                dataBlock.Demiliter = value;
                value = parser.ReadString();
            }
            else dataBlock.Demiliter = "=";

            bool canAcceptVars = false;
            bool hasParsedValue = false;
            bool valueIsName = parser.CurrentValueIsName;

            foreach (var allowedType in allowedTypes)
            {
                //Кавычки у значения могут быть только в случае значения типа "NAME"
                if (allowedType == EnumValueType.NAME && !valueIsName ||
                    allowedType != EnumValueType.NAME && valueIsName) continue;

                switch (allowedType)
                {
                    case EnumValueType.NONE: throw new NotImplementedException(allowedType.ToString());
                    case EnumValueType.VAR: canAcceptVars = true; break;

                    case EnumValueType.COUNTRY:
                        if (CountryManager.TryGetCountry(value, out var country))
                        {
                            dataBlock.ValueType = EnumValueType.COUNTRY;
                            dataBlock.SetSilentValue(value);
                            hasParsedValue = true;
                        }
                        break;
                    case EnumValueType.NAME:
                        dataBlock.ValueType = EnumValueType.NAME;
                        dataBlock.SetSilentValue(value);
                        hasParsedValue = true;
                        break;
                    case EnumValueType.IDEOLOGY: //TODO Проработать доп. типы данных
                    case EnumValueType.LOC_KEY:
                    case EnumValueType.STRING:
                        dataBlock.ValueType = EnumValueType.STRING;
                        dataBlock.SetSilentValue(value);
                        hasParsedValue = true;
                        break;
                    case EnumValueType.BOOLEAN:
                        dataBlock.ValueType = EnumValueType.BOOLEAN;
                        if (value == "yes")
                        {
                            dataBlock.SetSilentValue(true);
                            hasParsedValue = true;
                        }
                        else if (value == "no")
                        {
                            dataBlock.SetSilentValue(false);
                            hasParsedValue = true;
                        }
                        break;
                    case EnumValueType.INT:
                        if (int.TryParse(value, out int intValue))
                        {
                            dataBlock.ValueType = EnumValueType.INT;
                            dataBlock.SetSilentValue(intValue);
                            hasParsedValue = true;
                        }
                        break;
                    case EnumValueType.DECIMAL:
                    case EnumValueType.FLOAT:
                        if (Utils.TryParseFloat(value, out float floatValue))
                        {
                            dataBlock.ValueType = EnumValueType.FLOAT;
                            dataBlock.SetSilentValue(floatValue);
                            hasParsedValue = true;
                        }
                        break;
                    case EnumValueType.PROVINCE:
                        if (ushort.TryParse(value, out var provinceID) && ProvinceManager.TryGetProvince(provinceID, out var province))
                        {
                            dataBlock.ValueType = EnumValueType.PROVINCE;
                            dataBlock.SetSilentValue(province);
                            hasParsedValue = true;
                        }
                        break;
                }

                if (hasParsedValue) break;
            }

            if (!hasParsedValue && canAcceptVars)
            {
                if (value.Length > 0 && char.IsLetter(value[0]))
                {
                    dataBlock.ValueType = EnumValueType.VAR;
                    dataBlock.SetSilentValue(value);
                    hasParsedValue = true;
                }
            }

            if (!hasParsedValue)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_CANT_PARSE_VALUE,
                    new Dictionary<string, string>
                    {
                        { "{value}", value },
                        { "{name}", dataBlock.GetName() },
                        { "{allowedTypes}", string.Join(",", allowedTypes.ToArray()) }
                    }
                ));
        }


        private static void TryParseUniversalParameterValue(ParadoxParser parser, DataArgsBlock dataBlock, UniversalParamsInfo universalParamsInfo)
        {
            if (universalParamsInfo.AllowedValueTypes == null)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_HAS_NO_UNIVERSAL_PARAMS_ALLOWED_VALUE_TYPES,
                    new Dictionary<string, string>
                    {
                        { "{blockName}", dataBlock.GetName() },
                        { "{example}",  "\"allowedValueTypes\": [\"\", \"\",...]"}
                    }
                ));

            Parse(parser, dataBlock, universalParamsInfo.AllowedValueTypes, universalParamsInfo.AllowedDemiliters);
        }

        private static void TryParseBlockValue(ParadoxParser parser, DataArgsBlock dataBlock)
        {
            if (dataBlock.InfoArgsBlock == null)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_HAS_INFO_ARGS_BLOCK,
                    new Dictionary<string, string>
                    {
                        { "{blockName}", dataBlock.GetName() },
                        { "{infoBlockName}",  "InfoArgsBlock"}
                    }
                ));

            var allowedTypes = dataBlock.InfoArgsBlock.AllowedValueTypes;
            if (allowedTypes == null || allowedTypes.Length == 0)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_HAS_NO_ALLOWED_VALUE_TYPES,
                    new Dictionary<string, string>
                    {
                        { "{blockName}", dataBlock.GetName() },
                        { "{example}",  "\"allowedValueTypes\": [\"\", \"\",...]"}
                    }
                ));

            var allowedDemiliters = dataBlock.InfoArgsBlock.AllowedSpecialDemiliters;

            Parse(parser, dataBlock, allowedTypes, allowedDemiliters);
        }

    }
}
