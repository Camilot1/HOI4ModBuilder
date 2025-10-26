using HOI4ModBuilder.src.managers.statistics;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms.dataForms
{
    public partial class StatisticsForm : Form
    {
        public static StatisticsForm Instance { get; private set; }

        private string _filterStates = "";
        private string _filterRegions = "";
        private string _filterCountries = "";

        public StatisticsForm()
        {
            InitializeComponent();
            Instance?.Invoke((MethodInvoker)delegate { Instance?.Close(); });
            Instance = this;
        }

        public void InvokeAction(Action action)
            => Invoke((MethodInvoker)delegate { action(); });

        public void TryInvokeActionOrLog(Action tryAction, Action<Exception> catchAction)
            => Invoke((MethodInvoker)delegate { Logger.TryOrCatch(tryAction, catchAction); });

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

            Focus();
            Update();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Instance = null;
            GuiLocManager.formsReinitEvents.Remove(this);
        }

        private void LoadData()
            => Logger.TryOrLog(() =>
            {
                TextBox_Filters_States.Text = _filterStates;
                TextBox_Filters_Regions.Text = _filterRegions;
                TextBox_Filters_Countries.Text = _filterCountries;
            });

        private void ComboBox_Type_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Button_Update_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() => Update());

        private void Update()
        {
            var filter = new StatisticsFilters
            {
                statesIDs = ToHashSet(TextBox_Filters_States.Text, t => ushort.Parse(t)),
                regionsIDs = ToHashSet(TextBox_Filters_Regions.Text, t => ushort.Parse(t)),
                countriesTags = ToHashSet(TextBox_Filters_Countries.Text, t => t.ToUpper()),
            };

            var statistics = StatisticsManager.Collect(filter);

            var sb = new StringBuilder();
            statistics.Save(sb, "", "        ");
            RichTextBox_Statistics.Text = sb.ToString();
        }

        private HashSet<T> ToHashSet<T>(string text, Func<string, T> parser)
        {
            var set = new HashSet<T>();
            var args = text.Trim().Split(' ');
            foreach (var arg in args)
            {
                if (arg.Length == 0)
                    continue;

                set.Add(parser(arg));
            }
            return set;
        }

        private void Button_Filters_Clear_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                TextBox_Filters_States.Text = "";
                TextBox_Filters_Regions.Text = "";
                TextBox_Filters_Countries.Text = "";
            });

        private void TextBox_Filters_States_TextChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => _filterStates = TextBox_Filters_States.Text);
        private void TextBox_Filters_Regions_TextChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => _filterRegions = TextBox_Filters_Regions.Text);
        private void TextBox_Filters_Countries_TextChanged(object sender, EventArgs e)
            => Logger.TryOrLog(() => _filterCountries = TextBox_Filters_Countries.Text);
    }
}
