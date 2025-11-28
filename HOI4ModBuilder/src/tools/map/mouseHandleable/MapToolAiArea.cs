using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;
using HOI4ModBuilder.src.utils.structs;
using System;
using System.Collections;

namespace HOI4ModBuilder.src.tools.map.mouseHandleable
{
    class MapToolAiArea : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.AI_AREAS;

        public MapToolAiArea(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      shift = true,
                      key = Keys.A,
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
              )
        { }

        public override bool isHandlingMouseMove() => true;
        public override EnumEditLayer[] GetAllowedEditLayers() => new[] {
            EnumEditLayer.CONTINENTS, EnumEditLayer.STRATEGIC_REGIONS
        };
        public override Func<ICollection> GetParametersProvider()
            => () => AiAreaManager.GetAiAreasNames();
        public override Func<ICollection> GetParameterValuesProvider() => null;

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            if (!AiAreaManager.TryGetAiArea(parameter, out AiArea aiArea))
                return false;
            if (!ProvinceManager.TryGet(MapManager.GetColor(pos), out Province province))
                return false;

            int i = (int)pos.x + (int)pos.y * MapManager.MapSize.x;

            void AddContinent()
            {
                aiArea.AddContinentId(province.ContinentId);
                MapManager.HandleMapMainLayerChange(false);
            }
            ;
            void RemoveContinent()
            {
                aiArea.RemoveContinentId(province.ContinentId);
                MapManager.HandleMapMainLayerChange(false);
            }
            ;

            void AddRegion()
            {
                aiArea.AddRegion(province.Region);
                MapManager.HandleMapMainLayerChange(false);
            }
            void RemoveRegion()
            {
                aiArea.RemoveRegion(province.Region);
                MapManager.HandleMapMainLayerChange(false);
            }

            switch (enumEditLayer)
            {
                case EnumEditLayer.CONTINENTS:
                    if (mouseEventArgs.Button == MouseButtons.Left && !aiArea.HasContinentId(province.ContinentId))
                        MapManager.ActionsBatch.AddWithExecute(() => AddContinent(), () => RemoveContinent());
                    else if (mouseEventArgs.Button == MouseButtons.Right && aiArea.HasContinentId(province.ContinentId))
                        MapManager.ActionsBatch.AddWithExecute(() => RemoveContinent(), () => AddContinent());
                    break;
                case EnumEditLayer.STRATEGIC_REGIONS:
                    if (province.Region == null)
                        return false;

                    if (mouseEventArgs.Button == MouseButtons.Left && !aiArea.HasRegion(province.Region))
                        MapManager.ActionsBatch.AddWithExecute(() => AddRegion(), () => RemoveRegion());
                    else if (mouseEventArgs.Button == MouseButtons.Right && aiArea.HasRegion(province.Region))
                        MapManager.ActionsBatch.AddWithExecute(() => RemoveRegion(), () => AddRegion());
                    break;
            }

            return true;
        }
    }
}

