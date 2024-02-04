using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.dataObjects
{
    class XYUshortCoordinates : IParadoxObject
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        private static readonly string TOKEN_X = "x";
        private ushort? _x;
        public ushort? X { get => _x; set => Utils.Setter(ref _x, ref value, ref _needToSave); }

        private static readonly string TOKEN_Y = "y";
        private ushort? _y;
        public ushort? Y { get => _y; set => Utils.Setter(ref _y, ref value, ref _needToSave); }

        public override bool Equals(object obj)
        {
            return obj is XYUshortCoordinates coordinates &&
                   X == coordinates.X &&
                   Y == coordinates.Y;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            ParadoxUtils.SaveInline(sb, outTab, TOKEN_X, _x);
            ParadoxUtils.SaveInline(sb, outTab, TOKEN_Y, _y);
            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            token = token.ToLower();

            if (token == TOKEN_X)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _x, parser.ReadUInt16());
            else if (token == TOKEN_Y)
                Logger.CheckLayeredValueOverrideAndSet(prevLayer, token, ref _y, parser.ReadUInt16());
            else
                throw new UnknownTokenException(token);
        }

        public bool Validate(LinkedLayer prevLayer)
        {
            bool result = true;

            if (_x == null)
            {
                Logger.LogLayeredError(
                    prevLayer, EnumLocKey.BLOCK_DOESNT_HAVE_MANDATORY_INNER_PARAMETER,
                    new Dictionary<string, string> { { "{parameterName}", TOKEN_X } }
                );
                result = false;
            }

            if (_y == null)
            {
                Logger.LogLayeredError(
                    prevLayer, EnumLocKey.BLOCK_DOESNT_HAVE_MANDATORY_INNER_PARAMETER,
                    new Dictionary<string, string> { { "{parameterName}", TOKEN_Y } }
                );
                result = false;
            }

            return result;
        }
    }
}
