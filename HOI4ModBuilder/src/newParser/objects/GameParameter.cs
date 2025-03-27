using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.parser.objects;
using HOI4ModBuilder.src.parser.parameter;
using HOI4ModBuilder.src.parser;
using System;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameParameter<T> : IGameParameter where T : new()
    {
        private bool _needToSave;
        public bool IsNeedToSave() => _needToSave;
        public void SetNeedToSave(bool needToSave) => _needToSave = needToSave;


        private GameConstantOld _constant;
        public GameConstantOld Constant { get => _constant; set => _constant = value; }

        private GameComments _comments;
        public GameComments comments { get => _comments; set => Utils.Setter(ref _comments, ref value, ref _needToSave); }

        private EnumDemiliter _enumDemiliter;
        private bool _isAnyDemiliter;
        public bool IsAnyDemiliter() => _isAnyDemiliter;
        private bool _isQuoted;
        public bool IsQuoted() => _isQuoted;

        private T _value;
        public T GetValue() => _constant != null ? _constant.GetValue<T>() : _value;
        public void SetValue(T value)
        {
            if (_value == null && value != null || !_value.Equals(value))
            {
                if (value is GameConstantOld valueConstant)
                    _constant = valueConstant;
                else if (value is string valueString && valueString.StartsWith("@"))
                    ParseValueConstant(null, valueString);
                else
                {
                    _value = value;
                    Constant = null;
                }

                _needToSave = true;
            }
        }

        public void InitValue() => _value = new T();
        public void InitValueIfNull()
        {
            if (_value == null) InitValue();
        }


        private IParentable _parent;
        public IParentable GetParent() => _parent;
        public void SetParent(IParentable parent) => _parent = parent;
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
            GameConstantOld tempConstant = null;
            while (tempParent != null)
            {
                if (
                    tempParent is IConstantsOld iConstants &&
                    iConstants.TryGetConstant(rawValue, out tempConstant)
                ) break;

                tempParent = tempParent.GetParent();
            }

            if (tempConstant != null)
                _constant = tempConstant;
            else throw new Exception("Constant with name " + rawValue + " not found or this scope does not support constants: " + parser?.GetCursorInfo());
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
    }
}
