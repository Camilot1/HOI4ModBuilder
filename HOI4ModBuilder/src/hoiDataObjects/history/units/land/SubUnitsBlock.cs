using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.hoiDataObjects.common.units;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.land
{
    class SubUnitsBlock : IParadoxObject
    {
        private readonly string _name;
        private readonly List<SubUnitInfo> _subUnits = new List<SubUnitInfo>();

        public bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;

                foreach (var subUnit in _subUnits)
                    if (subUnit.NeedToSave) return true;

                return false;
            }
        }

        public int Count => _subUnits.Count;

        public SubUnitsBlock(string name)
        {
            _name = name;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (_subUnits.Count == 0) return false;

            var infos = new List<SubUnitInfo>(_subUnits);
            infos.OrderBy(o => o.Coords.X).ThenBy(o => o.Coords.Y);

            ParadoxUtils.StartBlock(sb, outTab, _name);

            foreach (var info in infos)
            {
                ParadoxUtils.StartInlineBlock(sb, outTab, info.SubUnit.Name);
                info.Coords.Save(sb, " ", "");
                ParadoxUtils.EndBlock(sb, " ");
            }

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(_name, () =>
            {
                if (!SubUnitManager.TryGetSubUnit(token, out SubUnit subUnit))
                    Logger.LogLayeredError(
                        prevLayer, token, EnumLocKey.SUB_UNIT_NOT_FOUND,
                        new Dictionary<string, string> { { "{name}", token } }
                    );

                Logger.WrapTokenCallbackExceptions(token, () =>
                {
                    var coords = parser.AdvancedParse(prevLayer, new XYUshortCoordinates(), out bool _);
                    var newInfo = new SubUnitInfo(subUnit, coords);

                    foreach (var info in _subUnits)
                    {
                        if (info.Coords.Equals(coords))
                            Logger.LogLayeredWarning(
                                prevLayer, EnumLocKey.SUB_UNITS_COORDINATES_OVERLAPPING,
                                new Dictionary<string, string>
                                {
                                        { "{firstSubUnitName}", info.SubUnit?.Name },
                                        { "{secondSubUnitName}", newInfo.SubUnit?.Name },
                                        { "{XY}", $"{coords.X}; {coords.Y}"}
                                }
                            );
                    }

                    _subUnits.Add(newInfo);
                });

            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
