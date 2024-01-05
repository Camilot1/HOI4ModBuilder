using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using HOI4ModBuilder.src.managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolStateCategory : IMouseHandleableMapTool
    {
        private static readonly EnumTool enumTool = EnumTool.STATE_CATEGORY;

        public MapToolStateCategory(Dictionary<EnumTool, IMouseHandleableMapTool> mapTools)
        {
            mapTools[enumTool] = this;
        }

        public void Handle(MouseButtons buttons, EnumMouseState mouseState, Point2D pos, EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter)
        {
            if (enumEditLayer != EnumEditLayer.STATES) return;
            if (!pos.InboundsPositiveBox(MapManager.MapSize)) return;

            StateCategoryManager.TryGetStateCategory(parameter, out StateCategory newCategory);

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province)) return;

            if (buttons == MouseButtons.Left)
            {
                if (province.state == null) return;
                StateCategory prevCategory = province.state.startStateCategory;

                //TODO Добавить поддержку корректного изменения категории с учётом текущей букмарки

                if (prevCategory == newCategory) return;

                Action<State, StateCategory> action = (state, stateCategory) =>
                {
                    state.startStateCategory = stateCategory;
                    MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                };

                action(province.state, newCategory);
                MapManager.actionPairs.Add(new ActionPair(() => action(province.state, prevCategory), () => action(province.state, newCategory)));
            }
            else if (buttons == MouseButtons.Right)
            {
                string newParameter = "";
                if (province.state != null && province.state.startStateCategory != null) newParameter = province.state.startStateCategory.name;

                MainForm.Instance.ComboBox_Tool_Parameter.Text = newParameter;
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }
        }
    }
}
