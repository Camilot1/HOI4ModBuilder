using HOI4ModBuilder.src.newParser.interfaces;
using System;
using System.Text;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.newParser.objects
{
    public enum EnumDemiliter
    {
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
        public IParentable GetParentRecursive(Func<IParentable, bool> checkFunc)
        {
            var tempParent = GetParent();
            while (tempParent != null)
            {
                if (checkFunc(tempParent))
                    return tempParent;
                tempParent = tempParent.GetParent();
            }
            return null;
        }
        public bool TryGetParentRecursive(Func<IParentable, bool> checkFunc, out IParentable matchedParent)
            => (matchedParent = GetParentRecursive(checkFunc)) != null;
        public void SetParent(IParentable parent) => _parent = parent;

        private GameComments _comments;
        public GameComments GetComments() => _comments;
        public void SetComments(GameComments comments) => _comments = comments;

        private EnumDemiliter _enumDemiliter = EnumDemiliter.EQUALS;
        private bool _isAnyDemiliter;
        public bool IsAnyDemiliter() => _isAnyDemiliter;

        private object _value;
        public object GetValueRaw() => _value;
        public T GetValue() => _value is GameConstant gameConstant ? gameConstant.GetValue<T>() : (_value != null ? (T)_value : default);
        public T GetValue(T defaultValue)
        {
            T value = GetValue();
            if (value != null)
                return value;
            return default;
        }
        public T GetValueRaw(T defaultValue)
        {
            if (_value == null)
                return defaultValue;
            else
                return (T)_value;
        }

        public void SetValue(T value)
        {
            if (_value == null && value != null || _value != null && !_value.Equals(value))
            {
                if (_valueSetAdapter != null)
                    _value = _valueSetAdapter(this, value);
                if (value is GameConstant valueConstant)
                    _value = valueConstant;
                else if (value is GameString valueString && valueString.stringValue.StartsWith("@"))
                    ParseValueConstant(null, valueString.stringValue);
                else
                    _value = value;

                _needToSave = true;
            }
        }
        public void SetSilentValue(T value) => _value = value;

        //obj, value, result
        private bool _forceValueInline;
        private Func<object, object, T> _valueParseAdapter;
        private Func<object, object, T> _valueSetAdapter;
        private Func<T, object> _valueSaveAdapter;
        public GameParameter<T> INIT_ForceValueInline(bool value)
        {
            _forceValueInline = value;
            return this;
        }
        public GameParameter<T> INIT_SetValueParseAdapter(Func<object, object, T> value)
        {
            _valueParseAdapter = value;
            return this;
        }
        public GameParameter<T> INIT_SetValueSetAdapter(Func<object, object, T> value)
        {
            _valueSetAdapter = value;
            return this;
        }

        public GameParameter<T> INIT_SetValueSaveAdapter(Func<T, object> value)
        {
            _valueSaveAdapter = value;
            return this;
        }

        private bool _isProhibitedOverwriting;
        public GameParameter<T> INIT_ProhibitOverwriting(bool flag)
        {
            _isProhibitedOverwriting = flag;
            return this;
        }

        public void InitValue()
        {
            _value = new T();
            if (_value is IParentable parentable)
                parentable.SetParent(this);
        }

        public void InitValueIfNull()
        {
            if (_value == null)
                InitValue();
        }

        public string AssemblePath() => ParserUtils.AsseblePath(this);

        public virtual void ParseCallback(GameParser parser)
        {
            parser.SkipWhiteSpaces();

            parser.ParseDemiliters();
            var demiliter = parser.PullParsedDataString();

            if (demiliter.Length != 1)
                throw new Exception("Invalid demiliter: " + demiliter + ": " + parser.GetCursorInfo());

            if (demiliter[0] != '=' && !_isAnyDemiliter)
                throw new Exception("Invalid demiliter: " + demiliter + ": " + parser.GetCursorInfo());
            else
                _enumDemiliter = (EnumDemiliter)demiliter[0];

            parser.SkipWhiteSpaces();

            if (_value == null)
                InitValue();
            else if (_isProhibitedOverwriting)
                throw new Exception("Value cannot be overriden: " + parser.GetCursorInfo());

            if (!_forceValueInline)
            {
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
            }

            if (parser.CurrentToken == Token.QUOTE)
                parser.ParseQuoted();
            else
                parser.ParseUnquotedValue();

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
            {
                if (_valueParseAdapter != null)
                    _value = _valueParseAdapter.Invoke(this, tempConstant);
                else
                    _value = tempConstant;
            }
            else if (parser == null)
            {
                if (_valueParseAdapter != null)
                    _value = _valueParseAdapter.Invoke(this, rawValue);
                else
                    _value = rawValue;
            }
            else
                throw new Exception("Constant with name " + rawValue + " not found or this scope does not support constants: " + parser?.GetCursorInfo());
        }

        private void ParseValueRaw(GameParser parser, string rawValue)
        {
            try
            {
                _value = _valueParseAdapter != null ?
                    _valueParseAdapter.Invoke(this, rawValue) :
                    ParserUtils.Parse<T>(rawValue);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to parse value type: {rawValue} {parser.GetCursorInfo()}", ex);
            }
        }

        public virtual void Save(StringBuilder sb, string outIndent, string key, SavePatternParameter savePatternParameter)
        {
            if (_value is ISaveable saveable && !_forceValueInline)
            {
                saveable.Save(sb, outIndent, key, savePatternParameter);
                return;
            }
            else if (_value == null)
                return;

            if (savePatternParameter.AddEmptyLineBefore)
                sb.Append(outIndent).Append(Constants.NEW_LINE);

            var comments = GetComments();
            comments?.SavePrevComments(sb, outIndent);

            sb.Append(outIndent).Append(key).Append(' ').Append((char)_enumDemiliter).Append(' ');

            if (_valueSaveAdapter != null && _value is T tValue)
                sb.Append(ParserUtils.ObjectToSaveString(_valueSaveAdapter.Invoke(tValue)));
            else
                sb.Append(ParserUtils.ObjectToSaveString(_value));

            sb.Append(' ');

            if (comments != null && comments.Inline.Length > 0)
                sb.Append(comments.Inline).Append(Constants.NEW_LINE);
            else if (!savePatternParameter.IsForceInline)
                sb.Append(Constants.NEW_LINE);
        }

        public SavePattern GetSavePattern() => null;

        public virtual void Validate(LinkedLayer layer)
        {
            if (_value is IValidatable validatable)
                validatable.Validate(layer);
        }
        public bool TryGetGameFile(out GameFile gameFile)
        {
            IParentable temp = this;

            while (temp != null)
            {
                if (temp is GameFile)
                {
                    gameFile = (GameFile)temp;
                    return true;
                }
                temp = temp.GetParent();
            }
            gameFile = null;
            return false;
        }

        public GameFile GetGameFile()
        {
            if (TryGetGameFile(out var gameFile))
                return gameFile;
            return null;
        }
    }
}
