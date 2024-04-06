using HOI4ModBuilder.src.hoiDataObjects.history.units.divisionTemplates;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOB : IParadoxObject
    {
        private readonly int _hashCode = NextHashCode;
        private static int _nextHashCode;
        private static int NextHashCode = _nextHashCode == int.MaxValue ? _nextHashCode = int.MinValue : _nextHashCode++;
        public override int GetHashCode() => _hashCode;

        private bool _needToSave;
        public bool NeedToSave
        {
            get
            {
                if (_needToSave ||
                    _units != null && _units.NeedToSave ||
                    _airWings != null && _airWings.NeedToSave ||
                    _instantEffect != null && _instantEffect.NeedToSave)
                {
                    return true;
                }

                foreach (var divisionTemplate in _divisionTemplates)
                    if (divisionTemplate.NeedToSave) return true;

                return false;
            }
        }

        public FileInfo FileInfo { get; set; }

        private List<DivisionTemplate> _divisionTemplates = new List<DivisionTemplate>();
        private List<DivisionTemplate> DivisionTemplates { get => _divisionTemplates; }

        private OOBUnits _units;
        public OOBUnits Units { get => _units; }

        private OOBAirWings _airWings;
        private OOBAirWings AirWings { get => _airWings; }

        private OOBInstantEffect _instantEffect;
        public OOBInstantEffect InstantEffect { get => _instantEffect; }

        public OOB(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        public bool Save(StringBuilder sb, string outTab, string tab)
        {
            bool newParagraphFlag = false;

            foreach (var divisionTemplate in _divisionTemplates)
            {
                ParadoxUtils.NewLineIfNeeded(sb, "", ref newParagraphFlag);
                newParagraphFlag = divisionTemplate.Save(sb, "", "\t");
            }

            if (_units != null && _units.HasAnyInnerInfo)
            {
                ParadoxUtils.NewLineIfNeeded(sb, "", ref newParagraphFlag);
                newParagraphFlag = _units.Save(sb, "", "\t");
            }

            if (_airWings != null && _airWings.HasAnyInnerInfo)
            {
                ParadoxUtils.NewLineIfNeeded(sb, "", ref newParagraphFlag);
                newParagraphFlag = _airWings.Save(sb, "", "\t");
            }

            if (_instantEffect != null && _instantEffect.HasAnyInnerInfo)
            {
                ParadoxUtils.NewLineIfNeeded(sb, "", ref newParagraphFlag);
                newParagraphFlag = _instantEffect.Save(sb, "", "\t");
            }

            return true;
        }

        public void TokenCallback(ParadoxParser parser, LinkedLayer prevLayer, string token)
        {
            Logger.WrapTokenCallbackExceptions(FileInfo.filePath, () =>
            {
                if (token == DivisionTemplate.BLOCK_NAME)
                    Logger.ParseLayeredListedValue(prevLayer, token, ref _divisionTemplates, parser, new DivisionTemplate());
                else if (token == OOBUnits.BLOCK_NAME)
                    Logger.ParseNewLayeredValueOrContinueOld(prevLayer, token, ref _units, parser, new OOBUnits());
                else if (token == OOBAirWings.BLOCK_NAME)
                    Logger.ParseNewLayeredValueOrContinueOld(prevLayer, token, ref _airWings, parser, new OOBAirWings());
                else if (token == OOBInstantEffect.BLOCK_NAME)
                    Logger.ParseNewLayeredValueOrContinueOld(prevLayer, token, ref _instantEffect, parser, new OOBInstantEffect());
                else
                    throw new UnknownTokenException(token);
            });
        }

        public bool Validate(LinkedLayer prevLayer) => true;
    }
}
