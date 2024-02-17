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
                _location != null && _location.HasChangedId;
        }

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private DivisionName _divisionName;
        public DivisionName DivisionName { get => _divisionName; set => Utils.Setter(ref _divisionName, ref value, ref _needToSave); }

        private static readonly string TOKEN_LOCATION = "location";
        private Province _location;
        public Province Location { get => _location; set => Utils.Setter(ref _location, ref value, ref _needToSave); }

        private static readonly string TOKEN_DIVISION_TEMPLATE = "division_template";
        private string _divisionTemplate; //TODO Implement DivisionTemplateManager
        public string DivisionTemplate { get => _divisionTemplate; set => Utils.Setter(ref _divisionTemplate, ref value, ref _needToSave); }

        private static readonly string TOKEN_START_EXPERIENCE_FACTOR = "start_experience_factor";
        private static readonly float DEFAULT_START_EXPERIENCE_FACTOR = 0;
        private float? _startExperienceFactor;
        public float? StartExperienceFactor { get => _startExperienceFactor; set => Utils.Setter(ref _startExperienceFactor, ref value, ref _needToSave); }

        private static readonly string TOKEN_START_EQUIPMENT_FACTOR = "start_equipment_factor";
        private static readonly float DEFAULT_START_EQUIPMENT_FACTOR = 1f;
        private float? _startEquipmentFactor;
        public float? StartEquipmentFactor { get => _startEquipmentFactor; set => Utils.Setter(ref _startEquipmentFactor, ref value, ref _needToSave); }

        private static readonly string TOKEN_START_MANPOWER_FACTOR = "start_manpower_factor";
        private float? _startManpowerFactor;
        public float? StartManpowerFactor { get => _startManpowerFactor; set => Utils.Setter(ref _startManpowerFactor, ref value, ref _needToSave); }

        private ForcedEquipmentVariants _forcedEquipmentVariants;
        public ForcedEquipmentVariants ForcedEquipmentVariants { get => _forcedEquipmentVariants; set => Utils.Setter(ref _forcedEquipmentVariants, ref value, ref _needToSave); }

        private DivisionOfficer _divisionOfficer;
        public DivisionOfficer DivisionOfficer { get => _divisionOfficer; set => Utils.Setter(ref _divisionOfficer, ref value, ref _needToSave); }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            var newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_NAME, _name);
            _divisionName?.Save(sb, newOutTab, tab);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_LOCATION, _location?.Id);
            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_DIVISION_TEMPLATE, _divisionTemplate);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_START_EXPERIENCE_FACTOR, _startExperienceFactor, DEFAULT_START_EXPERIENCE_FACTOR);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_START_EQUIPMENT_FACTOR, _startEquipmentFactor, DEFAULT_START_EQUIPMENT_FACTOR);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_START_MANPOWER_FACTOR, _startManpowerFactor);

            _forcedEquipmentVariants?.Save(sb, newOutTab, tab);
            _divisionOfficer?.Save(sb, newOutTab, tab);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions($"{BLOCK_NAME} ({TOKEN_NAME} = \"{_name}\")", () =>
            {
                Logger.Log(token);
                //Mandatory params
                if (token == TOKEN_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
                else if (token == DivisionName.BLOCK_NAME)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _divisionName, parser, new DivisionName());
                else if (token == TOKEN_LOCATION)
                {
                    var provinceId = parser.ReadUInt16();
                    if (ProvinceManager.TryGetProvince(provinceId, out Province newProvince))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _location, newProvince);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.PROVINCE_NOT_FOUND,
                            new Dictionary<string, string> { { "{provinceId}", "" + provinceId } }
                        );
                }
                else if (token == TOKEN_DIVISION_TEMPLATE)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _divisionTemplate, parser.ReadString());

                //Optional params
                else if (token == TOKEN_START_EXPERIENCE_FACTOR)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startExperienceFactor, parser.ReadFloat());
                else if (token == TOKEN_START_EQUIPMENT_FACTOR)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startEquipmentFactor, parser.ReadFloat());
                else if (token == TOKEN_START_MANPOWER_FACTOR)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startManpowerFactor, parser.ReadFloat());
                else if (token == ForcedEquipmentVariants.BLOCK_NAME)
                    Logger.ParseNewLayeredValueOrContinueOld(prevLayer, token, ref _forcedEquipmentVariants, parser, new ForcedEquipmentVariants());
                else if (token == DivisionOfficer.BLOCK_NAME)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _divisionOfficer, parser, new DivisionOfficer());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) //TODO Remake
        {
            bool result = true;

            var currentLayer = new LinkedLayer(prevLayer, $"({TOKEN_NAME} = \"{_name}\")");

            CheckAndLogUnit.WARNINGS
                .HasAtLeastOneMandatory(
                    ref result, prevLayer, BLOCK_NAME,
                    new string[] { TOKEN_NAME, DivisionName.BLOCK_NAME },
                    _name != null || _divisionName != null
                )
                /*
                .HasOnlyOneMutuallyExclusiveMandatory(
                    ref result, prevLayer, BLOCK_NAME,
                    new string[] { TOKEN_NAME, DivisionName.BLOCK_NAME },
                    new bool[] { _name != null, _divisionName != null }
                )
                */
                .HasMandatory(ref result, currentLayer, TOKEN_LOCATION, ref _location)
                .HasMandatory(ref result, currentLayer, TOKEN_DIVISION_TEMPLATE, ref _divisionTemplate)
                .CheckRangeAndClamp(
                    ref result, currentLayer, BLOCK_NAME,
                    (old, min, max) => Utils.ClampIfNeeded(old, min, max),
                    ref _startExperienceFactor, 0, 1
                )
                .CheckRangeAndClamp(
                    ref result, currentLayer, BLOCK_NAME,
                    (old, min, max) => Utils.ClampIfNeeded(old, min, max),
                    ref _startEquipmentFactor, 0, 1
                )
                .CheckRangeAndClamp(
                    ref result, currentLayer, BLOCK_NAME,
                    (old, min, max) => Utils.ClampIfNeeded(old, min, max),
                    ref _startManpowerFactor, 0, 1
                );

            return result;
        }
    }

    class DivisionName : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "division_name";

        public bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        private static readonly string TOKEN_IS_NAME_ORDERED = "is_name_ordered";
        public bool? _isNameOrdered;
        public bool? IsNameOrdered { get => _isNameOrdered; set => Utils.Setter(ref _isNameOrdered, ref value, ref _needToSave); }

        private static readonly string TOKEN_NAME_ORDER = "name_order";
        public int? _nameOrder;
        public int? NameOrder { get => _nameOrder; set => Utils.Setter(ref _nameOrder, ref value, ref _needToSave); }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            string newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            ParadoxUtils.Save(sb, newOutTab, TOKEN_IS_NAME_ORDERED, _isNameOrdered);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_NAME_ORDER, _nameOrder);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == TOKEN_IS_NAME_ORDERED)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isNameOrdered, parser.ReadBool());
                else if (token == TOKEN_NAME_ORDER)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _nameOrder, parser.ReadInt32());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_IS_NAME_ORDERED, ref _isNameOrdered)
                .HasMandatory(ref result, prevLayer, TOKEN_NAME_ORDER, ref _nameOrder);

            return result;
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
            foreach (var variant in _variants)
                if (variant.HasAnyInnerInfo) return true;
            return false;
        }

        private List<ForcedEquipmentVariant> _variants = new List<ForcedEquipmentVariant>();
        public List<ForcedEquipmentVariant> Variants { get; }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo()) return false;

            if (_variants.Count == 1)
            {
                ParadoxUtils.StartInlineBlock(sb, outTab, BLOCK_NAME);
                _variants[0].Save(sb, " ", " ");
                ParadoxUtils.EndBlock(sb, outTab);
            }
            else
            {
                string newOutTab = outTab + tab;
                ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

                foreach (var variant in _variants)
                {
                    variant.Save(sb, newOutTab, " ");
                    sb.Append(Constants.NEW_LINE);
                }

                ParadoxUtils.EndBlock(sb, outTab);
            }

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                //TODO Implement Archetype use
                Logger.ParseLayeredListedValue(prevLayer, token, ref _variants, parser, new ForcedEquipmentVariant(token));
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

        private static readonly string TOKEN_OWNER = "owner";
        private Country _owner;
        public Country Owner { get => _owner; set => Utils.Setter(ref _owner, ref value, ref _needToSave); }

        private static readonly string TOKEN_CREATOR = "creator";
        private Country _creator;
        public Country Creator { get => _creator; set => Utils.Setter(ref _creator, ref value, ref _needToSave); }

        private static readonly string TOKEN_AMOUNT = "amount";
        private uint? _amount;
        public uint? Amount { get => _amount; set => Utils.Setter(ref _amount, ref value, ref _needToSave); }

        private static readonly string TOKEN_VERSION_NAME = "version_name";
        private string _versionName;
        public string VersionName { get => _versionName; set => Utils.Setter(ref _versionName, ref value, ref _needToSave); }

        public ForcedEquipmentVariant(string archetype)
        {
            _archetype = archetype;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            ParadoxUtils.StartInlineBlock(sb, outTab, _archetype);

            ParadoxUtils.SaveQuotedInline(sb, tab, TOKEN_OWNER, _owner?.Tag);
            ParadoxUtils.SaveQuotedInline(sb, tab, TOKEN_CREATOR, _creator?.Tag);
            ParadoxUtils.SaveInline(sb, tab, TOKEN_AMOUNT, _amount);
            ParadoxUtils.SaveQuotedInline(sb, tab, TOKEN_VERSION_NAME, _versionName);

            ParadoxUtils.EndInlineBlock(sb, tab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(_archetype, () =>
            {
                if (token == TOKEN_OWNER)
                {
                    var value = parser.ReadString();
                    if (CountryManager.TryGetCountry(value, out Country newOwner))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _owner, newOwner);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.COUNTRY_NOT_FOUND,
                            new Dictionary<string, string> { { "{countryTag}", value } }
                        );
                }
                else if (token == TOKEN_CREATOR)
                {
                    var value = parser.ReadString();
                    if (CountryManager.TryGetCountry(value, out Country newCreator))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _creator, newCreator);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.COUNTRY_NOT_FOUND,
                            new Dictionary<string, string> { { "{countryTag}", value } }
                        );
                }
                else if (token == TOKEN_AMOUNT)
                {
                    var value = parser.ReadString();
                    if (uint.TryParse(value, out uint newAmount))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _amount, newAmount);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.INCORRECT_VALUE,
                            new Dictionary<string, string> { { "{value}", value } }
                        );
                }
                else if (token == TOKEN_VERSION_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _versionName, parser.ReadString());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true; //TODO implement later
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

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private DivisionOfficerPortraits _portraits;
        public DivisionOfficerPortraits Portraits { get => _portraits; set => Utils.Setter(ref _portraits, ref value, ref _needToSave); }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            string newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            ParadoxUtils.Save(sb, newOutTab, BLOCK_NAME, _name);
            _portraits?.Save(sb, newOutTab, tab);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions($"{BLOCK_NAME} ({TOKEN_NAME} = \"{_name}\")", () =>
            {
                if (token == TOKEN_NAME)
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

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            string newOutTab = outTab + tab;
            _armyGroup?.Save(sb, newOutTab, tab);

            return true;
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

        private static readonly string TOKEN_LARGE_PORTRAIT = "large";
        private string _largePortrait;
        public string LargePortrait { get => _largePortrait; set => Utils.Setter(ref _largePortrait, ref value, ref _needToSave); }

        private static readonly string TOKEN_SMALL_PORTRAIT = "small";
        private string _smallPortrait;
        public string SmallPortrait { get => _smallPortrait; set => Utils.Setter(ref _smallPortrait, ref value, ref _needToSave); }

        public DivisionOfficerPortraitsGroup(string categoryName)
        {
            _categoryName = categoryName;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            string newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, _categoryName);

            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_LARGE_PORTRAIT, _largePortrait);
            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_SMALL_PORTRAIT, _smallPortrait);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(_categoryName, () =>
            {
                if (token == TOKEN_LARGE_PORTRAIT)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _largePortrait, parser.ReadString());
                else if (token == TOKEN_SMALL_PORTRAIT)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _smallPortrait, parser.ReadString());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
