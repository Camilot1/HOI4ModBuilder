using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.hoiDataObjects.common.units;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.land
{
    class SubUnitInfo
    {
        private bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _subUnit != null && _subUnit.NeedToSave ||
                _coords != null && _coords.NeedToSave;
        }

        private SubUnit _subUnit;
        public SubUnit SubUnit { get => _subUnit; set => Utils.Setter(ref _subUnit, ref value, ref _needToSave); }

        private XYUshortCoordinates _coords;
        public XYUshortCoordinates Coords { get => _coords; set => Utils.Setter(ref _coords, ref value, ref _needToSave); }

        public SubUnitInfo(SubUnit subUnit, XYUshortCoordinates coords)
        {
            _subUnit = subUnit;
            _coords = coords;
        }
    }
}
