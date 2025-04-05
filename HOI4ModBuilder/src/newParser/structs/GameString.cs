using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.newParser.structs
{
    public struct GameString
    {
        public string value;

        public override string ToString()
        {
            return "GameString(" + value + ")";
        }
    }
}
