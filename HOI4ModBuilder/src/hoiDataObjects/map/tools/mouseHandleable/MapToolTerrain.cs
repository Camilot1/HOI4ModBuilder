using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolTerrain : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.TERRAIN;

        public MapToolTerrain(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (!(enumEditLayer == EnumEditLayer.PROVINCES || enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS)) return;
            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;

            ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province);
            if (buttons == MouseButtons.Left && parameter != null && province != null)
            {
                TerrainManager.TryGetProvincialTerrain(parameter, out ProvincialTerrain newProvincialTerrain);
                if (newProvincialTerrain != null)
                {
                    ProvincialTerrain prevTerrain = null;

                    if (enumEditLayer == EnumEditLayer.PROVINCES) prevTerrain = province.Terrain;
                    else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS && province.Region != null) prevTerrain = province.Region.Terrain;

                    Action<ProvincialTerrain> action;

                    if (enumEditLayer == EnumEditLayer.PROVINCES)
                    {
                        action = (t) =>
                        {
                            bool newToHandleMapChange = prevTerrain == null || !province.Terrain.Equals(t);
                            province.UpdateTerrain(t);
                            if (newToHandleMapChange) MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                        };
                    }
                    else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS)
                    {
                        action = (t) =>
                        {
                            bool newToHandleMapChange = prevTerrain == null || !province.Region.Terrain.Equals(t);
                            province.Region.UpdateTerrain(t);
                            if (newToHandleMapChange) MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                        };
                    }
                    else return;

                    action(newProvincialTerrain);
                    MapManager.actionPairs.Add(new ActionPair(() => action(prevTerrain), () => action(newProvincialTerrain)));
                }
            }
            else if (buttons == MouseButtons.Right && province != null)
            {
                string prevTerrain = MainForm.Instance.ComboBox_Tool_Parameter.Text;
                ProvincialTerrain newTerrainObj = null;

                if (enumEditLayer == EnumEditLayer.PROVINCES) newTerrainObj = province.Terrain;
                else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS && province.Region != null) newTerrainObj = province.Region.Terrain;

                if (newTerrainObj == null) return;

                string newTerrain = newTerrainObj.name;

                if (prevTerrain == newTerrain) return;

                Action<string> action = (t) =>
                {
                    MainForm.Instance.ComboBox_Tool_Parameter.Text = t;
                    MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
                };
                action(newTerrain);
                MapManager.actionPairs.Add(new ActionPair(() => action(prevTerrain), () => action(newTerrain)));
            }
        }
    }
}
