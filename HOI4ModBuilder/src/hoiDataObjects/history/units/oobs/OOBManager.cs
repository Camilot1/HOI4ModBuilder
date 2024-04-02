using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOBManager
    {
        private static readonly string directoryPath = @"history\units\";
        private static Dictionary<FileInfo, OOB> _oobsByFiles = new Dictionary<FileInfo, OOB>();

        private static Dictionary<string, TaskForceShipInstances> _taskForceShipInstances = new Dictionary<string, TaskForceShipInstances>();

        public static TaskForceShipInstances RequestTaskForceShipInstances(string name, LinkedLayer layer)
        {
            if (!_taskForceShipInstances.TryGetValue(name, out TaskForceShipInstances shipInstances))
            {
                shipInstances = new TaskForceShipInstances(name);
                _taskForceShipInstances[name] = shipInstances;
            }

            if (layer != null) shipInstances.requests.Add(layer);

            return shipInstances;
        }

        public static void Load(Settings settings)
        {
            _oobsByFiles = new Dictionary<FileInfo, OOB>();
            _taskForceShipInstances = new Dictionary<string, TaskForceShipInstances>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, directoryPath);

            foreach (FileInfo fileInfo in fileInfos.Values)
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
            foreach (var shipInstance in _taskForceShipInstances.Values)
            {
                if (shipInstance.TaskForceShips.Count == 0)
                {
                    StringBuilder sb = new StringBuilder();
                    string filePath = null;
                    string blockLayeredPath = null;

                    foreach (var layer in shipInstance.requests)
                    {
                        layer.AssembleLayeredPath(ref filePath, ref blockLayeredPath);
                        sb.Append("\t\nLayeredPath: \"").Append(blockLayeredPath).Append('\"');
                    }

                    Logger.LogWarning(
                        EnumLocKey.TASK_FORCES_USES_SHIP_THAT_DOESNT_DEFINED_ANYWHERE,
                        new Dictionary<string, string>
                        {
                            { "{requestsCount}", $"{shipInstance.requests.Count}" },
                            { "{taskForceShipName}", $"{shipInstance.Name}" },
                            { "{directoryPath}", $"{directoryPath}" },
                            { "{requestsList}", $"{sb}" }
                        }
                    );
                }
            }
        }

        public static void Save(Settings settings)
        {

        }
    }
}
