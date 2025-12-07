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
    public class GameDictionary<TKey, TValue> : AbstractParseObject, IDictionary<TKey, TValue>, IKeyValuePushable where TValue : new()
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        private bool _forceValueInline;
        private Func<GameDictionary<TKey, TValue>, string, TKey> _keyParseAdapter;
        private Func<GameDictionary<TKey, TValue>, TKey, object> _keySaveAdapter;
        private Func<GameDictionary<TKey, TValue>, object, string, TValue> _valueParseAdapter;
        private Func<GameDictionary<TKey, TValue>, TValue, object> _valueSaveAdapter;
        private bool _sortAtSaving;
        private bool _checkForPreload;

        public GameDictionary<TKey, TValue> INIT_SetCheckForPreload(bool value)
        {
            _checkForPreload = value;
            return this;
        }

        public GameDictionary<TKey, TValue> INIT_ForceValueInline(bool value)
        {
            _forceValueInline = value;
            return this;
        }
        public GameDictionary<TKey, TValue> INIT_SetKeyParseAdapter(Func<GameDictionary<TKey, TValue>, string, TKey> value)
        {
            _keyParseAdapter = value;
            return this;
        }
        public GameDictionary<TKey, TValue> INIT_SetKeySaveAdapter(Func<GameDictionary<TKey, TValue>, TKey, object> value)
        {
            _keySaveAdapter = value;
            return this;
        }

        public GameDictionary<TKey, TValue> INIT_SetValueParseAdapter(Func<GameDictionary<TKey, TValue>, object, string, TValue> value)
        {
            _valueParseAdapter = value;
            return this;
        }
        public GameDictionary<TKey, TValue> INIT_SetValueSaveAdapter(Func<GameDictionary<TKey, TValue>, TValue, object> value)
        {
            _valueSaveAdapter = value;
            return this;
        }

        public GameDictionary<TKey, TValue> INIT_SetSortAtSaving(bool value)
        {
            _sortAtSaving = value;
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
            parser.SkipWhiteSpaces();

            ParserUtils.ParseEqualsDemiliter(parser);

            parser.SkipWhiteSpaces();

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
                    var keyObj = ParseKey(token);
                    ParseKeyValue(parser, keyObj, tokenComments);
                    return false;
                }
            );
        }

        private TKey ParseKey(string token)
        {
            var keyObj = _keyParseAdapter != null ?
                _keyParseAdapter(this, token) :
                ParserUtils.Parse<TKey>(token);

            if (keyObj is IParentable keyParentable)
                keyParentable.SetParent(this);

            return keyObj;
        }

        private void ParseKeyValue(GameParser parser, TKey keyObj, GameComments tokenComments)
        {
            parser.SkipWhiteSpaces();
            ParserUtils.ParseEqualsDemiliter(parser);
            parser.SkipWhiteSpaces();

            if (parser.CurrentToken == Token.LEFT_CURLY)
            {
                parser.NextChar();
                ParseBlockValue(parser, keyObj, tokenComments);
            }
            else
            {
                if (parser.CurrentToken == Token.QUOTE)
                    parser.ParseQuoted();
                else
                    parser.ParseUnquotedValue();

                var value = parser.PullParsedDataString();
                ParseInlineValue(parser, keyObj, tokenComments, value);
            }
        }

        private void ParseBlockValue(GameParser parser, TKey keyObj, GameComments tokenComments)
        {
            var mergedComments = MergeComments(tokenComments, parser.ParseAndPullComments());
            if (keyObj is ICommentable commentableKey)
                commentableKey.SetComments(mergedComments);

            var valueObj = CreateValue(keyObj, null);

            if (_checkForPreload && TryGetGameFile(out var gameFile) && gameFile.IsPreload)
            {
                parser.SkipCurrentBlock();
            }
            else if (valueObj is IParseObject parseObject)
                parser.Parse(parseObject);

            _dictionary[keyObj] = valueObj;
        }

        private void ParseInlineValue(GameParser parser, TKey keyObj, GameComments tokenComments, string rawValue)
        {
            var valueObj = CreateValue(keyObj, rawValue);

            var mergedComments = MergeComments(tokenComments, parser.ParseAndPullComments());
            if (keyObj is ICommentable commentableKey)
                commentableKey.SetComments(mergedComments);

            if (valueObj is IParseObject parseObject)
                parser.Parse(parseObject);

            _dictionary[keyObj] = valueObj;
        }

        private TValue CreateValue(TKey keyObj, string rawValue)
        {
            var valueObj = _valueParseAdapter != null ?
                _valueParseAdapter.Invoke(this, keyObj, rawValue) :
                rawValue == null ? new TValue() : ParserUtils.Parse<TValue>(rawValue);

            if (valueObj is IParentable parentable)
                parentable.SetParent(this);

            return valueObj;
        }

        private static GameComments MergeComments(GameComments primary, GameComments secondary)
        {
            if (primary == null)
                return secondary;

            if (secondary == null)
                return primary;

            if (secondary.Previous.Length > 0)
            {
                var merged = new string[primary.Previous.Length + secondary.Previous.Length];
                primary.Previous.CopyTo(merged, 0);
                secondary.Previous.CopyTo(merged, primary.Previous.Length);
                primary.Previous = merged;
            }

            if (!string.IsNullOrEmpty(secondary.Inline))
                primary.Inline = secondary.Inline;
            return primary;
        }

        public override void Save(StringBuilder sb, string outIndent, string key, SavePatternParameter savePatternParameter)
        {
            if (_dictionary.Count == 0)
            {
                if (!savePatternParameter.SaveIfEmpty)
                    return;

                if (savePatternParameter.AddEmptyLineBefore)
                    sb.Append(outIndent).Append(Constants.NEW_LINE);

                sb.Append(outIndent).Append(key).Append(" = {}").Append(Constants.NEW_LINE);
                return;
            }

            if (savePatternParameter.AddEmptyLineBefore)
                sb.Append(outIndent).Append(Constants.NEW_LINE);

            EntrySave(sb, outIndent, key, savePatternParameter);

            return;
        }

        private void EntrySave(StringBuilder sb, string outIndent, string key, SavePatternParameter savePatternParameter)
        {
            string innerIndent = outIndent;

            if (key != null)
            {
                var comments = GetComments();
                if (comments == null)
                    comments = new GameComments();

                comments.SavePrevComments(sb, outIndent);

                sb.Append(outIndent).Append(key).Append(" = { ");

                if (comments.Inline.Length > 0)
                    sb.Append(comments.Inline).Append(' ');

                if (!savePatternParameter.IsForceInline && _dictionary.Count > 1)
                {
                    sb.Append(Constants.NEW_LINE);
                    innerIndent = outIndent + Constants.INDENT;
                }
                else
                    innerIndent = " ";
            }

            if (_sortAtSaving)
            {
                List<TKey> sortKeys = new List<TKey>(_dictionary.Keys);
                sortKeys.Sort();
                foreach (var sortedKey in sortKeys)
                    SaveKeyValueEntry(sb, outIndent, ref innerIndent, sortedKey, _dictionary[sortedKey], savePatternParameter);
            }
            else
            {
                foreach (var entry in _dictionary)
                    SaveKeyValueEntry(sb, outIndent, ref innerIndent, entry.Key, entry.Value, savePatternParameter);
            }

            if (key != null)
            {
                if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
                    sb.Append(outIndent);

                sb.Append("} ").Append(Constants.NEW_LINE);
            }
        }

        private void SaveKeyValueEntry(StringBuilder sb, string outIndent, ref string innerIndent, TKey key, TValue value, SavePatternParameter savePatternParameter)
        {
            object tempKey = key;
            if (_keySaveAdapter != null)
                tempKey = _keySaveAdapter.Invoke(this,key);

            string stringKey = ParserUtils.ObjectToSaveString(tempKey);

            object tempValue = value;
            if (_valueSaveAdapter != null)
                tempValue = _valueSaveAdapter.Invoke(this,value);

            if (sb.Length > 0)
            {
                var lastChar = sb[sb.Length - 1];
                if (!savePatternParameter.IsForceInline && lastChar == ' ' && _dictionary.Count > 1)
                    sb.Append(' ').Append(Constants.NEW_LINE);
            }

            if (tempValue is ISaveable saveable && !_forceValueInline)
            {
                if (savePatternParameter.IsForceInline)
                {
                    var inlineParameter = savePatternParameter;
                    inlineParameter.IsForceInline = true;

                    var tempSb = new StringBuilder();
                    saveable.Save(tempSb, string.Empty, stringKey, inlineParameter);

                    var inlineValue = tempSb.ToString().Trim();
                    if (inlineValue.Length > 0)
                    {
                        if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
                            sb.Append(innerIndent);
                        else if (sb.Length > 0 && sb[sb.Length - 1] != ' ')
                            sb.Append(' ');

                        sb.Append(inlineValue).Append(' ');
                    }
                }
                else
                {
                    var lastChar = sb[sb.Length - 1];
                    if (lastChar == ' ')
                        sb.Append(Constants.NEW_LINE);
                    saveable.Save(sb, outIndent, stringKey, savePatternParameter);
                }
            }
            else
            {
                GameComments comments = null;
                if (key is ICommentable commentable)
                    comments = commentable.GetComments();

                if (comments == null)
                    comments = new GameComments();

                if (comments.Previous.Length > 0)
                {
                    if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
                    {
                        sb.Append(Constants.NEW_LINE);
                        innerIndent = outIndent + Constants.INDENT;
                    }
                    comments.SavePrevComments(sb, innerIndent);
                }

                var stringValue = ParserUtils.ObjectToSaveString(tempValue);

                if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
                    sb.Append(innerIndent);
                sb.Append(stringKey).Append(' ').Append((char)EnumDemiliter.EQUALS).Append(' ').Append(stringValue).Append(' ');

                if (comments.Inline.Length > 0)
                {
                    sb.Append(comments.Inline).Append(' ').Append(Constants.NEW_LINE);
                    innerIndent = outIndent + Constants.INDENT;
                }
                else if (!savePatternParameter.IsForceInline && _dictionary.Count > 1)
                {
                    sb.Append(Constants.NEW_LINE);
                }
            }
        }

        public override SavePattern GetSavePattern() => null;
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

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < _dictionary.Count)
                throw new ArgumentException("The target array is too small.", nameof(array));

            int i = arrayIndex;
            foreach (var kvp in _dictionary)
                array[i++] = kvp;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary.TryGetValue(item.Key, out var value) &&
                EqualityComparer<TValue>.Default.Equals(value, item.Value))
                return _dictionary.Remove(item.Key);

            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        public bool SortIfNeeded()
        {
            bool needToSort = false;

            if (_dictionary.Count < 2)
                return needToSort;

            bool firstIsSet = false;
            TKey tempKey = default;

            foreach (var key in _dictionary.Keys)
            {
                if (!firstIsSet)
                {
                    tempKey = key;
                    if (!(tempKey is IComparable))
                        return needToSort;
                    firstIsSet = true;
                }
                else if ((tempKey as IComparable).CompareTo(key) > 0)
                {
                    needToSort = true;
                    break;
                }
            }

            if (needToSort)
            {
                var keys = new List<TKey>(_dictionary.Keys);
                keys.Sort();

                var values = new List<TValue>(_dictionary.Count);

                foreach (var sortedKey in keys)
                    values.Add(_dictionary[sortedKey]);

                _dictionary.Clear();

                for (int i = 0; i < keys.Count; i++)
                    _dictionary[keys[i]] = values[i];
            }

            return needToSort;
        }

        public void Push(string key, object value)
        {
            object tempKey = key;
            if (_keyParseAdapter != null)
                tempKey = _keyParseAdapter.Invoke(this,key);
            else
                tempKey = ParserUtils.Parse<TKey>(key);

            Logger.TryOrCatch(
                () => _dictionary[(TKey)tempKey] = (TValue)value,
                (ex) => Logger.LogException(
                    $"Could not cast KeyValuePair \"{tempKey}\" ({key.GetType()}) = \"{value}\" ({value.GetType()}) " +
                    $"to ({nameof(TKey)}) = ({nameof(TValue)}). Exception message: {ex.Message}",
                    ex));
        }

        public int RemoveEntryIf(Func<TKey, TValue, bool> checkFunc)
        {
            if (checkFunc == null)
                return 0;

            List<TKey> keysToRemove = new List<TKey>();
            foreach (var entry in _dictionary)
                if (checkFunc.Invoke(entry.Key, entry.Value))
                    keysToRemove.Add(entry.Key);

            foreach (var key in keysToRemove)
                _dictionary.Remove(key);

            return keysToRemove.Count;
        }
    }
}
