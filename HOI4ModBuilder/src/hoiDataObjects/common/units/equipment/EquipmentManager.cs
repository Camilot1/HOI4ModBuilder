using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.IO;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units.equipment
{
    public class EquipmentManager
    {
        private static readonly string directoryPath = @"common\units\equipments\";

        private static Dictionary<string, Equipment> _allEquipments = new Dictionary<string, Equipment>();

        public static bool TryGetEquipment(string name, out Equipment equipment)
            => _allEquipments.TryGetValue(name, out equipment);
        public static Equipment GetEquipment(string name) => _allEquipments[name];

        public static void Load(Settings settings)
        {

            var fileInfoPairs = FileManager.ReadFileInfos(settings, directoryPath, FileManager.TXT_FORMAT);

            foreach (FileInfo fileInfo in fileInfoPairs.Values)
            {
                Logger.TryOrCatch(
                    () =>
                    {
                        using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                        {
                            var equipmentsFile = ParadoxParser.AdvancedParse(
                                fs,
                                new LinkedLayer(null, fileInfo.filePath, true),
                                new EquipmentsFile(fileInfo, _allEquipments),
                                out bool validationResult
                            );
                        }
                    },
                    (ex) => Logger.LogException(ex));
            }
        }

    }
}
