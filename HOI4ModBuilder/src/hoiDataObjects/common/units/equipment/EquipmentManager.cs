using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units.equipment
{
    public class EquipmentManager
    {
        private static readonly string directoryPath = FileManager.AssembleFolderPath(new[] { "common", "units", "equipment" });


        private static Dictionary<string, List<string>> _equipmentInternalTypes;

        private static Dictionary<FileInfo, EquipmentsFile> _equipmentsByFiles;
        private static Dictionary<string, Equipment> _allEquipments;

        public static bool TryGetEquipment(string name, out Equipment equipment)
            => _allEquipments.TryGetValue(name, out equipment);
        public static Equipment GetEquipment(string name) => _allEquipments[name];

        public static void Load(Settings settings)
        {
            _equipmentsByFiles = new Dictionary<FileInfo, EquipmentsFile>();
            _allEquipments = new Dictionary<string, Equipment>();

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
                            _equipmentsByFiles[equipmentsFile.FileInfo] = equipmentsFile;
                        }
                    },
                    (ex) => Logger.LogException(ex));
            }
        }

        public static void Save(Settings settings)
        {
            foreach (var equipmentsFile in _equipmentsByFiles.Values)
            {
                if (equipmentsFile.FileInfo.needToDelete)
                {
                    File.Delete(settings.modDirectory + directoryPath + equipmentsFile.FileInfo.fileName);
                    continue;
                }

                if (equipmentsFile.FileInfo.needToSave)
                {
                    StringBuilder sb = new StringBuilder();
                    equipmentsFile.Save(sb, "", "\t");

                    File.WriteAllText(settings.modDirectory + directoryPath + equipmentsFile.FileInfo.fileName, sb.ToString());
                    sb.Length = 0;
                }
            }
        }

    }
}
