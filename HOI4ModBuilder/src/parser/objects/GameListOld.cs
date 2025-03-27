using HOI4ModBuilder.src.parser.parameter;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.parser.objects
{
    public class GameListOld<T> : AbstractParseObjectOld, IPushObject, IEnumerable<T> where T : new()
    {
        public new bool IsNeedToSave()
        {
            if (base.IsNeedToSave())
                return true;

            if (_list is List<AbstractParseObjectOld> @list)
                foreach (var item in @list)
                    if (item.IsNeedToSave())
                        return true;

            return false;
        }

        public override Dictionary<string, Func<object, object>> GetStaticAdapter() => null;
        public override Dictionary<string, DynamicGameParameterOld> GetDynamicAdapter() => null;
        public override IParseObjectOld GetEmptyCopy() => new GameListOld<T>();


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

        public void PushObject(object obj)
        {
            if (obj is T typeObj) Add(typeObj);
            else throw new Exception("Unexpected value type: " + obj.GetType());
        }
    }
}
