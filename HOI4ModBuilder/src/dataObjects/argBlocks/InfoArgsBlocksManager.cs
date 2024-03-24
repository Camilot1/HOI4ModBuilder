using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Newtonsoft.Json;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using static HOI4ModBuilder.src.dataObjects.argBlocks.InfoArgsBlock;

namespace HOI4ModBuilder.src.dataObjects.argBlocks
{
    class InfoArgsBlocksManager
    {
        public static readonly InfoArgsBlock limitInfoArgsBlock = new InfoArgsBlock("limit", new EnumScope[] { EnumScope.CONDITIONAL });
        public static readonly InfoArgsBlock ifInfoArgsBlock = new InfoArgsBlock("if", new Dictionary<string, InfoArgsBlock> { { "limit", limitInfoArgsBlock } });
        public static readonly InfoArgsBlock elseIfInfoArgsBlock = new InfoArgsBlock("else_if", new Dictionary<string, InfoArgsBlock> { { "limit", limitInfoArgsBlock } });
        public static readonly InfoArgsBlock elseInfoArgsBlock = new InfoArgsBlock("else");

        public static Dictionary<string, InfoArgsBlock> allInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        public static Dictionary<string, InfoArgsBlock> effectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        public static Dictionary<string, InfoArgsBlock> modifiersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        public static Dictionary<string, InfoArgsBlock> triggersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();

        public static Dictionary<string, InfoArgsBlock> scriptedEffectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        public static Dictionary<string, InfoArgsBlock> definedModifiersArgsBlocks = new Dictionary<string, InfoArgsBlock>();
        public static Dictionary<string, InfoArgsBlock> scriptedTriggersArgsBlocks = new Dictionary<string, InfoArgsBlock>();

        public static Dictionary<string, string> definitionFiles = new Dictionary<string, string>();

        private static string currentLoadingFilePath = null;

