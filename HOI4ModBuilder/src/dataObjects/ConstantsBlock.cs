using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.Pdoxcl2Sharp
{
    abstract class ConstantsBlock
    {
        private List<Constant> _constants;

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            if (_constants == null || _constants.Count == 0) return;

            foreach (Constant constant in _constants)
                sb.Append(outTab).Append(constant.name).Append(" = ").Append(constant.value).Append(Constants.NEW_LINE);

            sb.Append(Constants.NEW_LINE);
        }

        public bool TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            if (token[0] == '@') //TODO Add CheckLayerd
            {
                if (_constants == null) _constants = new List<Constant>();
                _constants.Add(new Constant(token, parser.ReadString())); //TODO Maybe add types check?
                return true;
            }
            else return false;
        }

        public bool Validate(LinkedLayer prevLayer) => true;

        public int Count => _constants != null ? _constants.Count : 0;
    }

    struct Constant
    {
        public string name;
        public object value;

        public Constant(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Constant constant &&
                   name == constant.name &&
                   EqualityComparer<object>.Default.Equals(value, constant.value);
        }

        public override int GetHashCode()
        {
            int hashCode = 1477024672;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(value);
            return hashCode;
        }
    }
}
