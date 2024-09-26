using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.common.units
{
    class SubUnit : IParadoxObject
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode => _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
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

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            throw new NotImplementedException();
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            //TODO Implementation
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }

    class SubUnitsFile : IParadoxObject
    {
        public FileInfo FileInfo { private set; get; }

        private static readonly string TOKEN_SUB_UNITS = "sub_units";
        private readonly Dictionary<string, SubUnit> _allSubUnits;
        private readonly Dictionary<string, SubUnit> _subUnits;

        public bool HasAnyInnerInfo => _subUnits != null && _subUnits.Count > 0;

        public SubUnitsFile(FileInfo currentFile, Dictionary<string, SubUnit> allSubUnits)
        {
            FileInfo = currentFile;
            _allSubUnits = allSubUnits;
            _subUnits = new Dictionary<string, SubUnit>();
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            if (!HasAnyInnerInfo) return false;

            ParadoxUtils.StartBlock(sb, outTab, TOKEN_SUB_UNITS);

            foreach (var subUnit in _subUnits.Values)
                subUnit.Save(sb, outTab + tab, tab);

            ParadoxUtils.EndBlock(sb, outTab);
            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(null, () =>
            {
                if (token == TOKEN_SUB_UNITS)
                    parser.AdvancedParse(prevLayer, InnerTokenCallback);
                else
                    throw new UnknownTokenException(token);
            });
        }

        public void InnerTokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(TOKEN_SUB_UNITS, () =>
            {
                if (_allSubUnits.ContainsKey(token))
                    Logger.LogLayeredWarning(
                        prevLayer, TOKEN_SUB_UNITS,
                        EnumLocKey.SUB_UNIT_DUPLICATE_DEFINITION,
                        new Dictionary<string, string> {
                            { "{name}", token },
                            { "{filePath}", _allSubUnits[token].FileInfo?.filePath }
                        }
                    );

                if (_subUnits.ContainsKey(token))
                    Logger.LogLayeredWarning( //TODO Change to Error after SubUnit.Save() implementation
                        prevLayer, TOKEN_SUB_UNITS,
                        EnumLocKey.SUB_UNIT_DUPLICATE_DEFINITION_IN_CURRENT_FILE,
                        new Dictionary<string, string> { { "{name}", token } }
                    ); ;

                SubUnit subUnit = new SubUnit(FileInfo, token);
                Logger.ParseLayeredValue(prevLayer, token, ref subUnit, parser, subUnit);

                _allSubUnits[subUnit.Name] = subUnit;
                _subUnits[subUnit.Name] = subUnit;
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
