
using System.Collections.Generic;
using System;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.structs;
using System.Collections;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class GameList<T> : AbstractParseObject, IEnumerable<T> where T : new()
    {
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
            parser.ParseInsideBlock((token) => Add(ParserUtils.Parse<T>(token)));
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
    }
}

