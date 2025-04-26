using HOI4ModBuilder.src.network;
using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms.messageForms
{
    public partial class CheckUpdateForm : Form
    {
        public bool IsClosed { get; private set; }
        public SyncInfo SyncInfo { get; private set; }
        public static CheckUpdateForm Instance { get; private set; }


        private static readonly Dictionary<string, string> _localization = new Dictionary<string, string>
        {
            { "ru", "Вышло новое обновление: " },
            { "en", "The new update has been released: " },
        };

        public static void Create(SyncInfo syncInfo)
        {
            if (Instance != null)
                return;
            SystemSounds.Exclamation.Play();
            new CheckUpdateForm(syncInfo).ShowDialog();
        }
        public static void CreateTasked(SyncInfo syncInfo)
        {
            if (Instance != null)
                return;
            SystemSounds.Exclamation.Play();
            Task.Run(() => new CheckUpdateForm(syncInfo).ShowDialog());
        }

        public CheckUpdateForm(SyncInfo syncInfo)
        {
            InitializeComponent();
            SyncInfo = syncInfo;
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
            IsClosed = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            GuiLocManager.formsReinitEvents.Remove(this);
            IsClosed = true;
            Instance = null;
        }

        private void LoadData()
        {
            Logger.TryOrLog(() =>
            {
                var settings = SettingsManager.Settings;

                switch (GuiLocManager.GetCurrentParentLanguageName)
                {
                    case "ru":
                        Label_Status.Text = _localization["ru"] + SyncInfo.LastVersion;
                        break;
                    case "en":
                        Label_Status.Text = _localization["en"] + SyncInfo.LastVersion;
                        break;
                    default:
                        Label_Status.Text = _localization["en"] + SyncInfo.LastVersion;
                        break;
                }

                if (!SyncInfo.ShortDescription.TryGetValue(GuiLocManager.GetCurrentParentLanguageName, out var shortDescription))
                    shortDescription = SyncInfo.ShortDescription["en"];

                RichTexBox_Description.Text = shortDescription;
            });
        }


        private void RichTexBox_Description_LinkClicked(object sender, LinkClickedEventArgs e)
            => NetworkManager.OpenLink(e.LinkText);

        private void Button_GoToRelease_Click(object sender, EventArgs e)
            => NetworkManager.OpenLink(NetworkManager.GitHubReleasesURL);

        private void Button_IgnoreRelease_Click(object sender, EventArgs e)
            => Logger.TryOrLog(() =>
            {
                var settings = SettingsManager.Settings;
                settings.ignoreUpdateChecks = true;
                settings.ignoreUpdateCheckVersion = SyncInfo != null ? SyncInfo.LastVersion : "";
                SettingsManager.SaveSettings();
                Close();
            });

        private void Button_Close_Click(object sender, EventArgs e)
            => Close();
    }
}
