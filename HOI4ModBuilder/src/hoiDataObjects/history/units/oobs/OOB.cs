using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.history.units.divisionTemplates;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOB : IParadoxObject
    {
        public FileInfo FileInfo { get; set; }

        private List<DivisionTemplate> _divisionTemplates = new List<DivisionTemplate>();
        private List<DivisionTemplate> DivisionTemplates { get => _divisionTemplates; }

        private Units _units;
        public Units Units { get => _units; }

        private AirWings _airWings;
        private AirWings AirWings { get => _airWings; }

        private InstantEffect _instantEffect;
        public InstantEffect InstantEffect { get => _instantEffect; }

        public OOB(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            bool newParagraphFlag = false;

            foreach (var divisionTemplate in _divisionTemplates)
            {
                ParadoxUtils.NewLineIfNeeded(sb, "", ref newParagraphFlag);
                newParagraphFlag = divisionTemplate.Save(sb, "", "\t");
            }

            if (_units != null && _units.HasAnyInnerInfo)
            {
                ParadoxUtils.NewLineIfNeeded(sb, "", ref newParagraphFlag);
                newParagraphFlag = _units.Save(sb, "", "\t");
            }

            if (_airWings != null && _airWings.HasAnyInnerInfo)
            {
                ParadoxUtils.NewLineIfNeeded(sb, "", ref newParagraphFlag);
                newParagraphFlag = _units.Save(sb, "", "\t");
            }

            if (_instantEffect != null && _instantEffect.HasAnyInnerInfo)
            {
                ParadoxUtils.NewLineIfNeeded(sb, "", ref newParagraphFlag);
                newParagraphFlag = _units.Save(sb, "", "\t");
            }

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(FileInfo.filePath, () =>
            {
                if (token == DivisionTemplate.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, ref _divisionTemplates, parser, new DivisionTemplate());
                else if (token == Units.BLOCK_NAME)
                    Logger.ParseNewLayeredValueOrContinueOld(prevLayer, token, ref _units, parser, new Units());
                else if (token == AirWings.BLOCK_NAME)
                    Logger.ParseNewLayeredValueOrContinueOld(prevLayer, token, ref _airWings, parser, new AirWings());
                else if (token == InstantEffect.BLOCK_NAME)
                    Logger.ParseNewLayeredValueOrContinueOld(prevLayer, token, ref _instantEffect, parser, new InstantEffect());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    class Units : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "units";

        public bool HasAnyInnerInfo => _divisions.Count != 0 || _fleets.Count != 0;

        private List<Division> _divisions = new List<Division>();
        public List<Division> Divisions { get; }

        private List<Fleet> _fleets = new List<Fleet>();
        public List<Fleet> Fleets { get; }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            string newOutTab = outTab + tab;

            foreach (var division in _divisions) division.Save(sb, newOutTab, tab);
            foreach (var fleet in _fleets) fleet.Save(sb, newOutTab, tab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == Division.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, ref _divisions, parser, new Division());
                else if (token == Fleet.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, ref _fleets, parser, new Fleet());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer linkedLayer) => true;
    }

    class AirWings : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "air_wings";

        public bool HasAnyInnerInfo => true;

        private Dictionary<State, AirWingsGroup> _stateAirWingsGroup = new Dictionary<State, AirWingsGroup>();
        private Dictionary<TaskForceShipInstances, AirWingsGroup> _carrierAirWingsGroup = new Dictionary<TaskForceShipInstances, AirWingsGroup>();


        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new NotImplementedException();
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
                        _stateAirWingsGroup.TryGetValue(state, out AirWingsGroup airWingsGroup);
                        Logger.ParseNewLayeredValueOrContinueOld(prevLayer, token, ref airWingsGroup, parser, new AirWingsGroup(state));
                        _stateAirWingsGroup[state] = airWingsGroup;
                    }
                }
                else if (parser.CurrentValueIsName)
                {
                    var taskForceShipIntances = OOBManager.RequestTaskForceShipInstances(token, prevLayer);
                    _carrierAirWingsGroup.TryGetValue(taskForceShipIntances, out AirWingsGroup airWingsGroup);
                    Logger.ParseNewLayeredValueOrContinueOld(prevLayer, token, ref airWingsGroup, parser, new AirWingsGroup(taskForceShipIntances));
                    _carrierAirWingsGroup[taskForceShipIntances] = airWingsGroup;
                }
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    class AirWingsGroup : IParadoxObject
    {
        public bool _needToSave;
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

        private TaskForceShipInstances _carrier;
        public TaskForceShipInstances Carrier { get => _carrier; set => Utils.Setter(ref _carrier, ref value, ref _needToSave); }

        private List<AirWing> _airWings;
        public List<AirWing> AirWings { get => _airWings; set => Utils.Setter(ref _airWings, ref value, ref _needToSave); }

        private AirWing _lastAirWing;

        public AirWingsGroup(State state)
        {
            _state = state;
        }

        public AirWingsGroup(TaskForceShipInstances carrier)
        {
            _carrier = carrier;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            //TODO Redo later (just temp checks)
            if (_state == null && _carrier == null)
            {
                Logger.LogSingleMessage("Airwing has no State nor Carrier base!");
                return false;
            }

            if (_state != null && _carrier != null)
            {
                Logger.LogSingleMessage("Airwing has both State and Carrier base!");
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

    class InstantEffect : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "instant_effect";

        public bool HasAnyInnerInfo => _effects != null && _effects.Count > 0;

        private List<DataArgsBlock> _effects;
        public List<DataArgsBlock> Effects { get => _effects; }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                DataArgsBlocksManager.ParseDataArgsBlock(parser, null, token, null, _effects);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            throw new NotImplementedException();
        }
    }
}
