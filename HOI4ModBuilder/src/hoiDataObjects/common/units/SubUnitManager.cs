using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units
{
    class SubUnitManager
    {
        private static readonly string DIRECTORY_PATH = @"common\units\";
        private static Dictionary<string, SubUnit> _allSubUnits = new Dictionary<string, SubUnit>();
        private static Dictionary<FileInfo, SubUnitsFile> _subUnitsFiles = new Dictionary<FileInfo, SubUnitsFile>();

        public static void Load(Settings settings)
        {
            _allSubUnits = new Dictionary<string, SubUnit>();
            _subUnitsFiles = new Dictionary<FileInfo, SubUnitsFile>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, DIRECTORY_PATH, FileManager.TXT_FORMAT);

            foreach (FileInfo fileInfo in fileInfoPairs.Values)
            {
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                {
                    var subUnitsFile = ParadoxParser.AdvancedParse(fs, new LinkedLayer(null, fileInfo.filePath, true), new SubUnitsFile(fileInfo, _allSubUnits), out bool validationResult);
                    _subUnitsFiles[fileInfo] = subUnitsFile;
                }
            }
        }

        public static void Save(Settings settings)
        {
            foreach (var file in _subUnitsFiles.Values)
            {
                var fileInfo = file.FileInfo;
                if (fileInfo.needToDelete)
                {
                    File.Delete(settings.modDirectory + DIRECTORY_PATH + fileInfo.fileName);
                    continue;
                }
                if (fileInfo.needToSave)
                {
                    StringBuilder sb = new StringBuilder();
                    file.Save(sb, "", "\t");
                    File.WriteAllText(settings.modDirectory + DIRECTORY_PATH + fileInfo.fileName, sb.ToString());
                    sb.Length = 0;
                }
            }
        }

        public static bool TryGetSubUnit(string name, out SubUnit subUnit)
            => _allSubUnits.TryGetValue(name, out subUnit);
    }
}
