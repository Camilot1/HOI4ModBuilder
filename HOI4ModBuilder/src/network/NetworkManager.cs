using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using HOI4ModBuilder.src.network;
using HOI4ModBuilder.src.forms.messageForms;
using HOI4ModBuilder.src;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using HOI4ModBuilder;

public static class NetworkManager
{
    public static readonly string GitHubRepoURL = "https://github.com/Camilot1/HOI4ModBuilder";
    public static readonly string GitHubReleasesURL = GitHubRepoURL + "/releases";

    public static string DocumentationURL = "https://discord.gg/9Y5K4v85wd";
    public static string DiscordServerURL = "https://discord.gg/bc4wF8PMhb";
    public static string TelegramURL = "https://t.me/hoi4modbuilder";

    private static bool HasNetwork => NetworkInterface.GetIsNetworkAvailable();

    public static void OpenLink(string link)
    {
        Logger.TryOrLog(() =>
        {
            using (Process.Start(link)) { }
        });
    }

    public static void SyncGithubInfo()
    {
        Task.Run(() =>
        {
            string data = null;

            Logger.TryOrCatch(() =>
            {
                data = DownloadString("https://raw.githubusercontent.com/Camilot1/HOI4ModBuilder/master/sync_info.json\r\n");
            }, ex =>
            {
                Logger.LogSingleErrorMessage(EnumLocKey.EXCEPTION_NETWORK_UNABLE_TO_CHECK_FOR_UPDATE);
                Logger.Log(ex.StackTrace);
            });

            SyncInfo syncInfo = null;

            if (data != null)
            {
                Logger.TryOrLog(() => syncInfo = JsonConvert.DeserializeObject<SyncInfo>(data));
            }

            var localFilePath = Path.Combine("configs", "local_sync.json");
            SyncInfo localSyncInfo = null;

            if (!Directory.Exists("configs"))
                Directory.CreateDirectory("configs");

            if (File.Exists(localFilePath))
                localSyncInfo = JsonConvert.DeserializeObject<SyncInfo>(File.ReadAllText(localFilePath));
            else
                localSyncInfo = new SyncInfo { Links = new Dictionary<string, string>() };

            if (syncInfo != null)
                localSyncInfo.Links = syncInfo.Links;

            localSyncInfo.Links.TryGetValue("documentation", out DocumentationURL);
            localSyncInfo.Links.TryGetValue("discord", out DiscordServerURL);
            localSyncInfo.Links.TryGetValue("telegram", out TelegramURL);

            File.WriteAllText(localFilePath, JsonConvert.SerializeObject(localSyncInfo, Formatting.Indented));

            var settings = SettingsManager.Settings;
            bool isIgnoreUpdateChecks = settings.ignoreUpdateChecks;
            bool isSameIgnoreUpdateCheckVersion =
                syncInfo != null &&
                (
                    settings.ignoreUpdateCheckVersion == null ||
                    settings.ignoreUpdateCheckVersion.Length == 0 ||
                    settings.ignoreUpdateCheckVersion == syncInfo.LastVersion
                );

            if (syncInfo != null && Logger.versionId < syncInfo.LastVersionId &&
                (!isIgnoreUpdateChecks || isIgnoreUpdateChecks && !isSameIgnoreUpdateCheckVersion))
            {
                CheckUpdateForm.Create(syncInfo);
            }
        });
    }

    // Currently is used ONLY for getting latest info from HOI4 Mod Builder public repository
    private static async Task<string> DownloadStringAsync(string rawUrl, int timeoutSeconds = 5)
    {
        if (!HasNetwork)
            throw new InvalidOperationException("No internet connection.");

        using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSeconds) })
        {
            http.DefaultRequestHeaders.UserAgent.ParseAdd($"HOI4 Mod Builder");

            try
            {
                var response = await http.GetAsync(rawUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex) when (ex.InnerException is SocketException se &&
                   (se.SocketErrorCode == SocketError.HostNotFound ||
                    se.SocketErrorCode == SocketError.NetworkUnreachable ||
                    se.SocketErrorCode == SocketError.TimedOut))
            {
                throw new InvalidOperationException("Failed to establish connection. Check your internet connection.", ex);
            }
        }
    }

    private static string DownloadString(string rawUrl, int timeoutSeconds = 5)
        => DownloadStringAsync(rawUrl, timeoutSeconds).GetAwaiter().GetResult();
}

