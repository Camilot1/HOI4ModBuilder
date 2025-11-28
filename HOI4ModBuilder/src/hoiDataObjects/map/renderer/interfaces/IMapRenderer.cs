using HOI4ModBuilder.hoiDataObjects.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    public enum MapRendererResult
    {
        CONTINUE,
        ABORT
    }

    public interface IMapRenderer
    {
        MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter, string parameterValue);
        bool TextRenderRecalculate(string parameter, string value);
    }
}
