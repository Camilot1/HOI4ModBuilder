using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils.exceptions;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval
{
    class ShipEquipmentVariant : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "equipment";

        public bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _owner != null && _owner.HasChangedTag;
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
                if (CountryManager.TryGetCountry(value, out Country country))
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _owner, country);
                else
                    Logger.LogLayeredError(
                        prevLayer, token, EnumLocKey.COUNTRY_NOT_FOUND,
                        new Dictionary<string, string> { { "{countryTag}", value } }
                    );
            }
            else if (token == TOKEN_VERSION_NAME)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _versionName, parser.ReadString());
            else throw new UnknownTokenException(token);
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            //CheckAndLogUnit.WARNINGS
            //    .HasMandatory(ref result, prevLayer, TOKEN_AMOUNT, ref _amount)
            //    .HasMandatory(ref result, prevLayer, TOKEN_OWNER, ref _owner);

            return result;
        }
    }
}
