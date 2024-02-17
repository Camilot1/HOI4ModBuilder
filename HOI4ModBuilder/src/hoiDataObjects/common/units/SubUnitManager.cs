using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units
{
    class SubUnitManager
    {
        private static Dictionary<string, SubUnit> _allSubUnits = new Dictionary<string, SubUnit>();

        public static void Load(Settings settings)
        {
            _allSubUnits = new Dictionary<string, SubUnit>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\units\");

            foreach (FileInfo fileInfo in fileInfos.Values)
            {
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                    ParadoxParser.AdvancedParse(fs, new LinkedLayer(null, fileInfo.filePath, true), new SubUnitsFile(fileInfo, _allSubUnits), out bool validationResult);
            }

            var test = "t";
        }

        public static bool TryGetSubUnit(string name, out SubUnit subUnit)
            => _allSubUnits.TryGetValue(name, out subUnit);
    }
}
