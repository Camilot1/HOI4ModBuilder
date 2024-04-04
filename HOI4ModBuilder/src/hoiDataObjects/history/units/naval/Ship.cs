using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils.exceptions;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval
{
    class Ship : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "ship";

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private bool _needToSave;
        public bool NeedToSave => _needToSave || _equipmentVariant != null && _equipmentVariant.NeedToSave;

        private ShipInstances _taskForceShipInstances;

        private static readonly string TOKEN_NAME = "name";
        private bool _hasChangedName;
        public bool HasChangedName { get => _hasChangedName; }
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave, ref _hasChangedName); }

        private static readonly string TOKEN_DEFINITION = "definition";
        private string _definition; //TODO implement naval subunit using
        public string Definition { get => _definition; set => Utils.Setter(ref _definition, ref value, ref _needToSave); }

        private static readonly string TOKEN_IS_PRIDE_OF_THE_FLEET = "pride_of_the_fleet";
        private bool? _isPrideOfTheFleet;
        public bool? IsPrideOfTheFleet { get => _isPrideOfTheFleet; set => Utils.Setter(ref _isPrideOfTheFleet, ref value, ref _needToSave); }

        private static readonly string TOKEN_START_EXPERIENCE_FACTOR = "start_experience_factor";
        private static readonly float DEFAULT_START_EXPERIENCE_FACTOR = 0;
        private float? _startExperienceFactor;
        public float? StartExperienceFactor { get => _startExperienceFactor; set => Utils.Setter(ref _startExperienceFactor, ref value, ref _needToSave); }

        private ShipEquipmentVariant _equipmentVariant;
        public ShipEquipmentVariant EquipmentVariant { get => _equipmentVariant; set => Utils.Setter(ref _equipmentVariant, ref value, ref _needToSave); }


        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            ParadoxUtils.StartInlineBlock(sb, outTab, BLOCK_NAME);
            ParadoxUtils.SaveQuotedInline(sb, " ", TOKEN_NAME, _name);
            ParadoxUtils.SaveInline(sb, " ", TOKEN_DEFINITION, _definition);
            ParadoxUtils.SaveInline(sb, " ", TOKEN_IS_PRIDE_OF_THE_FLEET, _isPrideOfTheFleet);
            ParadoxUtils.SaveInline(sb, " ", TOKEN_START_EXPERIENCE_FACTOR, _startExperienceFactor, DEFAULT_START_EXPERIENCE_FACTOR);
            _equipmentVariant?.Save(sb, " ", "");
            ParadoxUtils.EndBlock(sb, " ");

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            if (token == TOKEN_NAME)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
            else if (token == TOKEN_DEFINITION)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _definition, parser.ReadString());
            else if (token == TOKEN_IS_PRIDE_OF_THE_FLEET)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isPrideOfTheFleet, parser.ReadBool());
            else if (token == TOKEN_START_EXPERIENCE_FACTOR)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _startExperienceFactor, parser.ReadFloat());
            else if (token == ShipEquipmentVariant.BLOCK_NAME)
                Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _equipmentVariant, parser, new ShipEquipmentVariant());
            else throw new UnknownTokenException(token);
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            _taskForceShipInstances = OOBManager.RequestShipInstances(_name, null);
            _taskForceShipInstances.Ships.Add(this);

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_NAME, ref _name)
                .HasMandatory(ref result, prevLayer, TOKEN_DEFINITION, ref _definition)
                .HasMandatory(ref result, prevLayer, ShipEquipmentVariant.BLOCK_NAME, ref _equipmentVariant);

            return result;
        }
    }
}
