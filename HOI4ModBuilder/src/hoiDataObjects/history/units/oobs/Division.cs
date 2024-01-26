using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class Division : IParadoxReadAndValidate
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

        private OOB _currentOOB;
        public OOB CurrentOOB
        {
            get => _currentOOB;
            set
            {
                if (_currentOOB == value) return;

                NeedToSave = true;
                _currentOOB = value;
            }
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

        private DivisionName _divisionName;
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

        private Province _province;
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

        private string _divisionTemplate; //TODO Implement DivisionTemplateManager
        public string DivisionTemplate
        {
            get => _divisionTemplate;
            set
            {
                if (_divisionTemplate == value) return;

                NeedToSave = true;
                _divisionTemplate = value;
            }
        }

        private float _startExperienceFactor;
        public float StartExperienceFactor
        {
            get => _startExperienceFactor;
            set
            {
                if (_startExperienceFactor == value) return;

                NeedToSave = true;
                _startExperienceFactor = value;
            }
        }

        private static readonly float _defaultStartEquipmentFactor = 1f;
        private float _startEquipmentFactor = _defaultStartEquipmentFactor;
        public float StartEquipmentFactor
        {
            get => _startEquipmentFactor;
            set
            {
                if (_startEquipmentFactor == value) return;

                NeedToSave = true;
                _startEquipmentFactor = value;
            }
        }

        private bool _startManpowerFactorIsSet;
        private float? _startManpowerFactor;
        public float? StartManpowerFactor
        {
            get => _startManpowerFactor;
            set
            {
                if (_startManpowerFactor == value) return;
                _startManpowerFactorIsSet = value != null;

                NeedToSave = true;
                _startManpowerFactor = value;
            }
        }



        public Division(string name)
        {
            _name = name;
        }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            var newOutTab = outTab + tab;
            sb.Append(outTab).Append("division = {").Append(Constants.NEW_LINE);

            if (_name != null) sb.Append(outTab).Append(tab).Append("name = \"").Append(_name).Append('\"').Append(Constants.NEW_LINE);
            if (_divisionName != null) _divisionName.Save(sb, newOutTab, tab);

            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void Validate()
        {
            if (_name == null || _divisionName == null)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NO_NAME_NOR_DIVISION_NAME,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name }
                    }
                ));
            
            if (_name != null && _divisionName != null)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NAME_AND_DIVISION_NAME,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name }
                    }
                ));

            if (_province == null)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NO_LOCATION,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name }
                    }
                ));

            if (_divisionTemplate == null)
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NO_DIVISION_TEMPLATE,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name }
                    }
                ));

            if (Utils.ClampIfNeeded(_startExperienceFactor, 0, 1, out float newExperienceFactor))
            {
                Logger.LogWarning(
                    EnumLocKey.WARNING_WHILE_DIVISION_VALIDATION_PARAMETER_VALUE_IS_OUT_OF_RANGE_IN_FILE,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name },
                        { "{parameterName}", "start_experience_factor" },
                        { "{parameterValue}", $"{_startExperienceFactor}" },
                        { "{allowedRange}", "[0; 1]" },
                        { "{newParameterValue}", $"{newExperienceFactor}" }

                    }
                );
                _startExperienceFactor = newExperienceFactor;
            }

            if (Utils.ClampIfNeeded(_startEquipmentFactor, 0, 1, out float newEquipmentFactor))
            {
                Logger.LogWarning(
                    EnumLocKey.WARNING_WHILE_DIVISION_VALIDATION_PARAMETER_VALUE_IS_OUT_OF_RANGE_IN_FILE,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name },
                        { "{parameterName}", "start_equipment_factor" },
                        { "{parameterValue}", $"{_startEquipmentFactor}" },
                        { "{allowedRange}", "[0; 1]" },
                        { "{newParameterValue}", $"{newEquipmentFactor}" }

                    }
                );
                _startEquipmentFactor = newEquipmentFactor;
            }

            if (Utils.ClampIfNeeded(_startManpowerFactor, 0, 1, out float? newManpowerFactor))
            {
                Logger.LogWarning(
                    EnumLocKey.WARNING_WHILE_DIVISION_VALIDATION_PARAMETER_VALUE_IS_OUT_OF_RANGE_IN_FILE,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name },
                        { "{parameterName}", "start_manpower_factor" },
                        { "{parameterValue}", $"{_startManpowerFactor}" },
                        { "{allowedRange}", "[0; 1]" },
                        { "{newParameterValue}", $"{newManpowerFactor}" }

                    }
                );
                _startManpowerFactor = newManpowerFactor;
            }
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token) //TODO Дополнить
            {
                //Mandatory params
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

                //Optional params
                case "start_experience_factor": _startExperienceFactor = parser.ReadFloat(); break;
                case "start_equipment_factor": _startEquipmentFactor = parser.ReadFloat(); break;
                case "start_manpower_factor": _startManpowerFactor = parser.ReadFloat(); _startManpowerFactorIsSet = true; break;
                case "force_equipment_variants":
                    break;
                case "officer":
                    break;

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
