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
    class MapToolProvinceCoastal : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.PROVINCE_COASTAL;

        public MapToolProvinceCoastal(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (enumEditLayer != EnumEditLayer.PROVINCES) return;

            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;

            ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province);
            bool prevCoastal = province.IsCoastal;
            bool newCoastal = false, doAction = false;

            if (buttons == MouseButtons.Left && !prevCoastal)
            {
                newCoastal = true;
                doAction = true;
            }
            else if (buttons == MouseButtons.Right && prevCoastal)
            {
                newCoastal = false;
                doAction = true;
            }

            if (doAction)
            {
                Action<bool> action = (b) =>
                {
                    province.UpdateIsCoastal(b);
                    MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                };

                action(newCoastal);
                MapManager.actionPairs.Add(new ActionPair(() => action(prevCoastal), () => action(newCoastal)));
            }
        }
    }
}
