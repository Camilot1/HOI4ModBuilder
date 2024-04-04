using HOI4ModBuilder.src.utils;
using System.Collections.Generic;
using System.Text;

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

        private List<Ship> _ships = new List<Ship>();
        public List<Ship> Ships { get => _ships; }

        private List<LinkedLayer> _requests = new List<LinkedLayer>();
        public void AddRequest(LinkedLayer layer) => _requests.Add(layer);
        public int RequestsCount => _requests.Count;
        public void AssembleLayeredPathes(StringBuilder sb)
        {
            foreach (var layer in _requests)
            {
                string filePath = null;
                string blockLayeredPath = null;
                layer.AssembleLayeredPath(ref filePath, ref blockLayeredPath);
                sb.Append("\t\nLayeredPath: \"").Append(blockLayeredPath).Append('\"');
            }
        }

        public ShipInstances(string name)
        {
            _name = name;
        }
    }
}
