using HOI4ModBuilder.src.dataObjects.argBlocks.info;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace HOI4ModBuilder.src.dataObjects.argBlocks
{
    class InfoArgsBlocksManager
    {
        private static readonly string SCOPES_FILE_PATH = FileManager.AssembleFilePath(new[] { "data", "scopes.json" });
        private static readonly string EFFECTS_FILE_PATH = FileManager.AssembleFilePath(new[] { "data", "effects.json" });
        private static readonly string MODIFIERS_FILE_PATH = FileManager.AssembleFilePath(new[] { "data", "modifiers.json" });
        private static readonly string TRIGGERS_FILE_PATH = FileManager.AssembleFilePath(new[] { "data", "triggers.json" });

        private static readonly string CUSTOM_SCOPES_FILE_PATH = FileManager.AssembleFilePath(new[] { "data", "custom", "scopes.json" });
        private static readonly string CUSTOM_EFFECTS_FILE_PATH = FileManager.AssembleFilePath(new[] { "data", "custom", "effects.json" });
        private static readonly string CUSTOM_MODIFIERS_FILE_PATH = FileManager.AssembleFilePath(new[] { "data", "custom", "modifiers.json" });
        private static readonly string CUSTOM_TRIGGERS_FILE_PATH = FileManager.AssembleFilePath(new[] { "data", "custom", "triggers.json" });

        private static readonly string SCRIPTED_EFFECTS_FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "scripted_effects" });
        private static readonly string DEFINED_MODIFIERS_FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "modifier_definitions" });
        private static readonly string SCRIPTED_TRIGGERS_FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "scripted_triggers" });

        private static Dictionary<string, InfoArgsBlock> _allInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();

        private static Dictionary<string, InfoArgsBlock> _scopesInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _effectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _modifiersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _triggersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();

        private static Dictionary<string, InfoArgsBlock> _scriptedEffectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _definedModifiersArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _scriptedTriggersArgsBlocks = new Dictionary<string, InfoArgsBlock>();

        private static Dictionary<string, string> _definitionFiles = new Dictionary<string, string>();

        private static string currentLoadingFilePath = null;

        public static bool TryGetEffect(string name, out InfoArgsBlock block)
            => _effectsInfoArgsBlocks.TryGetValue(name, out block) ||
                _scriptedEffectsInfoArgsBlocks.TryGetValue(name, out block);

        public static bool TryGetModifier(string name, out InfoArgsBlock block)
            => _modifiersInfoArgsBlocks.TryGetValue(name, out block) ||
                _definedModifiersArgsBlocks.TryGetValue(name, out block);

        public static bool TryGetTrigger(string name, out InfoArgsBlock block)
            => _triggersInfoArgsBlocks.TryGetValue(name, out block) ||
                _scriptedTriggersArgsBlocks.TryGetValue(name, out block);

        public static void Load(Settings settings)
        {
            Logger.TryOrCatch(() =>
            {
                ClearDictionaryData();
                LoadGameInfoArgsBlocks();
                LoadGameScriptedInfoArgsBlocks(settings);
                LoadCustomInfoArgsBlocks();

            },
            (e) => throw new Exception($"JSON file: \"{currentLoadingFilePath}\".", e));

            CleanUpAfterLoading();
        }

        /** Метод для получения информационных блоков об эффектах, модификаторах и т.п. */
        public static bool TryGetInfoArgsBlock(string token, out InfoArgsBlock infoArgsBlock)
        {
            if (_allInfoArgsBlocks.TryGetValue(token, out infoArgsBlock)) return true;
            return false;
        }

        private static void ClearDictionaryData()
        {
            _definitionFiles = new Dictionary<string, string>();

            _allInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();

            _scopesInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
            _effectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
            _modifiersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
            _triggersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();

            _scriptedEffectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
            _definedModifiersArgsBlocks = new Dictionary<string, InfoArgsBlock>();
            _scriptedTriggersArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        }

        private static void LoadGameInfoArgsBlocks()
        {
            LoadInfoArgsBlocks(SCOPES_FILE_PATH, _scopesInfoArgsBlocks);
            LoadInfoArgsBlocks(EFFECTS_FILE_PATH, _effectsInfoArgsBlocks);
            LoadInfoArgsBlocks(MODIFIERS_FILE_PATH, _modifiersInfoArgsBlocks);
            LoadInfoArgsBlocks(TRIGGERS_FILE_PATH, _triggersInfoArgsBlocks);
        }

        private static void LoadGameScriptedInfoArgsBlocks(Settings settings)
        {
            LoadScriptedEffects(settings, SCRIPTED_EFFECTS_FOLDER_PATH);
            LoadDefinedModifiers(settings, DEFINED_MODIFIERS_FOLDER_PATH);
            LoadScriptedTriggers(settings, SCRIPTED_TRIGGERS_FOLDER_PATH);
        }

        private static void LoadCustomInfoArgsBlocks()
        {
            LoadInfoArgsBlocks(CUSTOM_SCOPES_FILE_PATH, _scopesInfoArgsBlocks);
            LoadInfoArgsBlocks(CUSTOM_EFFECTS_FILE_PATH, _effectsInfoArgsBlocks);
            LoadInfoArgsBlocks(CUSTOM_MODIFIERS_FILE_PATH, _modifiersInfoArgsBlocks);
            LoadInfoArgsBlocks(CUSTOM_TRIGGERS_FILE_PATH, _triggersInfoArgsBlocks);
        }

        private static void CleanUpAfterLoading()
        {
            currentLoadingFilePath = null;
            _definitionFiles = null;
        }

        private static void LoadInfoArgsBlocks(string filePath, Dictionary<string, InfoArgsBlock> dictionary)
        {
            currentLoadingFilePath = filePath;
            if (!File.Exists(filePath)) File.WriteAllText(filePath, "[]");

            foreach (var block in JsonConvert.DeserializeObject<List<InfoArgsBlock>>(File.ReadAllText(filePath)))
            {
                if (block.IsDisabled) continue;

                var newBlocks = block.GetReplaceTagCopies();
                if (newBlocks.Count > 0)
                {
                    foreach (var newBlock in newBlocks)
                    {
                        if (dictionary.ContainsKey(newBlock.Name))
                            Logger.LogWarning(
                                EnumLocKey.EXCEPTION_INFO_ARGS_BLOCKS_WITH_REPLACE_TAGS_DUPLICATE_BLOCK_NAMES_IN_SAME_FILE,
                                new Dictionary<string, string>
                                {
                                    { "{filePath}", filePath },
                                    { "{newBlockName}", newBlock.Name },
                                    { "{originalBlockName}", block.Name }
                                }
                            );
                        else if (_allInfoArgsBlocks.ContainsKey(newBlock.Name))
                            Logger.LogWarning(
                                EnumLocKey.EXCEPTION_INFO_ARGS_BLOCKS_WITH_REPLACE_TAGS_DUPLICATE_BLOCK_NAME_IN_FILE,
                                new Dictionary<string, string>
                                {
                                    { "{filePath}", filePath },
                                    { "{newBlockName}", newBlock.Name },
                                    { "{originalBlockName}", block.Name }
                                }
                            );
                        else
                        {
                            _allInfoArgsBlocks[newBlock.Name] = newBlock;
                            dictionary[newBlock.Name] = newBlock;
                            _definitionFiles[block.Name] = filePath;
                        }
                    }
                }
                else
                {
                    if (dictionary.ContainsKey(block.Name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_INFO_ARGS_BLOCKS_DUPLICATE_BLOCK_NAMES_IN_SAME_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", filePath },
                                { "{blockName}", block.Name }
                            }
                        );
                    else if (_allInfoArgsBlocks.ContainsKey(block.Name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_INFO_ARGS_BLOCKS_DUPLICATE_BLOCK_NAME_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", filePath },
                                { "{blockName}", block.Name }
                            }
                        );
                    else
                    {
                        _allInfoArgsBlocks[block.Name] = block;
                        dictionary[block.Name] = block;
                        _definitionFiles[block.Name] = filePath;
                    }
                }
            }
        }

        private static void LoadScriptedEffects(Settings settings, string folderPath)
        {
            foreach (var fileInfoPair in FileManager.ReadFileInfos(settings, folderPath, FileManager.ANY_FORMAT))
            {
                currentLoadingFilePath = fileInfoPair.Value.filePath;

                var list = new List<ScriptedEffect>();
                var file = new ScriptedEffectsFile(list);

                using (var fs = new FileStream(fileInfoPair.Value.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, file);

                foreach (var info in list)
                {
                    if (_scriptedEffectsInfoArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_SCRIPTED_EFFECT_DUPLICATE_NAME_WITH_OTHER_SCRIPTED_EFFECT_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", _definitionFiles[info.name] }
                            }
                        );
                    else if (_allInfoArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_SCRIPTED_EFFECT_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", _definitionFiles[info.name] }
                            }
                        );
                    else
                    {
                        var infoArgsBlock = new InfoArgsBlock(
                            info.name,
                            new EnumScope[] { EnumScope.ALL },
                            new EnumValueType[] { EnumValueType.BOOLEAN }
                        );

                        _allInfoArgsBlocks[info.name] = infoArgsBlock;
                        _scriptedEffectsInfoArgsBlocks[info.name] = infoArgsBlock;
                        _definitionFiles[info.name] = fileInfoPair.Value.filePath;
                    }
                }
            }
        }


        private static void LoadDefinedModifiers(Settings settings, string folderPath)
        {
            foreach (var fileInfoPair in FileManager.ReadFileInfos(settings, folderPath, FileManager.TXT_FORMAT))
            {
                currentLoadingFilePath = fileInfoPair.Value.filePath;

                var list = new List<DefinedModifier>();
                var file = new DefinedModifierFile(list);

                using (var fs = new FileStream(fileInfoPair.Value.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, file);

                foreach (var info in list)
                {
                    if (_definedModifiersArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_DEFINED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_DEFINED_MODIFIER_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", _definitionFiles[info.name] }
                            }
                        );
                    else if (_allInfoArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_DEFINED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", _definitionFiles[info.name] }
                            }
                        );
                    else
                    {
                        List<EnumScope> scopes = new List<EnumScope>();
                        foreach (var scope in info.categories)
                        {
                            var uppedScope = scope.ToUpper();
                            if (!Enum.TryParse(uppedScope, out EnumScope enumScope))
                                throw new Exception(GuiLocManager.GetLoc(
                                    EnumLocKey.EXCEPTION_DEFINED_MODIFIER_HAS_UNSUPPORTED_CATEGORY_IN_FILE,
                                    new Dictionary<string, string>
                                    {
                                        { "{filePath}", fileInfoPair.Value.filePath },
                                        { "{blockName}", info.name },
                                        { "{category}", uppedScope },
                                        { "{allowedCategories}", string.Join(",", Enum.GetValues(typeof(EnumScope))) }
                                    }
                                ));

                            scopes.Add(enumScope);
                        }
                        if (scopes.Count == 0) scopes.Add(EnumScope.ALL);

                        EnumValueType defaultValueType;
                        object defaultValue;

                        if (info.valueType == "number" || info.valueType == "percentage" || info.valueType == "percentage_in_hundred")
                        {
                            defaultValueType = EnumValueType.FLOAT;
                            defaultValue = 0;
                        }
                        else if (info.valueType == "yes_no")
                        {
                            defaultValueType = EnumValueType.BOOLEAN;
                            defaultValue = false;
                        }
                        else throw new Exception(GuiLocManager.GetLoc(
                                    EnumLocKey.EXCEPTION_DEFINED_MODIFIER_HAS_UNSUPPORTED_VALUE_TYPE_IN_FILE,
                                    new Dictionary<string, string>
                                    {
                                        { "{filePath}", fileInfoPair.Value.filePath },
                                        { "{blockName}", info.name },
                                        { "{valueType}", info.valueType },
                                        { "{allowedCategories}", string.Join(",", Enum.GetValues(typeof(EnumScope))) }
                                    }
                                ));

                        var infoArgsBlock = new InfoArgsBlock(
                            info.name,
                            scopes.ToArray(),
                            new EnumValueType[] { defaultValueType },
                            defaultValueType,
                            defaultValue
                        );

                        _allInfoArgsBlocks[info.name] = infoArgsBlock;
                        _definedModifiersArgsBlocks[info.name] = infoArgsBlock;
                        _definitionFiles[info.name] = fileInfoPair.Value.filePath;
                    }
                }
            }
        }

        private static void LoadScriptedTriggers(Settings settings, string folderPath)
        {
            foreach (var fileInfoPair in FileManager.ReadFileInfos(settings, folderPath, FileManager.TXT_FORMAT))
            {
                currentLoadingFilePath = fileInfoPair.Value.filePath;

                var list = new List<ScriptedTrigger>();
                var file = new ScriptedTriggerFile(list);

                using (var fs = new FileStream(fileInfoPair.Value.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, file);

                foreach (var info in list)
                {
                    if (_scriptedTriggersArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_SCRIPTED_TRIGGER_DUPLICATE_NAME_WITH_OTHER_SCRIPTED_TRIGGER_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", _definitionFiles[info.name] }
                            }
                        );
                    else if (_allInfoArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_SCRIPTED_TRIGGER_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", _definitionFiles[info.name] }
                            }
                        );
                    else
                    {
                        var infoArgsBlock = new InfoArgsBlock(
                            info.name,
                            new EnumScope[] { EnumScope.ALL },
                            new EnumValueType[] { EnumValueType.BOOLEAN }
                        );

                        _allInfoArgsBlocks[info.name] = infoArgsBlock;
                        _scriptedTriggersArgsBlocks[info.name] = infoArgsBlock;
                        _definitionFiles[info.name] = fileInfoPair.Value.filePath;
                    }
                }
            }
        }
    }

}
