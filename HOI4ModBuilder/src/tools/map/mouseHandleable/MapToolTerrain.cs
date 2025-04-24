using HOI4ModBuilder.hoiDataObjects.common.terrain;
using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.managers;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolTerrain : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.TERRAIN;

        public MapToolTerrain(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { key = Keys.T },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!(enumEditLayer == EnumEditLayer.PROVINCES || enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS))
                return;
            if (!pos.InboundsPositiveBox(MapManager.MapSize))
                return;

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return;

            if (mouseEventArgs.Button == MouseButtons.Left && parameter != null)
            {
                TerrainManager.TryGetProvincialTerrain(parameter, out ProvincialTerrain newProvincialTerrain);
                if (newProvincialTerrain != null)
                {
                    ProvincialTerrain prevTerrain = null;

                    if (enumEditLayer == EnumEditLayer.PROVINCES)
                        prevTerrain = province.Terrain;
                    else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS && province.Region != null)
                        prevTerrain = province.Region.Terrain;

                    Action<ProvincialTerrain> action;

                    if (enumEditLayer == EnumEditLayer.PROVINCES)
                    {
                        action = (t) =>
                        {
                            bool newToHandleMapChange = prevTerrain == null || !province.Terrain.Equals(t);
                            province.Terrain = t;
                            if (newToHandleMapChange)
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                        };
                    }
                    else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS)
                    {
                        action = (t) =>
                        {
                            bool newToHandleMapChange = prevTerrain == null || !province.Region.Terrain.Equals(t);
                            province.Region.UpdateTerrain(t);
                            if (newToHandleMapChange)
                                MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                        };
                    }
                    else return;

                    MapManager.ActionsBatch.AddWithExecute(
                        () => action(newProvincialTerrain),
                        () => action(prevTerrain)
                    );
                }
            }
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                string prevTerrain = MainForm.Instance.ComboBox_Tool_Parameter.Text;
                ProvincialTerrain newTerrainObj = null;

                if (enumEditLayer == EnumEditLayer.PROVINCES)
                    newTerrainObj = province.Terrain;
                else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS && province.Region != null)
                    newTerrainObj = province.Region.Terrain;

                if (newTerrainObj == null)
                    return;

                string newTerrain = newTerrainObj.name;

                if (prevTerrain == newTerrain)
                    return;

                void action(string t)
                {
                    MainForm.Instance.ComboBox_Tool_Parameter.Text = t;
                    MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
                }

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(newTerrain),
                    () => action(prevTerrain)
                );
            }
        }
    }
}
