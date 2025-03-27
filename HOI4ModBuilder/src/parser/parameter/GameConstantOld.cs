using HOI4ModBuilder.src.dataObjects.parameters;
using System;
using System.Collections.Generic;
using YamlDotNet.Core.Tokens;

namespace HOI4ModBuilder.src.parser.parameter
{
    public class GameConstantOld : IParseCallbacksOld
    {
        private bool _needToSave;
        public bool NeedToSave => _needToSave || Comments.NeedToSave;

        public CommentsOld Comments { get; private set; }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value) _needToSave = true;
                _value = value;
            }
        }

        private static readonly Dictionary<Type, Func<string, object>> _mapping = new Dictionary<Type, Func<string, object>>
        {
            { typeof(bool), (v) => {
                var val = v.ToLower();
                if (val == "yes") return true;
                else if (val == "no") return false;
                else throw new FormatException(val);
            } },
            { typeof(byte), (v) => byte.Parse(v) },
            { typeof(ushort), (v) => ushort.Parse(v) },
            { typeof(short), (v) => short.Parse(v) },
            { typeof(int), (v) => int.Parse(v) },
            { typeof(uint), (v) => uint.Parse(v) },
            { typeof(float), (v) => float.Parse(v) },
            { typeof(double), (v) => double.Parse(v) },
            { typeof(string), (v) => v },
        };

        public T GetValue<T>() => (T)_mapping[typeof(T)](_value);
        public void SetParserValue(string value) => _value = value;

        public void ParseCallback(OldGameParser parser)
        {

        }
    }
}
