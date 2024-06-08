using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units.divisionsNames
{
    class DivisionNamesGroup : IParadoxRead
    {
        public static readonly string BLOCK_NAME = "division_names_group";

        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        public bool _needToSave;
        public bool NeedToSave { get => _needToSave; }

        public FileInfo FileInfo { get; set; }
        public string Name { get; private set; }

        public DivisionNamesGroup(FileInfo fileInfo, string name)
        {
            FileInfo = fileInfo;
            Name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            //TODO implement
        }
    }

    class DivisionNamesGroupFile : IParadoxRead
    {
        private readonly FileInfo _currentFile;
        private readonly Dictionary<string, DivisionNamesGroup> _groups;

        public DivisionNamesGroupFile(FileInfo currentFile, Dictionary<string, DivisionNamesGroup> groups)
        {
            _currentFile = currentFile;
            _groups = groups;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            try
            {
                var group = parser.Parse(new DivisionNamesGroup(_currentFile, token));

                if (_groups.ContainsKey(group.Name))
                    Logger.LogWarning(
                        EnumLocKey.ERROR_DIVISION_NAMES_GROUP_DUPLICATE_NAME,
                        new Dictionary<string, string>
                        {
                            { "{name}", $"{group.Name}" },
                            { "{firstFilePath}", _groups[group.Name].FileInfo.filePath }
                        }
                    );

                _groups[group.Name] = group;
            }
            catch (Exception ex)
            {
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.ERROR_WHILE_DIVISION_NAMES_GROUP_LOADING,
                    new Dictionary<string, string> {
                        { "{token}", token },
                        { "{filePath}", _currentFile.filePath }
                    }
                ), ex);
            }
        }
    }
}
