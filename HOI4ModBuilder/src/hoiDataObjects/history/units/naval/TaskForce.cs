using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval
{
    class TaskForce : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "task_force";

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave ||
                    _location != null && _location.HasChangedId)
                {
                    return true;
                }

                foreach (var ship in _ships)
                    if (ship.NeedToSave) return true;

                return false;
            }
        }

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private static readonly string TOKEN_LOCATION = "location";
        private Province _location;
        public Province Location { get => _location; set => Utils.Setter(ref _location, ref value, ref _needToSave); }

        private List<Ship> _ships = new List<Ship>();

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == TOKEN_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
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
                else if (token == Ship.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, ref _ships, parser, new Ship());
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            var currentLayer = new LinkedLayer(prevLayer, $"({TOKEN_NAME} = \"{_name}\")");

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_NAME, ref _name)
                .HasMandatory(ref result, prevLayer, TOKEN_LOCATION, ref _location)
                .Check(
                    ref result, currentLayer,
                    _ships != null && _ships.Count > 0,
                    EnumLocKey.TASK_FORCE_MUST_HAVE_AT_LEAST_ONE_SHIP
                );

            return result;
        }
    }
}
