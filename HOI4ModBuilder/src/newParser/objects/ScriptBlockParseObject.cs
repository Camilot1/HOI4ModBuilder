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
        public IScriptBlockInfo ScriptBlockInfo => _scriptBlockInfo;

        private EnumDemiliter _demiliter;
        public EnumDemiliter Demiliter { get => _demiliter; set => Utils.Setter(ref _demiliter, ref value, ref _needToSave); }
        public EnumValueType ValueType { get; set; }

        private object _value;
        public object GetValue() => _value is GameConstant gameConstant ? gameConstant.Value : _value;
        public object GetValueRaw() => _value;
        public void SetValue(object value)
        {
            if (_value == null && value != null || !_value.Equals(value))
            {
                if (value is GameConstant valueConstant)
                    _value = valueConstant;
                else if (value is string valueString && valueString.StartsWith("@"))
                    ParseValueConstant(null, valueString);
                else
                    _value = value;

                _needToSave = true;
            }
        }

        public ScriptBlockParseObject() { }

        public ScriptBlockParseObject(IParentable parent, IScriptBlockInfo scriptBlockInfo)
        {
            SetParent(parent);
            _scriptBlockInfo = scriptBlockInfo;
        }

        public override void Validate(LinkedLayer layer)
        {
            if (_value is IValidatable validatable)
                validatable.Validate(new LinkedLayer(layer, _scriptBlockInfo?.GetBlockName()));
        }

        public override void ParseCallback(GameParser parser)
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

                if (isAllowedDemifiler)
                    _demiliter = (EnumDemiliter)demiliters[0];
                else
                    throw new Exception("Invalid data structure (demiliter): " + parser.GetCursorInfo());
            }
            else if (demiliters[0] == '=')
                _demiliter = (EnumDemiliter)demiliters[0];
            else
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

            ParseValueString(parser, value, isQuote);

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
                    (tokenComments, token) =>
                    {
                        InfoArgsBlockInnerParseCallback(parser, token, tokenComments, universalParamsCounter);
                        return false;
                    }
                );
            }
            else
                parser.ParseInsideBlock(
                    (comments) => SetComments(comments),
                    (tokenComments, token) =>
                    {
                        ScopeInnerParseCallback(parser, token, tokenComments);
                        return false;
                    }
                );
        }

        private void InfoArgsBlockInnerParseCallback(GameParser parser, string key, GameComments keyComments, int[] universalParamsCounter)
        {
            var innerList = (GameList<ScriptBlockParseObject>)_value;
            var infoArgsBlock = (InfoArgsBlock)_scriptBlockInfo;

            if (infoArgsBlock.CanHaveAnyInnerBlocks)
            {
                if (ParserUtils.TryParseScope(key, out var innerBlock))
                    ParseInnerScriptInfoBlock(parser, innerList, innerBlock, keyComments);
                else if (InfoArgsBlocksManager.TryGetInfoArgsBlock(key, out var innerBlock0))
                    ParseInnerScriptInfoBlock(parser, innerList, innerBlock0, keyComments);
                else
                    throw new Exception("Unknown token: " + parser.GetCursorInfo());
            }
            else if (infoArgsBlock.MandatoryInnerArgsBlocks != null && infoArgsBlock.MandatoryInnerArgsBlocks.TryGetValue(key, out var innerBlock1))
                ParseInnerScriptInfoBlock(parser, innerList, innerBlock1, keyComments);
            else if (infoArgsBlock.AllowedInnerArgsBlocks != null && infoArgsBlock.AllowedInnerArgsBlocks.TryGetValue(key, out var innerBlock2))
                ParseInnerScriptInfoBlock(parser, innerList, innerBlock2, keyComments);
            else if (infoArgsBlock.CanHaveUniversalParams)
            {
                var innerBlock3 = new ScriptBlockParseObject(
                    this, new InfoArgsBlock(key, infoArgsBlock.AllowedUniversalParamsInfo.AllowedValueTypes)
                );

                innerBlock3.ParseCallback(parser);

                var comments = innerBlock3.GetComments();
                if (comments != null)
                    comments.Previous = keyComments != null ? keyComments.Previous + comments.Previous : comments.Previous;
                else
                    innerBlock3.SetComments(keyComments);

                innerList.AddSilent(innerBlock3);
            }
            else
                throw new Exception("Unknown token: " + parser.GetCursorInfo());


        }

        private void ParseInnerScriptInfoBlock(GameParser parser, GameList<ScriptBlockParseObject> innerList, IScriptBlockInfo innerInfo, GameComments keyComments)
        {
            var innerBlock = new ScriptBlockParseObject(this, innerInfo);
            innerBlock.ParseCallback(parser);

            var comments = innerBlock.GetComments();
            if (comments != null)
                comments.Previous = keyComments != null ? keyComments.Previous + comments.Previous : comments.Previous;
            else
                innerBlock.SetComments(keyComments);

            innerList.AddSilent(innerBlock);
        }

        private void ScopeInnerParseCallback(GameParser parser, string key, GameComments keyComments)
        {
            var innerList = (GameList<ScriptBlockParseObject>)_value;

            if (ParserUtils.TryParseScope(key, out var scopeBlock))
                ParseInnerScriptInfoBlock(parser, innerList, scopeBlock, keyComments);
            else if (InfoArgsBlocksManager.TryGetInfoArgsBlock(key, out var infoArgsBlock)) //TODO reimplement
                ParseInnerScriptInfoBlock(parser, innerList, infoArgsBlock, keyComments);
        }

        private void ParseValueConstant(GameParser parser, string rawValue)
        {
            IParentable tempParent = GetParent();
            GameConstant tempConstant = null;

            if (tempParent is IConstantable commentable)
                commentable.TryGetConstantParentable(rawValue.Substring(1), out tempConstant);

            if (tempConstant != null)
                _value = tempConstant;
            else if (parser == null)
                _value = rawValue;
            else
                throw new Exception("Constant with name " + rawValue + " not found or this scope does not support constants: " + parser?.GetCursorInfo());
        }

        private void ParseValueString(GameParser parser, string value, bool valueIsName)
        {
            bool canAcceptVars = false;
            bool hasParsedValue = false;

            if (value.StartsWith("@"))
                ParseValueConstant(parser, value);

            GameConstant tempGameConstant = null;
            if (_value is GameConstant gameConstant)
            {
                tempGameConstant = gameConstant;
                value = gameConstant.Value;
            }

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

            if (tempGameConstant != null)
                _value = tempGameConstant;
        }

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => null;
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;

        public override IParseObject GetEmptyCopy() => new ScriptBlockParseObject();

        public override SaveAdapter GetSaveAdapter() => null;

        public override void Save(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter)
        {
            if (_value is ISaveable saveable)
            {
                saveable.Save(parser, sb, outIndent, _scriptBlockInfo.GetBlockName(), default);
                return;
            }

            if (_value == null)
                return;

            var comments = GetComments();
            if (comments == null)
                comments = GameComments.DEFAULT;

            if (comments.Previous.Length > 0)
                sb.Append(outIndent).Append(comments.Previous).Append(Constants.NEW_LINE);

            sb.Append(outIndent).Append(_scriptBlockInfo.GetBlockName()).Append(' ').Append((char)_demiliter)
                .Append(' ').Append(ParserUtils.ObjectToSaveString(_value));

            if (comments.Inline.Length > 0)
                sb.Append(outIndent).Append(comments.Inline);
        }
    }
}
