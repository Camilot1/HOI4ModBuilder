using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolStateCategory : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.STATE_CATEGORY;

        public MapToolStateCategory(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new HotKey { },
                  (e) => MainForm.Instance.SetSelectedTool(enumTool)
              )
        { }

        public override void Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (enumEditLayer != EnumEditLayer.STATES)
                return;
            if (!pos.InboundsPositiveBox(MapManager.MapSize))
                return;

            StateCategoryManager.TryGetStateCategory(parameter, out StateCategory newCategory);

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return;

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                if (province.State == null) return;
                StateCategory prevCategory = province.State.StateCategory.GetValue();

                //TODO Добавить поддержку корректного изменения категории с учётом текущей букмарки

                if (prevCategory == newCategory) return;

                Action<State, StateCategory> action = (state, stateCategory) =>
                {
                    state.StateCategory.SetValue(stateCategory);
                    MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, null);
                };

                MapManager.ActionsBatch.AddWithExecute(
                    () => action(province.State, newCategory),
                    () => action(province.State, prevCategory)
                );
            }
            else if (mouseEventArgs.Button == MouseButtons.Right)
            {
                string newParameter = "";
                if (province.State != null && province.State.StateCategory.GetValue() != null)
                    newParameter = province.State.StateCategory.GetValue().name;

                MainForm.Instance.ComboBox_Tool_Parameter.Text = newParameter;
                MainForm.Instance.ComboBox_Tool_Parameter.Refresh();
            }
        }
    }
}
