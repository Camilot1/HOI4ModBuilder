using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.Pdoxcl2Sharp
{
    public interface IParadoxObject
    {
        bool Save(StringBuilder sb, string outTab, string tab);
        void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token);
        bool Validate(LinkedLayer prevLayer);
    }
}
