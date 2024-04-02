using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.hoiDataObjects.common.ai_areas;

namespace HOI4ModBuilder.src.tools.map.mouseHandleable
{
    class MapToolAiArea : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.AI_AREAS;

        public MapToolAiArea(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;
            if (!AiAreaManager.TryGetAiArea(parameter, out AiArea aiArea)) return;
            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province)) return;

            int i = (int)pos.x + (int)pos.y * MapManager.MapSize.x;

            switch (enumEditLayer)
            {
                case EnumEditLayer.CONTINENTS:
                    if (buttons == MouseButtons.Left)
                    {
                        if (aiArea.HasContinentId(province.ContinentId)) return;

                        MapManager.ActionsBatch.AddWithExecute(
                            () =>
                            {
                                aiArea.AddContinentId(province.ContinentId);
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
                            },
                            () =>
                            {
                                aiArea.RemoveContinentId(province.ContinentId);
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
                            }
                        );
                    }
                    else if (buttons == MouseButtons.Right)
                    {
                        if (!aiArea.HasContinentId(province.ContinentId)) return;

                        MapManager.ActionsBatch.AddWithExecute(
                            () =>
                            {
                                aiArea.RemoveContinentId(province.ContinentId);
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
                            },
                            () =>
                            {
                                aiArea.AddContinentId(province.ContinentId);
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
                            }
                        );
                    }
                    break;
                case EnumEditLayer.STRATEGIC_REGIONS:
                    if (province.Region == null) return;

                    if (buttons == MouseButtons.Left)
                    {
                        if (aiArea.HasRegion(province.Region)) return;

                        MapManager.ActionsBatch.AddWithExecute(
                            () =>
                            {
                                aiArea.AddRegion(province.Region);
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
                            },
                            () => {
                                aiArea.RemoveRegion(province.Region);
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
                            }
                        );
                    }
                    else if (buttons == MouseButtons.Right)
                    {
                        if (!aiArea.HasRegion(province.Region)) return;

                        MapManager.ActionsBatch.AddWithExecute(
                            () =>
                            {
                                aiArea.RemoveRegion(province.Region);
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
                            },
                            () =>
                            {
                                aiArea.AddRegion(province.Region);
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);
                            }
                        );
                    }
                    break;
            }
        }
    }
}

