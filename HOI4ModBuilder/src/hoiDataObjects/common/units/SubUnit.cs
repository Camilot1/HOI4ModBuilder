using HOI4ModBuilder.src.utils;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units
{
    class SubUnit : IParadoxRead
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private bool _needToSave; //TODO Implement changes
        public bool NeedToSave { get => _needToSave; }

        public FileInfo FileInfo { get; set; }
        public string Name { get; private set; }

        public SubUnit(FileInfo fileInfo, string name)
        {
            FileInfo = fileInfo;
            Name = name;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            //TODO Implementation
        }
    }

    class SubUnits : IParadoxRead
    {
        private readonly FileInfo _currentFile;
        private readonly Dictionary<string, SubUnit> _subUnits;

        public SubUnits(FileInfo currentFile, Dictionary<string, SubUnit> subUnits)
        {
            _currentFile = currentFile;
            _subUnits = subUnits;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            try
            {
                var subUnit = parser.Parse(new SubUnit(_currentFile, token));

                if (_subUnits.ContainsKey(subUnit.Name))
                    throw new Exception(GuiLocManager.GetLoc(
                            EnumLocKey.ERROR_UNIT_DUPLICATE_NAME,
                            new Dictionary<string, string>
                            {
                                { "{name}", $"{subUnit.Name}" },
                                { "{firstFilePath}", _subUnits[subUnit.Name].FileInfo.filePath }
                            }
                        ));

                _subUnits[subUnit.Name] = subUnit;
            }
            catch (Exception ex)
            {
                throw new Exception(GuiLocManager.GetLoc(
                    EnumLocKey.ERROR_WHILE_SUB_UNIT_LOADING,
                    new Dictionary<string, string> { { "{token}", token } }
                ), ex);
            }
        }
    }

    class SubUnitsFile : IParadoxRead
    {
        private readonly FileInfo _currentFile;
        private readonly Dictionary<string, SubUnit> _subUnits;

        public SubUnitsFile(FileInfo currentFile, Dictionary<string, SubUnit> units)
        {
            _currentFile = currentFile;
            _subUnits = units;
        }

        public void TokenCallback(ParadoxParser parser, string token)
        {
            if (token != "sub_units") return;

            try
            {
                parser.Parse(new SubUnits(_currentFile, _subUnits));
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsError(
                    EnumLocKey.ERROR_WHILE_SUB_UNITS_LOADING,
                    new Dictionary<string, string> { { "{filePath}", _currentFile.filePath } },
                    ex
                );
            }
        }
    }
}
