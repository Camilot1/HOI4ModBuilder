using HOI4ModBuilder.src.dataObjects.argBlocks.info;
using HOI4ModBuilder.src.dataObjects.argBlocks.info.scripted;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace HOI4ModBuilder.src.dataObjects.argBlocks
{
    public class InfoArgsBlocksManager
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
        private static readonly string SCRIPTED_STATIC_MODIFIERS_FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "modifiers" });
        private static readonly string SCRIPTED_DYNAMIC_MODIFIERS_FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "dynamic_modifiers" });
        private static readonly string DEFINED_MODIFIERS_FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "modifier_definitions" });
        private static readonly string SCRIPTED_TRIGGERS_FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "scripted_triggers" });

        private static readonly string BUILDINGS_CUSTOM_ARGS_BLOCKS = FileManager.AssembleFilePath(new[] { "data", "buildings_custom_args_blocks.json" });

        private static Dictionary<string, InfoArgsBlock> _allInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();

        private static Dictionary<string, InfoArgsBlock> _scopesInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _effectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _modifiersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _triggersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();

        private static Dictionary<string, InfoArgsBlock> _scriptedEffectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _scriptedModifiersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _definedModifiersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        private static Dictionary<string, InfoArgsBlock> _scriptedTriggersArgsBlocks = new Dictionary<string, InfoArgsBlock>();

        private static Dictionary<Building, InfoArgsBlock> _buildingsCustomArgsBlocks = new Dictionary<Building, InfoArgsBlock>();

        private static Dictionary<string, string> _definitionFiles = new Dictionary<string, string>();

        private static string currentLoadingFilePath = null;

        public static bool TryGetScope(string name, out InfoArgsBlock block)
        {
            name = name.ToLower();
            return _scopesInfoArgsBlocks.TryGetValue(name, out block);
        }
        public static InfoArgsBlock GetScope(string name)
        {
            if (TryGetScope(name, out var block))
                return block;
            else
                return null;
        }

        public static bool TryGetEffect(string name, out InfoArgsBlock block)
        {
            name = name.ToLower();
            return _effectsInfoArgsBlocks.TryGetValue(name, out block) ||
                _scriptedEffectsInfoArgsBlocks.TryGetValue(name, out block);
        }
        public static InfoArgsBlock GetEffect(string name)
        {
            if (TryGetEffect(name, out var block))
                return block;
            else
                return null;
        }

        public static bool TryGetModifier(string name, out InfoArgsBlock block)
        {
            name = name.ToLower();
            return _modifiersInfoArgsBlocks.TryGetValue(name, out block) ||
                _scriptedModifiersInfoArgsBlocks.TryGetValue(name, out block) ||
                _definedModifiersInfoArgsBlocks.TryGetValue(name, out block);
        }
        public static InfoArgsBlock GetModifier(string name)
        {
            if (TryGetModifier(name, out var block))
                return block;
            else
                return null;
        }

        public static bool TryGetTrigger(string name, out InfoArgsBlock block)
        {
            name = name.ToLower();
            return _triggersInfoArgsBlocks.TryGetValue(name, out block) ||
                  _scriptedTriggersArgsBlocks.TryGetValue(name, out block);
        }
        public static InfoArgsBlock GetTrigger(string name)
        {
            if (TryGetTrigger(name, out var block))
                return block;
            else
                return null;
        }

        public static void Load(BaseSettings settings)
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
            token = token.ToLower();
            if (_allInfoArgsBlocks.TryGetValue(token, out infoArgsBlock))
                return true;
            return false;
        }

        public static bool TryGetBuildingCustomArgsBlock(string token, out InfoArgsBlock infoArgsBlock)
        {
            infoArgsBlock = null;
            if (!BuildingManager.TryGetBuilding(token, out var building))
                return false;

            if (_buildingsCustomArgsBlocks.TryGetValue(building, out infoArgsBlock))
                return true;

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
            _scriptedModifiersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
            _definedModifiersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
            _scriptedTriggersArgsBlocks = new Dictionary<string, InfoArgsBlock>();

        }

        private static void LoadGameInfoArgsBlocks()
        {
            LoadInfoArgsBlocks(SCOPES_FILE_PATH, EnumScope.SCOPE, _scopesInfoArgsBlocks);
            LoadInfoArgsBlocks(EFFECTS_FILE_PATH, EnumScope.EFFECT, _effectsInfoArgsBlocks);
            LoadInfoArgsBlocks(MODIFIERS_FILE_PATH, EnumScope.MODIFIER, _modifiersInfoArgsBlocks);
            LoadInfoArgsBlocks(TRIGGERS_FILE_PATH, EnumScope.TRIGGER, _triggersInfoArgsBlocks);
        }

        private static void LoadGameScriptedInfoArgsBlocks(BaseSettings settings)
        {
            if (settings == null)
                return;

            LoadScriptedEffects(settings, SCRIPTED_EFFECTS_FOLDER_PATH);
            LoadScriptedModifiers(settings, SCRIPTED_STATIC_MODIFIERS_FOLDER_PATH);
            LoadScriptedModifiers(settings, SCRIPTED_DYNAMIC_MODIFIERS_FOLDER_PATH);
            LoadDefinedModifiers(settings, DEFINED_MODIFIERS_FOLDER_PATH);
            LoadScriptedTriggers(settings, SCRIPTED_TRIGGERS_FOLDER_PATH);
        }

        private static void LoadCustomInfoArgsBlocks()
        {
            LoadInfoArgsBlocks(CUSTOM_SCOPES_FILE_PATH, EnumScope.SCOPE, _scopesInfoArgsBlocks);
            LoadInfoArgsBlocks(CUSTOM_EFFECTS_FILE_PATH, EnumScope.EFFECT, _effectsInfoArgsBlocks);
            LoadInfoArgsBlocks(CUSTOM_MODIFIERS_FILE_PATH, EnumScope.MODIFIER, _modifiersInfoArgsBlocks);
            LoadInfoArgsBlocks(CUSTOM_TRIGGERS_FILE_PATH, EnumScope.TRIGGER, _triggersInfoArgsBlocks);
        }

        public static void LoadBuildingsCustomArgsBlocks()
        {
            _buildingsCustomArgsBlocks = new Dictionary<Building, InfoArgsBlock>();
            LoadBuildingsCustomArgsBlocks(BUILDINGS_CUSTOM_ARGS_BLOCKS, EnumScope.BUILDING, _buildingsCustomArgsBlocks);
        }

        private static void CleanUpAfterLoading()
        {
            currentLoadingFilePath = null;
            _definitionFiles = null;
        }

        private static void LoadBuildingsCustomArgsBlocks(string filePath, EnumScope enumScope, Dictionary<Building, InfoArgsBlock> dictionary)
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "{}");
            }


            var block = JsonConvert.DeserializeObject<InfoArgsBlock>(File.ReadAllText(filePath));

            if (block.IsDisabled)
                return;

            block.Init(enumScope);

            var newBlocks = block.GetReplaceTagCopies();
            foreach (var newBlock in newBlocks)
            {
                var name = newBlock.Name;
                if (!BuildingManager.TryGetBuilding(name, out var building))
                {
                    throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_LOADING_BULDINGS_CUSTOM_ARGS_BLOCKS_BUILDING_WASNT_LOADED,
                        new Dictionary<string, string> { { "{name}", name } }
                        ));
                }

                dictionary[building] = newBlock;
            }

        }

        private static void LoadInfoArgsBlocks(string filePath, EnumScope enumScope, Dictionary<string, InfoArgsBlock> dictionary)
        {
            currentLoadingFilePath = filePath;
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
            }

            foreach (var block in JsonConvert.DeserializeObject<List<InfoArgsBlock>>(File.ReadAllText(filePath)))
            {
                if (block.IsDisabled)
                    continue;

                block.Init(enumScope);

                var newBlocks = block.GetReplaceTagCopies();
                if (newBlocks.Count > 0)
                {
                    foreach (var newBlock in newBlocks)
                    {
                        var name = newBlock.Name.ToLower();
                        if (dictionary.ContainsKey(name))
                            Logger.LogWarning(
                                EnumLocKey.EXCEPTION_INFO_ARGS_BLOCKS_WITH_REPLACE_TAGS_DUPLICATE_BLOCK_NAMES_IN_SAME_FILE,
                                new Dictionary<string, string>
                                {
                                    { "{filePath}", filePath },
                                    { "{newBlockName}", newBlock.Name },
                                    { "{originalBlockName}", block.Name }
                                }
                            );
                        else if (_allInfoArgsBlocks.ContainsKey(name))
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
                            _allInfoArgsBlocks[name] = newBlock;
                            dictionary[name] = newBlock;
                            _definitionFiles[name] = filePath;
                        }
                    }
                }
                else
                {
                    var name = block.Name.ToLower();
                    if (dictionary.ContainsKey(name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_INFO_ARGS_BLOCKS_DUPLICATE_BLOCK_NAMES_IN_SAME_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", filePath },
                                { "{blockName}", block.Name }
                            }
                        );
                    else if (_allInfoArgsBlocks.ContainsKey(name))
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
                        _allInfoArgsBlocks[name] = block;
                        dictionary[name] = block;
                        _definitionFiles[name] = filePath;
                    }
                }
            }
        }

        private static void LoadDefinedModifiers(BaseSettings settings, string folderPath)
        {
            foreach (var fileInfoPair in FileManager.ReadFileInfos(settings, folderPath, FileManager.TXT_FORMAT))
            {
                currentLoadingFilePath = fileInfoPair.Value.filePath;
                Logger.TryOrCatch(
                    () => LoadDefinedModifiersFile(fileInfoPair),
                    (ex) => Logger.LogExceptionAsWarning(
                        EnumLocKey.ERROR_COULD_NOT_LOAD_DEFINED_MODIFIERS_FILE,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", fileInfoPair.Value.filePath },
                            { "{cause}", ex.Message },
                        },
                        ex)
                    );
            }
        }

        private static void LoadScriptedModifiers(BaseSettings settings, string folderPath)
        {
            foreach (var fileInfoPair in FileManager.ReadFileInfos(settings, folderPath, FileManager.ANY_FORMAT))
            {
                currentLoadingFilePath = fileInfoPair.Value.filePath;

                Logger.TryOrCatch(
                    () => LoadScriptedModifiersFile(fileInfoPair),
                    (ex) => Logger.LogExceptionAsWarning(
                        EnumLocKey.ERROR_COULD_NOT_LOAD_SCRIPTED_MODIFIERS_FILE,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", fileInfoPair.Value.filePath },
                            { "{cause}", ex.Message },
                        },
                        ex)
                    );


            }
        }

        private static void LoadScriptedEffects(BaseSettings settings, string folderPath)
        {
            foreach (var fileInfoPair in FileManager.ReadFileInfos(settings, folderPath, FileManager.ANY_FORMAT))
            {
                currentLoadingFilePath = fileInfoPair.Value.filePath;

                Logger.TryOrCatch(
                    () => LoadScriptedEffectsFile(fileInfoPair),
                    (ex) => Logger.LogExceptionAsWarning(
                        EnumLocKey.ERROR_COULD_NOT_LOAD_SCRIPTED_EFFECTS_FILE,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", fileInfoPair.Value.filePath },
                            { "{cause}", ex.Message },
                        },
                        ex)
                    );


            }
        }

        private static void LoadScriptedTriggers(BaseSettings settings, string folderPath)
        {
            foreach (var fileInfoPair in FileManager.ReadFileInfos(settings, folderPath, FileManager.TXT_FORMAT))
            {
                currentLoadingFilePath = fileInfoPair.Value.filePath;
                Logger.TryOrCatch(
                    () => LoadScriptedTriggersFile(fileInfoPair),
                    (ex) => Logger.LogExceptionAsWarning(
                        EnumLocKey.ERROR_COULD_NOT_LOAD_SCRIPTED_TRIGGERS_FILE,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", fileInfoPair.Value.filePath },
                            { "{cause}", ex.Message },
                        },
                        ex)
                    );
            }
        }

        private static void LoadDefinedModifiersFile(KeyValuePair<string, FileInfo> fileInfoPair)
        {
            var parser = new GameParser();
            var file = new DefinedModifiersGameFile(fileInfoPair.Value, true);
            parser.ParseFile(file);

            foreach (var obj in file.DynamicModifiers)
            {
                var name = obj.name.ToLower();

                if (_definedModifiersInfoArgsBlocks.ContainsKey(name))
                    Logger.LogWarning(
                        EnumLocKey.EXCEPTION_DEFINED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_DEFINED_MODIFIER_IN_FILE,
                        new Dictionary<string, string>
                        {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", obj.name },
                                { "{otherFilePath}", _definitionFiles[name] }
                        }
                    );
                else if (_allInfoArgsBlocks.ContainsKey(name))
                    Logger.LogWarning(
                        EnumLocKey.EXCEPTION_DEFINED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                        new Dictionary<string, string>
                        {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", obj.name },
                                { "{otherFilePath}", _definitionFiles[name] }
                        }
                    );
                else
                {
                    List<EnumScope> scopes = new List<EnumScope>();
                    foreach (var scope in obj.Categories)
                    {
                        var key = scope.stringValue;

                        var uppedScope = key.ToUpper();
                        if (!Enum.TryParse(uppedScope, out EnumScope enumScope))
                            throw new Exception(GuiLocManager.GetLoc(
                                EnumLocKey.EXCEPTION_DEFINED_MODIFIER_HAS_UNSUPPORTED_CATEGORY_IN_FILE,
                                new Dictionary<string, string>
                                {
                                        { "{filePath}", fileInfoPair.Value.filePath },
                                        { "{blockName}", obj.name },
                                        { "{category}", uppedScope },
                                        { "{allowedCategories}", string.Join(",", Enum.GetValues(typeof(EnumScope))) }
                                }
                            ));

                        scopes.Add(enumScope);
                    }
                    if (scopes.Count == 0) scopes.Add(EnumScope.ALL);

                    EnumValueType defaultValueType;
                    object defaultValue;

                    var valueType = obj.ValueType.GetValue();

                    string valueTypeString = valueType is GameString gameString ?
                        gameString.stringValue :
                        "" + valueType;

                    if (valueTypeString == "number" || valueTypeString == "percentage" || valueTypeString == "percentage_in_hundred")
                    {
                        defaultValueType = EnumValueType.FLOAT;
                        defaultValue = 0;
                    }
                    else if (valueTypeString == "yes_no")
                    {
                        defaultValueType = EnumValueType.BOOLEAN;
                        defaultValue = false;
                    }
                    else throw new Exception(GuiLocManager.GetLoc(
                                EnumLocKey.EXCEPTION_DEFINED_MODIFIER_HAS_UNSUPPORTED_VALUE_TYPE_IN_FILE,
                                new Dictionary<string, string>
                                {
                                        { "{filePath}", fileInfoPair.Value.filePath },
                                        { "{blockName}", name },
                                        { "{valueType}", valueTypeString },
                                        { "{allowedCategories}", string.Join(",", Enum.GetValues(typeof(EnumScope))) }
                                }
                            ));

                    var infoArgsBlock = new InfoArgsBlock(
                        name, EnumScope.MODIFIER,
                        scopes.ToArray(),
                        new EnumValueType[] { defaultValueType },
                        defaultValueType,
                        defaultValue
                    );

                    _allInfoArgsBlocks[name] = infoArgsBlock;
                    _definedModifiersInfoArgsBlocks[name] = infoArgsBlock;
                    _definitionFiles[name] = fileInfoPair.Value.filePath;
                }
            }
        }

        private static void LoadScriptedModifiersFile(KeyValuePair<string, FileInfo> fileInfoPair)
        {
            var parser = new GameParser();
            var file = new ScriptedModifiersGameFile(fileInfoPair.Value, true);
            parser.ParseFile(file);

            foreach (var obj in file.DynamicModifiers)
            {
                var info = obj.ScriptBlockInfo;
                var name = info.GetBlockName().ToLower();

                if (_scriptedModifiersInfoArgsBlocks.ContainsKey(name))
                    Logger.LogWarning(
                        EnumLocKey.EXCEPTION_SCRIPTED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_SCRIPTED_MODIFIER_IN_FILE,
                        new Dictionary<string, string>
                        {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.GetBlockName() },
                                { "{otherFilePath}", _definitionFiles[name] }
                        }
                    );
                else if (_allInfoArgsBlocks.ContainsKey(name))
                    Logger.LogWarning(
                        EnumLocKey.EXCEPTION_SCRIPTED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                        new Dictionary<string, string>
                        {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.GetBlockName() },
                                { "{otherFilePath}", _definitionFiles[name] }
                        }
                    );
                else
                {
                    var infoArgsBlock = new InfoArgsBlock(
                        name, EnumScope.MODIFIER,
                        new EnumScope[] { EnumScope.ALL },
                        new EnumValueType[] { EnumValueType.BOOLEAN }
                    );

                    _allInfoArgsBlocks[name] = infoArgsBlock;
                    _scriptedModifiersInfoArgsBlocks[name] = infoArgsBlock;
                    _definitionFiles[name] = fileInfoPair.Value.filePath;
                }
            }
        }

        private static void LoadScriptedEffectsFile(KeyValuePair<string, FileInfo> fileInfoPair)
        {
            var parser = new GameParser();
            var file = new ScriptedEffectsGameFile(fileInfoPair.Value, true);
            parser.ParseFile(file);

            foreach (var obj in file.DynamicEffects)
            {
                var info = obj.ScriptBlockInfo;
                var name = info.GetBlockName().ToLower();

                if (_scriptedEffectsInfoArgsBlocks.ContainsKey(name))
                    Logger.LogWarning(
                        EnumLocKey.EXCEPTION_SCRIPTED_EFFECT_DUPLICATE_NAME_WITH_OTHER_SCRIPTED_EFFECT_IN_FILE,
                        new Dictionary<string, string>
                        {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.GetBlockName() },
                                { "{otherFilePath}", _definitionFiles[name] }
                        }
                    );
                else if (_allInfoArgsBlocks.ContainsKey(name))
                    Logger.LogWarning(
                        EnumLocKey.EXCEPTION_SCRIPTED_EFFECT_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                        new Dictionary<string, string>
                        {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.GetBlockName() },
                                { "{otherFilePath}", _definitionFiles[name] }
                        }
                    );
                else
                {
                    var infoArgsBlock = new InfoArgsBlock(
                        name, EnumScope.EFFECT,
                        new EnumScope[] { EnumScope.ALL },
                        new EnumValueType[] { EnumValueType.BOOLEAN }
                    );

                    _allInfoArgsBlocks[name] = infoArgsBlock;
                    _scriptedEffectsInfoArgsBlocks[name] = infoArgsBlock;
                    _definitionFiles[name] = fileInfoPair.Value.filePath;
                }
            }
        }

        private static void LoadScriptedTriggersFile(KeyValuePair<string, FileInfo> fileInfoPair)
        {
            var parser = new GameParser();
            var file = new ScriptedTriggersGameFile(fileInfoPair.Value, true);
            parser.ParseFile(file);

            foreach (var obj in file.DynamicTriggers)
            {
                var info = obj.ScriptBlockInfo;
                var name = info.GetBlockName().ToLower();

                if (_scriptedTriggersArgsBlocks.ContainsKey(name))
                    Logger.LogWarning(
                        EnumLocKey.EXCEPTION_SCRIPTED_TRIGGER_DUPLICATE_NAME_WITH_OTHER_SCRIPTED_TRIGGER_IN_FILE,
                        new Dictionary<string, string>
                        {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.GetBlockName() },
                                { "{otherFilePath}", _definitionFiles[name] }
                        }
                    );
                else if (_allInfoArgsBlocks.ContainsKey(name))
                    Logger.LogWarning(
                        EnumLocKey.EXCEPTION_SCRIPTED_TRIGGER_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                        new Dictionary<string, string>
                        {
                                { "{filePath}", fileInfoPair.Value.filePath },
                                { "{blockName}", info.GetBlockName() },
                                { "{otherFilePath}", _definitionFiles[name] }
                        }
                    );
                else
                {
                    var infoArgsBlock = new InfoArgsBlock(
                        name, EnumScope.TRIGGER,
                        new EnumScope[] { EnumScope.TRIGGER },
                        new EnumValueType[] { EnumValueType.BOOLEAN }
                    );

                    _allInfoArgsBlocks[name] = infoArgsBlock;
                    _scriptedTriggersArgsBlocks[name] = infoArgsBlock;
                    _definitionFiles[name] = fileInfoPair.Value.filePath;
                }
            }
        }
    }

}
