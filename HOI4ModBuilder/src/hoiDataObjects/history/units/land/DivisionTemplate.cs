using HOI4ModBuilder.src.hoiDataObjects.common.units.divisionsNames;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.land;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.divisionTemplates
{
    class DivisionTemplate : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "division_template";

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _namesGroup != null && _namesGroup.NeedToSave ||
                _regimentsSubUnits != null && _regimentsSubUnits.NeedToSave ||
                _supportSubUnits != null && _supportSubUnits.NeedToSave;
        }

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private DivisionNamesGroup _namesGroup;
        public DivisionNamesGroup NamesGroup { get => _namesGroup; set => Utils.Setter(ref _namesGroup, ref value, ref _needToSave); }

        private static readonly string TOKEN_IS_LOCKED = "is_locked";
        private bool? _isLocked;
        public bool? IsLocked { get => _isLocked; set => Utils.Setter(ref _isLocked, ref value, ref _needToSave); }

        private static readonly string TOKEN_IS_OBSOLETE = "obsolete";
        private bool? _isObsolete;
        public bool? IsObsolete { get => _isObsolete; set => Utils.Setter(ref _isObsolete, ref value, ref _needToSave); }


        private static readonly string TOKEN_FORCE_ALLOW_RECRUITING = "force_allow_recruiting";
        private bool? _forceAllowRecruiting;
        public bool? ForceAllowRecruiting { get => _forceAllowRecruiting; set => Utils.Setter(ref _forceAllowRecruiting, ref value, ref _needToSave); }

        private static readonly string TOKEN_DIVISION_CAP = "division_cap";
        private int? _divisionCap;
        public int? DivisionCap { get => _divisionCap; set => Utils.Setter(ref _divisionCap, ref value, ref _needToSave); }

        private static readonly string TOKEN_PRIORITY = "priority";
        private static readonly byte DEFAULT_PRIORITY = 1;
        private byte? _priority;
        public byte? Priority { get => _priority; set => Utils.Setter(ref _priority, ref value, ref _needToSave); }

        private static readonly string TOKEN_TEMPLATE_COUNTRER = "template_counter";
        private int? _templateCounter;
        public int? TemplateCounter { get => _templateCounter; set => Utils.Setter(ref _templateCounter, ref value, ref _needToSave); }

        private static readonly string TOKEN_OVERRIDE_MODEL = "override_model";
        private string _overrideModel; //TODO Implement entity models
        public string OverrideModel { get => _overrideModel; set => Utils.Setter(ref _overrideModel, ref value, ref _needToSave); }

        private static readonly string TOKEN_REGIMENTS = "regiments";
        private SubUnitsBlock _regimentsSubUnits;

        private static readonly string TOKEN_SUPPORT = "support";
        private SubUnitsBlock _supportSubUnits;

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            string newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_NAME, _name);
            ParadoxUtils.Save(sb, newOutTab, DivisionNamesGroup.BLOCK_NAME, _namesGroup?.Name);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_IS_LOCKED, _isLocked);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_IS_OBSOLETE, _isObsolete);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_FORCE_ALLOW_RECRUITING, _forceAllowRecruiting);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_DIVISION_CAP, _divisionCap);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_PRIORITY, _priority, DEFAULT_PRIORITY);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_TEMPLATE_COUNTRER, _templateCounter);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_OVERRIDE_MODEL, _overrideModel);

            _regimentsSubUnits?.Save(sb, newOutTab, tab);
            _supportSubUnits?.Save(sb, newOutTab, tab);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions($"{BLOCK_NAME} ({TOKEN_NAME} = \"{_name}\")", () =>
            {
                string value;

                if (token == TOKEN_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
                else if (token == DivisionNamesGroup.BLOCK_NAME)
                {
                    value = parser.ReadString();
                    if (!DivisionNamesGroupManager.TryGetNamesGroup(value, out _namesGroup))
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.DIVISION_NAMES_GROUP_NOT_FOUND,
                            new Dictionary<string, string> { { "{divisionNamesGroup}", value } }
                        );
                }
                else if (token == TOKEN_IS_LOCKED)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isLocked, parser.ReadBool());
                else if (token == TOKEN_IS_OBSOLETE)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isObsolete, parser.ReadBool());
                else if (token == TOKEN_FORCE_ALLOW_RECRUITING)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _forceAllowRecruiting, parser.ReadBool());
                else if (token == TOKEN_DIVISION_CAP)
                {
                    value = parser.ReadString();
                    if (int.TryParse(value, out int newDivisionCap))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _divisionCap, newDivisionCap);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.INCORRECT_VALUE,
                            new Dictionary<string, string> { { "{value}", value } }
                        );
                }
                else if (token == TOKEN_PRIORITY)
                {
                    value = parser.ReadString();
                    if (byte.TryParse(value, out byte newPriority))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _priority, newPriority);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.INCORRECT_VALUE,
                            new Dictionary<string, string> { { "{value}", value } }
                        );
                }
                else if (token == TOKEN_TEMPLATE_COUNTRER)
                {
                    value = parser.ReadString();
                    if (int.TryParse(value, out int newTemplateCounter))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _templateCounter, newTemplateCounter);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.INCORRECT_VALUE,
                            new Dictionary<string, string> { { "{value}", value } }
                        );
                }
                else if (token == TOKEN_OVERRIDE_MODEL)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _overrideModel, parser.ReadString());
                else if (token == TOKEN_REGIMENTS)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _regimentsSubUnits, parser, new SubUnitsBlock(token));
                else if (token == TOKEN_SUPPORT)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _supportSubUnits, parser, new SubUnitsBlock(token));
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            var currentLayer = new LinkedLayer(prevLayer, $"({TOKEN_NAME} = \"{_name}\")");

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_NAME, ref _name)
                .HasAtLeastOneMandatory(
                    ref result, currentLayer, BLOCK_NAME,
                    new string[] { TOKEN_REGIMENTS, TOKEN_SUPPORT },
                    _regimentsSubUnits != null || _supportSubUnits != null
                )
                .Check(
                    ref result, currentLayer,
                    _regimentsSubUnits != null && _regimentsSubUnits.Count > 0 || _supportSubUnits != null && _supportSubUnits.Count > 0,
                    EnumLocKey.DIVISION_TEMPLATE_MUST_HAVE_AT_LEAST_ONE_REGIMENT_OR_SUPPORT_SUB_UNIT
                );

            return result;
        }
    }
}
