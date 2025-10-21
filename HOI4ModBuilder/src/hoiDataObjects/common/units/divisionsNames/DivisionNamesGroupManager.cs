using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units.divisionsNames
{
    class DivisionNamesGroupManager
    {
        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "units", "names_divisions" });
        private static Dictionary<string, DivisionNamesGroup> _divisionNamesGroups = new Dictionary<string, DivisionNamesGroup>();
        private static Dictionary<FileInfo, DivisionNamesGroup> _divisionNamesGroupFiles = new Dictionary<FileInfo, DivisionNamesGroup>();

        public static void Load(BaseSettings settings)
        {
            if (!SettingsManager.Settings.IsWipEnabled(EnumWips.DIVISIONS_NAMES_GROUPS))
                return;

            _divisionNamesGroups = new Dictionary<string, DivisionNamesGroup>();
            _divisionNamesGroupFiles = new Dictionary<FileInfo, DivisionNamesGroup>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            foreach (FileInfo fileInfo in fileInfoPairs.Values)
            {
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, new DivisionNamesGroupFile(fileInfo, _divisionNamesGroups));
            }
        }
        public static void Save(BaseSettings settings)
        {
            if (!SettingsManager.Settings.IsWipEnabled(EnumWips.DIVISIONS_NAMES_GROUPS))
                return;

            foreach (var file in _divisionNamesGroupFiles.Values)
            {
                var fileInfo = file.FileInfo;
                if (fileInfo.needToDelete)
                {
                    File.Delete(settings.modDirectory + FOLDER_PATH + fileInfo.fileName);
                    continue;
                }
                if (fileInfo.needToSave)
                {
                    StringBuilder sb = new StringBuilder();
                    //TODO implement
                    //file.Save(sb, "", "\t");
                    File.WriteAllText(settings.modDirectory + FOLDER_PATH + fileInfo.fileName, sb.ToString());
                    sb.Length = 0;
                }
            }
        }

        public static bool TryGetNamesGroup(string name, out DivisionNamesGroup group)
            => _divisionNamesGroups.TryGetValue(name, out group);
    }
}
