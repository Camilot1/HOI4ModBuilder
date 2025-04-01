
using System.Collections.Generic;
using System;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using System.Collections;
using System.Text;
using HOI4ModBuilder.src.utils;
using static System.Net.Mime.MediaTypeNames;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameList<T> : AbstractParseObject, IValuePushable, IEnumerable<T> where T : new()
    {
        private bool _allowsInlineAdd;
        private bool _forceSeparateLineSave;
        private Func<string, T> _valueParseAdapter;
        private Func<T, object> _valueSaveAdapter;
        public GameList() : base() { }
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
        public GameList<T> INIT_SetValueParseAdapter(Func<string, T> value)
        {
            _valueParseAdapter = value;
            return this;
        }
        public GameList<T> INIT_SetValueParseAdapter(Func<T, object> value)
        {
            _valueSaveAdapter = value;
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
        public override bool CustomParseCallback(GameParser parser)
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
                return TryParseBlockValue(parser);
            else
                return TryParseInlineValue(parser);
        }

        private bool TryParseBlockValue(GameParser parser)
        {
            parser.ParseInsideBlock(
                (comments) => SetComments(comments),
                (tokenComments, token) =>
                {
                    T obj = _valueParseAdapter != null ? _valueParseAdapter(token) : ParserUtils.Parse<T>(token);
                    if (obj is ICommentable commentable)
                        commentable.SetComments(tokenComments);
                    AddSilent(obj);
                }
            );
            return true;
        }

        private bool TryParseInlineValue(GameParser parser)
        {
            if (parser.CurrentToken == Token.QUOTE)
                parser.ParseQuoted();
            else
                parser.ParseUnquotedValue();

            var value = parser.PullParsedDataString();
            if (!_allowsInlineAdd || value.Length == 0)
                throw new Exception("Invalid parse inside block structure: " + parser.GetCursorInfo());

            var obj = _valueParseAdapter != null ? _valueParseAdapter.Invoke(value) : ParserUtils.Parse<T>(value);

            var comments = parser.ParseAndPullComments();
            if (obj is ICommentable commentable)
                commentable.SetComments(comments);

            AddSilent(obj);

            return true;
        }

        public override IParseObject GetEmptyCopy() => new GameList<T>();


        private List<T> _list = new List<T>();

        public void Push(object value) => AddSilent((T)value);
        public void AddSilent(T item) => _list.Add(item);
        public void Add(T item)
        {
            _needToSave = true;
            _list.Add(item);
        }
        public T Get(int index) => _list[index];
        public void RemoveAt(int index)
        {
            _needToSave = true;
            _list.RemoveAt(index);
        }
        public void Remove(T item) => _needToSave |= _list.Remove(item);
        public int Count => _list.Count;

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //TODO impelemnt save
        public override SaveAdapter GetSaveAdapter() => null;
        public override bool CustomSave(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter)
        {
            if (_list.Count == 0)
                return true;

            if (saveParameter.AddEmptyLineBefore)
                sb.Append(outIndent).Append(Constants.NEW_LINE);

            if (_forceSeparateLineSave)
                SeparateLineSave(parser, sb, outIndent, key, saveParameter);
            else
                ListSave(parser, sb, outIndent, key, saveParameter);

            return true;
        }

        private void SeparateLineSave(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter)
        {
            foreach (var value in _list)
            {
                object tempValue = value;
                if (_valueSaveAdapter != null)
                    tempValue = _valueSaveAdapter.Invoke(value);

                if (value is ISaveable saveable)
                    saveable.Save(parser, sb, outIndent, key, saveParameter);
                else
                {
                    GameComments comments = null;
                    if (value is ICommentable commentable)
                        comments = commentable.GetComments();

                    if (comments != null && comments.Previous.Length > 0)
                        sb.Append(outIndent).Append(comments.Previous).Append(Constants.NEW_LINE);

                    sb.Append(outIndent).Append(key).Append(" = ").Append(tempValue);

                    if (comments != null && comments.Inline.Length > 0)
                        sb.Append(' ').Append(comments.Inline);

                    sb.Append(Constants.NEW_LINE);
                }
            }
        }

        private void ListSave(GameParser parser, StringBuilder sb, string outIndent, string key, SaveAdapterParameter saveParameter)
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
                    sb.Append(' ').Append(comments.Inline).Append(Constants.NEW_LINE);
                else if (saveParameter.IsForceMultiline)
                    sb.Append(Constants.NEW_LINE);
                else
                    innerIndent = " ";
            }

            foreach (var value in _list)
            {
                object tempValue = value;
                if (_valueSaveAdapter != null)
                    tempValue = _valueSaveAdapter.Invoke(value);

                if (value is ISaveable saveable)
                {
                    saveable.Save(parser, sb, innerIndent, key, saveParameter);

                    //TODO Refactor. Временный фикс некорректного переноса строк в случае,
                    //               если в объекте есть два списка без имени
                    //  infrastructure = yes (фикс-перенос)
                    //  infrastructure = yes (фикс-перенос)
                    //  set_variable = { test1 = 10 }
                    //  set_variable = { test2 = 14 }
                    if (key == null && value is ScriptBlockParseObject scriptBlockParseObject)
                        if (!(scriptBlockParseObject.GetValueRaw() is IValuePushable))
                            sb.Append(Constants.NEW_LINE);
                }
                else
                {
                    var comments = GameComments.DEFAULT;
                    if (value is ICommentable commentable)
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

                    sb.Append(innerIndent).Append(tempValue);

                    if (comments.Inline.Length > 0)
                    {
                        sb.Append(' ').Append(comments.Inline);
                        sb.Append(Constants.NEW_LINE);
                        innerIndent = outIndent + Constants.INDENT;
                    }
                    else if (saveParameter.IsForceMultiline)
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
    }
}

