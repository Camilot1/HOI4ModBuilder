using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;
using YamlDotNet.Core;
using Pdoxcl2Sharp;
using HOI4ModBuilder.src.dataObjects.argBlocks.scripted;
using static HOI4ModBuilder.src.dataObjects.argBlocks.InfoArgsBlock;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.dataObjects
{
    class DataArgsBlocksManager
    {
        private static readonly string CONDITIONAL_CHAIN_ERROR_MESSAGE = "Перед блоком \"else_if\" должны находиться или блок \"if\", или блок \"else_if\".";
        private static readonly string CONDITIONAL_IF_HAS_NO_LIMIT_MESSAGE = "В блоке \"if\" отсутствует блок \"limit\".";
        private static readonly string CONDITIONAL_ELSE_IF_HAS_NO_LIMIT_MESSAGE = "В блоке \"else_if\" отсутствует блок \"limit\".";

        private static readonly Dictionary<string, EnumDemiliter> demilitersDict = new Dictionary<string, EnumDemiliter>
        {
            { "<", EnumDemiliter.LESS_THAN },
            { ">", EnumDemiliter.GREATER_THAN }
        };

        private static readonly Dictionary<string, Action<ParadoxParser, List<DataArgsBlock>>> parseFunctions = new Dictionary<string, Action<ParadoxParser, List<DataArgsBlock>>>
        {
            {
                "AND", (parser, currentLevelDataBlocks) => {
                    var innerBlock = new LogicArgsBlock { SpecialName = "AND" };
                    parser.Parse(innerBlock);
                    currentLevelDataBlocks.Add(innerBlock);
                }
            },
            {
                "OR", (parser, currentLevelDataBlocks) => {
                    var innerBlock = new LogicArgsBlock { SpecialName = "OR" };
                    parser.Parse(innerBlock);
                    currentLevelDataBlocks.Add(innerBlock);
                }
            },
            {
                "NOT", (parser, currentLevelDataBlocks) => {
                    var innerBlock = new LogicArgsBlock { SpecialName = "NOT" };
                    parser.Parse(innerBlock);
                    currentLevelDataBlocks.Add(innerBlock);
                }
            },
            {
                "hidden_modifier", (parser, currentLevelDataBlocks) => {
                    var innerBlock = new DataArgsBlock { SpecialName = "hidden_modifier" };
                    parser.Parse(innerBlock);
                    currentLevelDataBlocks.Add(innerBlock);
                }
            },

            {
                "if", (parser, currentLevelDataBlocks) => {
                    var innerBlock = new ConditionalArgsBlock { CurrentLevelDataBlocks = currentLevelDataBlocks, InfoArgsBlock = InfoArgsBlocksManager.ifInfoArgsBlock };
                    parser.Parse(innerBlock);
                    if (innerBlock.limit == null)
                        throw new Exception(CONDITIONAL_IF_HAS_NO_LIMIT_MESSAGE);

                    currentLevelDataBlocks.Add(innerBlock);
                }
            },
            {
                "else_if", (parser, currentLevelDataBlocks) => {
                    if (currentLevelDataBlocks.Count == 0)
                        throw new Exception(CONDITIONAL_CHAIN_ERROR_MESSAGE);

                    var prevBlock = currentLevelDataBlocks[currentLevelDataBlocks.Count - 1];
                    var prevBlockName = prevBlock.GetName();
                    if (prevBlockName != "if" || prevBlockName != "else_if")
                        throw new Exception(CONDITIONAL_CHAIN_ERROR_MESSAGE);

                    var innerBlock = new ConditionalArgsBlock { CurrentLevelDataBlocks = currentLevelDataBlocks, InfoArgsBlock = InfoArgsBlocksManager.elseIfInfoArgsBlock };
                    parser.Parse(innerBlock);
                    if (innerBlock.limit == null) throw new Exception(CONDITIONAL_ELSE_IF_HAS_NO_LIMIT_MESSAGE);
                    currentLevelDataBlocks.Add(innerBlock);
                }
            },
            {
                "else", (parser, currentLevelDataBlocks) => {
                    if (currentLevelDataBlocks.Count == 0)
                        throw new Exception(CONDITIONAL_CHAIN_ERROR_MESSAGE);

                    var prevBlock = currentLevelDataBlocks[currentLevelDataBlocks.Count - 1];
                    var prevBlockName = prevBlock.GetName();
                    if (prevBlockName != "if" || prevBlockName != "else_if")
                        throw new Exception(CONDITIONAL_CHAIN_ERROR_MESSAGE);

                    var innerBlock = new ConditionalArgsBlock { CurrentLevelDataBlocks = currentLevelDataBlocks, InfoArgsBlock = InfoArgsBlocksManager.elseInfoArgsBlock };
                    parser.Parse(innerBlock);
                    currentLevelDataBlocks.Add(innerBlock);
                }
            }
        };

        public static void ParseDataArgsBlock(ParadoxParser parser, DataArgsBlock currentDataBlock, string token, List<DataArgsBlock> currentLevelDataBlocks, List<DataArgsBlock> innerLevelDataBlocks)
        {
            //Если не был получен список для внутренних блоков, то создаём его
            if (innerLevelDataBlocks == null) innerLevelDataBlocks = new List<DataArgsBlock>(0);

            if (TryParseInnerBlocks()) return;
            else if (TryParseConstantBlocks()) return;
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
                else if (infoBlock.CanHaveUniversalParams && (!infoBlock.HasAllowedBlocks || infoBlock.AllowedUniversalParamsInfo.IgnoreAllowedInnerBlocksCheck))
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


            bool TryParseConstantBlocks() //Попытаться распарсить захардкоженные блоки
            {
                if (parseFunctions.TryGetValue(token, out var functions))
                {
                    functions(parser, innerLevelDataBlocks);
                    return true;
                }
                else return false;
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
                    var dataBlock = new DataArgsBlock { SpecialName = country.tag, CurrentLevelDataBlocks = innerLevelDataBlocks };
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
                if (currentDataBlock != null && currentDataBlock.InfoArgsBlock.CanHaveUniversalParams)
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

        private static void Parse(ParadoxParser parser, DataArgsBlock dataBlock, EnumNewArgsBlockValueType[] allowedTypes, EnumDemiliter[] allowedDemiliters)
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
            foreach (var allowedType in allowedTypes)
            {
                switch (allowedType)
                {
                    case EnumNewArgsBlockValueType.NONE: throw new NotImplementedException(allowedType.ToString());
                    case EnumNewArgsBlockValueType.VAR: canAcceptVars = true; break;

                    case EnumNewArgsBlockValueType.SCOPE:
                        if (
                            int.TryParse(value, out int intValue) ||
                            CountryManager.TryGetCountry(value, out var country)
                        )
                        {
                            dataBlock.ValueType = EnumNewArgsBlockValueType.SCOPE;
                            dataBlock.Value = value;
                            hasParsedValue = true;
                        }
                        break;
                    case EnumNewArgsBlockValueType.IDEOLOGY: //TODO Проработать доп. типы данных
                    case EnumNewArgsBlockValueType.STRING:
                        dataBlock.ValueType = EnumNewArgsBlockValueType.STRING;
                        dataBlock.Value = value;
                        hasParsedValue = true;
                        break;
                    case EnumNewArgsBlockValueType.BOOLEAN:
                        dataBlock.ValueType = EnumNewArgsBlockValueType.BOOLEAN;
                        if (value == "yes")
                        {
                            dataBlock.Value = true;
                            hasParsedValue = true;
                        }
                        else if (value == "no")
                        {
                            dataBlock.Value = false;
                            hasParsedValue = true;
                        }
                        break;
                    case EnumNewArgsBlockValueType.INT:
                        if (int.TryParse(value, out intValue))
                        {
                            dataBlock.ValueType = EnumNewArgsBlockValueType.INT;
                            dataBlock.Value = intValue;
                            hasParsedValue = true;
                        }
                        break;
                    case EnumNewArgsBlockValueType.DECIMAL:
                    case EnumNewArgsBlockValueType.FLOAT:
                        if (float.TryParse(value.Replace('.', ','), out float floatValue))
                        {
                            dataBlock.ValueType = EnumNewArgsBlockValueType.FLOAT;
                            dataBlock.Value = floatValue;
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
                    dataBlock.ValueType = EnumNewArgsBlockValueType.VAR;
                    dataBlock.Value = value;
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
