using HOI4ModBuilder;
using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.air;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class AirWing : IParadoxObject
    {
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
                if (_state != null && _state.HasChangedId) return true;
                if (_carrier != null && _carrier.HasChangedName) return true;
                if (_owner != null && _owner.HasChangedTag) return true;
                if (_creator != null && _creator.HasChangedTag) return true;
                if (_ace != null && _ace.NeedToSave) return true;
                return false;
            }
        }

        public static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }
        public void ParseName(LinkedLayer prevLayer, string token, ParadoxParser parser)
            => Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());

        private State _state;
        public State State { get => _state; set => Utils.Setter(ref _state, ref value, ref _needToSave); }

        private Ship _carrier;
        public Ship Carrier { get => _carrier; set => Utils.Setter(ref _carrier, ref value, ref _needToSave); }

        private string _equipment; //TODO implement equipment usage
        public string Equipment { get => _equipment; set => Utils.Setter(ref _equipment, ref value, ref _needToSave); }

        private static readonly string TOKEN_AMOUNT = "amount";
        private byte? _amount;
        public byte? Amount { get => _amount; set => Utils.Setter(ref _amount, ref value, ref _needToSave); }

        private static readonly string TOKEN_OWNER = "owner";
        private Country _owner;
        public Country Owner { get => _owner; set => Utils.Setter(ref _owner, ref value, ref _needToSave); }

        private static readonly string TOKEN_CREATOR = "creator";
        private Country _creator;
        public Country Creator { get => _creator; set => Utils.Setter(ref _creator, ref value, ref _needToSave); }

        private static readonly string TOKEN_VERSION_NAME = "version_name";
        private string _versionName;
        public string VersionName { get => _versionName; set => Utils.Setter(ref _versionName, ref value, ref _needToSave); }


        private AirWingAce _ace;
        public AirWingAce Ace { get => _ace; set => Utils.Setter(ref _ace, ref value, ref _needToSave); }
        public void ParseAce(LinkedLayer prevLayer, string token, ParadoxParser parser)
            => Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _ace, parser, new AirWingAce());

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            ParadoxUtils.StartInlineBlock(sb, " ", _equipment);

            ParadoxUtils.SaveQuotedInline(sb, " ", TOKEN_OWNER, _owner?.Tag);
            ParadoxUtils.SaveQuotedInline(sb, " ", TOKEN_CREATOR, _creator?.Tag);
            ParadoxUtils.SaveInline(sb, " ", TOKEN_AMOUNT, _amount);
            ParadoxUtils.SaveQuotedInline(sb, " ", TOKEN_VERSION_NAME, _versionName);

            ParadoxUtils.EndInlineBlock(sb, " ");

            ParadoxUtils.SaveQuoted(sb, outTab, TOKEN_NAME, _name);
            _ace?.Save(sb, outTab, tab);
            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(token, () =>
            {
                if (token == TOKEN_OWNER)
                {
                    var value = parser.ReadString();
                    if (CountryManager.TryGetCountry(value, out Country country))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _owner, country);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.COUNTRY_NOT_FOUND,
                            new Dictionary<string, string> { { "{countryTag}", value } }
                        );
                }
                else if (token == TOKEN_CREATOR)
                {
                    var value = parser.ReadString();
                    if (CountryManager.TryGetCountry(value, out Country country))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _creator, country);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.COUNTRY_NOT_FOUND,
                            new Dictionary<string, string> { { "{countryTag}", value } }
                        );
                }
                else if (token == TOKEN_AMOUNT)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _amount, parser.ReadByte());
                else if (token == TOKEN_VERSION_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _versionName, parser.ReadString());
                else throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_OWNER, ref _owner)
                .HasMandatory(ref result, prevLayer, TOKEN_AMOUNT, ref _amount);

            return result;
        }
    }
}