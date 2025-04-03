using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameDictionary<TKey, TValue> : AbstractParseObject, IDictionary<TKey, TValue> where TValue : new()
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        private Func<string, TKey> _keyParseAdapter;
        private Func<TKey, object> _keySaveAdapter;
        private Func<object, string, TValue> _valueParseAdapter;
        private Func<TValue, object> _valueSaveAdapter;

        public GameDictionary<TKey, TValue> INIT_SetKeyParseAdapter(Func<string, TKey> value)
        {
            _keyParseAdapter = value;
            return this;
        }
        public GameDictionary<TKey, TValue> INIT_SetKeyParseAdapter(Func<TKey, object> value)
        {
            _keySaveAdapter = value;
            return this;
        }

        public GameDictionary<TKey, TValue> INIT_SetValueParseAdapter(Func<object, string, TValue> value)
        {
            _valueParseAdapter = value;
            return this;
        }
        public GameDictionary<TKey, TValue> INIT_SetValueParseAdapter(Func<TValue, object> value)
        {
            _valueSaveAdapter = value;
            return this;
        }

        public new bool IsNeedToSave()
        {
            if (base.IsNeedToSave())
                return true;

            foreach (var entry in _dictionary)
            {
                if (entry.Key is INeedToSave keyINeedToSave && keyINeedToSave.IsNeedToSave() ||
                    entry.Value is INeedToSave valueINeedToSave && valueINeedToSave.IsNeedToSave())
                {
                    return true;
                }
            }

            return false;
        }

        public override void ParseCallback(GameParser parser)
        {
            if (parser.SkipWhiteSpaces())
                throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

            parser.ParseDemiliters();
            var demiliters = parser.PullParsedDataString();

            if (demiliters.Length != 1 || demiliters[0] != '=')
                throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

            if (parser.SkipWhiteSpaces())
                throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

            if (parser.CurrentToken == Token.LEFT_CURLY)
                TryParseBlockKeys(parser);
            else
                throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());
        }

        private void TryParseBlockKeys(GameParser parser)
        {
            parser.ParseInsideBlock(
                (comments) => SetComments(comments),
                (tokenComments, token) =>
                {
                    TKey keyObj = _keyParseAdapter != null ?
                        _keyParseAdapter(token) :
                        ParserUtils.Parse<TKey>(token);

                    if (parser.SkipWhiteSpaces())
                        throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

                    parser.ParseDemiliters();
                    var demiliters = parser.PullParsedDataString();

                    if (demiliters.Length != 1)
                        throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

                    if (demiliters[0] != '=')
                        throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

                    if (parser.SkipWhiteSpaces())
                        throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

                    if (parser.CurrentToken == Token.LEFT_CURLY)
                    {
                        parser.NextChar();
                        var newComments = parser.ParseAndPullComments();

                        if (tokenComments == null)
                            tokenComments = newComments;
                        else if (newComments != null)
                            tokenComments.Inline = newComments.Inline;

                        if (keyObj is ICommentable commentable)
                            commentable.SetComments(tokenComments);

                        var valueObj = _valueParseAdapter != null ?
                            _valueParseAdapter.Invoke(keyObj, null) :
                            new TValue();

                        if (valueObj is IParseObject parseObject)
                            parser.Parse(parseObject);

                        _dictionary[keyObj] = valueObj;
                        return false;
                    }
                    else
                    {

                        if (parser.CurrentToken == Token.QUOTE)
                            parser.ParseQuoted();
                        else
                            parser.ParseUnquotedValue();

                        var value = parser.PullParsedDataString();

                        var valueObj = _valueParseAdapter != null ?
                            _valueParseAdapter.Invoke(keyObj, value) :
                            ParserUtils.Parse<TValue>(value);

                        var newComments = parser.ParseAndPullComments();

                        if (tokenComments == null)
                            tokenComments = newComments;
                        else if (newComments != null)
                            tokenComments.Inline = newComments.Inline;

                        if (keyObj is ICommentable commentable)
                            commentable.SetComments(tokenComments);

                        if (valueObj is IParseObject parseObject)
                            parser.Parse(parseObject);

                        _dictionary[keyObj] = valueObj;
                        return false;
                    }
                }
            );
        }

        public override void Save(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter)
        {
            if (_dictionary.Count == 0)
            {
                if (!saveParameter.SaveIfEmpty)
                    return;

                if (saveParameter.AddEmptyLineBefore)
                    sb.Append(outIndent).Append(Constants.NEW_LINE);

                sb.Append(outIndent).Append(key).Append(" = {}").Append(Constants.NEW_LINE);
                return;
            }

            if (saveParameter.AddEmptyLineBefore)
                sb.Append(outIndent).Append(Constants.NEW_LINE);

            EntrySave(parser, sb, outIndent, key, saveParameter);

            return;
        }

        private void EntrySave(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter)
        {
            string innerIndent = outIndent;

            if (key != null)
            {
                var comments = GetComments();
                if (comments == null)
                    comments = GameComments.DEFAULT;

                if (comments.Previous.Length > 0)
                    sb.Append(outIndent).Append(comments.Previous).Append(Constants.NEW_LINE);

                sb.Append(outIndent).Append(key).Append(" = {");

                if (comments.Inline.Length > 0)
                    sb.Append(' ').Append(comments.Inline);

                if (!saveParameter.IsForceInline && _dictionary.Count > 1)
                {
                    sb.Append(Constants.NEW_LINE);
                    innerIndent = outIndent + Constants.INDENT;
                }
                else
                    innerIndent = " ";
            }

            foreach (var entry in _dictionary)
            {
                object tempKey = entry.Key;
                if (_keySaveAdapter != null)
                    tempKey = _keySaveAdapter.Invoke(entry.Key);

                string stringKey = ParserUtils.ObjectToSaveString(tempKey);

                object tempValue = entry.Value;
                if (_valueSaveAdapter != null)
                    tempValue = _valueSaveAdapter.Invoke(entry.Value);

                if (tempValue is ISaveable saveable)
                {
                    innerIndent = outIndent + Constants.INDENT;
                    saveable.Save(parser, sb, innerIndent, stringKey, saveParameter);
                }
                else
                {
                    var comments = GameComments.DEFAULT;
                    if (entry.Key is ICommentable commentable)
                        comments = commentable.GetComments();

                    if (comments.Previous.Length > 0)
                    {
                        if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
                        {
                            sb.Append(Constants.NEW_LINE);
                            innerIndent = outIndent + Constants.INDENT;
                        }
                        sb.Append(innerIndent).Append(comments.Previous).Append(Constants.NEW_LINE);
                    }

                    var stringValue = ParserUtils.ObjectToSaveString(tempValue);
                    sb.Append(innerIndent).Append(stringKey).Append(' ').Append((char)EnumDemiliter.EQUALS).Append(' ').Append(stringValue);

                    if (comments.Inline.Length > 0)
                    {
                        sb.Append(' ').Append(comments.Inline);
                        sb.Append(Constants.NEW_LINE);
                        innerIndent = outIndent + Constants.INDENT;
                    }
                    else if (!saveParameter.IsForceInline)
                    {
                        sb.Append(Constants.NEW_LINE);
                        innerIndent = outIndent + Constants.INDENT;
                    }
                }
            }

            if (key != null)
            {
                if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
                    sb.Append(outIndent);
                else
                    sb.Append(' ');

                sb.Append('}');
                sb.Append(Constants.NEW_LINE);
            }
        }

        public override SaveAdapter GetSaveAdapter() => null;
        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => null;
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;

        public override void Validate(LinkedLayer layer)
        {
            foreach (var entry in _dictionary)
            {
                if (entry.Key is IValidatable keyValidatable)
                    keyValidatable.Validate(layer);
                if (entry.Value is IValidatable valueValidatable)
                    valueValidatable.Validate(layer);
            }
            base.Validate(layer);
        }

        public override IParseObject GetEmptyCopy() => new GameDictionary<TKey, TValue>();

        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public TValue this[TKey key] { get => _dictionary[key]; set => _dictionary[key] = value; }

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public void Add(TKey key, TValue value) => _dictionary.Add(key, value);

        public bool Remove(TKey key) => _dictionary.Remove(key);

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item) => _dictionary.Add(item.Key, item.Value);

        public void Clear() => _dictionary.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

    }
}
