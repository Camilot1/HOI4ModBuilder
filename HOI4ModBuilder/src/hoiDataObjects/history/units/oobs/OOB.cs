using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOB : IParadoxObject
    {
        public FileInfo FileInfo { get; set; }

        public void Save(StringBuilder sb, string outTab, string tab)
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
        private List<Division> _divisions = new List<Division>();
        public List<Division> Divisions { get; }

        private List<Fleet> _fleets = new List<Fleet>();
        public List<Fleet> Fleets { get; }

        public void Save(StringBuilder sb, string outTab, string tab)
        {

        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions("units", () =>
            {
                switch (token)
                {
                    case "division":
                        Logger.ParseLayeredListedValue(prevLayer, token, _divisions, parser, new Division());
                        break;
                    case "fleet":
                        Logger.ParseLayeredListedValue(prevLayer, token, _fleets, parser, new Fleet());
                        break;
                    default:
                        throw new UnknownTokenException(token);
                }
            });
        }

        public bool Validate(LinkedLayer linkedLayer) => true;
    }
}
