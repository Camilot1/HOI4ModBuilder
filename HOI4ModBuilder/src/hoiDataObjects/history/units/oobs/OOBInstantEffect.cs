using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOBInstantEffect : IParadoxObject
    {
        public static readonly string BLOCK_NAME = "instant_effect";

        public bool HasAnyInnerInfo => _effects != null && _effects.Count > 0;

        private List<DataArgsBlock> _effects;
        public List<DataArgsBlock> Effects { get => _effects; }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                DataArgsBlocksManager.ParseDataArgsBlock(parser, null, token, null, _effects);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
