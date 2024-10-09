using HOI4ModBuilder.src.managers.errors;
using HOI4ModBuilder.src.managers.warnings;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms
{
    public enum EnumSearchSettingsType
    {
        WARNINGS,
        ERRORS
    }

    public partial class SearchSettingsForm : Form
    {
        public static SearchSettingsForm instance;
        public static bool isLoading = false;
        private readonly EnumSearchSettingsType _type;

        public SearchSettingsForm(EnumSearchSettingsType type)
        {
            InitializeComponent();
            instance?.Invoke((MethodInvoker)delegate { instance?.Close(); });
            instance = this;
            _type = type;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LoadData();
            GuiLocManager.formsReinitEvents.Add(this, () =>
            {
                Invoke((MethodInvoker)delegate
                {
                    Controls.Clear();
                    InitializeComponent();
                    LoadData();
                    Focus();
                });
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            instance = null;
            GuiLocManager.formsReinitEvents.Remove(this);
        }

        private void LoadData()
        {
            checkedListBox1.Items.Clear();

            Type type;
            List<string> data;

            if (_type == EnumSearchSettingsType.WARNINGS)
            {
                type = typeof(EnumMapWarningCode);
                data = SettingsManager.Settings?.searchWarningsSettings?.enabled;
                Text = GuiLocManager.GetLoc(EnumLocKey.SEARCH_FORM_WARNINGS_SETTINGS_TITLE);
            }
            else if (_type == EnumSearchSettingsType.ERRORS)
            {
                type = typeof(EnumMapErrorCode);
                data = SettingsManager.Settings?.searchErrorsSettings?.enabled;
                Text = GuiLocManager.GetLoc(EnumLocKey.SEARCH_FORM_ERRORS_SETTINGS_TITLE);
            }
            else throw new Exception($"UNKNOWN TYPE: {_type}");

            if (data == null)
                return;

            var enums = Enum.GetValues(type);
            Dictionary<string, int> indexes = new Dictionary<string, int>(enums.Length);

            int i = 0;
            foreach (var enumObj in enums)
            {
                var name = enumObj.ToString();
                indexes[name] = i;
                checkedListBox1.Items.Add(name);
                i++;
            }

            foreach (string name in data)
            {
                if (indexes.TryGetValue(name, out int index))
                {
                    checkedListBox1.SetItemChecked(index, true);
                }
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
            => WriteValuesToSettings();

        private void WriteValuesToSettings()
        {
            Type type;
            List<string> data;

            if (_type == EnumSearchSettingsType.WARNINGS)
            {
                type = typeof(EnumMapWarningCode);
                data = SettingsManager.Settings.searchWarningsSettings.enabled;
            }
            else if (_type == EnumSearchSettingsType.ERRORS)
            {
                type = typeof(EnumMapErrorCode);
                data = SettingsManager.Settings.searchErrorsSettings.enabled;
            }
            else throw new Exception($"UNKNOWN TYPE: {_type}");

            data.Clear();

            foreach (var enumObj in Enum.GetValues(type))
            {
                string name = enumObj.ToString();
                if (checkedListBox1.CheckedItems.Contains(name))
                {
                    data.Add(name);
                }
            }
        }

        private void Button_EnableAll_Click(object sender, EventArgs e)
        {
            int errorsCount = checkedListBox1.Items.Count;
            for (int i = 0; i < errorsCount; i++) checkedListBox1.SetItemChecked(i, true);
            WriteValuesToSettings();
        }

        private void Button_DisableAll_Click(object sender, EventArgs e)
        {
            int errorsCount = checkedListBox1.Items.Count;
            for (int i = 0; i < errorsCount; i++) checkedListBox1.SetItemChecked(i, false);
            WriteValuesToSettings();
        }

        private void Button_Save_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => SettingsManager.SaveSettings());
    }
}
