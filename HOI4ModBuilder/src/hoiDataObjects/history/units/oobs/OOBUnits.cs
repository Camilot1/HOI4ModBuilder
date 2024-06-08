using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOBUnits : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "units";

        public bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;

                foreach (var division in _divisions)
                    if (division.NeedToSave) return true;
                foreach (var fleet in _fleets)
                    if (fleet.NeedToSave) return true;

                return false;
            }
        }

        public bool HasAnyInnerInfo => _divisions.Count != 0 || _fleets.Count != 0;

        private List<Division> _divisions = new List<Division>();
        public List<Division> Divisions { get; }

        private List<Fleet> _fleets = new List<Fleet>();
        public List<Fleet> Fleets { get; }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            ParadoxUtils.StartBlock(sb, outTab, BLOCK_NAME);

            string newOutTab = outTab + tab;

            foreach (var division in _divisions) division.Save(sb, newOutTab, tab);
            foreach (var fleet in _fleets) fleet.Save(sb, newOutTab, tab);

            ParadoxUtils.EndBlock(sb, outTab);

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
}
