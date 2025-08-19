using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.common.stateCategory;
using System;
using System.Collections.Generic;
using static HOI4ModBuilder.utils.Enums;
using static HOI4ModBuilder.utils.Structs;
using System.Windows.Forms;
using HOI4ModBuilder.src.utils.structs;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using System.Collections;

namespace HOI4ModBuilder.src.hoiDataObjects.map.tools
{
    class MapToolStateCategory : MapTool
    {
        private static readonly EnumTool enumTool = EnumTool.STATE_CATEGORY;

        public MapToolStateCategory(Dictionary<EnumTool, MapTool> mapTools)
            : base(
                  mapTools, enumTool, new EnumMainLayer[] { },
                  new HotKey
                  {
                      hotKeyEvent = (e) => MainForm.Instance.SetSelectedToolWithRefresh(enumTool)
                  },
                  (int)EnumMapToolHandleChecks.CHECK_INBOUNDS_MAP_BOX
              )
        { }

        public override EnumEditLayer[] GetAllowedEditLayers() => new[] {
            EnumEditLayer.STATES
        };
        public override Func<ICollection> GetParametersProvider()
            => () => StateCategoryManager.GetStateCategoriesNames();
        public override Func<ICollection> GetValuesProvider() => null;

        public override bool Handle(
            MouseEventArgs mouseEventArgs, EnumMouseState mouseState, Point2D pos, Point2D sizeFactor,
            EnumEditLayer enumEditLayer, Bounds4US bounds, string parameter, string value
        )
        {
            if (!base.Handle(mouseEventArgs, mouseState, pos, sizeFactor, enumEditLayer, bounds, parameter, value))
                return false;

            StateCategoryManager.TryGetStateCategory(parameter, out StateCategory newCategory);

            if (!ProvinceManager.TryGetProvince(MapManager.GetColor(pos), out Province province))
                return false;

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                if (province.State == null)
                    return false;
                StateCategory prevCategory = province.State.StateCategory.GetValue();

                //TODO Добавить поддержку корректного изменения категории с учётом текущей букмарки

                if (prevCategory == newCategory)
                    return false;

                Action<State, StateCategory> action = (state, stateCategory) =>
                {
                    state.StateCategory.SetValue(stateCategory);
                    MapManager.HandleMapMainLayerChange(false, MainForm.Instance.SelectedMainLayer, null);
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

            return true;
        }
    }
}
