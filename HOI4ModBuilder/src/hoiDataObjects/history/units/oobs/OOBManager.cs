using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOBManager
    {
        private static Dictionary<string, OOB> _oobs = new Dictionary<string, OOB>();

        private static Dictionary<string, TaskForceShipBase> _taskForceShipBases = new Dictionary<string, TaskForceShipBase>();

        //TODO Implement adding/changing etc
        public static bool TryGetTaskForceShipBase(string name, out TaskForceShipBase taskForceShipBase) => _taskForceShipBases.TryGetValue(name, out taskForceShipBase);

        public static void Load(Settings settings)
        {
            _oobs = new Dictionary<string, OOB>();
            _taskForceShipBases = new Dictionary<string, TaskForceShipBase>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"history\units\");

            foreach (FileInfo fileInfo in fileInfos.Values)
            {
                Logger.TryOrCatch(
                    () =>
                    {
                        using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                            ParadoxParser.AdvancedParse(fs, new LinkedLayer(null, fileInfo.filePath, true), new OOB(fileInfo), out bool validationResult);
                    },
                    (ex) => Logger.LogException(ex));
            }
        }

        public static void Save(Settings settings)
        {

        }
    }
}
