using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.managers;
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
        private static Dictionary<string, SubUnit> _subUnits = new Dictionary<string, SubUnit>();

        public static void Load(Settings settings)
        {
            _subUnits = new Dictionary<string, SubUnit>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\units\");

            foreach (FileInfo fileInfo in fileInfos.Values)
            {
                using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, new SubUnitsFile(fileInfo, _subUnits));
            }
        }

        public static bool TryGetSubUnit(string name, out SubUnit subUnit)
            => _subUnits.TryGetValue(name, out subUnit);
    }
}
