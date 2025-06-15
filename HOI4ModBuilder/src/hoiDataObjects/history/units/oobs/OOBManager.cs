using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOBManager
    {
        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "history", "units" });
        private static Dictionary<FileInfo, OOB> _oobsByFiles = new Dictionary<FileInfo, OOB>();

        private static Dictionary<string, ShipInstances> _shipInstances = new Dictionary<string, ShipInstances>();

        public static ShipInstances RequestShipInstances(string name, LinkedLayer layer)
        {
            if (!_shipInstances.TryGetValue(name, out ShipInstances shipInstances))
            {
                shipInstances = new ShipInstances(name);
                _shipInstances[name] = shipInstances;
            }

            if (layer != null) shipInstances.AddRequest(layer);

            return shipInstances;
        }

        public static void Load(Settings settings)
        {
            if (!SettingsManager.Settings.IsWipEnabled(EnumWips.OOBS))
                return;

            _oobsByFiles = new Dictionary<FileInfo, OOB>();
            _shipInstances = new Dictionary<string, ShipInstances>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            foreach (FileInfo fileInfo in fileInfoPairs.Values)
            {
                Logger.TryOrCatch(
                    () =>
                    {
                        using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                        {
                            var oob = ParadoxParser.AdvancedParse(fs, new LinkedLayer(null, fileInfo.filePath, true), new OOB(fileInfo), out bool validationResult);
                            _oobsByFiles[fileInfo] = oob;
                        }
                    },
                    (ex) => Logger.LogException(ex));
            }

            CheckTaskForceShipInstances();
        }

        public static void CheckTaskForceShipInstances()
        {
            foreach (var shipInstance in _shipInstances.Values)
            {
                if (shipInstance.Ships.Count == 0)
                {
                    StringBuilder sb = new StringBuilder();
                    shipInstance.AssembleLayeredPathes(sb);

                    Logger.LogWarning(
                        EnumLocKey.TASK_FORCES_USES_SHIP_THAT_DOESNT_DEFINED_ANYWHERE,
                        new Dictionary<string, string>
                        {
                            { "{requestsCount}", $"{shipInstance.RequestsCount}" },
                            { "{taskForceShipName}", $"{shipInstance.Name}" },
                            { "{directoryPath}", $"{FOLDER_PATH}" },
                            { "{requestsList}", $"{sb}" }
                        }
                    );
                }
            }
        }

        public static void Save(Settings settings)
        {
            if (!SettingsManager.Settings.IsWipEnabled(EnumWips.OOBS))
                return;

            foreach (var oob in _oobsByFiles.Values)
            {
                if (oob.FileInfo.needToDelete)
                {
                    File.Delete(settings.modDirectory + FOLDER_PATH + oob.FileInfo.fileName);
                    continue;
                }

                if (oob.FileInfo.needToSave)
                {
                    StringBuilder sb = new StringBuilder();
                    oob.Save(sb, "", "\t");

                    File.WriteAllText(settings.modDirectory + FOLDER_PATH + oob.FileInfo.fileName, sb.ToString());
                    sb.Length = 0;
                }
            }
        }
    }
}
