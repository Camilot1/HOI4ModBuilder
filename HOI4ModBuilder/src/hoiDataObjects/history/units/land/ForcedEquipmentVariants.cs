using HOI4ModBuilder.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.hoiDataObjects.history.countries;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.land
{

    class ForcedEquipmentVariants : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "force_equipment_variants";

        private bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;

                foreach (var variant in _variants)
                    if (variant.NeedToSave) return true;

                return false;
            }
        }

        public bool HasAnyInnerInfo()
        {
            foreach (var variant in _variants)
                if (variant.HasAnyInnerInfo) return true;
            return false;
        }

        private List<ForcedEquipmentVariant> _variants = new List<ForcedEquipmentVariant>();
        public List<ForcedEquipmentVariant> Variants { get => _variants; }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo()) return false;

            if (_variants.Count == 1)
            {
                ParadoxUtils.StartInlineBlock(sb, outTab, BLOCK_NAME);
                _variants[0].Save(sb, " ", " ");
                ParadoxUtils.EndBlock(sb, outTab);
            }
            else
            {
                string newOutTab = outTab + tab;
                ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

                foreach (var variant in _variants)
                {
                    variant.Save(sb, newOutTab, " ");
                    sb.Append(Constants.NEW_LINE);
                }

                ParadoxUtils.EndBlock(sb, outTab);
            }

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                //TODO Implement Archetype use
                Logger.ParseLayeredListedValue(prevLayer, token, ref _variants, parser, new ForcedEquipmentVariant(token));
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    class ForcedEquipmentVariant : IParadoxObject
    {
        private bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _owner != null && _owner.HasChangedTag ||
                _creator != null && _creator.HasChangedTag;
        }

        public bool HasAnyInnerInfo => _archetype != null || _owner != null || _creator != null || _amount != null || _versionName != null;

        private string _archetype; //TODO Implement archetypes
        public string Archetype { get => _archetype; set => Utils.Setter(ref _archetype, ref value, ref _needToSave); }

        private static readonly string TOKEN_OWNER = "owner";
        private Country _owner;
        public Country Owner { get => _owner; set => Utils.Setter(ref _owner, ref value, ref _needToSave); }

        private static readonly string TOKEN_CREATOR = "creator";
        private Country _creator;
        public Country Creator { get => _creator; set => Utils.Setter(ref _creator, ref value, ref _needToSave); }

        private static readonly string TOKEN_AMOUNT = "amount";
        private uint? _amount;
        public uint? Amount { get => _amount; set => Utils.Setter(ref _amount, ref value, ref _needToSave); }

        private static readonly string TOKEN_VERSION_NAME = "version_name";
        private string _versionName;
        public string VersionName { get => _versionName; set => Utils.Setter(ref _versionName, ref value, ref _needToSave); }

        public ForcedEquipmentVariant(string archetype)
        {
            _archetype = archetype;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            ParadoxUtils.StartInlineBlock(sb, outTab, _archetype);

            ParadoxUtils.SaveQuotedInline(sb, tab, TOKEN_OWNER, _owner?.Tag);
            ParadoxUtils.SaveQuotedInline(sb, tab, TOKEN_CREATOR, _creator?.Tag);
            ParadoxUtils.SaveInline(sb, tab, TOKEN_AMOUNT, _amount);
            ParadoxUtils.SaveQuotedInline(sb, tab, TOKEN_VERSION_NAME, _versionName);

            ParadoxUtils.EndInlineBlock(sb, tab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(_archetype, () =>
            {
                if (token == TOKEN_OWNER)
                {
                    var value = parser.ReadString();
                    if (CountryManager.TryGetCountry(value, out Country newOwner))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _owner, newOwner);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.COUNTRY_NOT_FOUND,
                            new Dictionary<string, string> { { "{countryTag}", value } }
                        );
                }
                else if (token == TOKEN_CREATOR)
                {
                    var value = parser.ReadString();
                    if (CountryManager.TryGetCountry(value, out Country newCreator))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _creator, newCreator);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.COUNTRY_NOT_FOUND,
                            new Dictionary<string, string> { { "{countryTag}", value } }
                        );
                }
                else if (token == TOKEN_AMOUNT)
                {
                    var value = parser.ReadString();
                    if (uint.TryParse(value, out uint newAmount))
                        Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _amount, newAmount);
                    else
                        Logger.LogLayeredError(
                            prevLayer, token, EnumLocKey.INCORRECT_VALUE,
                            new Dictionary<string, string> { { "{value}", value } }
                        );
                }
                else if (token == TOKEN_VERSION_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _versionName, parser.ReadString());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true; //TODO implement later
    }
}
