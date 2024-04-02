using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.common.ai_areas
{
    class AiAreaManager
    {
        public static Dictionary<FileInfo, AiAreasFile> _aiAreasFiles = new Dictionary<FileInfo, AiAreasFile>();
        public static Dictionary<string, AiArea> _allAiAreas = new Dictionary<string, AiArea>();
        public static Dictionary<string, FileInfo> _aiAreasDefinition = new Dictionary<string, FileInfo>();

        public static void Load(Settings settings)
        {
            _aiAreasFiles = new Dictionary<FileInfo, AiAreasFile>();
            _allAiAreas = new Dictionary<string, AiArea>();
            _aiAreasDefinition = new Dictionary<string, FileInfo>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\ai_areas\");

            foreach (var fileInfo in fileInfos.Values)
            {
                Logger.TryOrCatch(
                    () =>
                    {
                        using (var fs = new FileStream(fileInfo.filePath, FileMode.Open))
                        {
                            var aiAreasFile = ParadoxParser.AdvancedParse(fs, new LinkedLayer(null, fileInfo.filePath, true), new AiAreasFile(fileInfo), out bool validationResult);
                            _aiAreasFiles[fileInfo] = aiAreasFile;

                            foreach (var aiArea in aiAreasFile.AiAreas)
                            {
                                if (_aiAreasDefinition.ContainsKey(aiArea.Name))
                                {
                                    Logger.LogError(
                                        EnumLocKey.AI_AREA_WITH_THIS_NAME_ALREADY_DEFINED_IN_ANOTHER_FILE,
                                        new Dictionary<string, string>
                                        {
                                            { "{aiAreaName}", aiArea.Name },
                                            { "{currentFilePath}", fileInfo.filePath },
                                            { "{firstFilePath}", _aiAreasDefinition[aiArea.Name].filePath }
                                        }
                                    );
                                }

                                _allAiAreas[aiArea.Name] = aiArea;
                                _aiAreasDefinition[aiArea.Name] = fileInfo;

                            }
                        }
                    },
                    (ex) => Logger.LogException(ex));
            }
        }

        public static void Save(Settings settings)
        {
            var sb = new StringBuilder();

            foreach (var aiAreasFile in _aiAreasFiles.Values)
            {
                if (aiAreasFile.FileInfo.needToSave)
                {
                    Logger.TryOrCatch(
                        () => aiAreasFile.Save(sb, "", "\t"),
                        (ex) =>
                            throw new Exception(GuiLocManager.GetLoc(
                                EnumLocKey.EXCEPTION_WHILE_AI_AREAS_SAVING,
                                new Dictionary<string, string> { { "{filePath}", $"{aiAreasFile.FileInfo.filePath}" } }
                            ), ex)
                    );

                    File.WriteAllText(settings.modDirectory + @"common\ai_areas\" + aiAreasFile.FileInfo.fileName, sb.ToString());
                    sb.Length = 0;
                }

                if (!aiAreasFile.HasAnyInnerInfo)
                    File.Delete(settings.modDirectory + @"common\ai_areas\" + aiAreasFile.FileInfo.fileName);
            }
        }
    }
}
