using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.tools.map.mouseHandleable
{
    class MapToolAiArea : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.AI_AREAS;

        public MapToolAiArea(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { shift = true, key = Keys.A },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!pos.InboundsPositiveBox(MapManager.MapSize, sizeFactor))
                return;
            if (!AiAreaManager.TryGetAiArea(parameter, out AiArea aiArea))
                return;
            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return;

            int i = (int)pos.x + (int)pos.y * MapManager.MapSize.x;

            void AddContinent()
            {
                aiArea.AddContinentId(province.ContinentId);
                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
            };
            void RemoveContinent()
            {
                aiArea.RemoveContinentId(province.ContinentId);
                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
            };

            void AddRegion()
            {
                aiArea.AddRegion(province.Region);
                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
            }
            void RemoveRegion()
            {
                aiArea.RemoveRegion(province.Region);
                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
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
                        return;

                    if (mouseEventArgs.Button == MouseButtons.Left && !aiArea.HasRegion(province.Region))
                        MapManager.ActionsBatch.AddWithExecute(() => AddRegion(), () => RemoveRegion());
                    else if (mouseEventArgs.Button == MouseButtons.Right && aiArea.HasRegion(province.Region))
                        MapManager.ActionsBatch.AddWithExecute(() => RemoveRegion(), () => AddRegion());
                    break;
            }
        }
    }
}

