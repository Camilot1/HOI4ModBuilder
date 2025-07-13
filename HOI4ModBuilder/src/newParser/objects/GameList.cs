
using System.Collections.Generic;
using System;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using System.Collections;
using System.Text;
using HOI4ModBuilder.src.utils;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameList<T> : AbstractParseObject, IValuePushable, ISizable, IEnumerable<T> where T : new()
    {
        private bool _allowsInlineAdd;
        private bool _forceSeparateLineSave;
        private Func<object, string, T> _valueParseAdapter;
        private Func<T, object> _valueSaveAdapter;
        private bool _sortAtSaving;

        public GameList() : base() { }
        public GameList(bool allowsInlineAdd, bool forceSeparateLineSave) : this()
        {
            _allowsInlineAdd = allowsInlineAdd;
            _forceSeparateLineSave = forceSeparateLineSave;
        }
        public GameList<T> INIT_SetAllowsInlineAdd(bool value)
        {
            _allowsInlineAdd = value;
            return this;
        }
        public GameList<T> INIT_SetForceSeparateLineSave(bool value)
        {
            _forceSeparateLineSave = value;
            return this;
        }
        public GameList<T> INIT_SetValueParseAdapter(Func<object, string, T> value)
        {
            _valueParseAdapter = value;
            return this;
        }
        public GameList<T> INIT_SetValueSaveAdapter(Func<T, object> value)
        {
            _valueSaveAdapter = value;
            return this;
        }

        public GameList<T> INIT_SetSortAtSaving(bool value)
        {
            _sortAtSaving = value;
            return this;
        }

        public new bool IsNeedToSave()
        {
            if (base.IsNeedToSave())
                return true;

            if (_list is List<AbstractParseObject> @list)
                foreach (var item in @list)
                    if (item.IsNeedToSave())
                        return true;

            return false;
        }

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => null;
        public override Dictionary<string, DynamicGameParameter> GetDynamicAdapter() => null;
        public override void ParseCallback(GameParser parser)
        {
            parser.SkipWhiteSpaces();

            parser.ParseDemiliters();
            var demiliters = parser.PullParsedDataString();

            if (demiliters.Length != 1 || demiliters[0] != '=')
                throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

            parser.SkipWhiteSpaces();

            if (parser.CurrentToken == Token.LEFT_CURLY)
                TryParseBlockValue(parser);
            else
                TryParseInlineValue(parser);
        }

        private void TryParseBlockValue(GameParser parser)
        {
            parser.ParseInsideBlock(
                (comments) => SetComments(comments),
                (tokenComments, token) =>
                {
                    T obj = _valueParseAdapter != null ?
                        _valueParseAdapter(this, token) :
                        ParserUtils.Parse<T>(token);

                    if (obj is IParentable parentable)
                        parentable.SetParent(this);

                    if (obj == null)
                        throw new Exception("Parsed value is null for token \"" + token + "\". It is not defined and not found while parsing: " + parser.GetCursorInfo());

                    if (obj is IParseObject parseObject)
                        parseObject.ParseCallback(parser);
                    else if (obj is ICommentable commentable)
                        commentable.SetComments(tokenComments);
                    AddSilent(obj);

                    return false;
                }
            );
        }

        private void TryParseInlineValue(GameParser parser)
        {
            if (parser.CurrentToken == Token.QUOTE)
                parser.ParseQuoted();
            else
                parser.ParseUnquotedValue();

            var value = parser.PullParsedDataString();
            if (!_allowsInlineAdd || value.Length == 0)
                throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

            var obj = _valueParseAdapter != null ?
                _valueParseAdapter.Invoke(this, value) :
                ParserUtils.Parse<T>(value);

            if (obj is IParentable parentable)
                parentable.SetParent(this);

            if (obj is IParseObject parseObject)
                parseObject.ParseCallback(parser);
            else
            {
                var comments = parser.ParseAndPullComments();
                if (obj is ICommentable commentable)
                    commentable.SetComments(comments);
            }

            AddSilent(obj);
        }

        public override IParseObject GetEmptyCopy() => new GameList<T>();


        private List<T> _list = new List<T>();

        public int RemoveIf(Func<T, bool> predicate)
        {
            int count = 0;
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (predicate(_list[i]))
                {
                    _list.RemoveAt(i);
                    count++;
                }
            }
            return count;
        }

        public bool RemoveLastIf(Func<T, bool> predicate)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (predicate(_list[i]))
                {
                    _list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool TryGetFirstFromEndIf(Func<T, bool> predicate, out T result)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (predicate(_list[i]))
                {
                    result = _list[i];
                    return true;
                }
            }
            result = default;
            return false;
        }

        public bool HasAny(Func<T, bool> predicate)
        {
            foreach (var obj in _list)
            {
                if (predicate(obj))
                    return true;
            }
            return false;
        }

        public bool RemoveAllExceptLast(Func<T, bool> predicate, out int matchCount, out int lastResultIndex)
        {
            bool isLast = true;
            bool isRemovedAny = false;
            matchCount = 0;
            lastResultIndex = -1;
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (predicate(_list[i]))
                {
                    matchCount++;
                    if (isLast)
                    {
                        isLast = false;
                        lastResultIndex = i;
                        continue;
                    }
                    else
                    {
                        _list.RemoveAt(i);
                        lastResultIndex--;
                        isRemovedAny = true;
                    }
                }
            }
            return isRemovedAny;
        }

        public int GetSize() => _list.Count;

        public T this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        public void Push(object value) => AddSilent((T)value);
        public void AddSilent(T item) => _list.Add(item);
        public void Add(T item)
        {
            _needToSave = true;
            _list.Add(item);
        }
        public void Sort() => _list.Sort();
        public void Sort(Comparison<T> comparison) => _list.Sort(comparison);
        public T Get(int index) => _list[index];
        public void RemoveAt(int index)
        {
            _needToSave = true;
            _list.RemoveAt(index);
        }
        public bool Remove(T item)
        {
            var result = _list.Remove(item);
            _needToSave |= result;
            return result;
        }
        public bool Contains(T obj) => _list.Contains(obj);
        public int Count => _list.Count;

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //TODO impelemnt save
        public override SavePattern GetSavePattern() => null;
        public override void Save(StringBuilder sb, string outIndent, string key, SavePatternParameter savePatternParameter)
        {
            if (_list.Count == 0)
            {
                if (_forceSeparateLineSave)
                    return;
                if (!savePatternParameter.SaveIfEmpty)
                    return;

                if (savePatternParameter.AddEmptyLineBefore)
                    sb.Append(outIndent).Append(Constants.NEW_LINE);

                sb.Append(outIndent).Append(key).Append(" = {}").Append(Constants.NEW_LINE);
                return;
            }

            if (savePatternParameter.AddEmptyLineBefore)
                sb.Append(outIndent).Append(Constants.NEW_LINE);

            if (_sortAtSaving)
                _list.Sort();

            if (_forceSeparateLineSave)
                SeparateLineSave(sb, outIndent, key, savePatternParameter);
            else
                ListSave(sb, outIndent, key, savePatternParameter);
        }

        private void SeparateLineSave(StringBuilder sb, string outIndent, string key, SavePatternParameter savePatternParameter)
        {
            foreach (var value in _list)
            {
                object tempValue = value;
                if (_valueSaveAdapter != null)
                    tempValue = _valueSaveAdapter.Invoke(value);

                if (tempValue is ISaveable saveable)
                    saveable.Save(sb, outIndent, key, savePatternParameter);
                else
                {
                    GameComments comments = null;
                    if (value is ICommentable commentable)
                        comments = commentable.GetComments();

                    comments?.SavePrevComments(sb, outIndent);

                    sb.Append(outIndent).Append(key).Append(" = ").Append(tempValue);

                    if (comments != null && comments.Inline.Length > 0)
                        sb.Append(' ').Append(comments.Inline);

                    sb.Append(Constants.NEW_LINE);
                }
            }
        }

        private void ListSave(StringBuilder sb, string outIndent, string key, SavePatternParameter savePatternParameter)
        {
            string innerIndent = outIndent;

            if (key != null)
            {
                var comments = GetComments();
                if (comments == null)
                    comments = new GameComments();

                var parent = GetParent();
                if (parent is ICommentable parentCommentable)
                {
                    var innerComments = parentCommentable.GetComments();
                    if (innerComments == null)
                        innerComments = new GameComments();

                    if (innerComments.Previous.Length > 0)
                        comments.Previous = innerComments.Previous;
                    if (innerComments.Inline.Length > 0)
                        comments.Inline = innerComments.Inline;
                }

                comments.SavePrevComments(sb, outIndent);

                sb.Append(outIndent).Append(key).Append(" = { ");

                if (comments.Inline.Length > 0)
                {
                    sb.Append(comments.Inline).Append(Constants.NEW_LINE);
                    innerIndent = outIndent + Constants.INDENT;
                }
                else if (savePatternParameter.IsForceMultiline || !savePatternParameter.IsForceInline && _list.Count > 0)
                {
                    sb.Append(Constants.NEW_LINE);
                    innerIndent = outIndent + Constants.INDENT;
                }
                else
                    innerIndent = " ";
            }

            if (_sortAtSaving)
                _list.Sort();

            foreach (var value in _list)
                SaveListValue(sb, outIndent, ref innerIndent, key, value, savePatternParameter);

            if (key != null)
            {
                if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
                    sb.Append(outIndent);

                sb.Append('}');
                sb.Append(Constants.NEW_LINE);
            }
        }

        private void SaveListValue(StringBuilder sb, string outIndent, ref string innerIndent, string key, T value, SavePatternParameter savePatternParameter)
        {
            object tempValue = value;
            if (_valueSaveAdapter != null)
                tempValue = _valueSaveAdapter.Invoke(value);

            if (value is ISaveable saveable)
            {
                saveable.Save(sb, innerIndent, key, savePatternParameter);

                if (sb.Length > 0)
                {
                    var lastChar = sb[sb.Length - 1];
                    if (lastChar == ' ' && !savePatternParameter.IsForceInline)
                        sb.Append(Constants.NEW_LINE);
                }
            }
            else
            {
                var comments = new GameComments();
                if (value is ICommentable commentable)
                    comments = commentable.GetComments();

                if (comments.Previous.Length > 0)
                {
                    if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
                    {
                        sb.Append(Constants.NEW_LINE);
                        innerIndent = outIndent + Constants.INDENT;
                    }
                    comments.SavePrevComments(sb, innerIndent);
                }

                sb.Append(tempValue).Append(innerIndent);

                if (comments.Inline.Length > 0)
                {
                    sb.Append(comments.Inline).Append(' ').Append(Constants.NEW_LINE);
                    innerIndent = outIndent + Constants.INDENT;
                }
                else if (savePatternParameter.IsForceMultiline)
                {
                    sb.Append(Constants.NEW_LINE);
                    innerIndent = outIndent + Constants.INDENT;
                }
            }
        }

        public override void Validate(LinkedLayer layer)
        {
            foreach (var value in _list)
            {
                if (value is IValidatable validatable)
                    validatable.Validate(layer);
            }
            base.Validate(layer);
        }
    }
}

