using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOB : IParadoxObject
    {
        public FileInfo FileInfo { get; set; }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            throw new NotImplementedException();
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            throw new NotImplementedException();
        }
    }

    class Units : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "units";

        public bool HasAnyInnerInfo => _divisions.Count != 0 || _fleets.Count != 0;

        private List<Division> _divisions = new List<Division>();
        public List<Division> Divisions { get; }

        private List<Fleet> _fleets = new List<Fleet>();
        public List<Fleet> Fleets { get; }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            string newOutTab = outTab + tab;

            foreach (var division in _divisions) division.Save(sb, newOutTab, tab);
            foreach (var fleet in _fleets) fleet.Save(sb, newOutTab, tab);

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                if (token == Division.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, _divisions, parser, new Division());
                else if (token == Fleet.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, _fleets, parser, new Fleet());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer linkedLayer) => true;
    }
}
