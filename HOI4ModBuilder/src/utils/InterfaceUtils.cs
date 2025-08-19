using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HOI4ModBuilder.utils.Enums;

namespace HOI4ModBuilder.src.utils
{
    public class InterfaceUtils
    {
        public static void ResizeComboBox(GroupBox groupBox, ComboBox comboBox)
        {
            int groupBoxTextWidth = TextRenderer.MeasureText(
                    groupBox.Text, groupBox.Font, Size.Empty, TextFormatFlags.SingleLine
                ).Width;
            groupBoxTextWidth += 20;

            int comboBoxTextWidth = 0;
            foreach (var item in comboBox.Items)
            {
                int tempComboBoxTextWidth = TextRenderer.MeasureText(
                    item.ToString(), comboBox.Font, Size.Empty, TextFormatFlags.SingleLine
                ).Width;

                if (tempComboBoxTextWidth > comboBoxTextWidth)
                    comboBoxTextWidth = tempComboBoxTextWidth;
            }

            comboBoxTextWidth += SystemInformation.VerticalScrollBarWidth + 15;

            if (comboBoxTextWidth < groupBoxTextWidth)
                comboBoxTextWidth = groupBoxTextWidth;

            if (comboBox.Width != comboBoxTextWidth)
            {
                comboBox.Width = comboBoxTextWidth;
                comboBox.Parent?.PerformLayout();
            }
        }
        public static void UpdateComboBoxValues(GroupBox groupBox, ComboBox comboBox, ICollection items)
        {
            if (items != null)
            {
                groupBox.Visible = true;
                comboBox.Items.Clear();

                var values = new string[items.Count];
                int index = 0;

                foreach (var itemObj in items)
                {
                    values[index] = itemObj.ToString();
                }

                comboBox.Items.AddRange(values);

                if (comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;

            }
            else groupBox.Visible = false;

            groupBox.Refresh();

            int itemHeight = comboBox.GetItemHeight(0);
            int visibleItems = Math.Min(comboBox.Items.Count, 30);
            int dropDownHeight = itemHeight * visibleItems + SystemInformation.BorderSize.Height * 2;
            comboBox.DropDownHeight = dropDownHeight;

            comboBox.Refresh();

            ResizeComboBox(groupBox, comboBox);
        }

        public static void UpdateComboBoxToolValues(GroupBox groupBox, ComboBox comboBox, ICollection items, string[] prefereds, EnumTool tool)
        {
            if (items != null)
            {
                groupBox.Visible = true;
                comboBox.Items.Clear();

                var values = new string[items.Count];

                var prefered = prefereds[(int)tool];

                int indexToSelect = -1;
                int index = 0;

                foreach (var itemObj in items)
                {
                    var item = itemObj.ToString();
                    values[index] = item;
                    if (prefered == item)
                        indexToSelect = index;
                    index++;
                }

                comboBox.Items.AddRange(values);

                if (indexToSelect >= 0)
                    comboBox.SelectedIndex = indexToSelect;
                else if (comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;
            }
            else groupBox.Visible = false;

            groupBox.Refresh();

            int itemHeight = comboBox.GetItemHeight(0);
            int visibleItems = Math.Min(comboBox.Items.Count, 30);
            int dropDownHeight = itemHeight * visibleItems + SystemInformation.BorderSize.Height * 2;
            comboBox.DropDownHeight = dropDownHeight;

            comboBox.Refresh();

            ResizeComboBox(groupBox, comboBox);
        }

        public static void ResizeButton(GroupBox groupBox, Button button, int maxWidth)
        {
            int groupBoxTextWidth = TextRenderer.MeasureText(groupBox.Text, groupBox.Font, Size.Empty, TextFormatFlags.SingleLine).Width;
            groupBoxTextWidth += 20;

            if (maxWidth > 0)
                button.Text = Utils.TruncateText(button.Text, button.Font, maxWidth);

            int buttonWidth = TextRenderer.MeasureText(button.Text, button.Font, Size.Empty, TextFormatFlags.SingleLine).Width;

            buttonWidth += SystemInformation.VerticalScrollBarWidth;

            if (buttonWidth < groupBoxTextWidth)
                buttonWidth = groupBoxTextWidth;

            if (button.Width != buttonWidth)
            {
                button.Width = buttonWidth;
                button.Parent?.PerformLayout();
            }
        }
    }
}
