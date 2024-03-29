﻿using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.hoiDataObjects.common.resources
{
    class Resource : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public string tag;
        public uint iconFrame;
        public float factoryCostPerUnit;
        public float convoysPerUnit;

        public Resource(string tag)
        {
            this.tag = tag;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "icon_frame":
                    iconFrame = parser.ReadUInt32();
                    break;
                case "cic":
                    factoryCostPerUnit = parser.ReadFloat();
                    break;
                case "convoys":
                    convoysPerUnit = parser.ReadFloat();
                    break;
            }
        }
    }
}
