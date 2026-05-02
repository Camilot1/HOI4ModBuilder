using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.openTK.text.layerDefinitions
{
    public sealed class NoTextLayerDefinition<TContext> : ITextLayerDefinition<TContext>
    {
        public bool Rebuild(FontRenderController controller, TContext context)
            => controller.ClearTextLayer();
    }
}
