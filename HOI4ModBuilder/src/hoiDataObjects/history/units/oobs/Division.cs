using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class Division : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "division";

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
        }

        private OOB _currentOOB;
        public OOB CurrentOOB { get => _currentOOB; set => Utils.Setter(ref _currentOOB, ref value, ref _needToSave); }

        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private DivisionName _divisionName;
        public DivisionName DivisionName { get => _divisionName; set => Utils.Setter(ref _divisionName, ref value, ref _needToSave); }

        private Province _province;
        public Province Province { get => _province; set => Utils.Setter(ref _province, ref value, ref _needToSave); }

        private string _divisionTemplate; //TODO Implement DivisionTemplateManager
        public string DivisionTemplate { get => _divisionTemplate; set => Utils.Setter(ref _divisionTemplate, ref value, ref _needToSave); }

        private static readonly float _defaultStartExperienceFactor = 0;
        private float? _startExperienceFactor;
        public float? StartExperienceFactor { get => _startExperienceFactor; set => Utils.Setter(ref _startExperienceFactor, ref value, ref _needToSave); }

        private static readonly float _defaultStartEquipmentFactor = 1f;
        private float? _startEquipmentFactor;
        public float? StartEquipmentFactor { get => _startEquipmentFactor; set => Utils.Setter(ref _startEquipmentFactor, ref value, ref _needToSave); }

        private float? _startManpowerFactor;
        public float? StartManpowerFactor { get => _startManpowerFactor; set => Utils.Setter(ref _startManpowerFactor, ref value, ref _needToSave); }

        private ForcedEquipmentVariants _forcedEquipmentVariants;
        public ForcedEquipmentVariants ForcedEquipmentVariants { get => _forcedEquipmentVariants; set => Utils.Setter(ref _forcedEquipmentVariants, ref value, ref _needToSave); }

        private DivisionOfficer _divisionOfficer;
        public DivisionOfficer DivisionOfficer { get => _divisionOfficer; set => Utils.Setter(ref _divisionOfficer, ref value, ref _needToSave); }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            var newOutTab = outTab + tab;
            sb.Append(outTab).Append(BLOCK_NAME).Append(" = {").Append(Constants.NEW_LINE);

            if (_name != null)
                sb.Append(newOutTab).Append("name = \"").Append(_name).Append('\"').Append(Constants.NEW_LINE);

            _divisionName?.Save(sb, newOutTab, tab);

            if (_province != null)
                sb.Append(newOutTab).Append("location = ").Append(_province.Id).Append(Constants.NEW_LINE);

            if (_divisionTemplate != null)
                sb.Append(newOutTab).Append("division_template = \"").Append(_divisionTemplate).Append('\"').Append(Constants.NEW_LINE);

            if (_startExperienceFactor != _defaultStartExperienceFactor)
                sb.Append(newOutTab).Append("start_experience_factor = ").Append(_startExperienceFactor).Append(Constants.NEW_LINE);

            if (_startEquipmentFactor != _defaultStartEquipmentFactor)
                sb.Append(newOutTab).Append("start_equipment_factor = ").Append(_startEquipmentFactor).Append(Constants.NEW_LINE);

            if (_startManpowerFactor != null)
                sb.Append(newOutTab).Append("start_manpower_factor = ").Append(_startManpowerFactor).Append(Constants.NEW_LINE);

            _forcedEquipmentVariants?.Save(sb, newOutTab, tab);
            _divisionOfficer?.Save(sb, newOutTab, tab);

            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                //Mandatory params
                if (token == "name")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
                else if (token == DivisionName.BLOCK_NAME)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _divisionName, parser, new DivisionName());
                else if (token == "location")
                {
                    var provinceId = parser.ReadUInt16();
                    if (!ProvinceManager.TryGetProvince(provinceId, out Province newProvince))
                        Logger.WrapException(token, new ProvinceNotFoundException(provinceId));
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _province, newProvince);
                }
                else if (token == "division_template")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _divisionTemplate, parser.ReadString());

                //Optional params
                else if (token == "start_experience_factor")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startExperienceFactor, parser.ReadFloat());
                else if (token == "start_equipment_factor")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startEquipmentFactor, parser.ReadFloat());
                else if (token == "start_manpower_factor")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startManpowerFactor, parser.ReadFloat());
                else if (token == ForcedEquipmentVariants.BLOCK_NAME)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _forcedEquipmentVariants, parser, new ForcedEquipmentVariants());
                else if (token == DivisionOfficer.BLOCK_NAME)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _divisionOfficer, parser, new DivisionOfficer());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) //TODO Remake
        {
            bool result = true;

            if (_name == null || _divisionName == null)
            {
                Logger.LogError(
                    EnumLocKey.ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NO_NAME_NOR_DIVISION_NAME,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name }
                    }
                );
                result = false;
            }

            if (_name != null && _divisionName != null)
            {
                Logger.LogError(
                    EnumLocKey.ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NAME_AND_DIVISION_NAME,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name }
                    }
                );
                result = false;
            }

            if (_province == null)
            {
                Logger.LogError(
                    EnumLocKey.ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NO_LOCATION,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name }
                    }
                );
                result = false;
            }

            if (_divisionTemplate == null)
            {
                Logger.LogError(
                    EnumLocKey.ERROR_WHILE_DIVISION_VALIDATION_DIVISION_HAS_NO_DIVISION_TEMPLATE,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{divisionName}", _name }
                    }
                );
                result = false;
            }

            if (Utils.ClampIfNeeded(_startExperienceFactor, 0, 1, out float? newExperienceFactor))
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
                result = false;
            }

            if (Utils.ClampIfNeeded(_startEquipmentFactor, 0, 1, out float? newEquipmentFactor))
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
                result = false;
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
                result = false;
            }

            return result;
        }
    }

    class DivisionName : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "division_name";

        public bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        public bool? _isNameOrdered;
        public bool? IsNameOrdered { get => _isNameOrdered; set => Utils.Setter(ref _isNameOrdered, ref value, ref _needToSave); }

        public int? _nameOrder;
        public int? NameOrder { get => _nameOrder; set => Utils.Setter(ref _nameOrder, ref value, ref _needToSave); }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            string newOutTab = outTab + tab;
            sb.Append(outTab).Append(BLOCK_NAME).Append(" = {").Append(Constants.NEW_LINE);

            sb.Append(newOutTab).Append("is_name_ordered = ").Append(_isNameOrdered == true ? "yes" : "no").Append(Constants.NEW_LINE);

            if (_nameOrder != null)
                sb.Append(newOutTab).Append("name_order = ").Append(_nameOrder).Append(Constants.NEW_LINE);

            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == "is_name_ordered")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isNameOrdered, parser.ReadBool());
                else if (token == "name_order")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _nameOrder, parser.ReadInt32());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) //TODO Remake
        {
            return _isNameOrdered != null;
        }
    }

    class ForcedEquipmentVariants : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "force_equipment_variants";

        public bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;

                foreach (var variant in _variants)
                    if (variant.NeedToSave) return true;

                return false;
            }
        }

        public bool HasAnyInnerInfo()
        {
            foreach (var variant in _variants) if (variant.HasAnyInnerInfo) return true;
            return false;
        }

        private List<ForcedEquipmentVariant> _variants = new List<ForcedEquipmentVariant>();
        public List<ForcedEquipmentVariant> Variants { get; }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo()) return;


            if (_variants.Count == 1)
            {
                sb.Append(outTab).Append(BLOCK_NAME).Append(" = { ");
                _variants[0].Save(sb, "", "");
                sb.Append(" }").Append(Constants.NEW_LINE);
            }
            else
            {
                string newOutTab = outTab + tab;
                sb.Append(outTab).Append(BLOCK_NAME).Append(" = { ").Append(Constants.NEW_LINE);
                foreach (var variant in _variants)
                {
                    sb.Append(newOutTab);
                    variant.Save(sb, "", "");
                    sb.Append(Constants.NEW_LINE);
                }
                sb.Append(outTab).Append(" }").Append(Constants.NEW_LINE);
            }
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                //TODO Implement Archetype use
                Logger.ParseLayeredListedValue(prevLayer, token, _variants, parser, new ForcedEquipmentVariant(token));
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    class ForcedEquipmentVariant : IParadoxObject
    {
        private bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        public bool HasAnyInnerInfo => _archetype != null || _owner != null || _creator != null || _amount != null || _versionName != null;

        private string _archetype; //TODO Implement archetypes
        public string Archetype { get => _archetype; set => Utils.Setter(ref _archetype, ref value, ref _needToSave); }

        private Country _owner;
        public Country Owner { get => _owner; set => Utils.Setter(ref _owner, ref value, ref _needToSave); }

        private Country _creator;
        public Country Creator { get => _creator; set => Utils.Setter(ref _creator, ref value, ref _needToSave); }

        private uint? _amount;
        public uint? Amount { get => _amount; set => Utils.Setter(ref _amount, ref value, ref _needToSave); }

        private string _versionName;
        public string VersionName { get => _versionName; set => Utils.Setter(ref _versionName, ref value, ref _needToSave); }

        public ForcedEquipmentVariant(string archetype)
        {
            _archetype = archetype;
        }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return;

            sb.Append(_archetype).Append(" = {");
            if (_owner != null) sb.Append(" owner = \"").Append(_owner.tag).Append('\"');
            if (_creator != null) sb.Append(" creator = \"").Append(_creator.tag).Append('\"');
            if (_amount != null) sb.Append(" amount = ").Append(_amount);
            if (_versionName != null) sb.Append(" version_name = \"").Append(_amount).Append('\"');
            sb.Append(" }");
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(_archetype, () =>
            {
                string value;

                if (token == "owner")
                {
                    value = parser.ReadString();
                    if (!CountryManager.TryGetCountry(value, out Country newOwner))
                        Logger.WrapException(token, new CountryNotFoundException(value));
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _owner, newOwner);
                }
                else if (token == "creator")
                {
                    value = parser.ReadString();
                    if (!CountryManager.TryGetCountry(value, out Country newCreator))
                        Logger.WrapException(token, new CountryNotFoundException(value));
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _creator, newCreator);
                }
                else if (token == "amount")
                {
                    value = parser.ReadString();
                    if (!uint.TryParse(value, out uint newAmount))
                        Logger.WrapException(token, new IncorrectValueException(value));
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _amount, newAmount);
                }
                else if (token == "version_name")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _versionName, parser.ReadString());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    class DivisionOfficer : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "officer";

        private bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _portraits != null && _portraits.NeedToSave;
        }
        public bool HasAnyInnerInfo =>
            _name != null ||
            _portraits != null && _portraits.HasAnyInnerInfo;

        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private DivisionOfficerPortraits _portraits;
        public DivisionOfficerPortraits Portraits { get => _portraits; set => Utils.Setter(ref _portraits, ref value, ref _needToSave); }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return;

            string newOutTab = outTab + tab;

            sb.Append(outTab).Append(BLOCK_NAME).Append(" = {").Append(Constants.NEW_LINE);
            sb.Append(outTab).Append(tab).Append("name = ").Append(_name);
            _portraits?.Save(sb, newOutTab, tab);
            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == "name")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
                if (token == BLOCK_NAME)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _portraits, parser, new DivisionOfficerPortraits());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    class DivisionOfficerPortraits : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "portraits";

        private bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _armyGroup != null && _armyGroup.NeedToSave;
        }
        public bool HasAnyInnerInfo =>
            _armyGroup != null && _armyGroup.HasAnyInnerInfo;

        private DivisionOfficerPortraitsGroup _armyGroup;
        public DivisionOfficerPortraitsGroup ArmyGroup { get => _armyGroup; set => Utils.Setter(ref _armyGroup, ref value, ref _needToSave); }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return;

            string newOutTab = outTab + tab;
            if (_armyGroup != null) _armyGroup.Save(sb, newOutTab, tab);
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == "army")
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _armyGroup, parser, new DivisionOfficerPortraitsGroup(token));
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    class DivisionOfficerPortraitsGroup : IParadoxObject
    {
        public bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        public bool HasAnyInnerInfo =>
            _largePortrait != null ||
            _smallPortrait != null;

        private string _categoryName;
        public string CategoryName { get => _categoryName; set => Utils.Setter(ref _categoryName, ref value, ref _needToSave); }

        private string _largePortrait;
        public string LargePortrait { get => _largePortrait; set => Utils.Setter(ref _largePortrait, ref value, ref _needToSave); }

        private string _smallPortrait;
        public string SmallPortrait { get => _smallPortrait; set => Utils.Setter(ref _smallPortrait, ref value, ref _needToSave); }

        public DivisionOfficerPortraitsGroup(string categoryName)
        {
            _categoryName = categoryName;
        }

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return;

            sb.Append(outTab).Append(_categoryName).Append(" = {").Append(Constants.NEW_LINE);
            if (_largePortrait != null) sb.Append(outTab).Append(tab).Append("large = ").Append(_largePortrait).Append(Constants.NEW_LINE);
            if (_smallPortrait != null) sb.Append(outTab).Append(tab).Append("small = ").Append(_smallPortrait).Append(Constants.NEW_LINE);
            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(_categoryName, () =>
            {
                if (token == "large")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _largePortrait, parser.ReadString());
                else if (token == "small")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _smallPortrait, parser.ReadString());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