        public static void Load(Settings settings)
        {
            try
            {
                definitionFiles = new Dictionary<string, string>();

                allInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
                effectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
                modifiersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
                triggersInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();

                scriptedEffectsInfoArgsBlocks = new Dictionary<string, InfoArgsBlock>();
                definedModifiersArgsBlocks = new Dictionary<string, InfoArgsBlock>();
                scriptedTriggersArgsBlocks = new Dictionary<string, InfoArgsBlock>();

                LoadInfoArgsBlocks(@"data\effects.json", effectsInfoArgsBlocks);
                LoadInfoArgsBlocks(@"data\custom\effects.json", effectsInfoArgsBlocks);
                LoadInfoArgsBlocks(@"data\modifiers.json", effectsInfoArgsBlocks);
                LoadInfoArgsBlocks(@"data\custom\modifiers.json", effectsInfoArgsBlocks);

                scriptedEffectsInfoArgsBlocks = LoadScriptedEffects(settings);
                definedModifiersArgsBlocks = LoadDefinedModifiers(settings);
                scriptedTriggersArgsBlocks = LoadScriptedTriggers(settings);

                LoadInfoArgsBlocks(@"data\triggers.json", triggersInfoArgsBlocks);
                LoadInfoArgsBlocks(@"data\custom\triggers.json", triggersInfoArgsBlocks);

                currentLoadingFilePath = null;
            }
            catch (Exception e)
            {
                //Очищаем
                definitionFiles = new Dictionary<string, string>();
                throw new Exception($"JSON file: \"{currentLoadingFilePath}\".", e);
            }

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
                        else if (allInfoArgsBlocks.ContainsKey(newBlock.Name))
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
                            allInfoArgsBlocks[newBlock.Name] = newBlock;
                            dictionary[newBlock.Name] = newBlock;
                            definitionFiles[block.Name] = filePath;
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
                    else if (allInfoArgsBlocks.ContainsKey(block.Name))
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
                        allInfoArgsBlocks[block.Name] = block;
                        dictionary[block.Name] = block;
                        definitionFiles[block.Name] = filePath;
                    }
                }
            }
        }

        private static Dictionary<string, InfoArgsBlock> LoadScriptedEffects(Settings settings)
        {
            Dictionary<string, InfoArgsBlock> dictionary = new Dictionary<string, InfoArgsBlock>();
            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\scripted_effects\");
            if (fileInfos.Count == 0) return dictionary;

            foreach (var fileInfo in fileInfos)
            {
                currentLoadingFilePath = fileInfo.Value.filePath;

                var list = new List<ScriptedEffect>();
                var file = new ScriptedEffectsFile(list);

                using (var fs = new FileStream(fileInfo.Value.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, file);

                foreach (var info in list)
                {
                    if (scriptedEffectsInfoArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_SCRIPTED_EFFECT_DUPLICATE_NAME_WITH_OTHER_SCRIPTED_EFFECT_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfo.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", definitionFiles[info.name] }
                            }
                        );
                    else if (allInfoArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_SCRIPTED_EFFECT_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfo.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", definitionFiles[info.name] }
                            }
                        );
                    else
                    {
                        var infoArgsBlock = new InfoArgsBlock(
                                    info.name,
                                    new EnumScope[] { EnumScope.ALL },
                                    new EnumNewArgsBlockValueType[] { EnumNewArgsBlockValueType.BOOLEAN }
                                );

                        allInfoArgsBlocks[info.name] = infoArgsBlock;
                        scriptedEffectsInfoArgsBlocks[info.name] = infoArgsBlock;
                        definitionFiles[info.name] = fileInfo.Value.filePath;
                    }
                }
            }
            return dictionary;
        }


        private static Dictionary<string, InfoArgsBlock> LoadDefinedModifiers(Settings settings)
        {
            Dictionary<string, InfoArgsBlock> dictionary = new Dictionary<string, InfoArgsBlock>();
            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\modifier_definitions\");
            if (fileInfos.Count == 0) return dictionary;

            foreach (var fileInfo in fileInfos)
            {
                currentLoadingFilePath = fileInfo.Value.filePath;

                var list = new List<DefinedModifierInfo>();
                var file = new DefinedModifierFile(list);

                using (var fs = new FileStream(fileInfo.Value.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, file);

                foreach (var info in list)
                {
                    if (definedModifiersArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_DEFINED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_DEFINED_MODIFIER_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfo.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", definitionFiles[info.name] }
                            }
                        );
                    else if (allInfoArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_DEFINED_MODIFIER_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfo.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", definitionFiles[info.name] }
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
                                        { "{filePath}", fileInfo.Value.filePath },
                                        { "{blockName}", info.name },
                                        { "{category}", uppedScope },
                                        { "{allowedCategories}", string.Join(",", Enum.GetValues(typeof(EnumScope))) }
                                    }
                                ));

                            scopes.Add(enumScope);
                        }
                        if (scopes.Count == 0) scopes.Add(EnumScope.ALL);

                        EnumNewArgsBlockValueType defaultValueType;
                        object defaultValue;

                        if (info.valueType == "number" || info.valueType == "percentage" || info.valueType == "percentage_in_hundred")
                        {
                            defaultValueType = EnumNewArgsBlockValueType.FLOAT;
                            defaultValue = 0;
                        }
                        else if (info.valueType == "yes_no")
                        {
                            defaultValueType = EnumNewArgsBlockValueType.BOOLEAN;
                            defaultValue = false;
                        }
                        else throw new Exception(GuiLocManager.GetLoc(
                                    EnumLocKey.EXCEPTION_DEFINED_MODIFIER_HAS_UNSUPPORTED_VALUE_TYPE_IN_FILE,
                                    new Dictionary<string, string>
                                    {
                                        { "{filePath}", fileInfo.Value.filePath },
                                        { "{blockName}", info.name },
                                        { "{valueType}", info.valueType },
                                        { "{allowedCategories}", string.Join(",", Enum.GetValues(typeof(EnumScope))) }
                                    }
                                ));

                        var infoArgsBlock = new InfoArgsBlock(
                                    info.name,
                                    scopes.ToArray(),
                                    new EnumNewArgsBlockValueType[] { defaultValueType },
                                    defaultValueType,
                                    defaultValue
                                );

                        allInfoArgsBlocks[info.name] = infoArgsBlock;
                        definedModifiersArgsBlocks[info.name] = infoArgsBlock;
                        definitionFiles[info.name] = fileInfo.Value.filePath;
                    }
                }
            }

            return dictionary;
        }

        private static Dictionary<string, InfoArgsBlock> LoadScriptedTriggers(Settings settings)
        {
            Dictionary<string, InfoArgsBlock> dictionary = new Dictionary<string, InfoArgsBlock>();
            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\scripted_triggers\");
            if (fileInfos.Count == 0) return dictionary;

            foreach (var fileInfo in fileInfos)
            {
                currentLoadingFilePath = fileInfo.Value.filePath;

                var list = new List<ScriptedTrigger>();
                var file = new ScriptedTriggerFile(list);

                using (var fs = new FileStream(fileInfo.Value.filePath, FileMode.Open))
                    ParadoxParser.Parse(fs, file);

                foreach (var info in list)
                {
                    if (scriptedTriggersArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_SCRIPTED_TRIGGER_DUPLICATE_NAME_WITH_OTHER_SCRIPTED_TRIGGER_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfo.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", definitionFiles[info.name] }
                            }
                        );
                    else if (allInfoArgsBlocks.ContainsKey(info.name))
                        Logger.LogWarning(
                            EnumLocKey.EXCEPTION_SCRIPTED_TRIGGER_DUPLICATE_NAME_WITH_OTHER_ARGS_BLOCK_IN_FILE,
                            new Dictionary<string, string>
                            {
                                { "{filePath}", fileInfo.Value.filePath },
                                { "{blockName}", info.name },
                                { "{otherFilePath}", definitionFiles[info.name] }
                            }
                        );
                    else
                    {
                        var infoArgsBlock = new InfoArgsBlock(
                                    info.name,
                                    new EnumScope[] { EnumScope.ALL },
                                    new EnumNewArgsBlockValueType[] { EnumNewArgsBlockValueType.BOOLEAN }
                                );

                        allInfoArgsBlocks[info.name] = infoArgsBlock;
                        scriptedTriggersArgsBlocks[info.name] = infoArgsBlock;
                        definitionFiles[info.name] = fileInfo.Value.filePath;
                    }
                }
            }
            return dictionary;
        }

        /** Метод для получения информационных блоков об эффектах, модификаторах и т.п. */
        public static bool TryGetInfoArgsBlock(string token, out InfoArgsBlock infoArgsBlock)
        {
            if (allInfoArgsBlocks.TryGetValue(token, out infoArgsBlock)) return true;
            return false;
        }
    }

    class ScriptedEffectsFile : IParadoxRead
    {
        private List<ScriptedEffect> _list;

        public ScriptedEffectsFile(List<ScriptedEffect> list)
        {
            _list = list;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token.StartsWith("@"))
            { //TODO Reimplement @CONSTANTs
                parser.ReadString();
                return;
            }

            var info = new ScriptedEffect(token);
            Logger.Log($"\tLoading ScriptedEffect \"{info.name}\"");
            parser.Parse(info);
            _list.Add(info);
        }
    }

    class ScriptedEffect : IParadoxRead
    {
        public string name;

        public ScriptedEffect(string name)
        {
            this.name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            //TODO Implement
        }
    }

    class DefinedModifierFile : IParadoxRead
    {
        private List<DefinedModifierInfo> _list;

        public DefinedModifierFile(List<DefinedModifierInfo> list)
        {
            _list = list;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token.StartsWith("@"))
            { //TODO Reimplement @CONSTANTs
                parser.ReadString();
                return;
            }

            var info = new DefinedModifierInfo { name = token };
            Logger.Log($"\tLoading DefinedModifier \"{info.name}\"");
            parser.Parse(info);
            _list.Add(info);
        }
    }

    class DefinedModifierInfo : IParadoxRead
    {
        public string name;
        public string colorType;
        public string valueType;
        public byte precision;
        public string postfix;
        public List<string> categories = new List<string>(0);

        public void TokenCallback(ParadoxParser parser, string token)
        {
            switch (token)
            {
                case "color_type": colorType = parser.ReadString(); break;
                case "value_type": valueType = parser.ReadString(); break;
                case "precision": precision = parser.ReadByte(); break;
                case "postfix": postfix = parser.ReadString(); break;
                case "category": categories.Add(parser.ReadString()); break;
            }
        }
    }

    class ScriptedTriggerFile : IParadoxRead
    {
        private List<ScriptedTrigger> _list;

        public ScriptedTriggerFile(List<ScriptedTrigger> list)
        {
            _list = list;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token.StartsWith("@"))
            { //TODO Reimplement @CONSTANTs
                parser.ReadString();
                return;
            }

            var info = new ScriptedTrigger(token);
            Logger.Log($"\tLoading ScriptedTrigger \"{info.name}\"");
            parser.Parse(info);
            _list.Add(info);
        }
    }

    class ScriptedTrigger : IParadoxRead
    {
        public string name;

        public ScriptedTrigger(string name)
        {
            this.name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            //TODO Implement
        }
    }
}
