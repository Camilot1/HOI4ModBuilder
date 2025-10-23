using HOI4ModBuilder.managers;
using HOI4ModBuilder.src.utils;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.tools.autotools
{
    public abstract class AbstractAutoTool
    {
        protected static void PostAction(bool recalculateAllText, int count)
        {
            MapManager.HandleMapMainLayerChange(recalculateAllText, MainForm.Instance.SelectedMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);

            MessageBox.Show(
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_MESSAGE_BOX_TEXT,
                    new Dictionary<string, string> { { "{count}", $"{count}" } }
                ),
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_MESSAGE_BOX_TITLE
                ),
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification
            );
        }
        protected static void PostExtendedAction(bool recalculateAllTextint, int count, int success, string unsuccessInfo)
        {
            MapManager.HandleMapMainLayerChange(recalculateAllTextint, MainForm.Instance.SelectedMainLayer, MainForm.Instance.ComboBox_Tool_Parameter.Text);

            MessageBox.Show(
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_EXTENDED_MESSAGE_BOX_TEXT,
                    new Dictionary<string, string> {
                        { "{count}", $"{count}" },
                        { "{success}", $"{success}" },
                        { "{unsuccess}", $"{count - success}" },
                        { "{unsuccessInfo}", unsuccessInfo }
                    }
                ),
                GuiLocManager.GetLoc(
                    EnumLocKey.AUTOTOOL_RESULT_EXTENDED_MESSAGE_BOX_TITLE
                ),
                MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification
            );
        }
    }
}
