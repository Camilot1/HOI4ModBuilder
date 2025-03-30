
using System.Collections.Generic;
using System;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using System.Collections;
using System.Text;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameList<T> : AbstractParseObject, IEnumerable<T> where T : new()
    {
        private readonly bool _allowsInlineAdd;
        private readonly bool _forceSeparateLineSave;
        public GameList() : base() { }
        public GameList(bool allowsInlineAdd, bool forceSeparateLineSave) : this()
        {
            _allowsInlineAdd = allowsInlineAdd;
            _forceSeparateLineSave = forceSeparateLineSave;
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
            SetComments(parser.ParseAndPullComments());
            parser.ParseInsideBlock(this, (token) => Add(ParserUtils.Parse<T>(token)));
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

            SetComments(parser.ParseAndPullComments());

            return true;
        }

        public override IParseObject GetEmptyCopy() => new GameList<T>();


        private List<T> _list = new List<T>();

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
        public override SaveAdapter GetSaveAdapter()
        {
            throw new NotImplementedException();
        }
        public override bool CustomSave(GameParser parser, StringBuilder sb, SaveAdapterParameter saveParameter, string outIndent, string key)
        {
            //TODO implement
            return true;
        }
    }
}

