using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.dataObjects.replaceTags;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.scripts;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts.objects.interfaces;
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

        private EnumDemiliter _demiliter = EnumDemiliter.EQUALS;
        public EnumDemiliter Demiliter { get => _demiliter; set => Utils.Setter(ref _demiliter, ref value, ref _needToSave); }
        public EnumValueType ValueType { get; set; }

        private object _value;
        public object GetValue() => _value is GameConstant gameConstant ? gameConstant.Value : _value;
        public object GetValueRaw() => _value;
        public ScriptBlockParseObject SetValue(object value)
        {
            var resolvedValue = value;

            if (resolvedValue is string valueString && valueString.StartsWith("@"))
                resolvedValue = ParseValueConstant(null, valueString, false);
            else if (resolvedValue is IParentable parentableValue)
            {
                parentableValue.SetParent(this);
            }

            if (!Equals(_value, resolvedValue))
            {
                _value = resolvedValue;
                _needToSave = true;
            }

            return this;
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
            parser.SkipWhiteSpaces();

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

            parser.SkipWhiteSpaces();

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
            var list = new GameList<ScriptBlockParseObject>();
            list.SetParent(this);
            _value = list;

            if (_scriptBlockInfo is InfoArgsBlock infoArgsBlock)
            {
                if (infoArgsBlock.SkipAnyInnerBlocks)
                {
                    parser.SkipInsideBlock();
                    return;
                }

                parser.ParseInsideBlock(
                    (comments) => SetComments(comments),
                    (tokenComments, token) =>
                    {
                        InfoArgsBlockInnerParseCallback(parser, token, tokenComments);
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

        private void InfoArgsBlockInnerParseCallback(GameParser parser, string key, GameComments keyComments)
        {
            var innerList = (GameList<ScriptBlockParseObject>)_value;
            var infoArgsBlock = (InfoArgsBlock)_scriptBlockInfo;

            if (infoArgsBlock.SkipAnyInnerBlocks)
                ParseCallback(parser);

            if (infoArgsBlock.CanHaveAnyInnerBlocks)
            {
                if (ParserUtils.TryParseScope(key, out var innerBlock))
                {
                    ParseInnerScriptInfoBlock(parser, innerList, innerBlock, keyComments);
                    return;
                }
                else if (InfoArgsBlocksManager.TryGetInfoArgsBlock(key, out var innerBlock0))
                {
                    ParseInnerScriptInfoBlock(parser, innerList, innerBlock0, keyComments);
                    return;
                }
            }

            if (infoArgsBlock.MandatoryInnerArgsBlocks != null && infoArgsBlock.MandatoryInnerArgsBlocks.TryGetValue(key, out var innerBlock1))
                ParseInnerScriptInfoBlock(parser, innerList, innerBlock1, keyComments);
            else if (infoArgsBlock.AllowedInnerArgsBlocks != null && infoArgsBlock.AllowedInnerArgsBlocks.TryGetValue(key, out var innerBlock2))
                ParseInnerScriptInfoBlock(parser, innerList, innerBlock2, keyComments);
            else if (infoArgsBlock.CanHaveUniversalParams)
            {
                var innerBlock3 = new ScriptBlockParseObject(
                    this, new InfoArgsBlock(key, infoArgsBlock.AllowedUniversalParamsInfo.AllowedValueTypes)
                );

                innerBlock3.ParseCallback(parser);

                /*
                var comments = innerBlock3.GetComments();
                if (comments != null)
                    comments.Previous = keyComments != null ? keyComments.Previous + comments.Previous : comments.Previous;
                else
                */
                innerBlock3.SetComments(keyComments);

                innerList.AddSilent(innerBlock3);
            }
            else if (infoArgsBlock.CanHaveAnyInnerBlocks)
            {
                var info = new InfoArgsBlock(key, null)
                {
                    CanHaveAnyInlineValue = true,
                    CanHaveAnyInnerBlocks = true
                };
                var innerBlock3 = new ScriptBlockParseObject(this, info);

                innerBlock3.ParseCallback(parser);

                /*
                var comments = innerBlock3.GetComments();
                if (comments != null)
                    comments.Previous = keyComments != null ? keyComments.Previous + comments.Previous : comments.Previous;
                else
                */
                innerBlock3.SetComments(keyComments);

                innerList.AddSilent(innerBlock3);
            }
            else
                throw new Exception("Unknown token: " + key + " " + parser.GetCursorInfo());


        }

        public bool TryAddUniversalParams(List<(string, EnumValueType, object)> universalParams)
        {
            if (!(ScriptBlockInfo is InfoArgsBlock infoArgsBlock))
                return false;

            if (!infoArgsBlock.CanHaveUniversalParams)
                return false;

            GameList<ScriptBlockParseObject> innerList;
            if (_value == null)
                innerList = new GameList<ScriptBlockParseObject>();
            else if (_value is GameList<ScriptBlockParseObject>)
                innerList = (GameList<ScriptBlockParseObject>)_value;
            else
                return false;

            var maxUniversalParamsCount = infoArgsBlock.AllowedUniversalParamsInfo.MaxUniversalParamsCount;
            if (maxUniversalParamsCount >= 0 && universalParams.Count + innerList.Count > maxUniversalParamsCount)
                return false;

            foreach (var obj in universalParams)
                if (Utils.Contains(infoArgsBlock.AllowedUniversalParamsInfo.AllowedValueTypes, obj.Item2))
                    return false;

            foreach (var obj in universalParams)
                foreach (var objList in innerList)
                    if (objList.ScriptBlockInfo.GetBlockName() == obj.Item1)
                        return false;

            foreach (var obj in universalParams)
            {
                var innerBlock = new ScriptBlockParseObject(
                    this, new InfoArgsBlock(obj.Item1, infoArgsBlock.AllowedUniversalParamsInfo.AllowedValueTypes)
                ).SetValue(obj.Item3);
                innerBlock.SetParent(innerList);
                innerList.Add(innerBlock);
            }

            if (_value == null)
                SetValue(innerList);

            return true;
        }

        private void ParseInnerScriptInfoBlock(GameParser parser, GameList<ScriptBlockParseObject> innerList, IScriptBlockInfo innerInfo, GameComments keyComments)
        {
            var innerBlock = new ScriptBlockParseObject(this, innerInfo);
            innerBlock.ParseCallback(parser);

            /*
            var comments = innerBlock.GetComments();
            if (comments != null)
                comments.Previous = keyComments != null ? keyComments.Previous + comments.Previous : comments.Previous;
            else
            */
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

        private object ParseValueConstant(GameParser parser, string rawValue, bool setValue = true)
        {
            IParentable tempParent = GetParent();
            GameConstant tempConstant = null;

            if (tempParent is IConstantable commentable)
                commentable.TryGetConstantParentable(rawValue.Substring(1), out tempConstant);

            object result;

            if (tempConstant != null)
                result = tempConstant;
            else if (parser == null)
                result = rawValue;
            else
                throw new Exception("Constant with name " + rawValue + " not found or this scope does not support constants: " + parser?.GetCursorInfo());

            if (setValue)
                _value = result;

            return result;
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
                            if (CountryManager.TryGet(value, out var country))
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
                        case EnumValueType.PROVINCE:
                            if (ushort.TryParse(value, out var provinceID) && ProvinceManager.TryGet(provinceID, out var province))
                            {
                                ValueType = EnumValueType.PROVINCE;
                                _value = province;
                                hasParsedValue = true;
                            }
                            break;
                    }
                    if (hasParsedValue) break;
                }
            }
            else if (_scriptBlockInfo is InfoArgsBlock infoBlock && infoBlock.CanHaveAnyInlineValue)
            {
                _value = ParserUtils.ParseObject(value);
                hasParsedValue = true;
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

        public override SavePattern GetSavePattern() => null;

        public override void Save(StringBuilder sb, string outIndent, string key, SavePatternParameter savePatternParameter)
        {
            var comments = GetComments();

            if (_value is ISaveable saveable)
            {
                SavePatternParameter innerSaveParameter = default;
                if (_value is ISizable sizable && sizable.GetSize() <= 1)
                    innerSaveParameter.IsForceInline = true;

                saveable.Save(sb, outIndent, _scriptBlockInfo.GetBlockName(), innerSaveParameter);
                return;
            }

            if (_value == null)
                return;

            if (comments == null)
                comments = new GameComments();

            comments?.SavePrevComments(sb, outIndent);

            if (sb.Length > 0)
            {
                var lastChar = sb[sb.Length - 1];
                if (lastChar == '\n')
                    sb.Append(outIndent);
            }

            sb.Append(_scriptBlockInfo.GetBlockName()).Append(' ').Append((char)_demiliter)
                .Append(' ').Append(ParserUtils.ObjectToSaveString(_value)).Append(' ');

            if (comments.Inline.Length > 0)
                sb.Append(comments.Inline).Append(' ').Append(Constants.NEW_LINE);
        }

        public static GameList<ScriptBlockParseObject> LoadFromListObject(IParentable parent, IListObject from, GameList<ScriptBlockParseObject> to)
        {
            to.Clear();
            to.SetParent(parent);

            from.ForEach(entryObj =>
            {
                var entry = (PairObject)entryObj;
                var keyString = (IStringObject)entry.GetKey();
                var key = (string)keyString.GetValue();

                var block = ParserUtils.GetAnyScriptBlockParseObject(to, key);

                if (entry.GetValue() is ListObject listValue)
                    block.SetValue(LoadFromListObject(block, listValue, new GameList<ScriptBlockParseObject>()));
                else
                {
                    var valueString = ParserUtils.ObjectToSaveString(((IScriptObject)entry.GetValue()).GetValue());
                    block.ParseValueString(null, valueString, valueString.StartsWith("\"") && valueString.EndsWith("\""));
                }

                if (entryObj is PairCommentedObject pairCommentedObject && pairCommentedObject.comments.HasAnyData())
                {
                    var comments = new GameComments();
                    pairCommentedObject.comments.CopyTo(comments);
                    block.SetComments(comments);
                }

                to.Add(block);
            });

            return to;
        }

        public void SaveToListObject(ListObject list)
        {
            if (_value is GameList<ScriptBlockParseObject> listValue)
            {
                var pair = new PairCommentedObject(new StringObject(), new ListObject());
                list.Add(pair);

                pair.SetKey(new StringObject(_scriptBlockInfo.GetBlockName()));

                var innerList = new ListObject();
                pair.SetValue(innerList);

                foreach (var innerValue in listValue)
                    innerValue.SaveToListObject(innerList);

                GetComments()?.CopyTo(pair.comments);
            }
            else
            {
                var value = ScriptParser.ParseValue(GetValue().ToString());

                if (value == null)
                {
                    var tempValue = ParserUtils.ObjectToSaveString(GetValue());
                    value = ScriptParser.ParseValue(tempValue);

                    if (value == null)
                        value = new StringObject(tempValue);
                }

                var pair = new PairCommentedObject(new StringObject(), value.GetEmptyCopy());

                list.Add(pair);

                pair.SetKey(new StringObject(_scriptBlockInfo.GetBlockName()));
                pair.SetValue(value);

                GetComments()?.CopyTo(pair.comments);
            }
        }
    }
}
