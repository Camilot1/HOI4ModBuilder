using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class Division : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _divisionName != null && _divisionName.NeedToSave ||
                _province != null && _province.HasChangedId;
            private set => NeedToSave = value;
        }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;

                NeedToSave = true;
                _name = value;
            }
        }

        public DivisionName _divisionName;
        public DivisionName DivisionName
        {
            get => _divisionName;
            set
            {
                if (_divisionName == value) return;

                NeedToSave = true;
                _divisionName = value;
            }
        }

        public Province _province;
        public Province Province
        {
            get => _province;
            set
            {
                if (_province == value) return;

                NeedToSave = true;
                _province = value;
            }
        }

        public string _divisionTemplate; //TODO Implement DivisionTemplateManager
        public string DivisionTemplate
        {
            get => _divisionTemplate;
            set
            {
                if (_divisionTemplate != value)
                {
                    NeedToSave = true;
                    _divisionTemplate = value;
                }
            }
        }

        public Division(string name)
        {
            _name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token) //TODO Дополнить
            {
                case "name": _name = parser.ReadString(); break;
                case "division_name": _divisionName = parser.Parse(new DivisionName()); break;
                case "location":
                    ushort provinceId = parser.ReadUInt16();
                    if (!ProvinceManager.TryGetProvince(provinceId, out _province))
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.ERROR_DIVISION_PROVINCE_NOT_FOUND,
                            new Dictionary<string, string> { { "{provinceId}", $"{provinceId}" } }
                        ));
                    break;
                case "division_template": _divisionTemplate = parser.ReadString(); break;

            }
        }
    }

    class DivisionName : IParadoxRead
    {
        public bool NeedToSave { get; private set; }

        public bool _isNameOrdered;
        public bool IsNameOrdered
        {
            get { return _isNameOrdered; }
            set
            {
                if (_isNameOrdered != value)
                {
                    NeedToSave = true;
                    _isNameOrdered = value;
                }
            }
        }
        public int _nameOrder;
        public int NameOrder
        {
            get { return _nameOrder; }
            set
            {
                if (_nameOrder != value)
                {
                    NeedToSave = true;
                    _nameOrder = value;
                }
            }
        }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            sb.Append(outTab).Append("division_name = {").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("is_name_ordered = ").Append(IsNameOrdered ? "yes" : "no").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("name_order = ").Append(NameOrder).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "is_name_ordered") _isNameOrdered = parser.ReadBool();
            else if (token == "name_order") _nameOrder = parser.ReadInt32();
            else throw new Exception(GuiLocManager.GetLoc(
                EnumLocKey.ERROR_DIVISION_NAME_INCORRECT_TOKEN,
                new Dictionary<string, string> { { "{token}", token } }
            ));
        }
    }
}
