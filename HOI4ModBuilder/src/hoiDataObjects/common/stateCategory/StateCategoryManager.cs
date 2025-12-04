using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.src.hoiDataObjects.common.buildings;
using HOI4ModBuilder.src.hoiDataObjects.common.ideologies;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.managers.settings;
using HOI4ModBuilder.src.newParser;
using HOI4ModBuilder.src.newParser.interfaces;
using HOI4ModBuilder.src.newParser.objects;
using HOI4ModBuilder.src.newParser.structs;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HOI4ModBuilder.src.hoiDataObjects.common.stateCategory
{
    public class StateCategoryManager
    {
        public static StateCategoryManager Instance { get; private set; }

        private static readonly string FOLDER_PATH = FileManager.AssembleFolderPath(new[] { "common", "state_category" });
        private static Dictionary<string, StateCategory> _allStateCategories = new Dictionary<string, StateCategory>();

        public static void Load(BaseSettings settings)
        {
            Instance = new StateCategoryManager();
            _allStateCategories = new Dictionary<string, StateCategory>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, FOLDER_PATH, FileManager.TXT_FORMAT);

            bool raisedAnyException = false;
            foreach (var fileInfo in fileInfoPairs.Values)
            {
                LoadFile(new GameParser(), new StateCategoryGameFile(fileInfo), out var raisedException);
                raisedAnyException |= raisedException;
            }

            if (raisedAnyException)
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_WHILE_STATE_CATEGORY_LOADING));
        }

        private static void LoadFile(GameParser parser, StateCategoryGameFile file, out bool raisedException)
        {
            raisedException = false;
            try
            {
                parser.ParseFile(file);

                if (file.StateCategory.Count == 0)
                    return;

                //if (file.StateCategory.Count > 1)
                //    throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_MORE_THAN_ONE_STATE_CATEGORY_IN_FILE));
                // var category = file.StateCategory.Last().Value;

                foreach (var category in file.StateCategory.Values)
                {
                    if (!_allStateCategories.ContainsKey(category.name))
                        _allStateCategories[category.name] = category;
                    else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_STATE_CATEGORY_DUPLICATE_NAME_IN_FILE,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", file.FilePath},
                            { "{categoryName}", category.name }
                        }
                    ));
                }

                
            }
            catch (Exception ex)
            {
                raisedException = true;
                var category = file.StateCategory.Count == 0 ? null : file.StateCategory.Last().Value;

                Logger.LogExceptionAsError(
                    EnumLocKey.ERROR_WHILE_STATE_CATEGORY_LOADING,
                    new Dictionary<string, string>
                    {
                        { "{name}", category?.name },
                        { "{filePath}", file.FilePath }
                    },
                    ex
                );
            }
        }

        public static Dictionary<string, StateCategory>.KeyCollection GetNames()
            => _allStateCategories.Keys;

        public static bool TryGet(string name, out StateCategory stateCategory)
            => _allStateCategories.TryGetValue(name, out stateCategory);

        public static StateCategory Get(string name)
        {
            if (_allStateCategories.TryGetValue(name, out var stateCategory))
                return stateCategory;
            return null;
        }
    }
}
