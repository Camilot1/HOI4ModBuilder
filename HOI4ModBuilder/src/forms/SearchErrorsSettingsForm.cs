using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms
{
    public partial class SearchErrorsSettingsForm : Form
    {
        public static SearchErrorsSettingsForm instance;
        public static bool isLoading = false;

        public SearchErrorsSettingsForm()
        {
            InitializeComponent();
            instance?.Invoke((MethodInvoker)delegate { instance?.Close(); });
            instance = this;
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
            var settings = SettingsManager.settings;

            checkedListBox1.Items.Clear();
            if (settings == null || settings.searchErrorsSettings == null) return;

            var enums = Enum.GetValues(typeof(EnumMapErrorCode));
            Dictionary<string, int> indexes = new Dictionary<string, int>(enums.Length);

            int i = 0;
            foreach (var enumObj in enums)
            {
                var name = enumObj.ToString();
                indexes[name] = i;
                checkedListBox1.Items.Add(name);
                i++;
            }

            foreach (string name in settings.searchErrorsSettings.errors)
            {
                if (indexes.TryGetValue(name, out int index))
                {
                    checkedListBox1.SetItemChecked(index, true);
                }
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            WriteValuesToSettings();
        }

        private void WriteValuesToSettings()
        {
            var data = SettingsManager.settings.searchErrorsSettings.errors;

            data.Clear();

            foreach (var enumObj in Enum.GetValues(typeof(EnumMapErrorCode)))
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
        {
            Logger.TryOrLog(() => SettingsManager.SaveSettings());
        }
    }
}
