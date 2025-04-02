using HOI4ModBuilder.src.newParser.structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class SaveAdapter
    {
        private static readonly Dictionary<string, SaveAdapter> _adapters = new Dictionary<string, SaveAdapter>();
        public static void LoadAll()
        {
            foreach (var adapter in _adapters.Values)
                adapter.Load();
        }

        private readonly string _fullPath;
        private readonly string _directoryPath;
        private readonly string _fileName;
        private List<SaveAdapterParameter> _parameters;
        private List<SaveAdapterParameter> _defaultParameters;
        private List<SaveAdapterParameter> _customParameters;
        public List<SaveAdapterParameter> Parameters
            => _customParameters != null ?
                _customParameters :
                _defaultParameters != null ?
                    _defaultParameters :
                    _parameters;

        public SaveAdapter(string directoryPath, string fileName)
        {
            _directoryPath = directoryPath;
            _fileName = fileName + ".json";
            _fullPath = Path.Combine(_directoryPath, _fileName);

            _parameters = new List<SaveAdapterParameter>();

            if (_adapters.TryGetValue(_fullPath, out var _))
                throw new Exception("SaveAdapter with path " + _fullPath + " already registered");

            _adapters[_fullPath] = this;
        }
        public SaveAdapter(string[] directoryPath, string fileName) : this(Path.Combine(directoryPath), fileName)
        { }

        public SaveAdapter Load()
        {
            Load(Path.Combine("data", "saveAdapters"), ref _defaultParameters);
            Load(Path.Combine("data", "custom", "saveAdapters"), ref _customParameters);
            return this;
        }

        private void Load(string pathPrefix, ref List<SaveAdapterParameter> list)
        {
            string directoryPath = Path.Combine(pathPrefix, _directoryPath);
            string filePath = Path.Combine(directoryPath, _fileName);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (File.Exists(filePath))
                list = JsonConvert.DeserializeObject<List<SaveAdapterParameter>>(File.ReadAllText(filePath));
            else
                list = new List<SaveAdapterParameter>();

            var usedNames = new HashSet<string>();
            foreach (var parameter in list)
                usedNames.Add(parameter.Name);

            foreach (var parameter in _parameters)
            {
                if (usedNames.Contains(parameter.Name))
                    continue;

                usedNames.Add(parameter.Name);
                list.Add(parameter);
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(list, Formatting.Indented));
        }

        public SaveAdapter(string directoryPath, string fileName, string[] parameters) : this(directoryPath, fileName)
        {
            foreach (var parameter in parameters)
                _parameters.Add(new SaveAdapterParameter(parameter, false, false, false));
        }

        public SaveAdapter Add(IEnumerable<string> parameters)
        {
            foreach (var parameter in parameters)
                _parameters.Add(new SaveAdapterParameter(parameter, false, false, false));
            return this;
        }
    }
}
