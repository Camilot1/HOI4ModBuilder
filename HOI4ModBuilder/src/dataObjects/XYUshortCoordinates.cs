using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.dataObjects
{
    class XYUshortCoordinates : IParadoxRead
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }

        public override bool Equals(object obj)
        {
            return obj is XYUshortCoordinates coordinates &&
                   X == coordinates.X &&
                   Y == coordinates.Y;
        }

        public override int GetHashCode() => X << 16 | Y;

        public void TokenCallback(ParadoxParser parser, string token)
        {
            token = token.ToLower();
            if (token == "x") X = parser.ReadUInt16();
            else if (token == "y") Y = parser.ReadUInt16();
            else throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.EXCEPTION_COORDINATES_UNKNOWN_TOKEN,
                    new Dictionary<string, string> { { "{token}", token } }
                ));
        }

        public void Save(StringBuilder sb)
        {
            sb.Append("{ x = ").Append(X).Append(" y = ").Append(Y).Append(" }");
        }
    }
}
