using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.parser.objects;
using HOI4ModBuilder.src.parser.parameter;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.parser
{
    public abstract class AbstractParseObjectOld : IParseObjectOld, IConstantsOld
    {
        private IParseObjectOld _parent;
        public IParseObjectOld GetParent() => _parent;
        public void SetParent(IParseObjectOld parent) => _parent = parent;
        private Dictionary<string, GameConstantOld> _constants;
        public Dictionary<string, GameConstantOld> GetConstants() => _constants;

        public bool TryGetConstant(string name, out GameConstantOld constant)
        {
            if (_constants != null)
                return _constants.TryGetValue(name, out constant);

            constant = null;
            return false;
        }

        protected bool _needToSave;
        public bool IsNeedToSave()
        {
            if (_needToSave)
                return true;

            var staticAdapter = GetStaticAdapter();
            if (staticAdapter != null)
            {
                foreach (var entry in staticAdapter)
                {
                    var param = entry.Value(this);
                    if (param != null && param is INeedToSave iNeedSave && iNeedSave.IsNeedToSave())
                        return true;
                }
            }

            var dynamicAdapter = GetDynamicAdapter();
            if (dynamicAdapter != null)
            {
                foreach (var entry in dynamicAdapter)
                {
                    var param = entry.Value.parameterProvider(this);
                    if (param != null && param is INeedToSave iNeedSave && iNeedSave.IsNeedToSave())
                        return true;
                }
            }

            return false;
        }
        public void SetNeedToSave(bool needToSave) => _needToSave = needToSave;

        public IParseObjectOld GetThis() => this;

        public abstract Dictionary<string, Func<object, object>> GetStaticAdapter();
        public abstract Dictionary<string, DynamicGameParameterOld> GetDynamicAdapter();
        public abstract IParseObjectOld GetEmptyCopy();

        public void ParseCallback(OldGameParser parser)
        {
            if (parser.CurrentToken == Token.CONSTANT)
                ParseConstant(parser);
            else if (parser.CurrentToken == Token.UNTYPED)
                ParseAdapters(parser);
            else throw new Exception("Unexpected token " + parser.CurrentToken + " = " + parser.CurrentChar + ": " + parser.GetCursorInfo());
        }

        private void ParseConstant(OldGameParser parser)
        {
            if (_constants == null)
                throw new Exception("Constant declaration is not supported in this scope: " + parser.GetCursorInfo());

            var constantName = parser.ParseValue(OldGameParser.MASK_VALUE_ALLOWED_CHARS);

            if (parser.SkipWhiteSpaces())
                throw new Exception("Constant declaration must have '=' demiliter on the same line: " + parser.GetCursorInfo());

            if (parser.CurrentToken != Token.EQUALS)
                throw new Exception("Constant must have '=' demiliter token: " + parser.GetCursorInfo());

            if (parser.SkipWhiteSpaces() || !parser.CheckCurrentToken(OldGameParser.MASK_CONSTANT_VALUE_ALLOWED_START_CHAR))
                throw new Exception("Constant declaration must have allowed value after '=' demiliter on the same line: " + parser.GetCursorInfo());

            var constantValue = parser.ParseValue(OldGameParser.MASK_VALUE_ALLOWED_CHARS);

            if (_constants.TryGetValue(constantName, out var constant))
                Logger.LogWarning(
                    EnumLocKey.WARNING_CONSTANT_OVERRIDE, new Dictionary<string, string> {
                        { "{filePath}", parser.FileInfo.filePath },
                        { "{lineIndex}", parser.LineIndex.ToString() },
                        { "{lineCharIndex}", parser.LineCharIndex.ToString() },
                        { "{constantName}", constantName },
                        { "{constantOldValue}", constant.Value },
                        { "{constantNewValue}", constantValue },
                    });
            else
            {
                constant = new GameConstantOld();
                _constants.Add(constantName, constant);
            }
;
            var constantComments = constant.Comments;
            var comments = parser.GetComments();

            if (comments.Length > 0)
            {
                constantComments.Previous = comments;
                parser.ClearComments();
            }

            constant.SetParserValue(constantValue);

            if (!parser.SkipWhiteSpaces())
            {
                if (parser.CurrentToken == Token.COMMENT)
                    parser.ParseComment();

                constantComments.Inline = parser.GetComments();
                parser.ClearComments();
            }
        }

        private void ParseAdapters(OldGameParser parser)
        {
            var key = parser.ParseValue(OldGameParser.MASK_KEY_ALLOWED_CHARS);

            var staticParameter = ParseStaticAdapter(key);
            if (staticParameter != null)
            {
                if (staticParameter is IGameParameterOld gameParameter)
                    gameParameter.ParseCallback(parser);
            }


            throw new Exception("Unknown token " + parser.GetCursorInfo());
        }

        private object ParseStaticAdapter(string key) => GetStaticAdapter()?[key](this);
        private object ParseDynamicAdapter(string key)
        {
            var dynamicAdapter = GetDynamicAdapter();
            if (dynamicAdapter == null)
                return null;

            foreach (var entry in dynamicAdapter)
            {
                var payload = entry.Value.payloadFactory(this, key);
                if (payload == null)
                    continue;

                var parameter = entry.Value.parameterProvider(this);
                if (parameter is IPushObject iPushObject)
                    iPushObject.PushObject(payload);
            }
            return null;
        }

        public AbstractParseObjectOld()
        {
            var staticAdapter = GetStaticAdapter();
            if (staticAdapter != null)
            {
                foreach (var provider in staticAdapter.Values)
                {
                    var parameter = provider(this);

                    if (parameter is IParentObjectOld gameParameter)
                        gameParameter.SetParent(this);
                }
            }
        }
    }
}
