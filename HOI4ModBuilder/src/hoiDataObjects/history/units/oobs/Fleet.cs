using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class Fleet : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "fleet";

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;

                foreach (var fleetTaskForce in _taskForces)
                    if (fleetTaskForce.NeedToSave) return true;

                return false;
            }
        }

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private static readonly string TOKEN_NAVAL_BASE = "naval_base";
        private Province _navalBase;
        public Province NavalBase { get => _navalBase; set => Utils.Setter(ref _navalBase, ref value, ref _needToSave); }

        private List<FleetTaskForce> _taskForces = new List<FleetTaskForce>();

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            string newOutTab = outTab + tab;
            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_NAME, _name);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == TOKEN_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
                else if (token == TOKEN_NAVAL_BASE)
                {
                    var provinceId = parser.ReadUInt16();
                    if (!ProvinceManager.TryGetProvince(provinceId, out Province newProvince))
                        Logger.WrapException(token, new ProvinceNotFoundException(provinceId));
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _navalBase, newProvince);
                }
                else if (token == FleetTaskForce.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, _taskForces, parser, new FleetTaskForce());
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_NAME, ref _name)
                .HasMandatory(ref result, prevLayer, TOKEN_NAVAL_BASE, ref _navalBase);
            //TODO add check for _taskForce.Count == 0

            return result;
        }
    }

    class FleetTaskForce : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "task_force";

        public bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;
                if (_location != null && _location.HasChangedId) return true;

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

        private List<TaskForceShip> _ships = new List<TaskForceShip>();

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
                    if (!ProvinceManager.TryGetProvince(provinceId, out Province newProvince))
                        Logger.WrapException(token, new ProvinceNotFoundException(provinceId));
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _location, newProvince);
                }
                else if (token == TaskForceShip.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, _ships, parser, new TaskForceShip());
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_NAME, ref _name)
                .HasMandatory(ref result, prevLayer, TOKEN_LOCATION, ref _location);
            //TODO add check for _ships.Count == 0

            return result;
        }
    }

    class TaskForceShip : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "ship";

        public bool _needToSave;
        public bool NeedToSave => _needToSave || _equipmentVariant != null && _equipmentVariant.NeedToSave;

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

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

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_NAME, ref _name)
                .HasMandatory(ref result, prevLayer, TOKEN_DEFINITION, ref _definition)
                .HasMandatory(ref result, prevLayer, ShipEquipmentVariant.BLOCK_NAME, ref _equipmentVariant);

            return result;
        }
    }

    class ShipEquipmentVariant : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "equipment";

        public bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;

                return false;
            }
        }

        private string _name; //TODO implement equipment usage
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private static readonly string TOKEN_AMOUNT = "amount";
        private byte? _amount;
        public byte? Amount { get => _amount; set => Utils.Setter(ref _amount, ref value, ref _needToSave); }

        private static readonly string TOKEN_OWNER = "owner";
        private Country _owner;
        public Country Owner { get => _owner; set => Utils.Setter(ref _owner, ref value, ref _needToSave); }

        private static readonly string TOKEN_VERSION_NAME = "version_name";
        private string _versionName;
        public string VersionName { get => _versionName; set => Utils.Setter(ref _versionName, ref value, ref _needToSave); }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            ParadoxUtils.StartInlineBlock(sb, outTab, BLOCK_NAME);

            ParadoxUtils.StartInlineBlock(sb, " ", _name);

            ParadoxUtils.SaveInline(sb, " ", TOKEN_AMOUNT, _amount);
            ParadoxUtils.SaveInline(sb, " ", TOKEN_OWNER, _owner?.Tag);
            ParadoxUtils.SaveQuotedInline(sb, " ", TOKEN_VERSION_NAME, _versionName);

            ParadoxUtils.EndInlineBlock(sb, " ");

            ParadoxUtils.EndInlineBlock(sb, " ");

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            if (_name != null)
            {
                Logger.LogLayeredError(
                    prevLayer, EnumLocKey.LAYERED_LEVELS_PARAMETER_VALUE_OVERRIDDEN,
                    new Dictionary<string, string>
                    {
                        { "{oldParameterValue}", _name?.ToString() },
                        { "{newParameterValue}", token?.ToString() }
                    }
                );

                _name = token;
                parser.AdvancedParse(prevLayer, InnerTokenCallback);
            }
        }

        public void InnerTokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            if (token == TOKEN_AMOUNT)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _amount, parser.ReadByte());
            else if (token == TOKEN_OWNER)
            {
                var value = parser.ReadString();
                if (!CountryManager.TryGetCountry(value, out Country country))
                    Logger.WrapException(token, new CountryNotFoundException(value));
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _owner, country);
            }
            else if (token == TOKEN_VERSION_NAME)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _versionName, parser.ReadString());
            else throw new UnknownTokenException(token);
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_AMOUNT, ref _amount)
                .HasMandatory(ref result, prevLayer, TOKEN_OWNER, ref _owner);

            return result;
        }
    }
}
