using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms
{
    public partial class TextBoxMessageForm : Form
    {
        private string _title, _mainText;
        private string _richText;

        public bool IsClosed { get; private set; }

        public static void CreateTasked(string title, string mainText, string richText) => CreateTasked(title, mainText, richText, false, null);
        public static void CreateTasked(string title, string mainText, string richText, bool hasSound) => CreateTasked(title, mainText, richText, hasSound, null);

        public static void CreateTasked(string title, string mainText, string richText, bool hasSound, List<TextBoxMessageForm> listedForms)
        {
            Task.Run(() =>
            {
                var form = new TextBoxMessageForm(title, mainText, richText);
                listedForms?.Add(form);
                form.ShowDialog();
            });
            if (hasSound) SystemSounds.Exclamation.Play();
        }


        public TextBoxMessageForm(string title, string mainText, string richText)
        {
            InitializeComponent();
            _title = title;
            _mainText = mainText;
            _richText = richText;
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
            IsClosed = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            GuiLocManager.formsReinitEvents.Remove(this);
            IsClosed = true;
        }

        private void LoadData()
        {
            Text = _title;
            Label_MainText.Text = _mainText;
            RichTextBox_Main.Text = _richText;
        }

        private void Button_CopyToClipboard_Click(object sender, EventArgs e)
            => MainForm.Instance.InvokeAction(() => Clipboard.SetText(_richText));

        private void Button_OpenLogsDirectory_Click(object sender, EventArgs e)
        {
            Logger.TryOrLog(() =>
            {
                using (Process.Start("explorer", Application.StartupPath + @"\logs\")) { };
            });
        }

        private void Button_Close_Click(object sender, EventArgs e)
            => Close();
    }
}
