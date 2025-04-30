using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using HOI4ModBuilder.src.utils;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.tools.auto
{
    class AutoTools
    {
        private static readonly int[] _changesCounter = new int[1];
        private static void PostAction()
        {
            MapManager.HandleMapMainLayerChange(MainForm.Instance.enumMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);

            MessageBox.Show(
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_MESSAGE_BOX_TEXT,
                    new Dictionary<string, string> { { "{count}", $"{_changesCounter[0]}" } }
                ),
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_MESSAGE_BOX_TITLE
                ),
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification
            );
            _changesCounter[0] = 0;
        }

        public static void FixProvincesCoastalType()
        {
            ProvinceManager.ForEachProvince((p) =>
            {
                var newIsCoastal = p.CheckCoastalType();
                if (p.IsCoastal != newIsCoastal)
                {
                    _changesCounter[0]++;
                    p.IsCoastal = newIsCoastal;
                }
            });
            PostAction();
        }

        public static void RemoveSeaAndLakesContinents()
        {
            ProvinceManager.ForEachProvince((p) =>
            {
                if (p.Type != EnumProvinceType.LAND && p.ContinentId != 0)
                {
                    _changesCounter[0]++;
                    p.ContinentId = 0;
                }
            });
            PostAction();
        }

        public static void RemoveSeaProvincesFromStates()
        {
            ProvinceManager.ForEachProvince((p) =>
            {
                if (p.Type == EnumProvinceType.SEA && p.State != null)
                {
                    _changesCounter[0]++;
                    p.State.RemoveProvince(p);
                }
            });
            PostAction();
        }

        public static void ValidateAllStates()
        {
            StateManager.ForEachState((s) =>
            {
                s.Validate(out bool hasChanged);
                if (hasChanged) _changesCounter[0]++;
            });
        }

        public static void ValidateAllRegions()
        {
            StrategicRegionManager.ForEachRegion((r) =>
            {
                r.Validate(out bool hasChanged);
                if (hasChanged) _changesCounter[0]++;
            });
        }
    }
}
