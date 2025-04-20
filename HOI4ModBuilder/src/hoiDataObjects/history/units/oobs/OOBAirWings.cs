using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{

    class OOBAirWings : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "air_wings";

        private bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;

                foreach (var entry in _stateAirWingsGroup)
                    if (entry.Key.HasChangedId || entry.Value.NeedToSave) return true;
                foreach (var entry in _carrierAirWingsGroup)
                    if (entry.Key.NeedToSave || entry.Value.NeedToSave) return true;

                return false;
            }
        }

        public bool HasAnyInnerInfo => _stateAirWingsGroup.Count > 0 || _carrierAirWingsGroup.Count > 0;

        private Dictionary<State, OOBAirWingsGroup> _stateAirWingsGroup = new Dictionary<State, OOBAirWingsGroup>();
        private Dictionary<ShipInstances, OOBAirWingsGroup> _carrierAirWingsGroup = new Dictionary<ShipInstances, OOBAirWingsGroup>();


        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            string newOutTab = outTab + tab;
            bool newParagraphFlag = false;
            bool result = false;

            List<State> states = new List<State>(_stateAirWingsGroup.Keys);
            states.Sort((x, y) => x.Id.GetValue().CompareTo(y.Id.GetValue()));
            foreach (var state in states)
            {
                ParadoxUtils.NewLineIfNeeded(sb, newOutTab, ref newParagraphFlag);
                newParagraphFlag = _stateAirWingsGroup[state].Save(sb, newOutTab, tab);
                result |= newParagraphFlag;
            }

            foreach (var airWingsGroup in _carrierAirWingsGroup.Values)
            {
                ParadoxUtils.NewLineIfNeeded(sb, newOutTab, ref newParagraphFlag);
                newParagraphFlag = airWingsGroup.Save(sb, newOutTab, tab);
                result |= newParagraphFlag;
            }

            return result;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (ushort.TryParse(token, out ushort stateId))
                {
                    if (!StateManager.TryGetState(stateId, out State state))
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.STATE_NOT_FOUND_BY_ID,
                            new Dictionary<string, string> { { "{stateId}", token } }
                        );
                    else
                    {
                        if (!_stateAirWingsGroup.TryGetValue(state, out OOBAirWingsGroup airWingsGroup))
                            airWingsGroup = new OOBAirWingsGroup(state);

                        Logger.ParseLayeredValue(prevLayer, token, airWingsGroup, parser);
                        _stateAirWingsGroup[state] = airWingsGroup;
                    }
                }
                else if (parser.CurrentValueIsName)
                {
                    var taskForceShipIntances = OOBManager.RequestShipInstances(token, prevLayer);
                    if (!_carrierAirWingsGroup.TryGetValue(taskForceShipIntances, out OOBAirWingsGroup airWingsGroup))
                        airWingsGroup = new OOBAirWingsGroup(taskForceShipIntances);

                    Logger.ParseLayeredValue(prevLayer, token, airWingsGroup, parser);
                    _carrierAirWingsGroup[taskForceShipIntances] = airWingsGroup;
                }
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
