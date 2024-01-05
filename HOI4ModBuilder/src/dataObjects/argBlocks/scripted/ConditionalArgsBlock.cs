using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.dataObjects.argBlocks.scripted
{
    class ConditionalArgsBlock : DataArgsBlock
    {
        public ConditionalArgsBlock prev, next;
        public LimitArgsBlock limit;

        public override void TokenCallback(ParadoxParser parser, string token)
        {

        }
    }

    class LimitArgsBlock : DataArgsBlock
    {
        protected new InfoArgsBlock _infoArgsBlock = InfoArgsBlocksManager.limitInfoArgsBlock;

        public override void TokenCallback(ParadoxParser parser, string token)
        {
            throw new NotImplementedException();
        }
    }
}
