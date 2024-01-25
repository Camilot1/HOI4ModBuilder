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
        private static Dictionary<string, DivisionNamesGroup> _divisionNamesGroups = new Dictionary<string, DivisionNamesGroup>();

        public static void Load(Settings settings)
        {
            _divisionNamesGroups = new Dictionary<string, DivisionNamesGroup>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\units\names_divisions\");

            foreach (FileInfo fileInfo in fileInfos.Values)
            {
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, new DivisionNamesGroupFile(fileInfo, _divisionNamesGroups));
            }
        }

        public static bool TryGetNamesGroup(string name, out DivisionNamesGroup group)
            => _divisionNamesGroups.TryGetValue(name, out group);
    }
}
