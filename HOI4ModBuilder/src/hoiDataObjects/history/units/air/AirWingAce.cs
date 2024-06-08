using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils.exceptions;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.air
{

    class AirWingAce : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "ace";

        public bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        public bool HasAnyInnerInfo => _modifier != null || _name != null || _surname != null || _callsign != null || _isFemale != null;

        private static readonly string TOKEN_MODIFIER = "modifier";
        private string _modifier;
        public string Modifier { get => _modifier; set => Utils.Setter(ref _modifier, ref value, ref _needToSave); }

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private static readonly string TOKEN_SURNAME = "surname";
        private string _surname;
        public string Surname { get => _surname; set => Utils.Setter(ref _surname, ref value, ref _needToSave); }

        private static readonly string TOKEN_CALLSIGN = "callsign";
        private string _callsign;
        public string Callsign { get => _callsign; set => Utils.Setter(ref _callsign, ref value, ref _needToSave); }

        private static readonly string TOKEN_IS_FEMALE = "is_female";
        private bool? _isFemale;
        public bool? IsFemale { get => _isFemale; set => Utils.Setter(ref _isFemale, ref value, ref _needToSave); }

        private static readonly string TOKEN_PORTRAIT = "portrait";
        private string _portrait;
        public string Portrait { get => _portrait; set => Utils.Setter(ref _portrait, ref value, ref _needToSave); }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            var newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_MODIFIER, _modifier);
            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_NAME, _name);
            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_SURNAME, _surname);
            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_CALLSIGN, _callsign);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_IS_FEMALE, _isFemale);
            ParadoxUtils.Save(sb, newOutTab, TOKEN_PORTRAIT, _portrait);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            if (token == TOKEN_MODIFIER)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _modifier, parser.ReadString());
            else if (token == TOKEN_NAME)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
            else if (token == TOKEN_SURNAME)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _surname, parser.ReadString());
            else if (token == TOKEN_CALLSIGN)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _callsign, parser.ReadString());
            else if (token == TOKEN_IS_FEMALE)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _isFemale, parser.ReadBool());
            else if (token == TOKEN_PORTRAIT)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _portrait, parser.ReadString());
            else throw new UnknownTokenException(token);
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            CheckAndLogUnit.WARNINGS
                .HasMandatory(ref result, prevLayer, TOKEN_MODIFIER, ref _modifier)
                .HasMandatory(ref result, prevLayer, TOKEN_NAME, ref _name)
                .HasMandatory(ref result, prevLayer, TOKEN_SURNAME, ref _surname)
                .HasMandatory(ref result, prevLayer, TOKEN_CALLSIGN, ref _callsign);

            return result;
        }
    }


}
