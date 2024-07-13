using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.air;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{

    class OOBAirWingsGroup : IParadoxObject
    {
        private bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;
                if (_state != null && _state.HasChangedId) return true;

                if (_airWings != null)
                {
                    foreach (var airWing in _airWings)
                        if (airWing.NeedToSave) return true;
                }
                return false;
            }
        }

        public bool HasAnyInnerInfo => _airWings != null && _airWings.Count > 0;

        private State _state;
        public State State { get => _state; set => Utils.Setter(ref _state, ref value, ref _needToSave); }

        private ShipInstances _carrier;
        public ShipInstances Carrier { get => _carrier; set => Utils.Setter(ref _carrier, ref value, ref _needToSave); }

        private List<AirWing> _airWings;
        public List<AirWing> AirWings { get => _airWings; set => Utils.Setter(ref _airWings, ref value, ref _needToSave); }

        private AirWing _lastAirWing;

        public OOBAirWingsGroup(State state)
        {
            _state = state;
        }

        public OOBAirWingsGroup(ShipInstances carrier)
        {
            _carrier = carrier;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            //TODO Redo later (just temp checks)
            if (_state == null && _carrier == null)
            {
                Logger.LogSingleErrorMessage("Airwing has no State nor Carrier base!");
                return false;
            }

            if (_state != null && _carrier != null)
            {
                Logger.LogSingleErrorMessage("Airwing has both State and Carrier base!");
                return false;
            }

            var newOutTab = outTab + tab;

            if (_state != null)
                ParadoxUtils.StartBlock(sb, outTab, $"{_state.Id}");
            else if (_carrier != null)
                ParadoxUtils.StartBlock(sb, outTab, $"\"{_carrier.Name}\"");

            bool savedAnyData = false;
            foreach (var airWing in AirWings)
            {
                if (savedAnyData) sb.Append(newOutTab).Append(Constants.NEW_LINE);
                savedAnyData = airWing.Save(sb, newOutTab, tab);
            }

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(_state != null ? $"{_state?.Id}" : $"\"{_carrier.Name}\"", () =>
            {
                if (token == AirWing.TOKEN_NAME)
                {
                    if (_lastAirWing == null) throw new Exception(); //TODO implement
                    _lastAirWing.ParseName(prevLayer, token, parser);
                }
                else if (token == AirWingAce.BLOCK_NAME)
                {
                    if (_lastAirWing == null) throw new Exception(); //TODO implement
                    _lastAirWing.ParseAce(prevLayer, token, parser);
                }
                else if (true) //TODO implement equipment check
                {
                    Logger.ParseLayeredValue(prevLayer, token, ref _lastAirWing, parser, new AirWing());
                }
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
