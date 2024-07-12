using HOI4ModBuilder.src.managers;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units.divisionsNames
{
    class DivisionNamesGroupManager
    {
        private static readonly string DIRECTORY_PATH = @"common\units\names_divisions\";
        private static Dictionary<string, DivisionNamesGroup> _divisionNamesGroups = new Dictionary<string, DivisionNamesGroup>();
        private static Dictionary<FileInfo, DivisionNamesGroup> _divisionNamesGroupFiles = new Dictionary<FileInfo, DivisionNamesGroup>();

        public static void Load(Settings settings)
        {
            _divisionNamesGroups = new Dictionary<string, DivisionNamesGroup>();
            _divisionNamesGroupFiles = new Dictionary<FileInfo, DivisionNamesGroup>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, DIRECTORY_PATH, FileManager.TXT_FORMAT);

            foreach (FileInfo fileInfo in fileInfoPairs.Values)
            {
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, new DivisionNamesGroupFile(fileInfo, _divisionNamesGroups));
            }
        }
        public static void Save(Settings settings)
        {
            foreach (var file in _divisionNamesGroupFiles.Values)
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
                    //TODO implement
                    //file.Save(sb, "", "\t");
                    File.WriteAllText(settings.modDirectory + DIRECTORY_PATH + fileInfo.fileName, sb.ToString());
                    sb.Length = 0;
                }
            }
        }

        public static bool TryGetNamesGroup(string name, out DivisionNamesGroup group)
            => _divisionNamesGroups.TryGetValue(name, out group);
    }
}
