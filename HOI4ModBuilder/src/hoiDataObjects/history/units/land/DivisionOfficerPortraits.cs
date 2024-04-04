using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.land
{
    class DivisionOfficerPortraits : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "portraits";

        private bool _needToSave;
        public bool NeedToSave
        {
            get => _needToSave ||
                _armyGroup != null && _armyGroup.NeedToSave;
        }
        public bool HasAnyInnerInfo =>
            _armyGroup != null && _armyGroup.HasAnyInnerInfo;

        private DivisionOfficerPortraitsGroup _armyGroup;
        public DivisionOfficerPortraitsGroup ArmyGroup { get => _armyGroup; set => Utils.Setter(ref _armyGroup, ref value, ref _needToSave); }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            string newOutTab = outTab + tab;
            _armyGroup?.Save(sb, newOutTab, tab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == "army")
                    Logger.ParseLayeredValueAndCheckOverride(prevLayer, token, ref _armyGroup, parser, new DivisionOfficerPortraitsGroup(token));
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
