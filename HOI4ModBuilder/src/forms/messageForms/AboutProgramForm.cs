using HOI4ModBuilder.src.utils;
using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.forms.messageForms
{
    public partial class AboutProgramForm : Form
    {
        public bool IsClosed { get; private set; }
        public static AboutProgramForm Instance { get; private set; }

        private static readonly Dictionary<string, string> _localization = new Dictionary<string, string>
        {
            { "ru",
                $"Версия приложения: {Logger.version}\n" +
                $"Автор приложения: Camilot\n\n" +
                $"GitHub Релизы: {NetworkManager.GitHubReleasesURL}\n" +
                $"GitHub Репозиторий: {NetworkManager.GitHubRepoURL}\n" +
                $"Discord сервер: {NetworkManager.DiscordServerURL}\n" +
                $"Telegram канал: {NetworkManager.TelegramURL}"
            },
            { "en",
                $"Application version: {Logger.version}\n" +
                $"Application creator: Camilot\n\n" +
                $"GitHub Releases: {NetworkManager.GitHubReleasesURL}\n" +
                $"GitHub Repository: {NetworkManager.GitHubRepoURL}\n" +
                $"Discord server: {NetworkManager.DiscordServerURL}\n" +
                $"Telegram channel: {NetworkManager.TelegramURL}"
            }
        };

        public static void CreateTasked()
        {
            if (Instance != null)
                return;
            Task.Run(() => new AboutProgramForm().ShowDialog());
            SystemSounds.Exclamation.Play();
        }

        public AboutProgramForm()
        {
            InitializeComponent();
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
                    case "ru": RichTextBox1.Text = _localization["ru"]; break;
                    case "en": RichTextBox1.Text = _localization["en"]; break;
                    default: RichTextBox1.Text = _localization["en"]; break;
                }
            });
        }

        private void Button_GitHubReleases_Click(object sender, EventArgs e)
            => NetworkManager.OpenLink(NetworkManager.GitHubReleasesURL);
        private void Button_GitHubRepo_Click(object sender, EventArgs e)
            => NetworkManager.OpenLink(NetworkManager.GitHubRepoURL);
        private void Button_Discord_Click(object sender, EventArgs e)
            => NetworkManager.OpenLink(NetworkManager.DiscordServerURL);
        private void Button_Telegram_Click(object sender, EventArgs e)
            => NetworkManager.OpenLink(NetworkManager.TelegramURL);

        private void RichTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
            => NetworkManager.OpenLink(e.LinkText);
    }
}
