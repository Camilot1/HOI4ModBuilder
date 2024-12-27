using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.units.divisionTemplates;
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

        private bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave) return true;

                foreach (var effect in _effects)
                    if (effect.NeedToSave) return true;

                return false;
            }
        }

        public bool HasAnyInnerInfo => _effects.Count > 0;

        private List<DataArgsBlock> _effects = new List<DataArgsBlock>();
        public List<DataArgsBlock> Effects { get => _effects; }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(BLOCK_NAME, () =>
            {
                DataArgsBlocksManager.ParseDataArgsBlocks(parser, token, _effects);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
