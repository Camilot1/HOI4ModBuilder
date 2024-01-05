using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.src.hoiDataObjects.common.ideologies;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.stateCategory
{
    class StateCategoryManager : IParadoxRead
    {
        public static StateCategoryManager Instance { get; private set; }
        private static FileInfo _currentFile = null;
        private static Dictionary<FileInfo, List<StateCategory>> _stateCategoriesByFilesMap = new Dictionary<FileInfo, List<StateCategory>>();
        private static Dictionary<string, StateCategory> _allStateCategories = new Dictionary<string, StateCategory>();

        public static void Load(Settings settings)
        {
            Instance = new StateCategoryManager();
            _stateCategoriesByFilesMap = new Dictionary<FileInfo, List<StateCategory>>();
            _allStateCategories = new Dictionary<string, StateCategory>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\state_category\");

            foreach (var fileInfo in fileInfos.Values)
            {
                _currentFile = fileInfo;
                var fs = new System.IO.FileStream(fileInfo.filePath, System.IO.FileMode.Open);
                ParadoxParser.Parse(fs, Instance);
            }
        }

        public static Dictionary<string, StateCategory>.KeyCollection GetStateCategoriesNames()
        {
            return _allStateCategories.Keys;
        }

        public static bool TryGetStateCategory(string name, out StateCategory stateCategory)
        {
            return _allStateCategories.TryGetValue(name, out stateCategory);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "state_categories")
            {
                var stateCategoryList = new List<StateCategory>();
                var list = new StateCategoryList(stateCategoryList);
                parser.Parse(list);
                _stateCategoriesByFilesMap.Add(_currentFile, stateCategoryList);
                foreach (var category in stateCategoryList)
                {
                    if (!_allStateCategories.ContainsKey(category.name)) _allStateCategories[category.name] = category;
                    else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_STATE_CATEGORY_DUPLICATE_NAME_IN_FILE,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", _currentFile.filePath},
                            { "{categoryName}", category.name }
                        }
                    ));
                }
            }
        }
    }

    class StateCategoryList : IParadoxRead
    {
        private List<StateCategory> _stateCategories;
        public StateCategoryList(List<StateCategory> stateCategories)
        {
            _stateCategories = stateCategories;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            StateCategory stateCategory = new StateCategory(token);
            parser.Parse(stateCategory);
            _stateCategories.Add(stateCategory);
        }
    }
}
