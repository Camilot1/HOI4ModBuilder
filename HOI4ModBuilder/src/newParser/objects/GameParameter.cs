using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.parser;
using System;
using System.Text;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.newParser.objects
{
    public enum EnumDemiliter
    {
        NONE,
        EQUALS = '=',
        LESS_THAN = '<',
        GREATER_THAN = '>',
    }

    public class GameParameter<T> : IGameParameter where T : new()
    {
        private bool _needToSave;
        public bool IsNeedToSave() => _needToSave;
        public void SetNeedToSave(bool needToSave) => _needToSave = needToSave;

        private IParentable _parent;
        public IParentable GetParent() => _parent;
        public void SetParent(IParentable parent) => _parent = parent;

        private GameComments _comments;
        public GameComments GetComments() => _comments;
        public void SetComments(GameComments comments) => _comments = comments;

        private EnumDemiliter _enumDemiliter;
        private bool _isAnyDemiliter;
        public bool IsAnyDemiliter() => _isAnyDemiliter;

        private object _value;
        public object GetValueRaw() => _value;
        public T GetValue() => _value is GameConstant gameConstant ? gameConstant.GetValue<T>() : (T)_value;
        public void SetValue(T value)
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

        public void InitValue()
        {
            _value = new T();
            if (_value is IParentable parentable) parentable.SetParent(this);
        }
        public void InitValueIfNull()
        {
            if (_value == null) InitValue();
        }

        public string AssemblePath() => ParserUtils.AsseblePath(this);

        public void ParseCallback(GameParser parser)
        {
            if (parser.SkipWhiteSpaces())
                throw new Exception("Invalid parameter structure: " + parser.GetCursorInfo());

            parser.ParseDemiliters();
            var demiliter = parser.PullParsedDataString();

            if (demiliter.Length != 1)
                throw new Exception("Invalid demiliter: " + demiliter + ": " + parser.GetCursorInfo());

            if (demiliter[0] != '=' && !_isAnyDemiliter)
                throw new Exception("Invalid demiliter: " + demiliter + ": " + parser.GetCursorInfo());

            if (parser.SkipWhiteSpaces())
                throw new Exception("Invalid parameter structure: " + parser.GetCursorInfo());

            InitValueIfNull();

            if (_value is IParseObject obj)
            {
                parser.Parse(obj);
                return;
            }


            if (_value is IPostParseCallbackable obj1)
            {
                obj1.PostParseCallback(parser);
                return;
            }

            if (parser.CurrentToken == Token.QUOTE)
                parser.ParseQuoted();
            else
                parser.ParseUnquoted();

            var rawValue = parser.PullParsedDataString();

            if (rawValue.Length == 0)
                throw new Exception("Invalid value: " + parser.GetCursorInfo());

            if (rawValue[0] == '@')
                ParseValueConstant(parser, rawValue);
            else
                ParseValueRaw(parser, rawValue);

            _comments = parser.ParseAndPullComments();

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

        private void ParseValueRaw(GameParser parser, string rawValue)
        {
            try
            {
                _value = ParserUtils.Parse<T>(rawValue);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to parse value type: " + rawValue + parser.GetCursorInfo(), ex);
            }
        }

        public bool CustomSave(GameParser parser, StringBuilder sb, SaveAdapterParameter saveParameter, string outIndent, string key) => false;
        public void Save(GameParser parser, StringBuilder sb, SaveAdapterParameter saveParameter, string outIndent, string key)
        {
            if (_value is ISaveable saveable)
            {
                saveable.Save(parser, sb, saveParameter, outIndent, key);
                return;
            }

            sb.Append(outIndent).Append(key).Append(' ').Append(_enumDemiliter).Append(' ').Append(_value);

            if (!saveParameter.IsForceInline)
                sb.Append(Constants.NEW_LINE);
        }

        public SaveAdapter GetSaveAdapter() => null;

    }
}
