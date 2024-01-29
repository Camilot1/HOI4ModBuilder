using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.hoiDataObjects.common.units;
using HOI4ModBuilder.src.hoiDataObjects.common.units.divisionsNames;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.divisionTemplates
{
    class DivisionTemplate : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "division_template";

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        private OOB _currentOOB;
        public OOB CurrentOOB { get => _currentOOB; set => Utils.Setter(ref _currentOOB, ref value, ref _needToSave); }

        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private DivisionNamesGroup _namesGroup;
        public DivisionNamesGroup NamesGroup { get => _namesGroup; set => Utils.Setter(ref _namesGroup, ref value, ref _needToSave); }

        private bool? _isLocked;
        public bool? IsLocked { get => _isLocked; set => Utils.Setter(ref _isLocked, ref value, ref _needToSave); }

        private bool? _forceAllowRecruiting;
        public bool? ForceAllowRecruiting { get => _forceAllowRecruiting; set => Utils.Setter(ref _forceAllowRecruiting, ref value, ref _needToSave); }

        private int? _divisionCap;
        public int? DivisionCap { get => _divisionCap; set => Utils.Setter(ref _divisionCap, ref value, ref _needToSave); }

        private static readonly int DefaultPriority = 1;
        public byte? _priority;
        public byte? Priority { get => _priority; set => Utils.Setter(ref _priority, ref value, ref _needToSave); }

        public int? _templateCounter;
        public int? TemplateCounter { get => _templateCounter; set => Utils.Setter(ref _templateCounter, ref value, ref _needToSave); }

        public string _overrideModel; //TODO Implement entity models
        public string OverrideModel { get => _overrideModel; set => Utils.Setter(ref _overrideModel, ref value, ref _needToSave); }

        private SubUnitsBlock _regimentsSubUnits;
        private SubUnitsBlock _supportSubUnits;

        public void Save(StringBuilder sb, string outTab, string tab)
        {
            string newOutTab = outTab + tab;

            sb.Append(outTab).Append(BLOCK_NAME).Append(" = {").Append(Constants.NEW_LINE);
            sb.Append(newOutTab).Append("name = \"").Append(_name).Append('\"').Append(Constants.NEW_LINE);

            if (_namesGroup != null)
                sb.Append(newOutTab).Append(DivisionNamesGroup.BLOCK_NAME).Append(" = ").Append(_namesGroup.Name).Append(Constants.NEW_LINE);

            if (_isLocked == true)
                sb.Append(newOutTab).Append("is_locked = yes").Append(Constants.NEW_LINE);

            if (_forceAllowRecruiting == true)
                sb.Append(newOutTab).Append("force_allow_recruiting = yes").Append(Constants.NEW_LINE);

            if (_divisionCap != null)
                sb.Append(newOutTab).Append("division_cap = ").Append(_divisionCap).Append(Constants.NEW_LINE);

            if (_priority != DefaultPriority)
                sb.Append(newOutTab).Append("priority = ").Append(_priority).Append(Constants.NEW_LINE);

            if (_templateCounter != null)
                sb.Append(newOutTab).Append("template_counter = ").Append(_templateCounter).Append(Constants.NEW_LINE);

            if (_overrideModel != null)
                sb.Append(newOutTab).Append("override_model = ").Append(_overrideModel).Append(Constants.NEW_LINE);

            _regimentsSubUnits?.Save(sb, newOutTab, tab);
            _supportSubUnits?.Save(sb, newOutTab, tab);

            sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions($"{BLOCK_NAME} (name = {_name})", () =>
            {
                string value;

                if (token == "name")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
                else if (token == "division_names_group")
                {
                    value = parser.ReadString();
                    if (!DivisionNamesGroupManager.TryGetNamesGroup(value, out _namesGroup))
                        throw new DivisionNamesGroupNotFoundException(value);
                }
                else if (token == "is_locked")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isLocked, parser.ReadBool());
                else if (token == "force_allow_recruiting")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _forceAllowRecruiting, parser.ReadBool());
                else if (token == "division_cap")
                {
                    value = parser.ReadString();
                    if (!int.TryParse(value, out int newDivisionCap))
                        Logger.WrapException(token, new IncorrectValueException(value));
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _divisionCap, newDivisionCap);
                }
                else if (token == "priority")
                {
                    value = parser.ReadString();
                    if (!byte.TryParse(value, out byte newPriority))
                        Logger.WrapException(token, new IncorrectValueException(value));
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _priority, newPriority);
                }
                else if (token == "template_counter")
                {
                    value = parser.ReadString();
                    if (!int.TryParse(value, out int newTemplateCounter))
                        Logger.WrapException(token, new IncorrectValueException(value));
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _templateCounter, newTemplateCounter);
                }
                else if (token == "override_model")
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _overrideModel, parser.ReadString());
                else if (token == "regiments")
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _regimentsSubUnits, parser, new SubUnitsBlock(token));
                else if (token == "support")
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _supportSubUnits, parser, new SubUnitsBlock(token));
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer linkedLayer)
        {
            bool result = true;

            if (_name == null)
            {
                Logger.LogWarning(
                    EnumLocKey.WARNING_BLOCK_HAS_NO_MANDATORY_INNER_PARAMETER_IN_FILE,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{blockName}", BLOCK_NAME },
                        { "{parameterName}", "name" },
                    }
                );
                result = false;
            }

            if (_regimentsSubUnits == null)
            {
                Logger.LogWarning(
                    EnumLocKey.WARNING_BLOCK_HAS_NO_MANDATORY_INNER_PARAMETER_IN_FILE,
                    new Dictionary<string, string>
                    {
                        { "{filePath}", _currentOOB?.FileInfo?.filePath },
                        { "{blockName}", BLOCK_NAME },
                        { "{parameterName}", "regiments" },
                    }
                );
                result = false;
            }

            return result;
        }

        private class SubUnitsBlock : IParadoxObject
        {
            private readonly string _name;
            private readonly Dictionary<XYUshortCoordinates, SubUnit> _subUnits;

            public SubUnitsBlock(string name)
            {
                _name = name;
                _subUnits = new Dictionary<XYUshortCoordinates, SubUnit>();
            }

            public void Save(StringBuilder sb, string outTab, string tab)
            {
                if (_subUnits.Count == 0) return;

                sb.Append(outTab).Append(_name).Append(" = {").Append(Constants.NEW_LINE);

                var coords = new List<XYUshortCoordinates>(_subUnits.Keys);
                coords.OrderBy(o => o.X).ThenBy(o => o.Y);

                foreach (var XY in coords)
                {
                    var subUnit = _subUnits[XY];
                    sb.Append(outTab).Append(tab).Append(subUnit.Name).Append(" = ");
                    XY.Save(sb);
                    sb.Append(Constants.NEW_LINE);
                }

                sb.Append(outTab).Append('}').Append(Constants.NEW_LINE);
            }

            public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
            {
                if (!SubUnitManager.TryGetSubUnit(token, out SubUnit subUnit))
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_SUB_UNIT_NOT_FOUND,
                        new Dictionary<string, string> { { "{name}", token } }
                    ));


                XYUshortCoordinates coords = null;
                try
                {
                    coords = parser.Parse(new XYUshortCoordinates());
                }
                catch (Exception ex)
                {
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_DIVISION_TEMPLATE_INCORRECT_SUB_UNIT_COORDINATES,
                        new Dictionary<string, string> { { "{subUnitName}", subUnit.Name } }
                    ), ex);
                }

                if (_subUnits.TryGetValue(coords, out SubUnit otherSubUnit))
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_DIVISION_TEMPLATE_SUB_UNITS_COORDINATES_OVERLAPPING,
                        new Dictionary<string, string>
                        {
                            { "{subUnitName}", subUnit.Name },
                            { "{otherSubUnitName}", otherSubUnit.Name },
                            { "{X}", $"{coords.X}" },
                            { "{Y}", $"{coords.Y}" }
                        }
                    ));

                _subUnits[coords] = subUnit;
            }

            public bool Validate(LinkedLayer prevLayer) => true;
        }
    }


}
