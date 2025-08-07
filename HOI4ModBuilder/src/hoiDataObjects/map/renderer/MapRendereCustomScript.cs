using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.scripts.objects.interfaces;
using HOI4ModBuilder.src.scripts.objects;
using HOI4ModBuilder.src.scripts;
using System;
using HOI4ModBuilder.managers;

namespace HOI4ModBuilder.src.hoiDataObjects.map.renderer
{
    internal class MapRendereCustomScript : IMapRenderer
    {
        public MapRendererResult Execute(bool recalculateAllText, ref Func<Province, int> func, ref Func<Province, int, int> customFunc, string parameter)
        {
            if (ScriptParser.MapMainLayerCustomScriptName == null)
            {
                func = null;
                return MapRendererResult.ABORT;
            }

            if (recalculateAllText)
                if (!TextRenderRecalculate())
                    return MapRendererResult.ABORT;

            ScriptParser.IsDebug = false;

            customFunc = (p, idx) =>
            {
                if (idx >= ScriptParser.MapMainLayerCustomScriptTasks)
                {
                    return Utils.ArgbToInt(255, 0, 0, 0);
                }

                var action = ScriptParser.MapMainLayerCustomScriptActions[idx];
                var varsScope = ScriptParser.MapMainLayerCustomScriptMainVarsScopes[idx];

                varsScope.PutLocalVariable("province_id", new IntObject(p.Id));
                varsScope.PutLocalVariable("red", new IntObject());
                varsScope.PutLocalVariable("green", new IntObject());
                varsScope.PutLocalVariable("blue", new IntObject());
                action();
                byte r = 0, g = 0, b = 0;

                if (varsScope.TryGetLocalValue("red", out var variable) && variable is INumberObject redObj)
                    r = Convert.ToByte(redObj.GetValue());
                if (varsScope.TryGetLocalValue("green", out variable) && variable is INumberObject greenObj)
                    g = Convert.ToByte(greenObj.GetValue());
                if (varsScope.TryGetLocalValue("blue", out variable) && variable is INumberObject blueObj)
                    b = Convert.ToByte(blueObj.GetValue());

                varsScope.ClearLocalVars();

                return Utils.ArgbToInt(255, r, g, b);
            };

            return MapRendererResult.CONTINUE;
        }

        public bool TextRenderRecalculate()
        {
            MapManager.FontRenderController.TryStart(out var result)?
                .ClearAll()
                .End();

            return result;
        }
    }
}
