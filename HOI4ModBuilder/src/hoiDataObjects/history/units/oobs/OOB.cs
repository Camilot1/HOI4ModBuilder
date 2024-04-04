using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.dataObjects;
using HOI4ModBuilder.src.dataObjects.argBlocks;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using HOI4ModBuilder.src.hoiDataObjects.history.units.divisionTemplates;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.air;
using HOI4ModBuilder.src.hoiDataObjects.history.units.oobs.naval;
using HOI4ModBuilder.src.Pdoxcl2Sharp;
using HOI4ModBuilder.src.utils;
using HOI4ModBuilder.src.utils.exceptions;
using Pdoxcl2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HOI4ModBuilder.src.hoiDataObjects.history.units.oobs
{
    class OOB : IParadoxObject
    {
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
                newParagraphFlag = _units.Save(sb, "", "\t");
            }

            if (_instantEffect != null && _instantEffect.HasAnyInnerInfo)
            {
                ParadoxUtils.NewLineIfNeeded(sb, "", ref newParagraphFlag);
                newParagraphFlag = _units.Save(sb, "", "\t");
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
