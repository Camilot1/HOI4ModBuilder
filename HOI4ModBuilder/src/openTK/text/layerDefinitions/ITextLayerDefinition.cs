using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.openTK.text.layerDefinitions
{
    public interface ITextLayerDefinition<in TContext>
    {
        bool Rebuild(FontRenderController controller, TContext context);
    }
}
