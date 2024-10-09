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

        public static readonly string GitHubReleasesURL = "https://github.com/Camilot1/HOI4ModBuilder/releases";
        public static readonly string GitHubRepoURL = "https://github.com/Camilot1/HOI4ModBuilder";
        public static readonly string DiscordServerURL = "https://discord.gg/bc4wF8PMhb";
        public static readonly string DiscordDocumentationURL = "https://discord.gg/9Y5K4v85wd";
        public static readonly string TelegramURL = "https://t.me/hoi4modbuilder";

        private static readonly Dictionary<string, string> localization = new Dictionary<string, string>
        {
            { "ru",
                $"Версия приложения: {Logger.version}\n" +
                $"Автор приложения: Camilot\n\n" +
                $"GitHub Релизы: {GitHubReleasesURL}\n" +
                $"GitHub Репозиторий: {GitHubRepoURL}\n" +
                $"Discord сервер: {DiscordServerURL}\n" +
                $"Telegram канал: {TelegramURL}"
            },
            { "en",
                $"Application version: {Logger.version}\n" +
                $"Application creator: Camilot\n\n" +
                $"GitHub Releases: {GitHubReleasesURL}\n" +
                $"GitHub Repository: {GitHubRepoURL}\n" +
                $"Discord server: {DiscordServerURL}\n" +
                $"Telegram channel: {TelegramURL}"
            }
        };

        public static void CreateTasked()
        {
            if (Instance != null) return;
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
                    case "ru": RichTextBox1.Text = localization["ru"]; break;
                    case "en": RichTextBox1.Text = localization["en"]; break;
                    default: RichTextBox1.Text = localization["en"]; break;
                }
            });
        }

        private void Button_GitHubReleases_Click(object sender, EventArgs e)
            => LinksUtils.OpenLink(GitHubReleasesURL);
        private void Button_GitHubRepo_Click(object sender, EventArgs e)
            => LinksUtils.OpenLink(GitHubRepoURL);
        private void Button_Discord_Click(object sender, EventArgs e)
            => LinksUtils.OpenLink(DiscordServerURL);
        private void Button_Telegram_Click(object sender, EventArgs e)
            => LinksUtils.OpenLink(TelegramURL);

        private void RichTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
            => LinksUtils.OpenLink(e.LinkText);
    }
}
