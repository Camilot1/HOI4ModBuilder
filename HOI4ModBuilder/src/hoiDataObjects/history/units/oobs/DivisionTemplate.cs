using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.hoiDataObjects.common.units;
using HOI4ModBuilder.src.hoiDataObjects.common.units.divisionsNames;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.divisionTemplates
{
    class DivisionTemplate : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave;
            private set => NeedToSave = value;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;

                _name = value;
                NeedToSave = true;
            }
        }

        private DivisionNamesGroup _namesGroup;
        public DivisionNamesGroup NamesGroup
        {
            get => _namesGroup;
            set
            {
                if (_namesGroup == value) return;

                _namesGroup = value;
                NeedToSave = true;
            }
        }

        private bool _isLocked;
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked == value) return;

                _isLocked = value;
                NeedToSave = true;
            }
        }

        private bool _forceAllowRecruiting;
        public bool ForceAllowRecruiting
        {
            get => _forceAllowRecruiting;
            set
            {
                if (_forceAllowRecruiting == value) return;

                _forceAllowRecruiting = value;
                NeedToSave = true;
            }
        }

        private int _divisionCap;
        public int DivisionCap
        {
            get => _divisionCap;
            set
            {
                if (_divisionCap == value) return;

                _divisionCap = value;
                NeedToSave = true;
            }
        }

        private static readonly int DefaultPriority = 1;
        public int _priority = DefaultPriority;
        public int Priority
        {
            get => _priority;
            set
            {
                if (_priority == value) return;

                _priority = value;
                NeedToSave = true;
            }
        }

        private static readonly int DefaultTemplateCounter = -1;
        public int _templateCounter = DefaultTemplateCounter;
        public int TemplateCounter
        {
            get => _templateCounter;
            set
            {
                if (_templateCounter == value) return;

                _templateCounter = value;
                NeedToSave = true;
            }
        }
        public string _overrideModel; //TODO Implement entity models
        public string OverrideModel
        {
            get => _overrideModel;
            set
            {
                if (_overrideModel == value) return;

                _overrideModel = value;
                NeedToSave = true;
            }
        }

        private SubUnitsBlock _regimentsSubUnits;
        private SubUnitsBlock _supportSubUnits;

        public void Save(StringBuilder sb, string tab)
        {
            sb.Append("division_template = {").Append(Constants.NEW_LINE);
            sb.Append(tab).Append("name = \"").Append(Name).Append('\"').Append(Constants.NEW_LINE);

            if (NamesGroup != null) sb.Append(tab).Append("division_names_group = ").Append(NamesGroup.Name).Append(Constants.NEW_LINE);
            if (IsLocked) sb.Append(tab).Append("is_locked = yes").Append(Constants.NEW_LINE);
            if (ForceAllowRecruiting) sb.Append(tab).Append("force_allow_recruiting = yes").Append(Constants.NEW_LINE);
            if (DivisionCap != 0) sb.Append(tab).Append("division_cap = ").Append(DivisionCap).Append(Constants.NEW_LINE);
            if (Priority != DefaultPriority) sb.Append(tab).Append("priority = ").Append(Priority).Append(Constants.NEW_LINE);
            if (TemplateCounter != DefaultTemplateCounter) sb.Append(tab).Append("template_counter = ").Append(TemplateCounter).Append(Constants.NEW_LINE);
            if (OverrideModel != null) sb.Append(tab).Append("override_model = ").Append(OverrideModel).Append(Constants.NEW_LINE);

            _regimentsSubUnits?.Save(sb, tab, tab);
            _supportSubUnits?.Save(sb, tab, tab);
            sb.Append('}').Append(Constants.NEW_LINE);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "name": _name = parser.ReadString(); break;
                case "division_names_group":
                    var namesGroupStr = parser.ReadString();
                    if (!DivisionNamesGroupManager.TryGetNamesGroup(namesGroupStr, out _namesGroup))
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.ERROR_DIVISION_NAMES_GROUP_NOT_FOUND,
                            new Dictionary<string, string> { { "{name}", namesGroupStr } }
                        ));
                    break;
                case "is_locked": _isLocked = parser.ReadBool(); break;
                case "force_allow_recruiting": _forceAllowRecruiting = parser.ReadBool(); break;
                case "division_cap": _divisionCap = parser.ReadInt32(); break;
                case "priority": _priority = parser.ReadInt32(); break;
                case "template_counter": _templateCounter = parser.ReadUInt16(); break;
                case "override_model": _overrideModel = parser.ReadString(); break;

                case "regiments":
                    try
                    {
                        _regimentsSubUnits = parser.Parse(new SubUnitsBlock(token));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_WHILE_BLOCK_LOADING,
                            new Dictionary<string, string> { { "{name}", token } }
                        ), ex);
                    }
                    break;
                case "support":
                    try
                    {
                        _supportSubUnits = parser.Parse(new SubUnitsBlock(token));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.EXCEPTION_WHILE_BLOCK_LOADING,
                            new Dictionary<string, string> { { "{name}", token } }
                        ), ex);
                    }
                    break;
                default:
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.ERROR_DIVISION_TEMPLATE_UNKNOWN_TOKEN,
                        new Dictionary<string, string> { { "{token}", token } }
                    ));
            }
        }

        private class SubUnitsBlock : IParadoxRead
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

            public void TokenCallback(ParadoxParser parser, string token)
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
        }
    }


}
