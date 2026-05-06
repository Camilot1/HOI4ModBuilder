using System;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using HOI4ModBuilder.src.network;
using HOI4ModBuilder.src.forms.messageForms;
using HOI4ModBuilder.src;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text;
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
            string data = DownloadString("https://raw.githubusercontent.com/Camilot1/HOI4ModBuilder/master/sync_info.json");
            if (data == null)
                Logger.LogSingleErrorMessage(EnumLocKey.EXCEPTION_NETWORK_UNABLE_TO_CHECK_FOR_UPDATE);

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
    private static string DownloadString(string rawUrl, int timeoutSeconds = 5)
    {
        if (!HasNetwork)
            return null;

        try
        {
            var request = (HttpWebRequest)WebRequest.Create(rawUrl);
            request.Method = "GET";
            request.Timeout = timeoutSeconds * 1000;
            request.ReadWriteTimeout = timeoutSeconds * 1000;
            request.UserAgent = "HOI4 Mod Builder";

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null)
                    return null;

                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
        }
        catch (WebException ex)
        {
            Logger.Log($"Network update check failed: {ex}");
            return null;
        }
    }
}
