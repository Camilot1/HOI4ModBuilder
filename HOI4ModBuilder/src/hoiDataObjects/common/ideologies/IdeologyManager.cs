using HOI4ModBuilder.hoiDataObjects.common.resources;
using HOI4ModBuilder.src.managers;
using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.ideologies
{
    class IdeologyManager : IParadoxRead
    {
        public static IdeologyManager Instance { get; private set; }
        private static FileInfo _currentFile = null;
        private static Dictionary<FileInfo, List<IdeologyGroup>> _ideologyGroupsByFilesMap = new Dictionary<FileInfo, List<IdeologyGroup>>();
        private static Dictionary<string, IdeologyGroup> _allIdeologiyGroups = new Dictionary<string, IdeologyGroup>();

        public static void Load(Settings settings)
        {
            Instance = new IdeologyManager();
            _ideologyGroupsByFilesMap = new Dictionary<FileInfo, List<IdeologyGroup>>();
            _allIdeologiyGroups = new Dictionary<string, IdeologyGroup>();

            var fileInfoPairs = FileManager.ReadFileInfos(settings, @"common\ideologies\", FileManager.TXT_FORMAT);

            foreach (var fileInfo in fileInfoPairs.Values)
            {
                _currentFile = fileInfo;
                using (var fs = new System.IO.FileStream(fileInfo.filePath, System.IO.FileMode.Open))
                    ParadoxParser.Parse(fs, Instance);
            }
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "ideologies")
            {
                var ideologyGroups = new List<IdeologyGroup>();
                var list = new IdeologyGroupList(ideologyGroups);
                parser.Parse(list);
                _ideologyGroupsByFilesMap.Add(_currentFile, ideologyGroups);

                foreach (var category in ideologyGroups)
                {
                    if (!_allIdeologiyGroups.ContainsKey(category.name)) _allIdeologiyGroups[category.name] = category;
                    else throw new Exception(GuiLocManager.GetLoc(
                        EnumLocKey.EXCEPTION_IDEOLOGY_GROUP_DUPLICATE_GROUP_NAME_INF_FILE,
                        new Dictionary<string, string>
                        {
                            { "{filePath}", _currentFile.filePath },
                            { "{ideologyGroupName}", category.name }
                        }
                    ));
                }
            }
        }
    }

    class IdeologyGroupList : IParadoxRead
    {
        private List<IdeologyGroup> _ideologyGroups;

        public IdeologyGroupList(List<IdeologyGroup> ideologyGroups)
        {
            _ideologyGroups = ideologyGroups;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            IdeologyGroup ideologyGroup = new IdeologyGroup(token);
            parser.Parse(ideologyGroup);
            _ideologyGroups.Add(ideologyGroup);
        }
    }
}
