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
using System.Collections;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolTerrain : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.TERRAIN;

        public MapToolTerrain(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey { shift = true, key = Keys.T },
                  (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool),
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
              )
        { }

        public override EnumEditLayer[] GetAllowedEditLayers() => new[] {
            EnumEditLayer.PROVINCES, EnumEditLayer.STRATEGIC_REGIONS
        };
        public override Func<ICollection> GetParametersProvider()
            => () => TerrainManager.GetAllTerrainKeys;
        public override Func<ICollection> GetValuesProvider() => null;

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return false;

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
                                MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, null);
                        };
                    }
                    else if (enumEditLayer == EnumEditLayer.STRATEGIC_REGIONS)
                    {
                        action = (t) =>
                        {
                            bool newToHandleMapChange = prevTerrain == null || !province.Region.Terrain.Equals(t);
                            province.Region.UpdateTerrain(t);
                            if (newToHandleMapChange)
                                MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, null);
                        };
                    }
                    else return false;

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
                    return false;

                string newTerrain = newTerrainObj.name;

                if (prevTerrain == newTerrain)
                    return false;

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

            return true;
        }
    }
}
