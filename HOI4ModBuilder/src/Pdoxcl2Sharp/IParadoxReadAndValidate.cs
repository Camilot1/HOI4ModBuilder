using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.Pdoxcl2Sharp
{
    public interface IParadoxReadAndValidate
    {
        void TokenCallback(ParadoxParser parser, string token);
        void Validate();
    }
}
