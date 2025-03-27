using HOI4ModBuilder.src.parser.objects;
using HOI4ModBuilder.src.parser.parameter;
using System;

namespace HOI4ModBuilder.src.parser
{
    public class GameParameterOld<T> : IGameParameterOld where T : new()
    {
        private bool _needToSave;
        public bool IsNeedToSave() => _needToSave;
        public void SetNeedToSave(bool needToSave) => _needToSave = needToSave;

        private IParseObjectOld _parent;
        public IParseObjectOld GetParent() => _parent;

        private GameConstantOld _constant;
        public GameConstantOld Constant { get => _constant; set => _constant = value; }

        public CommentsOld comments { get; private set; }

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
                {
                    IParseObjectOld tempParent = GetParent();
                    GameConstantOld tempConstant = null;
                    while (tempParent != null)
                    {
                        if (
                            tempParent is IConstantsOld iConstants &&
                            iConstants.TryGetConstant(valueString, out tempConstant)
                        ) break;

                        tempParent = tempParent.GetParent();
                    }

                    if (tempConstant != null)
                        _constant = tempConstant;
                    else throw new Exception("Constant with name " + valueString + " not found or this scope does not support constants");
                }
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
        public void SetParent(IParseObjectOld parent) => _parent = parent;

        public object GetObject() => _value;
        public void SetObject(object obj) => SetValue((T)obj);

        public void ParseCallback(OldGameParser parser)
        {
            InitValueIfNull();
            if (_value is IParseCallbacksOld iParseCallbacks)
            {
                iParseCallbacks.ParseCallback(parser);
                return;
            }

            //TODO parse other params types
        }
    }
}
