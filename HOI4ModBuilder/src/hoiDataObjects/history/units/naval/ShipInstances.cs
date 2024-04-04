using HOI4ModBuilder.src.utils;
using System.Collections.Generic;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval
{

    class ShipInstances
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private bool _needToSave;
        public bool NeedToSave => _needToSave;

        private bool _hasChangedName;
        public bool HasChangedName { get => _hasChangedName; }
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave, ref _hasChangedName); }

        private List<Ship> _taskForceShips = new List<Ship>();
        public List<Ship> TaskForceShips { get => _taskForceShips; }

        public List<LinkedLayer> requests = new List<LinkedLayer>();

        public ShipInstances(string name)
        {
            _name = name;
        }
    }
}
