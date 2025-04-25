using HOI4ModBuilder.src.newParser.structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HOI4ModBuilder.src.newParser.objects
{
    public class SavePattern
    {
        private static readonly Dictionary<string, SavePattern> _savePatterns = new Dictionary<string, SavePattern>();
        public static void LoadAll()
        {
            foreach (var savePattern in _savePatterns.Values)
                savePattern.Load();
        }

        private readonly string _fullPath;
        private readonly string _directoryPath;
        private readonly string _fileName;

        private List<SavePatternParameter> _parameters;
        private List<SavePatternParameter> _defaultParameters;
        private List<SavePatternParameter> _customParameters;
        public List<SavePatternParameter> Parameters
            => _customParameters != null && SettingsManager.Settings.IsUseCustomSavePatterns() ?
                _customParameters :
                _defaultParameters != null ?
                    _defaultParameters :
                    _parameters;

        public SavePattern(string directoryPath, string fileName)
        {
            _directoryPath = directoryPath;
            _fileName = fileName + ".json";
            _fullPath = Path.Combine(_directoryPath, _fileName);

            _parameters = new List<SavePatternParameter>();

            if (_savePatterns.TryGetValue(_fullPath, out var _))
                throw new Exception("SavePattern with path " + _fullPath + " already registered");

            _savePatterns[_fullPath] = this;
        }
        public SavePattern(string[] directoryPath, string fileName) : this(Path.Combine(directoryPath), fileName)
        { }

        public SavePattern Load()
        {
            _defaultParameters = null;
            _customParameters = null;
            Load(false, Path.Combine("data", "savePatterns"), ref _defaultParameters);
            Load(true, Path.Combine("data", "custom", "savePatterns"), ref _customParameters);
            return this;
        }

        private void Load(bool isCustom, string pathPrefix, ref List<SavePatternParameter> list)
        {
            string directoryPath = Path.Combine(pathPrefix, _directoryPath);
            string filePath = Path.Combine(directoryPath, _fileName);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (File.Exists(filePath))
                list = JsonConvert.DeserializeObject<List<SavePatternParameter>>(File.ReadAllText(filePath));
            else if (!isCustom)
                list = new List<SavePatternParameter>();
            else
                return;

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

            if (!isCustom || isCustom && SettingsManager.Settings.IsUseCustomSavePatterns())
                File.WriteAllText(filePath, JsonConvert.SerializeObject(list, Formatting.Indented));
        }

        public SavePattern(string directoryPath, string fileName, string[] parameters) : this(directoryPath, fileName)
        {
            foreach (var parameter in parameters)
                _parameters.Add(new SavePatternParameter(parameter, false, false, false, false));
        }

        public SavePattern Add(IEnumerable<string> parameters)
        {
            foreach (var parameter in parameters)
                _parameters.Add(new SavePatternParameter(parameter, false, false, false, false));
            return this;
        }
    }
}
