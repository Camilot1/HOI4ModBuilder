using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.land
{
    class DivisionOfficer : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "officer";

        private bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _portraits != null && _portraits.NeedToSave;
        }
        public bool HasAnyInnerInfo =>
            _name != null ||
            _portraits != null && _portraits.HasAnyInnerInfo;

        private static readonly string TOKEN_NAME = "name";
        private string _name;
        public string Name { get => _name; set => Utils.Setter(ref _name, ref value, ref _needToSave); }

        private static readonly string TOKEN_PORTRAITS = "portraits";
        private DivisionOfficerPortraits _portraits;
        public DivisionOfficerPortraits Portraits { get => _portraits; set => Utils.Setter(ref _portraits, ref value, ref _needToSave); }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            string newOutTab = outTab + tab;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            ParadoxUtils.Save(sb, newOutTab, BLOCK_NAME, _name);
            _portraits?.Save(sb, newOutTab, tab);

            ParadoxUtils.EndBlock(sb, outTab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions($"{BLOCK_NAME} ({TOKEN_NAME} = \"{_name}\")", () =>
            {
                if (token == TOKEN_NAME)
                    Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _name, parser.ReadString());
                else if (token == TOKEN_PORTRAITS)
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _portraits, parser, new DivisionOfficerPortraits());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
