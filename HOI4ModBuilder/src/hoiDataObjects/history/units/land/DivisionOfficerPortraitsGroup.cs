using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.land
{

    class DivisionOfficerPortraitsGroup : IParadoxObject
    {
        public bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        public bool HasAnyInnerInfo =>
            _largePortrait != null ||
            _smallPortrait != null;

        private string _categoryName;
        public string CategoryName { get => _categoryName; set => Utils.Setter(ref _categoryName, ref value, ref _needToSave); }

        private static readonly string TOKEN_LARGE_PORTRAIT = "large";
        private string _largePortrait;
        public string LargePortrait { get => _largePortrait; set => Utils.Setter(ref _largePortrait, ref value, ref _needToSave); }

        private static readonly string TOKEN_SMALL_PORTRAIT = "small";
        private string _smallPortrait;
        public string SmallPortrait { get => _smallPortrait; set => Utils.Setter(ref _smallPortrait, ref value, ref _needToSave); }

        public DivisionOfficerPortraitsGroup(string categoryName)
        {
            _categoryName = categoryName;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            string newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, _categoryName);

            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_LARGE_PORTRAIT, _largePortrait);
            ParadoxUtils.SaveQuoted(sb, newOutTab, TOKEN_SMALL_PORTRAIT, _smallPortrait);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(_categoryName, () =>
            {
                if (token == TOKEN_LARGE_PORTRAIT)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _largePortrait, parser.ReadString());
                else if (token == TOKEN_SMALL_PORTRAIT)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _smallPortrait, parser.ReadString());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
