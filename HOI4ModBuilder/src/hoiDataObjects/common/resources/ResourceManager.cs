using HOI4ModBuilder.src;
using HOI4ModBuilder.src.managers;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.hoiDataObjects.common.resources
{
    class ResourceManager : IParadoxRead
    {
        public static ResourceManager Instance { get; private set; }
        private static FileInfo _currentFile = null;
        private static Dictionary<FileInfo, Dictionary<string, Resource>> _resourcesByFilesMap = new Dictionary<FileInfo, Dictionary<string, Resource>>(0);
        private static Dictionary<string, Resource> _allResources = new Dictionary<string, Resource>(0);

        public static void Load(Settings settings)
        {
            Instance = new ResourceManager();
            _resourcesByFilesMap = new Dictionary<FileInfo, Dictionary<string, Resource>>();
            _allResources = new Dictionary<string, Resource>();

            var fileInfos = FileManager.ReadMultiTXTFileInfos(settings, @"common\resources\");

            foreach (FileInfo fileInfo in fileInfos.Values)
            {
                _currentFile = fileInfo;
                var fs = new System.IO.FileStream(fileInfo.filePath, System.IO.FileMode.Open);
                ParadoxParser.Parse(fs, Instance);
            }
        }

        public static bool TryGetResource(string tag, out Resource resource)
        {
            return _allResources.TryGetValue(tag, out resource);
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token == "resources")
            {
                var dictionary = new Dictionary<string, Resource>();
                var resources = new ResourceDictionary(dictionary, _allResources);
                parser.Parse(resources);
                _resourcesByFilesMap[_currentFile] = dictionary;
            }
        }
    }

    class ResourceDictionary : IParadoxRead
    {
        private Dictionary<string, Resource> _fileDictionary, _allDictionary;
        public ResourceDictionary(Dictionary<string, Resource> fileDictionary, Dictionary<string, Resource> allDictionary)
        {
            _fileDictionary = fileDictionary;
            _allDictionary = allDictionary;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            Resource resource = new Resource(token);
            parser.Parse(resource);
            _fileDictionary[token] = resource;
            _allDictionary[token] = resource;
        }
    }
}
