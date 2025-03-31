using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class ScriptBlockParseObject : AbstractParseObject
    {
        private IScriptBlockInfo _scriptBlockInfo;

        private char _demiliter;
        public char Demiliter { get => _demiliter; set => Utils.Setter(ref _demiliter, ref value, ref _needToSave); }
        public EnumValueType ValueType { get; set; }

        private object _value;
        public object Value { get => _value; set => Utils.Setter(ref _value, ref value, ref _needToSave); }

        public ScriptBlockParseObject() { }

        public ScriptBlockParseObject(IParentable parent, IScriptBlockInfo scriptBlockInfo)
        {
            SetParent(parent);
            _scriptBlockInfo = scriptBlockInfo;
        }

        public override bool CustomParseCallback(GameParser parser)
        {
            if (parser.SkipWhiteSpaces())
                throw new Exception("Invalid data structure: " + parser.GetCursorInfo());

            //Парсим символ-разделитель после ключа
            parser.ParseDemiliters();
            var demiliters = parser.PullParsedDataString();

            if (demiliters.Length != 1)
                throw new Exception("Invalid data structure: " + parser.GetCursorInfo());

            var allowedSpecialDemiliters = _scriptBlockInfo.GetAllowedSpecialDemiliters();
            if (allowedSpecialDemiliters != null)
            {
                bool isAllowedDemifiler = false;
                foreach (var allowedDemiliter in allowedSpecialDemiliters)
                {
                    if ((char)allowedDemiliter == demiliters[0])
                    {
                        isAllowedDemifiler = true;
                        break;
                    }
                }

                if (!isAllowedDemifiler)
                    throw new Exception("Invalid data structure (demiliter): " + parser.GetCursorInfo());
            }
            else if (demiliters[0] != '=')
                throw new Exception("Invalid data structure (demiliter): " + parser.GetCursorInfo());

            if (parser.SkipWhiteSpaces())
                throw new Exception("Invalid data structure: " + parser.GetCursorInfo());


            if (_scriptBlockInfo == null)
                throw new Exception("_scriptBlockInfo is null: " + parser.GetCursorInfo());

            if (parser.CurrentToken == Token.LEFT_CURLY && _scriptBlockInfo.IsAllowsBlockValue())
                ParseBlockValue(parser);
            else if (_scriptBlockInfo.IsAllowsInlineValue())
                ParseInlineValue(parser);
            else
                throw new Exception("Invalid data structure: " + parser.GetCursorInfo());

            return true;
        }

        private void ParseInlineValue(GameParser parser)
        {
            bool isQuote = parser.CurrentToken == Token.QUOTE;
            if (isQuote)
                parser.ParseQuoted();
            else
                parser.ParseUnquotedValue();

            var value = parser.PullParsedDataString();

            if (value.Length == 0)
                throw new Exception("Invalid data structure: " + parser.GetCursorInfo());

            ParseValueString(value, isQuote);

            SetComments(parser.ParseAndPullComments());
        }

        private void ParseBlockValue(GameParser parser)
        {
            _value = new GameList<ScriptBlockParseObject>();
            if (_scriptBlockInfo is InfoArgsBlock)
            {
                int[] universalParamsCounter = new int[1];
                parser.ParseInsideBlock(
                    (comments) => SetComments(comments),
                    (token) => InfoArgsBlockInnerParseCallback(parser, token, universalParamsCounter)
                );
            }
            else
                parser.ParseInsideBlock(
                    (comments) => SetComments(comments),
                    (token) => ScopeInnerParseCallback(parser, token)
                );
        }

        private void InfoArgsBlockInnerParseCallback(GameParser parser, string key, int[] universalParamsCounter)
        {
            var innerList = (GameList<ScriptBlockParseObject>)_value;
            var infoArgsBlock = (InfoArgsBlock)_scriptBlockInfo;

            if (infoArgsBlock.CanHaveAnyInnerBlocks)
            {
                if (ParserUtils.TryParseScope(key, out var innerBlock))
                    ParseInner(innerBlock);
                else if (InfoArgsBlocksManager.TryGetInfoArgsBlock(key, out var innerBlock0))
                    ParseInner(innerBlock0);
                else
                    throw new Exception("Unknown token: " + parser.GetCursorInfo());
            }
            else if (infoArgsBlock.MandatoryInnerArgsBlocks != null && infoArgsBlock.MandatoryInnerArgsBlocks.TryGetValue(key, out var innerBlock1))
                ParseInner(innerBlock1);
            else if (infoArgsBlock.AllowedInnerArgsBlocks != null && infoArgsBlock.AllowedInnerArgsBlocks.TryGetValue(key, out var innerBlock2))
                ParseInner(innerBlock2);
            else if (infoArgsBlock.CanHaveUniversalParams)
            {
                var innerBlock3 = new ScriptBlockParseObject(
                    this, new InfoArgsBlock(key, infoArgsBlock.AllowedUniversalParamsInfo.AllowedValueTypes)
                );

                innerBlock3.CustomParseCallback(parser);
                innerList.AddSilent(innerBlock3);
            }
            else
                throw new Exception("Unknown token: " + parser.GetCursorInfo());

            void ParseInner(IScriptBlockInfo innerInfo)
            {
                var innerBlock = new ScriptBlockParseObject(this, innerInfo);
                innerBlock.CustomParseCallback(parser);
                innerList.AddSilent(innerBlock);
            }
        }

        private void ScopeInnerParseCallback(GameParser parser, string key)
        {
            throw new NotImplementedException(); //TODO implement
        }

        private void ParseValueString(string value, bool valueIsName)
        {
            bool canAcceptVars = false;
            bool hasParsedValue = false;

            var allowedValueTypes = _scriptBlockInfo.GetAllowedValueTypes();
            if (allowedValueTypes != null)
            {
                foreach (var allowedType in allowedValueTypes)
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
                                ValueType = EnumValueType.COUNTRY;
                                _value = country;
                                hasParsedValue = true;
                            }
                            break;
                        case EnumValueType.NAME:
                            ValueType = EnumValueType.NAME;
                            _value = value;
                            hasParsedValue = true;
                            break;
                        case EnumValueType.IDEOLOGY: //TODO Проработать доп. типы данных
                        case EnumValueType.LOC_KEY:
                        case EnumValueType.STRING:
                            ValueType = EnumValueType.STRING;
                            _value = value;
                            hasParsedValue = true;
                            break;
                        case EnumValueType.BOOLEAN:
                            ValueType = EnumValueType.BOOLEAN;
                            if (value == "yes")
                            {
                                _value = true;
                                hasParsedValue = true;
                            }
                            else if (value == "no")
                            {
                                _value = false;
                                hasParsedValue = true;
                            }
                            break;
                        case EnumValueType.INT:
                            if (int.TryParse(value, out var intValue))
                            {
                                ValueType = EnumValueType.INT;
                                _value = intValue;
                                hasParsedValue = true;
                            }
                            else if (float.TryParse(value, out var floatValueInt))
                            {
                                ValueType = EnumValueType.INT;
                                _value = (int)floatValueInt;
                                hasParsedValue = true;
                            }
                            break;
                        case EnumValueType.DECIMAL:
                        case EnumValueType.FLOAT:
                            if (Utils.TryParseFloat(value, out var floatValue))
                            {
                                ValueType = EnumValueType.FLOAT;
                                _value = floatValue;
                                hasParsedValue = true;
                            }
                            break;
                    }
                    if (hasParsedValue) break;
                }
            }

            if (!hasParsedValue && canAcceptVars)
            {
                if (value.Length > 0 && char.IsLetter(value[0]))
                {
                    ValueType = EnumValueType.VAR;
                    _value = value;
                    hasParsedValue = true;
                }
            }

            if (!hasParsedValue)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_DATA_ARGS_BLOCK_CANT_PARSE_VALUE,
                    new Dictionary<string, string>
                    {
                        { "{value}", value },
                        { "{name}", _scriptBlockInfo.GetBlockName() },
                        { "{allowedTypes}", string.Join(",", allowedValueTypes) }
                    }
                ));
        }

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => null;
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;

        public override IParseObject GetEmptyCopy() => new ScriptBlockParseObject();

        public override SaveAdapter GetSaveAdapter() => null;

        public override bool CustomSave(GameParser parser, StringBuilder sb, SaveAdapterParameter saveParameter, string outIndent, string key)
        {
            throw new NotImplementedException();
        }
    }
}
